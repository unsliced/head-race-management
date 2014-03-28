using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Logic.Domain
{
    public enum FinishType 
    {
		// TODO - calculate finish type
        Finished = 0,
		// TODO - handle the time only finishers
        TimeOnly = 1,
		// TODO - crews which started but did not finish
        DNF = 2, 
		// TODO - crews which were disqualified 
        DSQ = 3,
		// TODO - crews which did not start
        DNS = 4,
		// TODO - are there any other questionable areas?
        Query = -1,
    }
    
}
