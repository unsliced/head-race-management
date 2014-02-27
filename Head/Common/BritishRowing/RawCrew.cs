using System;
using FileHelpers;

namespace Head.Common.BritishRowing
{
	[DelimitedRecord(","), IgnoreFirst(1)] 
	public class RawCrew
	{
		[FieldQuoted()]
		public int crewId;
		[FieldQuoted()]
		public string crewName;
		[FieldQuoted()]
		public int eventId;
		[FieldQuoted()]
		public string eventIdentity;
		[FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", "N"), FieldNullValue(false)]
		public bool composite;
		[FieldQuoted()]
		public string compositeCode;
		[FieldQuoted()]
		public string submittingClub;
		[FieldQuoted()]
		public string submittingClubIndex;
		[FieldQuoted()]
		public string submittingAdministratorName;
		[FieldQuoted()]
		public string submittingAdministratorEmail;
		[FieldQuoted()]
		public string entriesSecretaryName;
		[FieldQuoted()]
		public string entriesSecretaryEmail;
		[FieldQuoted()]
		public string emergencyContactName;
		[FieldQuoted()]
		public string emergencyContactHomeTelephone;
		[FieldQuoted()]
		public string emergencyContactMobileTelephone;
		[FieldQuoted()]
		public string emergencyContactWorkTelephone;
		[FieldQuoted()]
		public string emergencyContactWorkEmail;
		[FieldQuoted(), FieldConverter(ConverterKind.Date, "dd/MM/yyyy")]
		public DateTime dateSubmitted;
		[FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", "N"), FieldNullValue(false)]
		public bool paid;
		[FieldQuoted(), FieldConverter(ConverterKind.Date, "dd/MM/yyyy")]
		public DateTime? paymentDate;
		[FieldQuoted()]
		public string paymentType;
		[FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", "N"), FieldNullValue(false)]
		public bool refunded;
		[FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", "N"), FieldNullValue(false)]
		public bool accepted;
		[FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", "N"), FieldNullValue(false)]
		public bool rejected;
		[FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", "N"), FieldNullValue(false)]
		public bool withdrawn;
		[FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", "N"), FieldNullValue(false)]
		public bool scratched;
		[FieldQuoted()]
		public string clubNotes;
		[FieldQuoted()]
		public string competitionNotes;
		[FieldQuoted()]
		public string udf1;
		[FieldQuoted()]
		public string udf2;
		[FieldQuoted()]
		public string boatId;
		[FieldQuoted()]
		public string boatName;
		[FieldQuoted()]
		public string boatCode;
		[FieldQuoted()]
		public string boatingPermissionClubName;
		[FieldQuoted()]
		public string boatingPermissionClubIndexCode;
		[FieldQuoted()]
		public string boatingPermissionClubEmail;
		[FieldQuoted()]
		public string coachName;
		[FieldQuoted()]
		public string divisionAssigned;
	}
}

