using System;
using System.Collections.Generic;
using Head.Common.Interfaces.Enums;
using Head.Common.Internal.Categories;
using Logic.Domain;

namespace Head.Common.Domain
{
	public interface ICrew
	{
		bool IsTimeOnly { get;} 
		Gender Gender { get; } 
		EventCategory EventCategory { get; } 
		bool IsForeign { get; } 
		bool IsMasters { get; } 
		void IncludeInCategory(ICategory category);
		IEnumerable<ICategory> Categories { get;} 
		string Name { get; }
		string RawName { get; }
		string AthleteName (int showAthlete, bool full);
		void AddAthlete (IAthlete athlete);
		IClub BoatingLocation { get; } 
		string ShortName { get; } 
		int CrewId { get; } 
		int StartNumber { get; }
		int? PreviousYear { get; }
		string BoatingLocationContact { get; }
		bool IsScratched { get; } 
		bool IsAccepted { get; } 
		bool IsPaid { get; } 
		bool IsNovice { get; } 
		bool IsJunior { get; } 
		int CategoryPosition (ICategory category);
		string CategoryName { get; } // for things like showing masters category and points 
		string VoecNotes { get; } 
		void SetCategoryOrder (ICategory category, int order); 
        string MastersCategory { get; }
        int CRI(bool max);
        int Points { get;}
        string Notes { get; }

		IEnumerable<IAthlete> Athletes { get; } 
		string SubmittingEmail { get; } 

		void SetTimeStamps (IEnumerable<DateTime> starts, IEnumerable<DateTime> finishes);
		string QueryReason { get;} 
		void SetAdjusted(TimeSpan adjustment);
		DateTime StartTime { get; } 
		DateTime FinishTime { get; } 
		TimeSpan Elapsed { get; }
		TimeSpan Adjustment { get; } 
		TimeSpan Adjusted { get; } 
		FinishType FinishType { get; } 
		void SetPenalty(TimeSpan penalty, string citation);
		void Disqualify(string citation);
		string Citation { get; }

        int? StartingBehind { get; }
	}

}
