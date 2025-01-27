using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Services.MapGenerators.GenerationSteps
{
    public class StepDungeonGenerator : MonoBehaviour, IMapGenerator
    {
        [SerializeField] private List<GenerationStep> generationSteps = new();
        [SerializeField] private int seed = 0;
        [SerializeField] private GenerateMapSettings settings = null!;
        
        [Inject] private IRoomDecorator _roomDecorator = null!;

        
        public event Action MapGenerated;
        public event Action MapGeneratedLate;
        
        public MapData MapData { get; private set; }
        
        public void GenerateMap()
        {
            var data = new GenerateMapData(settings);
            var random = seed == 0 ? new Random() : new Random(seed);
            
            foreach (var step in generationSteps)
            {
                step.Generate(data, settings, random);
            }
            
            _roomDecorator.DecorateRooms(data.Rooms, settings.tileSize);
            
            MapData = CreateMapData(data);
            MapGenerated?.Invoke();
            
            StartCoroutine(DelayedInvoke());
        }

        private IEnumerator DelayedInvoke()
        {
            yield return new WaitForEndOfFrame();
            MapGeneratedLate?.Invoke();
        }

        private MapData CreateMapData(GenerateMapData data)
        {
            return new MapData(data.GridSize, data.Grid, settings.tileSize, data.Rooms);
        }
    }
}