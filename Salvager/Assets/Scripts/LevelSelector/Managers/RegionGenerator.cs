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
        
        [SerializeField] private int _count = 10;
        [SerializeField] private int _minJumps = 3;
        [SerializeField] private float _maxJumpDistance = 0.4f;
        [SerializeField] private float _minDistance = 0.2f;

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
            locations.ForEach(region.AddLocation);
            
            Debug.Log($"Generated {count} levels with minDistance: {minDistance}.");
            return region;
        }

        private List<Location> GenerateLocations(int count, float minDistance)
        {
            var locations = new List<Location>();
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
            } while (positions.Any(pos => Vector2.Distance(pos, randomPosition) < minDistance) && attempts < maxAttempts);

            return attempts >= maxAttempts ? Vector2.negativeInfinity : randomPosition;
        }

        private void AssignStartAndEnd(List<Location> locations)
        {
            var startLocation = locations[Random.Range(0, locations.Count)];
            var endLocation = locations.OrderByDescending(l => Vector2.Distance(startLocation.Position, l.Position)).First();
            startLocation.Type = LevelType.StartNode;
            endLocation.Type = LevelType.EndNode;
        }

        private void GenerateConnections(List<Location> locations, float maxJumpDistance)
        {
            foreach (var location in locations)
            {
                location.Neighbours = locations
                    .Where(l => l != location && Vector2.Distance(location.Position, l.Position) <= maxJumpDistance)
                    .OrderBy(l => Vector2.Distance(location.Position, l.Position))
                    .Take(4)
                    .ToList();
            }
        }

        private void EnsureGraphConnectivity(List<Location> locations)
        {
            var visited = new HashSet<Location>();
            var stack = new Stack<Location>();
            stack.Push(locations[0]);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (!visited.Add(current)) continue;
                current.Neighbours.ForEach(neighbor => stack.Push(neighbor));
            }

            foreach (var location in locations.Where(location => !visited.Contains(location)))
            {
                var closest = visited.OrderBy(l => Vector2.Distance(l.Position, location.Position)).FirstOrDefault();
                if (closest != null)
                {
                    location.Neighbours.Add(closest);
                    closest.Neighbours.Add(location);
                    visited.Add(location);
                }
            }
        }
    }
}
