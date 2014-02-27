using System;
using System.Linq;
using Head.Common.Domain;

namespace Logic.Domain
{
	public class Race : IRace
	{
		readonly string _name;
		readonly DateTime _date;
        readonly int _identifier;

		public Race(string name, DateTime date, int identifier)
		{
			_name = name;
			_date = date;
            _identifier = identifier;
		}

        #region IRace implementation

        public DateTime RaceDate { get { return _date; } } 
        public int Identifier { get { return _identifier; } }
        public string Name { get { return _name; } } 



		public System.Collections.Generic.IList<ICrew> Crews {
			get {
				throw new NotImplementedException ();
			}
		}

		public System.Collections.Generic.IList<IStartPosition> StartPositions {
			get {
				throw new NotImplementedException ();
			}
		}

        #endregion	

        public override string ToString()
        {
			return string.Format("[Race: {2}, RaceDate={0}, Crews={3}]", RaceDate.ToString("yyyy-MM-dd"), Identifier, Name, Crews.Count);
        }
	}

}

