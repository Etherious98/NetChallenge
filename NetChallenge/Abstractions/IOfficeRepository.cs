using NetChallenge.Domain;
using System.Collections.Generic;

namespace NetChallenge.Abstractions
{
    public interface IOfficeRepository : IRepository<Office>
    {
        Office GetById(int id);
        IEnumerable<Office> GetAllByLocation(int locationId);
    }
}