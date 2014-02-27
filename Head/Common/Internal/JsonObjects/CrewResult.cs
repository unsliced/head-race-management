using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Logic.Domain
{
	/*
    public class CrewResult : ICrewResult
    {
        readonly ICrew _crew;
        TimeSpan? _elapsed;
        TimeSpan? _adjusted;
        FinishType _finishType;
        StringBuilder _citation = new StringBuilder(); 
        readonly int _startSequence;
        readonly int _finishSequence;

        public CrewResult(ICrew crew, TimeSpan? elapsed, TimeSpan? adjusted, FinishType finishType, int startSequence, int finishSequence)
        {
            _crew = crew;
            _elapsed = elapsed;
            _adjusted = adjusted;
            _finishType = finishType;
            _finishSequence = finishSequence;
            _startSequence = startSequence;
        }

        #region ICrewResult implementation        
        public int Overall { get ; set; } 
        public ICrew Crew
        {
            get { return _crew; }
        }        
        public TimeSpan? Elapsed
        {
            get { return _elapsed; } 
        }        
        public TimeSpan? Adjusted
        {
            get { return _adjusted; }
        }        
        public int EventOrder { get ; set; } 
        public int GenderOrder { get ; set; }        
        public int AdjustedOrder { get ; set; }        
        public int ForeignOrder { get ; set; }        

        public string Citation { 
            get { return _citation.ToString(); } 
        }

        public int StartSequence { get { return _startSequence; } }
        public int FinishSequence { get { return _finishSequence; } }

        public FinishType FinishType { get { return _finishType; } } 
               
        public void Disqualify(string citation)
        {
            _citation.AppendFormat("{0}. ", citation);
            _finishType = Logic.Domain.FinishType.DSQ;
        }

        public void Penalise(int seconds, string citation)
        {
            _citation.AppendFormat("{0}. ", citation);
            if(_elapsed.HasValue)
                _elapsed.Value.Add(new TimeSpan(0,0, seconds, 0, 0));
        }

        public void AwardPrize(string citation)
        {
            _citation.AppendFormat("{0}. ", citation);
        }

        public void Adjust(IDictionary<string, int> adjustments)
        {
            if(_elapsed.HasValue)
                _adjusted = _elapsed.Value.Add(-TimeSpan.FromSeconds(adjustments[_crew.Category.MastersCategory]));
        }


        #endregion       

        public static IList<string> HeaderRow { 
            get
            {
				// todo - another VH vs SH difference 
				// return new List<string> {"Finish","Start", "Crew", "Category", "ElapsedTime", "AdjustedTime", "Cat Position", "Adjusted Position", "Foreign Position", "Notes"};
				return new List<string> {"Finish","Start", "Sculler", "Club", "Category", "Time", "Cat Position", "GenderOrder", "Notes"};
            }
        }

        public IList<string> Dump
        {
            get 
            {
                List<string> rv = new List<string> { 
                    FinishType == FinishType.Finished ? Overall.ToString().PadLeft(3) : FinishType.ToString()};
                rv.Add(Crew.StartNumber.ToString().PadLeft(3));
				rv.Add(Crew.Athletes.ToList()[0].Name);
                rv.Add(Crew.FullyQualifiedName); 
                rv.Add(Crew.Category.Name.PadRight(17)); // todo - showing the masters category, where appropriate Crew.CategoryName 
                // rv.AddRange(Crew.DrawDetails);
                rv.Add(((FinishType == FinishType.Finished || FinishType == FinishType.TimeOnly) && Elapsed.HasValue)
                                    ? String.Format("{0}:{1:00.00}", Math.Floor(Elapsed.Value.TotalMinutes), Elapsed.Value.TotalSeconds % 60)
                       : String.Empty);
				//                rv.Add(((FinishType == FinishType.Finished) && Adjusted.HasValue)
				//     ? String.Format("{0}:{1:00.00}", Math.Floor(Adjusted.Value.TotalMinutes), Adjusted.Value.TotalSeconds % 60)
				//     : String.Empty);

                                 //Adjusted.HasValue ? Adjusted.Value.TotalSeconds : 0, 
                rv.Add(FinishType == FinishType.Finished ? EventOrder.ToString().PadLeft(2) : String.Empty);
				rv.Add(FinishType == FinishType.Finished ? GenderOrder.ToString().PadLeft(2) : String.Empty);
				//rv.Add(FinishType == FinishType.Finished ? AdjustedOrder.ToString().PadLeft(3) : String.Empty);
				//rv.Add(FinishType == FinishType.Finished && ForeignOrder > 0 ? ForeignOrder.ToString().PadLeft(3) : String.Empty);
                rv.Add(Citation);
                return rv;
            }
        }
    }
    */
}
