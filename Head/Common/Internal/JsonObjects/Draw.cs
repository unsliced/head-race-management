using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Logic.Domain
{
	/*
    public class Draw : IDraw
    {
        ILog Logger = LogManager.GetLogger(typeof(Draw));

        readonly IDictionary<int, ICrew> _draw;
        readonly IList<IClub> _clubs;

        public Draw(int size, IList<IClub> clubs)
        {
            _draw = new Dictionary<int, ICrew>(size);
            _clubs = clubs;
        }

        public void AddCrew(ICrew nextCrew)
        {
            int index = _draw.Keys.ToList().DefaultIfEmpty(0).Max() + 1;
            AddCrew(index, nextCrew);
        }

        public void AddCrew(int index, ICrew nextCrew) 
        {
            _draw.Add(index, nextCrew);
        }

        #region IDraw implementation

        public IDictionary<int, ICrew> RaceOrder
        {
            get { return _draw; } 
        }

        public bool IsValid
        {
            get { return true; } 
        }

        public int Count
        {
            get { return _draw.Count; } 
        }
      
        public void DumpStartPositions()
        {
            var startPositions = _draw.Select(kvp => new StartPosition { CrewId = kvp.Value.CrewId, StartNumber = kvp.Key}).ToList();
            string json = JsonConvert.SerializeObject(startPositions, Formatting.None);
            Logger.InfoFormat("Dumping out the StartPositions overrides - save this to a file if you want to keep it.{0}{1}", Environment.NewLine, json);
        }

        public void Dump(bool verbose)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var kvp in _draw.OrderBy(k => k.Key))
            {
                 
                sb.AppendLine(DrawString(kvp.Value, verbose));
            }
            Logger.InfoFormat("Draw ({0} crews):{1}{2}", _draw.Count, Environment.NewLine, sb.ToString());
            foreach (var pair in _draw.Select(c => c.Value).GroupBy(cw => cw.BoatingCode).Select(g => new { name = g, count = g.Count()}))
            {
                Logger.InfoFormat("{0}: {1}", pair.name.Key, pair.count);
            }
        }

        public IList<string> DrawDetails { 
            get  
            {
                return new List<string>
                {
                    StartNumber.ToString().PadLeft(3),
                    ((_crewOverride != null && !String.IsNullOrEmpty(_crewOverride.CrewName)) ? _crewOverride.CrewName : (_athletes.Count == 0 ? "?" : _athletes[0].Name)).PadRight(25), 
                    ClubName.PadRight(29), 
                    CategoryName.PadRight(17)
                };
            }
        }


        public void DoNames()
        {
            foreach(ICrew crew in _draw.OrderBy(k => k.Key).Select(k => k.Value))
            {
                IList<string> clubNames = 
                    _clubs
                        .Where(c => crew.ClubIndices.Contains(c.Index))
                        .Select(cl => cl.Name)
                        .ToList();
                string name = 
                    clubNames.Count == 0 || !String.IsNullOrEmpty(crew.Name)
                        ? crew.Name 
                        : clubNames.Aggregate((h,t) => String.Format("{0}/{1}", h, t));   
                
                crew.FullyQualifiedName = name;
            }
        }

        public string DrawString(ICrew crew, bool verbose) 
        {
            // todo - look up the club indices 


            string extras = String.Format("[{0}, {1}, {2}, {3}, {5}] {4}",                                      
                                          String.Join(",", crew.ClubIndices),
                                          crew.BoatingCode, 
                                          crew.CrewId, 
                                          crew.PreviousYear.HasValue ? crew.PreviousYear.Value.ToString().PadLeft(3) : "   ",
                                          crew.Notes,
                                          crew.Birthdays
                                          );                                         

            var notes = crew.VoecNotes;
            return String.Format("{0}, {8}, {1}, {2}, {3}{4}{5}{6} {7}", 
                                 crew.StartNumber.ToString().PadLeft(3),
                                 crew.FullyQualifiedName,
								 crew.TimeOnly ? "TimeOnly" : crew.Category.Name,                                                                      
                                 crew.Paid ? String.Empty.PadRight(6) : "unpaid", 
                                 String.IsNullOrEmpty(notes) ? String.Empty : ", " + notes,
                                 "," + crew.BoatingLocation,
                                 "," + crew.SubmittingEmail,
                                 /*
                                  * todo - sort out the collection point for the scullers 
                                 ((_crewOverride == null || String.IsNullOrEmpty(_crewOverride.CollectionPoint))
                                        ? (String.IsNullOrEmpty(_club.CollectionPoint) 
                                            ? String.Empty : _club.CollectionPoint )
                                        : _crewOverride.CollectionPoint).PadRight(11)                    
                                 ,

                                 verbose ? extras : String.Empty, 
                                 crew.Athletes.ToList()[0].Name
                                 );
        }

        #endregion
    }
*/
}

