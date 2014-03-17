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
		public int Age { get { return DateTime.Today.Year - _dateOfBirth.Year; } } 
		public DateTime DateOfBirth { get { return _dateOfBirth; } } 
		public string ClubIndex { get { return _clubIndex; } } 
		public string ClubName { get { return _clubName; } } 
		public string Licence { get { return _brMembershipNumber; } }

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
		[FieldQuoted(), FieldConverter(ConverterKind.Date, "dd/MM/yyyy")]
		DateTime _dateOfBirth;
		[FieldQuoted()]
		string _gender;
		[FieldQuoted(), FieldConverter(typeof(PointsConverter))]
		int _rowingPoints;
		[FieldQuoted(), FieldConverter(typeof(PointsConverter))]
		int _scullingPoints;
		[FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", ""), FieldNullValue(false)]
		bool _cox;
		[FieldQuoted()]
		string _clubName;
		[FieldQuoted()]
		string _clubIndex;			
	}
}

