using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Logic.Domain
{
    public enum FinishType 
    {
        Finished = 0,
        TimeOnly = 1,
        DNF = 2, 
        DSQ = 3,
        DNS = 4,
        Query = -1,
    }
    
}
