using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    [Flags]
    public enum OrderLevel
    {
       None = 0,
       A = 1,
        B = 2,
        C =4,
        D = 8,
        E = 16,
        F = 32,
        
    }
}
