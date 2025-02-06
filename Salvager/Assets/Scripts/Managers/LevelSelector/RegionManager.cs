using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using Zenject;

namespace Managers.LevelSelector
{
    public interface IRegionManager
    {
        public event Action RegionChanged;
        public Region Region { get; }
        public void SetRegion(Region region, Guid currentLocationId);
        public Guid CurrentLocationId { get; }
        public Location CurrentLocation { get; }
        public int GetDistance(Guid fromGuid, Guid toGuid);
        public int GetDistance(Location from, Location to);
        void ChangeCurrentLocation(Location selectedLocation);
    }


    public class RegionManager : MonoBehaviour, IRegionManager
    {
        public event Action RegionChanged;

        [Inject] private IDataManager _dataManager;
        [Inject] private IRegionGenerator _regionGenerator;

        public Region Region { get; private set; }
        public Guid CurrentLocationId { get; set; }
        public Location CurrentLocation { get; set; }

        public int GetDistance(Guid fromGuid, Guid toGuid)
        {
            return GetDistance(
                Region.Locations.First(l => l.Id == fromGuid),
                Region.Locations.First(l => l.Id == toGuid)
            );
        }

        public int GetDistance(Location from, Location to)
        {
            if (from == to)
                return 0;

            Queue<(Location location, int distance)> queue = new();
            HashSet<Location> visited = new();

            queue.Enqueue((from, 0));
            visited.Add(from);

            while (queue.Count > 0)
            {
                var (current, distance) = queue.Dequeue();

                foreach (var neighbour in current.Neighbours)
                {
                    if (!visited.Contains(neighbour))
                    {
                        if (neighbour == to)
                        {
                            return distance + 1; // Found shortest path
                        }

                        queue.Enqueue((neighbour, distance + 1));
                        visited.Add(neighbour);
                    }
                }
            }

            return -1; // No path found
        }

        public void ChangeCurrentLocation(Location selectedLocation)
        {
            CurrentLocation = selectedLocation;
            CurrentLocationId = selectedLocation.Id;

            foreach (var location in Region.Locations)
            {
                location.DistanceToCurrent = GetDistance(location, CurrentLocation);
            }

            RegionChanged?.Invoke();
        }


        public void SetRegion(Region region, Guid currentLocationId)
        {
            Region = region;

            CurrentLocationId = currentLocationId;

            CurrentLocation = region.Locations.First(l => l.Id == currentLocationId);

            foreach (var location in region.Locations)
            {
                location.DistanceToCurrent = GetDistance(location, CurrentLocation);
            }

            RegionChanged?.Invoke();
        }
    }
}