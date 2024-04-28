using System.Collections.Generic;
using System.Linq;
using NetChallenge.Abstractions;
using NetChallenge.Domain;

namespace NetChallenge.Infrastructure
{
    public class LocationRepository : ILocationRepository
    {
        private readonly List<Location> _locations;

        public LocationRepository()
        {
            _locations = new List<Location>();
        }

        public IEnumerable<Location> AsEnumerable()
        {
            return _locations.ToList();
        }

        public void Add(Location item)
        {
            item.Id = _locations.Count + 1;
            _locations.Add(item);
        }

        public Location GetById(int id)
        {
            return _locations.FirstOrDefault(l => l.Id == id);
        }

        public Location GetByName(string name)
        {
            return _locations.Where(x=>x.Name == name).FirstOrDefault();
        }
    }
}