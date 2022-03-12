using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafs
{
    static class Randomiser
    {
        static Random _rnd = new Random();

        public static Random Random { get { return _rnd; } set { _rnd = value; } }
    }
}
