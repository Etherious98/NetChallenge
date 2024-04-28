using System.Collections.Generic;
using System.Linq;
using NetChallenge.Abstractions;
using NetChallenge.Domain;

namespace NetChallenge.Infrastructure
{
    public class OfficeRepository : IOfficeRepository
    {
        private readonly List<Office> _offices;
        public OfficeRepository() 
        {
            _offices = new List<Office>();
        }
        public IEnumerable<Office> AsEnumerable()
        {
            return _offices.AsEnumerable();
        }

        public void Add(Office item)
        {
            item.Id = _offices.Count + 1;
            _offices.Add(item);
        }

        public Office GetById(int id)
        {
            return _offices.FirstOrDefault(o => o.Id == id);
        }

        public IEnumerable<Office> GetAllByLocation(int locationId)
        {
            return _offices.Where(o => o.LocationId == locationId).ToList();
        }
    }
}