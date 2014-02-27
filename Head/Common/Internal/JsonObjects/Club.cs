using System;
using Head.Common.Domain;
using Head.Common.Internal.Overrides;

namespace Head.Common.Internal.JsonObjects
{
	public class Club : IClub, IEquatable<Club>
    {
        readonly ClubDetails _clubDetails; 

		public Club(ClubDetails clubdetails)
        {
            _clubDetails = clubdetails;
        }

		public string Index { get  { return _clubDetails.Index; } } 
        public string Name { 
            get 
            { 
                return String.Format("{0}{1}", 
					_clubDetails.Name,
                        String.IsNullOrEmpty(Country) ? String.Empty : " (" + Country + ")");                                  
            } 
        } 

        public string Country { get { return _clubDetails.Country; } } 
		public bool IsBoatingLocation { get { return _clubDetails.IsBoatingLocation; } } 

        #region IEquatable implementation
        public bool Equals(Club other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other == null)
                return false;
            return this.Index == other.Index;

        }
        #endregion

        public override int GetHashCode()
        {
            return Index.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(obj is Club) return Equals((Club)obj);
            return false;
        }

        public override string ToString()
        {
			return string.Format("[{4}: Index={0}, Name={1}, Country={2}, IsBoatingLocation={3}]", Index, Name, Country, IsBoatingLocation, GetType().Name);
        }
    }
}

