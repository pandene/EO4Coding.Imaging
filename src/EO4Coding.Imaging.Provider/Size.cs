using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EO4Coding.Imaging
{
    public class Size
    {

        public int MaxX { get; set; }
        public int MaxY { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
