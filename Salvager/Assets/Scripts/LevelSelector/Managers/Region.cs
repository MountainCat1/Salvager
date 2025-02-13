using System;
using System.Collections.Generic;

namespace Managers.LevelSelector
{
    public class Region
    {
        public IReadOnlyList<LocationData> Locations => _locations;
        public string Name { get; set; }

        private List<LocationData> _locations = new();
        
        public void AddLocation(LocationData location)
        {
            _locations.Add(location);
        }

        public LocationData GetLocation(Guid locationId)
        {
            return _locations.Find(l => l.Id == locationId);
        }
    }
}
