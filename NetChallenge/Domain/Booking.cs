using System;

namespace NetChallenge.Domain
{
    public class Booking
    {
        public string LocationName { get; set; }
        public string OfficeName { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public string User { get; set; }
    }
}