using System.Collections.Generic;

namespace Managers.LevelSelector
{
    public class Region
    {
        public IReadOnlyList<Location> Locations => _locations;
        public string Name { get; set; }

        private List<Location> _locations = new();
        
        public void AddLocation(Location location)
        {
            _locations.Add(location);
        }
    }
}
