using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileMover
{
    public class FileMoverOptions
    {
        public string Source { get; set; }
        public string Destination { get; set; }

        public int Interval { get; set; }

    }
}
