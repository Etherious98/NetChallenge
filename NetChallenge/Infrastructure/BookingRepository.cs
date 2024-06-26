﻿using System.Collections.Generic;
using System.Linq;
using NetChallenge.Abstractions;
using NetChallenge.Domain;

namespace NetChallenge.Infrastructure
{
    public class BookingRepository : IBookingRepository
    {
        private readonly List<Booking> _bookings;

        public BookingRepository()
        {
            _bookings = new List<Booking>();
        }

        public IEnumerable<Booking> AsEnumerable()
        {
            return _bookings.AsEnumerable();
        }

        public void Add(Booking item)
        {
            _bookings.Add(item);
        }
    }
}