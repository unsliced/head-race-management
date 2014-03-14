using System;
using System.Collections.Generic;
using Head.Common.Interfaces.Enums;
using Head.Common.Internal.Categories;

namespace Head.Common.Domain
{
	public interface ICrew
	{
		bool IsTimeOnly { get;} 
		Gender Gender { get; } 
		EventCategory EventCategory { get; } 
		// TODO - chris - can override but otherwise is if any of the athletes' clubs are non empty country fields 
		bool IsForeign { get; } 
		bool IsMasters { get; } 
		void IncludeInCategory(ICategory category);
		IEnumerable<ICategory> Categories { get;} 
		string Name { get; }
		IClub BoatingLocation { get; } 
		int CrewId { get; } 
		int StartNumber { get; }
		int? PreviousYear { get; }

//		IList<ICategory> Categories { get; }
//		IEnumerable<IAthlete> Athletes { get; } // should this not be hidden and just the sums exposed? 
//		
//		bool Paid { get; } 
//		bool Heavy { get ; set; } 
		//		// TODO - chris - perhaps the StartNumber should be an object? 
//		
//		
//
//		string Notes { get; } 
//		string VoecNotes { get; } 
//		string SubmittingEmail { get; } 
//		bool Include { get; } 
//		string FullyQualifiedName { get; set;} 
//
//		void AddAthlete(IAthlete athlete);
//		void DoNotDraw ();
	}

}
