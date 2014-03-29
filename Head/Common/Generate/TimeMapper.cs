using System;
using System.Collections.Generic;
using Head.Common.Interfaces.Utils;
using Head.Common.Csv;
using Common.Logging;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using System.Linq;
using Head.Common.Internal.Categories;
using Head.Common.Interfaces.Enums;
using System.Text;
using Head.Common.Internal.JsonObjects;

namespace Head.Common.Generate
{
	public class TimeMapper
	{
		static ILog Logger = LogManager.GetCurrentClassLogger ();

		public static void Map (IList<ICrew> crews, IEnumerable<ISequenceItem> starttimes, IEnumerable<ISequenceItem> finishtimes)
		{
			var lStarts = starttimes.ToList ();
			var lFinishes = finishtimes.ToList ();
			foreach (var crew in crews) 
			{
				var starts = lStarts.Where (cr => cr.StartNumber == crew.StartNumber).Select(cr => cr.TimeStamp);
				var finishes = lFinishes.Where(cr => cr.StartNumber == crew.StartNumber).Select(cr => cr.TimeStamp);
				crew.SetTimeStamps (starts, finishes);
				// remove them here otherwise the lazy IEnumerable will remove them before they're consumed in the method call 
				lStarts.RemoveAll(cr => cr.StartNumber == crew.StartNumber);
				lFinishes.RemoveAll(cr => cr.StartNumber == crew.StartNumber);
			}
			foreach (var number in lStarts.Select(s => s.StartNumber).Union(lFinishes.Select(f => f.StartNumber))) 
			{
				crews.Add (new UnidentifiedCrew (number, lStarts.Where (cr => cr.StartNumber == number).Select(cr => cr.TimeStamp), lFinishes.Where (cr => cr.StartNumber == number).Select(cr => cr.TimeStamp)));
			}
		}

		public static void Penalise (IEnumerable<ICrew> crews, IEnumerable<IPenalty> penalties)
		{
			Logger.Warn ("no penalties have been added");
		}

		public static void Adjust (IEnumerable<ICrew> crews, IEnumerable<IAdjustment> adjustments)
		{
			Logger.Warn ("no adjustments have been applied");
		}
	}
}