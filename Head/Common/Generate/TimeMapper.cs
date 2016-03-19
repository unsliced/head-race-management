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
			ILog logger = LogManager.GetCurrentClassLogger ();
			if (penalties == null)
				return; 
			var lPenalties = penalties.ToList ();
			if (lPenalties.Count == 0)
				return;
			foreach (var penalty in lPenalties) {
				var crew = crews.Where (cr => cr.StartNumber == penalty.StartNumber).FirstOrDefault ();
				if (crew == null) {
					logger.ErrorFormat ("penalty cannot be applied to a crew that doesn't exist: {0}", penalty.StartNumber);
					return;
				}
				if (penalty.Disqualified)
					crew.Disqualify (penalty.Citation);
				else
					crew.SetPenalty (TimeSpan.FromSeconds( penalty.Seconds), penalty.Citation);
			}
		}

		public static void Adjust (IEnumerable<ICrew> crews, IEnumerable<IAdjustment> adjustments)
		{
			IDictionary<Gender, IDictionary<string, TimeSpan>> offsets = new Dictionary<Gender, IDictionary<string, TimeSpan>> ();
			foreach (Gender gender in (Gender[]) Enum.GetValues(typeof(Gender))) {

				var fastest = crews.Where (cr => cr.FinishType == FinishType.Finished && cr.Gender == gender).Min (cr => cr.Elapsed);
				var floor = adjustments.Where(a => a.Minutes == fastest.Minutes).First();
				var ceiling = adjustments.Where(a => a.Minutes == fastest.Minutes+1).First();
				var offset = (fastest - new TimeSpan (0, fastest.Minutes, 0)).TotalSeconds;
				Logger.InfoFormat ("{0} adjustments based on time of {1}", gender, fastest);
				IDictionary<string, TimeSpan> local = new Dictionary<string, TimeSpan>();
				foreach(var kvp in floor.Adjustments)
				{
					var adjustment = (int)Math.Round(kvp.Value + ((ceiling.Adjustments[kvp.Key]-kvp.Value)*(offset/60.0d)),0);
					local.Add(kvp.Key, TimeSpan.FromSeconds(adjustment));
					Logger.InfoFormat("{0}: {1}", kvp.Key, adjustment);
				}
				offsets.Add (gender, local);
			}
		
			// return; // urgent - validate 
			foreach (var crew in crews) {
				if (!crew.IsMasters)
					continue;
				crew.SetAdjusted (offsets [crew.Gender] [crew.EventCategory.MastersCategory]);
			}

		}
	}
}