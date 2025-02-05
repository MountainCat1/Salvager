using System;
using UnityEngine;

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
            var region = new Region();

            region.GenerateLevels(rooms, count, minJumps, maxJumpDistance, minDistance);

            return region;
        }
    }
}