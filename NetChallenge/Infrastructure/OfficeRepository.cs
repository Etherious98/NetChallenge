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
            if(_offices.Contains(item)) 
            {
                throw new System.Exception("El objeto que intenta insertar ya existe!");
            }
            _offices.Add(item);
        }
    }
}