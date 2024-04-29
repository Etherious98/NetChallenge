using System.Collections.Generic;

namespace NetChallenge.Domain
{
    public class Office
    {
        public string LocationName { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public string[] OfficeResource { get; set; }
    }
}