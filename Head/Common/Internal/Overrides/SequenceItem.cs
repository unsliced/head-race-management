using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Head.Common.Domain;

namespace Head.Common.Internal.Overrides 
{

	[DataContract]
	public class SequenceItem : ISequenceItem
	{
		public DateTime TimeStamp { get { return Time; } } 
			
		[JsonProperty]
		public string Race  { get ; set; }
		[JsonProperty]
		public string Token { get ; set; }
		[JsonProperty]
		public string Location { get ; set; }
		[JsonProperty]
		public string GPS { get ; set; }
		[JsonProperty]
		public int StartNumber { get ; set; }
		[JsonProperty]
		public DateTime Time { get ; set; }
		[JsonProperty]
		public string Notes { get ; set; }
		[JsonProperty]
		public string FileNameStub { get ; set; }


		public bool Equals(ISequenceItem other)
		{
			if (ReferenceEquals(this, other))
				return true;
			if (other == null)
				return false;
			return this.StartNumber == other.StartNumber;

		}

		public override int GetHashCode()
		{
			return StartNumber.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if(obj is ISequenceItem) return Equals((ISequenceItem)obj);
			return false;
		}
	}
}
