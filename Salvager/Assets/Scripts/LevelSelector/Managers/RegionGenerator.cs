using System;
using System.Collections.Generic;
using System.Linq;
using Constants;
using LevelSelector;
using LevelSelector.Managers;
using UnityEngine;
using Utilities;
using Zenject;

namespace Managers.LevelSelector
{
    public interface IRegionGenerator
    {
        RegionData Generate(RegionType regionType = null);
        void SetSeed(int seed);
        public RegionType GetRegionType(string typeName);
    }

    public class RegionGenerator : MonoBehaviour, IRegionGenerator
    {
        [Inject] private ILocationGenerator _locationGenerator;

        [Inject] private IShopGenerator _shopGenerator;

        [SerializeField] private int count = 10;
        [SerializeField] private int minJumps = 3;
        [SerializeField] private float maxJumpDistance = 0.4f;
        [SerializeField] private float minDistance = 0.2f;
        [SerializeField] private int shopCount = 2;

        private System.Random _random;

        private RegionType[] _regionTypes;

        private void Awake()
        {
            _regionTypes = Resources.LoadAll<RegionType>("RegionTypes");
        }

        public void SetSeed(int seed)
        {
            _random = new System.Random(seed);
        }

        public RegionType GetRegionType(string typeName)
        {
            return _regionTypes.FirstOrDefault(rt => rt.typeName == typeName);
        }
        
        public RegionData Generate(RegionType regionType = null)
        {
            if (_random == null)
            {
                Debug.LogWarning("Seed was not provided, getting a random one instead");
                
                SetSeed(
                    Guid.NewGuid().GetHashCode()
                ); // Generate a seed if one hasn't been set
            }

            if (regionType is null)
            {
                regionType = PickRegionType();
            }
            else
            {
                Debug.LogWarning($"Region type was overridden with {regionType.typeName}");
            }
            
            
            return GenerateLevels(count, minJumps, maxJumpDistance, minDistance, regionType);
        }

        private RegionType PickRegionType()
        {
            return _regionTypes.RandomElement();
        }

        private RegionData GenerateLevels(
            int count,
            int minJumps,
            float maxJumpDistance,
            float minDistance,
            RegionType regionType
        )
        {
            var region = new RegionData() { Name = Names.Regions.RandomElement() };
            var locations = GenerateLocations(count, minDistance);

            if (locations.Count == 0)
                return region;

            AssignNodeTypes(locations);
            GenerateConnections(locations, maxJumpDistance);
            EnsureGraphConnectivity(locations);

            AddFeatures(locations, regionType);
            AddBossNodeFeature(locations, regionType);
            GenerateShops(locations, shopCount);
            locations.ForEach(region.AddLocation);
            
            region.Type = regionType.typeName;
            
            Debug.Log($"Generated {count} levels with minDistance: {minDistance}.");
            
            region.RecalculateNeighbours();
            
            return region;
        }

        private void AddBossNodeFeature(List<LocationData> locations, RegionType regionType)
        {
            var endNode = locations.Single(l => l.Type == LocationType.BossNode);

            _locationGenerator.AddEndFeature(endNode, regionType);
        }

        private void AddFeatures(List<LocationData> locations, RegionType regionType)
        {
            foreach (var location in locations)
            {
                _locationGenerator.AddFeatures(location, regionType);
            }
        }

        private void GenerateShops(ICollection<LocationData> locations, int shopCount)
        {
            for (int i = 0; i < shopCount; i++)
            {
                var location = locations
                    .Where(
                        x =>
                            x.Type != LocationType.StartNode && x.Type != LocationType.EndNode
                                                             && x.ShopData is null
                    )
                    .RandomElement();

                if (location == null)
                {
                    Debug.LogWarning("No more locations to add shops to.");
                    break;
                }

                location.ShopData = _shopGenerator.GenerateShop();
            }
        }

