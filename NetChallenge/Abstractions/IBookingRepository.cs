using NetChallenge.Domain;
using System.Collections.Generic;

namespace NetChallenge.Abstractions
{
    public interface IBookingRepository : IRepository<Booking>
    {
        IEnumerable<Booking> GetByOffice(int officeId);
    }
}