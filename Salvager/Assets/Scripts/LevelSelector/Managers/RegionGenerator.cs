using System.Collections.Generic;
using System.Linq;
using Constants;
using UnityEngine;
using Utilities;

namespace Managers.LevelSelector
{
    public interface IRegionGenerator
    {
        public Region Generate();
    }

    public class RegionGenerator : MonoBehaviour, IRegionGenerator
    {
        // Serialized Fields
        [SerializeField] private RoomBlueprint[] rooms;

        [SerializeField] int count = 10;
        [SerializeField] int minJumps = 3;
        [SerializeField] float maxJumpDistance = 0.4f;
        [SerializeField] float minDistance = 0.2f;

        public Region Generate()
        {
            var region = GenerateLevels(rooms, count, minJumps, maxJumpDistance, minDistance);

            return region;
        }

        public Region GenerateLevels(
            RoomBlueprint[] blueprints,
            int count,
            int minJumps,
            float maxJumpDistance,
            float minDistance = 0.1f)
        {
            var region = new Region();
            region.Name = Names.Regions.RandomElement();
            
            List<Location> locations = new();

            List<Vector2> positions = new();
            int maxAttempts = 100;

            // 1. Generate levels with unique positions
            for (int i = 0; i < count; i++)
            {
                Vector2 randomPosition;
                int attempts = 0;

                do
                {
                    randomPosition = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
                    attempts++;
                } while (positions.Any(pos => Vector2.Distance(pos, randomPosition) < minDistance) &&
                         attempts < maxAttempts);

                if (attempts >= maxAttempts)
                {
                    Debug.LogWarning($"Failed to place level {i} with min distance {minDistance}");
                    continue;
                }

                Location location = Location.GenerateRandom(blueprints);
                location.Position = randomPosition;

                locations.Add(location);
                positions.Add(randomPosition);
            }

            // 2. Select start and end levels ensuring minJumps
            Location startLocation = locations[UnityEngine.Random.Range(0, locations.Count)];
            Location endLocation = GetFarthestLevel(startLocation, positions, locations);

            startLocation.Type = LevelType.StartNode;
            endLocation.Type = LevelType.EndNode;

            // 3. Generate initial connections with maxJumpDistance
            GenerateConnections(positions, maxJumpDistance, locations);

            // 4. Ensure the graph is fully connected
            EnsureGraphConnectivity(locations);

            Debug.Log(
                $"Generated {count} levels with minDistance: {minDistance}. Start: {startLocation.Id}, End: {endLocation.Id}");

            foreach (var location in locations)
            {
                region.AddLocation(location);
            }
            
            return region;
        }

        private void GenerateConnections(List<Vector2> positions, float maxJumpDistance, List<Location> locations)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                Location currentLocation = locations[i];
                Vector2 currentPosition = positions[i];

                var validNeighbors = locations.Select((level, index) => (level, pos: positions[index]))
                    .Where(t => t.level != currentLocation &&
                                Vector2.Distance(currentPosition, t.pos) <= maxJumpDistance)
                    .OrderBy(t => Vector2.Distance(currentPosition, t.pos))
                    .Take(4)
                    .Select(t => t.level)
                    .ToList();

                currentLocation.Neighbours.AddRange(validNeighbors);
            }
        }

        private void EnsureGraphConnectivity(List<Location> locations)
        {
            HashSet<Location> visited = new HashSet<Location>();
            Stack<Location> stack = new Stack<Location>();
            stack.Push(locations[0]);

            while (stack.Count > 0)
            {
                Location current = stack.Pop();
                if (!visited.Contains(current))
                {
                    visited.Add(current);
                    foreach (var neighbor in current.Neighbours)
                    {
                        if (!visited.Contains(neighbor))
                            stack.Push(neighbor);
                    }
                }
            }

            foreach (var level in locations)
            {
                if (!visited.Contains(level))
                {
                    Location closestConnectedLocation = visited
                        .OrderBy(l => Vector2.Distance(GetPosition(l, locations), GetPosition(level, locations)))
                        .FirstOrDefault();

                    if (closestConnectedLocation != null)
                    {
                        level.Neighbours.Add(closestConnectedLocation);
                        closestConnectedLocation.Neighbours.Add(level);
                        visited.Add(level);
                    }
                }
            }
        }

        private Location GetFarthestLevel(Location startLocation, List<Vector2> positions, List<Location> locations)
        {
            int startIndex = locations.IndexOf(startLocation);
            return locations
                .Select((level, index) => (level, pos: positions[index]))
                .OrderByDescending(t => Vector2.Distance(positions[startIndex], t.pos))
                .FirstOrDefault().level;
        }

        private Vector2 GetPosition(Location location, List<Location> locations)
        {
            int index = locations.IndexOf(location);
            return index >= 0 ? new Vector2(index / 10f, index % 10f) : Vector2.zero; // Dummy mapping if needed
        }
    }
}