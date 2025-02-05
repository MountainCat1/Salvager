using System.Collections.Generic;
using System.Linq;
using Constants;
using UnityEngine;
using Utilities;

namespace Managers.LevelSelector
{
    public class Region
    {
        public IReadOnlyList<Level> Levels => _levels;
        public string Name { get; set; }

        private List<Level> _levels = new();

        public void GenerateLevels(
            RoomBlueprint[] blueprints, 
            int count, 
            int minJumps,
            float maxJumpDistance,
            float minDistance = 0.1f)
        {
            Name = Names.Regions.RandomElement();
            _levels = new List<Level>();

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

                Level level = Level.GenerateRandom(blueprints);
                level.Position = randomPosition;
                
                _levels.Add(level);
                positions.Add(randomPosition);
            }

            // 2. Select start and end levels ensuring minJumps
            Level startLevel = _levels[UnityEngine.Random.Range(0, _levels.Count)];
            Level endLevel = GetFarthestLevel(startLevel, positions, minJumps);

            startLevel.Type = LevelType.StartNode;
            endLevel.Type = LevelType.EndNode;

            // 3. Generate initial connections with maxJumpDistance
            GenerateConnections(positions, maxJumpDistance);

            // 4. Ensure the graph is fully connected
            EnsureGraphConnectivity();

            Debug.Log($"Generated {count} levels with minDistance: {minDistance}. Start: {startLevel}, End: {endLevel}");
        }

        private void GenerateConnections(List<Vector2> positions, float maxJumpDistance)
        {
            for (int i = 0; i < _levels.Count; i++)
            {
                Level currentLevel = _levels[i];
                Vector2 currentPosition = positions[i];

                var validNeighbors = _levels.Select((level, index) => (level, pos: positions[index]))
                    .Where(t => t.level != currentLevel && Vector2.Distance(currentPosition, t.pos) <= maxJumpDistance)
                    .OrderBy(t => Vector2.Distance(currentPosition, t.pos))
                    .Take(4)
                    .Select(t => t.level)
                    .ToList();

                currentLevel.Neighbours.AddRange(validNeighbors);
            }
        }

        private void EnsureGraphConnectivity()
        {
            HashSet<Level> visited = new HashSet<Level>();
            Stack<Level> stack = new Stack<Level>();
            stack.Push(_levels[0]);

            while (stack.Count > 0)
            {
                Level current = stack.Pop();
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

            foreach (var level in _levels)
            {
                if (!visited.Contains(level))
                {
                    Level closestConnectedLevel = visited
                        .OrderBy(l => Vector2.Distance(GetPosition(l), GetPosition(level)))
                        .FirstOrDefault();
                    
                    if (closestConnectedLevel != null)
                    {
                        level.Neighbours.Add(closestConnectedLevel);
                        closestConnectedLevel.Neighbours.Add(level);
                        visited.Add(level);
                    }
                }
            }
        }

        private Level GetFarthestLevel(Level startLevel, List<Vector2> positions, int minJumps)
        {
            int startIndex = _levels.IndexOf(startLevel);
            return _levels
                .Select((level, index) => (level, pos: positions[index]))
                .OrderByDescending(t => Vector2.Distance(positions[startIndex], t.pos))
                .FirstOrDefault().level;
        }

        private Vector2 GetPosition(Level level)
        {
            int index = _levels.IndexOf(level);
            return index >= 0 ? new Vector2(index / 10f, index % 10f) : Vector2.zero; // Dummy mapping if needed
        }
        
        public void AddLevel(Level level)
        {
            _levels.Add(level);
        }
    }
}
