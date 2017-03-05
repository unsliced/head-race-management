using System;
using FileHelpers;
using Head.Common.Interfaces.Enums;

namespace Head.Common.BritishRowing
{
	[DelimitedRecord(","), IgnoreFirst(1)] 
	public class RawEvent
	{
		public Gender Gender { get { return (Gender)Enum.Parse(typeof(Gender), eventGender); } } 

		[FieldQuoted()]
		public int eventId;
		[FieldQuoted()]
		public string eventIdentity;
		//[FieldQuoted()]
		//public string customEventID;
		[FieldQuoted()]
		public int numberOfRowers;
		[FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", "N")] 
		public bool scullingStatus;
		[FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", "N")]
		public bool coxedStatus;
		[FieldQuoted()]
		public string eventGender;
		[FieldQuoted()]
		public string eventType;
		[FieldQuoted(), FieldNullValue(0)]
		public int minimumAge;
		//[FieldQuoted(), FieldNullValue(0)]
        //public int maximumAge;
        //[FieldQuoted()]
        //public string crewStatus;
        [FieldQuoted()]
        public string subCategory;
        [FieldQuoted()]
        public string eventInfo;
        [FieldQuoted()]
        public string notes;
        [FieldQuoted()]
        public int fee;
        [FieldQuoted()]
		public string overrideName;
		[FieldQuoted()]
		public string previousEventID;
	}
}

