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
			var starts = starttimes.ToList ();
			var finishes = finishtimes.ToList ();
			foreach (var crew in crews) 
			{
				ISequenceItem start = starttimes.Where (cr => cr.StartNumber == crew.StartNumber).FirstOrDefault();
				starts.Remove (start);
				ISequenceItem finish = finishtimes.Where(cr => cr.StartNumber == crew.StartNumber).FirstOrDefault();
				finishes.Remove (finish);
				crew.SetTimeStamps (start == null ? DateTime.MinValue : start.TimeStamp, finish == null ? DateTime.MinValue : finish.TimeStamp);
			}
			foreach (var number in starts.Select(s => s.StartNumber).Union(finishes.Select(f => f.StartNumber))) 
			{
				ISequenceItem start = starttimes.Where (cr => cr.StartNumber == number).FirstOrDefault();
				ISequenceItem finish = finishtimes.Where (cr => cr.StartNumber == number).FirstOrDefault();
				crews.Add (new UnidentifiedCrew (number, start == null ? DateTime.MinValue : start.TimeStamp, finish == null ? DateTime.MinValue : finish.TimeStamp));
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

	public class CategoryResultsGenerator
	{
		static ILog Logger = LogManager.GetCurrentClassLogger ();

		public static void Generate (IEnumerable<ICategory> categories)
		{
			foreach (var category in categories) 
			{
				category.SetOrdering ();
			}
		}
	}
}