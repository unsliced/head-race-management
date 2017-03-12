using System;
using FileHelpers;
using Head.Common.Internal.Converters;

namespace Head.Common.BritishRowing
{
	[DelimitedRecord(","), IgnoreFirst(1)] 
	public class RawCrew
	{
		[FieldQuoted()]
		public int crewId;
		[FieldQuoted()]
		public string crewName;
        [FieldQuoted(), FieldConverter(ConverterKind.Date, "dd/MM/yyyy")]
        public DateTime dateSubmitted;
        [FieldQuoted()]
		public int eventId;
        [FieldQuoted()]
        public string eventIdentity;
        [FieldQuoted()]
        public string eventOverrideName;
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
        public string competitionContactName;
        [FieldQuoted()]
        public string competitionContactHomeTelephone;
        [FieldQuoted()]
        public string competitionContactMobileTelephone;
        [FieldQuoted()]
        public string competitionContactWorkTelephone;
        [FieldQuoted()]
        public string competitionContactWorkEmail;
        [FieldQuoted()]
        public string preEventContactName;
        [FieldQuoted()]
        public string preEventContactHomeTelephone;
        [FieldQuoted()]
        public string preEventContactMobileTelephone;
        [FieldQuoted()]
        public string preEventContactWorkTelephone;
        [FieldQuoted()]
        public string preEventContactWorkEmail;
        [FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", "N"), FieldNullValue(false)]
		public bool paid;
		[FieldQuoted(), FieldConverter(ConverterKind.Date, "dd/MM/yyyy")]
		public DateTime? paymentDate;
		[FieldQuoted()]
		public string paymentType;
		[FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", "N"), FieldNullValue(false)]
		public bool refunded;
        [FieldQuoted()]
        public string currentCrewStatus;
        [FieldQuoted()]
        public string notes;
        [FieldQuoted()]
        public string boatName;
        [FieldQuoted()]
        public string boatingPermissionClubName;
        [FieldQuoted()]
        public string boatingPermissionClubIndexCode;
        [FieldQuoted()]
        public string boatingPermissionClubEmail;
        [FieldQuoted()]
        public string divisionAssigned;
        [FieldQuoted()]
        public string clubCode;
        [FieldQuoted()]
        public string bandName;
        [FieldQuoted()]
        public string crewLetter;
        [FieldQuoted()]
        public string competitionNotes;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _rowingPointsPri;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _rowingPointsPriMax;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _scullingPointsPri;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _scullingPointsPriMax;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _rowingPointsTotal;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _scullingPointsTotal;
        [FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", "N"), FieldNullValue(false)]
		public bool accepted;
		[FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", "N"), FieldNullValue(false)]
		public bool rejected;
		[FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", "N"), FieldNullValue(false)]
		public bool withdrawn;
		[FieldQuoted(), FieldConverter(ConverterKind.Boolean, "Y", "N"), FieldNullValue(false)]
		public bool scratched;
	}
}

