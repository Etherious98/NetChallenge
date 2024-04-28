using NetChallenge.Domain;

namespace NetChallenge.Abstractions
{
    public interface ILocationRepository : IRepository<Location>
    {
        Location GetById(int id);
        Location GetByName(string name);
    }
}