        private List<LocationData> GenerateLocations(int count, float minDistance)
        {
            var locations = new List<LocationData>();
            var positions = new List<Vector2>();
            const int maxAttempts = 100;

            for (int i = 0; i < count; i++)
            {
                Vector2 position = GenerateUniquePosition(
                    positions,
                    minDistance,
                    maxAttempts
                );
                if (position == Vector2.negativeInfinity)
                    continue;

                var location = _locationGenerator.GenerateLocation(_random);
                location.Position = position;
                locations.Add(location);
                positions.Add(position);
            }

            return locations;
        }

        private Vector2 GenerateUniquePosition(
            List<Vector2> positions,
            float minDistance,
            int maxAttempts
        )
        {
            int attempts = 0;
            Vector2 randomPosition;

            do
            {
                randomPosition = new Vector2(_random.Value(), _random.Value());
                attempts++;
            } while (
                positions.Any(pos => Vector2.Distance(pos, randomPosition) < minDistance)
                && attempts < maxAttempts
            );

            return attempts >= maxAttempts ? Vector2.negativeInfinity : randomPosition;
        }

        private void AssignNodeTypes(List<LocationData> locations)
        {
            var startLocation = locations[_random.Next(0, locations.Count)];
            var endLocation = locations
                .OrderByDescending(l => Vector2.Distance(startLocation.Position, l.Position))
                .First();
            var bossLocation = locations
                .OrderByDescending(l => Vector2.Distance(startLocation.Position, l.Position))
                .Skip(1)
                .First();
            
            startLocation.Type = LocationType.StartNode;
            endLocation.Type = LocationType.EndNode;
            bossLocation.Type = LocationType.BossNode;
        }

        private void GenerateConnections(List<LocationData> locations, float maxJumpDistance)
        {
            foreach (var location in locations)
            {
                var neighbours = locations
                    .Where(
                        l =>
                            l != location
                            && Vector2.Distance(location.Position, l.Position)
                            <= maxJumpDistance
                    )
                    .OrderBy(l => Vector2.Distance(location.Position, l.Position))
                    .Take(4)
                    .ToList();

                foreach (var neighbour in neighbours)
                {
                    AddMutualConnection(location, neighbour);
                }
            }
        }

        private void EnsureGraphConnectivity(List<LocationData> locations)
        {
            var visited = new HashSet<LocationData>();
            var components = new List<List<LocationData>>();

            // 1. Find all connected components
            foreach (var location in locations)
            {
                if (visited.Contains(location))
                    continue;

                var component = new List<LocationData>();
                var stack = new Stack<LocationData>();
                stack.Push(location);

                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    if (!visited.Add(current))
                        continue;

                    component.Add(current);
                    foreach (var neighbor in current.Neighbours)
                    {
                        if (!visited.Contains(neighbor))
                        {
                            stack.Push(neighbor);
                        }
                    }
                }

                components.Add(component);
            }

            // 2. Find the largest component
            var largestComponent = components.OrderByDescending(c => c.Count).FirstOrDefault();
            if (largestComponent == null)
                return; // Handle the case where there are no locations

            // 3. Connect each disconnected component to the closest node in the largest
            // component
            foreach (var component in components)
            {
                if (component == largestComponent)
                    continue; // Skip the largest component

                // Find the closest node in the largest component to this component
                var closestA = component
                    .OrderBy(l =>
                        largestComponent.Min(l2 => Vector2.Distance(l.Position, l2.Position))
                    )
                    .FirstOrDefault();
                var closestB = largestComponent
                    .OrderBy(l => Vector2.Distance(closestA.Position, l.Position))
                    .FirstOrDefault();

                // Ensure the connection is bidirectional and update NeighbourIds
                if (closestA != null && closestB != null)
                {
                    AddMutualConnection(closestA, closestB);
                }
            }
        }

        private void AddMutualConnection(LocationData a, LocationData b)
        {
            if (!a.Neighbours.Contains(b))
            {
                a.Neighbours.Add(b);
                a.NeighbourIds = a.Neighbours.Select(l => l.Id).ToArray();
            }

            if (!b.Neighbours.Contains(a))
            {
                b.Neighbours.Add(a);
                b.NeighbourIds = b.Neighbours.Select(l => l.Id).ToArray();
            }
        }
    }
}
