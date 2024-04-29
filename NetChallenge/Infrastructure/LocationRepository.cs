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
            if(_locations.Contains(item))
            {
                throw new System.Exception("El objeto que intenta insertar ya existe!");
            }
            _locations.Add(item);
        }
    }
}