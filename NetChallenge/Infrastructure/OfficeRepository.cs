using System;
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
            _offices.Add(item);
        }

        public bool OfficeAlreadyExist(Office item)
        {
            return _offices.Exists(x => x.LocationName == item.LocationName && x.Name == item.Name);
        }
    }
}