using System.Collections.Generic;
using System.Linq;
using Constants;
using UnityEngine;
using Utilities;

namespace Managers.LevelSelector
{
    public class Region
    {
        public IReadOnlyDictionary<Vector2, Level> Levels => _levels;
        public IReadOnlyDictionary<Vector2, List<Vector2>> Connections => _connections;
        public string Name { get; set; }

        private Dictionary<Vector2, Level> _levels = new();
        private List<Vector2> _nodePositions = new();
        private Dictionary<Vector2, List<Vector2>> _connections = new();

        public void GenerateLevels(RoomBlueprint[] blueprints, int count, int minJumps, float maxJumpDistance,
            float minDistance = 0.1f)
        {
            Name = Names.Regions.RandomElement();
            
            _levels = new Dictionary<Vector2, Level>();
            _connections = new Dictionary<Vector2, List<Vector2>>();
            _nodePositions = new List<Vector2>();

            int maxAttempts = 100; // Prevent infinite loops

            // 1. Generate random positions for levels
            for (int i = 0; i < count; i++)
            {
                Vector2 randomPosition;
                int attempts = 0;

                do
                {
                    randomPosition = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
                    attempts++;
                } while (_nodePositions.Any(pos => Vector2.Distance(pos, randomPosition) < minDistance) &&
                         attempts < maxAttempts);

                if (attempts >= maxAttempts)
                {
                    Debug.LogWarning($"Failed to place level {i} with min distance {minDistance}");
                    continue;
                }

                _nodePositions.Add(randomPosition);
                _levels[randomPosition] = Level.GenerateRandom(blueprints);
            }

            // 2. Select start and end nodes ensuring minJumps
            Vector2 startNode = _nodePositions[UnityEngine.Random.Range(0, _nodePositions.Count)];
            Vector2 endNode = GetFarthestNode(startNode, minJumps);

            _levels[startNode].Type = LevelType.StartNode;
            _levels[endNode].Type = LevelType.EndNode;

            // 3. Generate initial connections with maxJumpDistance
            GenerateConnections(maxJumpDistance);

            // 4. Ensure the graph is fully connected
            EnsureGraphConnectivity();

            Debug.Log($"Generated {count} levels with minDistance: {minDistance}. Start: {startNode}, End: {endNode}");
        }


        private void GenerateConnections(float maxJumpDistance)
        {
            foreach (var node in _nodePositions)
            {
                _connections[node] = new List<Vector2>();

                // Connect to nearest nodes within maxJumpDistance
                var validNeighbors = _nodePositions
                    .Where(n => n != node && Vector2.Distance(node, n) <= maxJumpDistance)
                    .OrderBy(n => Vector2.Distance(node, n))
                    .Take(4) // Try to create 3-4 connections per node
                    .ToList();

                foreach (var neighbor in validNeighbors)
                {
                    if (!_connections[node].Contains(neighbor))
                    {
                        _connections[node].Add(neighbor);
                        if (!_connections.ContainsKey(neighbor))
                            _connections[neighbor] = new List<Vector2>();
                        _connections[neighbor].Add(node);
                    }
                }
            }
        }

        private void EnsureGraphConnectivity()
        {
            // Perform a flood fill (DFS or BFS) to ensure all nodes are reachable
            HashSet<Vector2> visited = new HashSet<Vector2>();
            Stack<Vector2> stack = new Stack<Vector2>();
            stack.Push(_nodePositions[0]);

            while (stack.Count > 0)
            {
                Vector2 current = stack.Pop();
                if (!visited.Contains(current))
                {
                    visited.Add(current);
                    foreach (var neighbor in _connections[current])
                    {
                        if (!visited.Contains(neighbor))
                            stack.Push(neighbor);
                    }
                }
            }

            // If not all nodes are visited, forcefully connect them within maxJumpDistance
            foreach (var node in _nodePositions)
            {
                if (!visited.Contains(node))
                {
                    Vector2 closestConnectedNode = visited
                        .Where(n => Vector2.Distance(n, node) <= 0.7f)
                        .OrderBy(n => Vector2.Distance(n, node))
                        .FirstOrDefault();

                    if (closestConnectedNode != default)
                    {
                        _connections[closestConnectedNode].Add(node);
                        _connections[node] = new List<Vector2> { closestConnectedNode };
                        visited.Add(node);
                    }
                }
            }
        }

        private Vector2 GetFarthestNode(Vector2 startNode, int minJumps)
        {
            return _nodePositions
                .OrderByDescending(n => Vector2.Distance(startNode, n))
                .FirstOrDefault();
        }

        public void AddLevel(Level level, Vector2 levelDataPosition)
        {
            _levels[levelDataPosition] = level;
        }


        public void AddConnection(Vector2 from, List<Vector2> to)
        {
            _connections[from] = to;
        }
    }
}