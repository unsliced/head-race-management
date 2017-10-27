using System;
using FileHelpers;
using Head.Common.Internal.Converters;

namespace Head.Common.BritishRowing
{
	[DelimitedRecord(","), IgnoreFirst(1)] 
	public class RawCrew
	{
        // "CrewID","Crew Name","Date Submitted","EventID","Event Identity","Event Override Name","Composite","CompositeCode","Submitting Club","Submitting Club Index",
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
        // "Submitting Administrator Name","Submitting Administrator Email","Entries Secretary","Entries Secretary Email","Competition Contact Name","Competition Contact Home Telephone",
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
        // "Competition Contact Mobile Telephone","Competition Contact Work Telephone","Competition Contact Email","PreEvent Contact Name","PreEvent Contact Home Telephone",
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
        // "PreEvent Contact Mobile Telephone","PreEvent Contact Work Telephone","PreEvent Contact Email","Paid","Payment Date","Payment Type","Refunded","Current Crew Status",
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
        // "Notes","Boat Name","Boat ID","Boating Permissions Club Name","Boating Permissions Club Index Code","Boating Permissions Club Email","Division Assigned","Club Code",
        [FieldQuoted()]
        public string notes;
        [FieldQuoted()]
        public string boatName;
        [FieldQuoted()]
        public string boatID;
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
        // "Band Name","CrewLetter","CompetitionNotes","Rowing CRI","Rowing CRI Max","Sculling CRI","Sculling CRI Max","Rowing Status Points","Sculling Status Points",
        [FieldQuoted()]
        public string bandName;
        [FieldQuoted()]
        public string crewLetter;
        [FieldQuoted()]
        public string competitionNotes;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        public int rowingPointsCri;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        public int rowingPointsCriMax;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        public int scullingPointsCri;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        public int scullingPointsCriMax;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _rowingPointsTotal;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _scullingPointsTotal;
        // "Baseline Rowing CRI","Baseline Rowing CRI Max","Baseline Sculling CRI","Baseline Sculling CRI Max","Baseline Rowing Status Points","Baseline Sculling Status Points",
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _baselineRowingCri;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _baselineRowingCriMax;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _BaselineScullingCri;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _baselineScullingCriMax;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _baselineRowingStatusPoints;
        [FieldQuoted(), FieldConverter(typeof(PointsConverter))]
        int _baselineScullingStatusPoints;
        // "Accepted","Rejected","Withdrawn","Scratched",
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

