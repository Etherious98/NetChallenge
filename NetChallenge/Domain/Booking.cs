using System;

namespace NetChallenge.Domain
{
    public class Booking
    {
        public int Id { get; set; }
        public int OfficeId { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public string User { get; set; }
    }
}