using System.Collections.Generic;
using System.Linq;
using Constants;
using LevelSelector.Managers;
using UnityEngine;
using Utilities;
using Zenject;

namespace Managers.LevelSelector
{
    public interface IRegionGenerator
    {
        Region Generate();
    }

    public class RegionGenerator : MonoBehaviour, IRegionGenerator
    {
        [Inject] private ILocationGenerator _locationGenerator;
        [Inject] private IShopGenerator _shopGenerator;

        [SerializeField] private int _count = 10;
        [SerializeField] private int _minJumps = 3;
        [SerializeField] private float _maxJumpDistance = 0.4f;
        [SerializeField] private float _minDistance = 0.2f;
        [SerializeField] private int _shopCount = 2;

        public Region Generate()
        {
            return GenerateLevels(_count, _minJumps, _maxJumpDistance, _minDistance);
        }

        private Region GenerateLevels(int count, int minJumps, float maxJumpDistance, float minDistance)
        {
            var region = new Region { Name = Names.Regions.RandomElement() };
            var locations = GenerateLocations(count, minDistance);

            if (locations.Count == 0) return region;

            AssignStartAndEnd(locations);
            GenerateConnections(locations, maxJumpDistance);
            EnsureGraphConnectivity(locations);
            GenerateShops(locations, _shopCount);
            locations.ForEach(region.AddLocation);

            Debug.Log($"Generated {count} levels with minDistance: {minDistance}.");
            return region;
        }

        private void GenerateShops(ICollection<LocationData> locations, int shopCount)
        {
            for (int i = 0; i < shopCount; i++)
            {
                var location = locations
                    .Where(x => x.Type != LevelType.StartNode && x.Type != LevelType.EndNode)
                    .Where(x => x.ShopData is null)
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
                Vector2 position = GenerateUniquePosition(positions, minDistance, maxAttempts);
                if (position == Vector2.negativeInfinity) continue;

                var location = _locationGenerator.GenerateLocation();
                location.Position = position;
                locations.Add(location);
                positions.Add(position);
            }

            return locations;
        }

        private Vector2 GenerateUniquePosition(List<Vector2> positions, float minDistance, int maxAttempts)
        {
            int attempts = 0;
            Vector2 randomPosition;

            do
            {
                randomPosition = new Vector2(Random.value, Random.value);
                attempts++;
            } while (positions.Any(pos => Vector2.Distance(pos, randomPosition) < minDistance) &&
                     attempts < maxAttempts);

            return attempts >= maxAttempts ? Vector2.negativeInfinity : randomPosition;
        }

        private void AssignStartAndEnd(List<LocationData> locations)
        {
            var startLocation = locations[Random.Range(0, locations.Count)];
            var endLocation = locations.OrderByDescending(l => Vector2.Distance(startLocation.Position, l.Position))
                .First();
            startLocation.Type = LevelType.StartNode;
            endLocation.Type = LevelType.EndNode;
        }

        private void GenerateConnections(List<LocationData> locations, float maxJumpDistance)
        {
            foreach (var location in locations)
            {
                location.Neighbours = locations
                    .Where(l => l != location && Vector2.Distance(location.Position, l.Position) <= maxJumpDistance)
                    .OrderBy(l => Vector2.Distance(location.Position, l.Position))
                    .Take(4)
                    .ToList();

                location.NeighbourIds = location.Neighbours.Select(l => l.Id.ToString()).ToArray();
            }
        }


        private void EnsureGraphConnectivity(List<LocationData> locations)
        {
            var visited = new HashSet<LocationData>();
            var components = new List<List<LocationData>>();

            // 1. Find all connected components
            foreach (var location in locations)
            {
                if (visited.Contains(location)) continue;

                var component = new List<LocationData>();
                var stack = new Stack<LocationData>();
                stack.Push(location);

                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    if (!visited.Add(current)) continue;

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
            if (largestComponent == null) return; // Handle the case where there are no locations

            // 3. Connect each disconnected component to the closest node in the largest component
            foreach (var component in components)
            {
                if (component == largestComponent) continue; // Skip the largest component

                // Find the closest node in the largest component to this component
                var closestA = component
                    .OrderBy(l => largestComponent.Min(l2 => Vector2.Distance(l.Position, l2.Position)))
                    .FirstOrDefault();
                var closestB = largestComponent.OrderBy(l => Vector2.Distance(closestA.Position, l.Position))
                    .FirstOrDefault();

                // Ensure the connection is bidirectional and update NeighbourIds
                if (closestA != null && closestB != null && !closestA.Neighbours.Contains(closestB))
                {
                    closestA.Neighbours.Add(closestB);
                    closestA.NeighbourIds = closestA.Neighbours.Select(l => l.Id.ToString()).ToArray();
                }

                if (closestB != null && closestA != null && !closestB.Neighbours.Contains(closestA))
                {
                    closestB.Neighbours.Add(closestA);
                    closestB.NeighbourIds = closestB.Neighbours.Select(l => l.Id.ToString()).ToArray();
                }
            }
        }
    }
}