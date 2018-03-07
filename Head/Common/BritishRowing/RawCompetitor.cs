using System;
using FileHelpers;
using Head.Common.Internal.Converters;

namespace Head.Common.BritishRowing
{
	[DelimitedRecord(","), IgnoreFirst(1)] 
	public class RawCompetitor
	{
		public int Position { get { return _position; } }
		public int CrewId { get { return _crewId; } }
		public string Surname { get { return _surname; } }
		public string FullName { get { return String.Format("{0} {1}", _firstNames, _surname); } }
		public string Initial { get { return _firstNames[0].ToString(); } } 
		public int? Age { get { return _years; } }
        public string ClubIndex { get { return _clubIndex; } } 
		public string ClubName { get { return _clubName; } } 
		public string Licence { get { return _brMembershipNumber; } }
		public bool IsCox { get { return _cox; } } 

		public int Points(bool sculling) {
			return sculling ? _scullingPoints : _rowingPoints; 
		}

        // "Crew ID","Position","Membership Number","Surname","First Names","Age","DofB","Gender","Cox","Members Club Name","Members Club Index Code",
        // "Rowing PRI","Sculling PRI","Rowing PRI Max","Sculling PRI Max","RowingPoints","ScullingPoints","Substitute",

        [FieldQuoted()]
		int _crewId;       
		[FieldQuoted(), FieldConverter(typeof(PositionConverter))]
		int _position;
		[FieldQuoted()]
		string _brMembershipNumber;
		[FieldQuoted()]
		string _surname;
        [FieldQuoted()]
        string _firstNames;
        [FieldQuoted(), FieldConverter(typeof(YearsConverter))]
        int? _years;
        //[FieldQuoted()]
        //string _dob;
        [FieldQuoted()]
		string _gender;
        [FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", ""), FieldNullValue(false)]
        bool _cox;
        [FieldQuoted()]
        string _clubName;
        // TODO - if this is empty then the first one is used, which isn't great if there are several (and for the foreign vets, there usually are) 
        [FieldQuoted()]
        string _clubIndex;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _rowingPointsPri;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _scullingPointsPri;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _rowingPointsPriMax;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _scullingPointsPriMax;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _coxingPri;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _coxingPriMax;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter)), FieldNullValue(0)]
        int _rowingPoints;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter)), FieldNullValue(0)]
        int _scullingPoints;
        [FieldQuoted()] //, FieldConverter(ConverterKind.Boolean, "Yes", "No"), FieldNullValue(false)]
        string _substitute;
    }
}

