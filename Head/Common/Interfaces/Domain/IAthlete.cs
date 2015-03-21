using System;
using System.Collections.Generic;
using Head.Common.Interfaces.Enums;
using Head.Common.Internal.Categories;

namespace Head.Common.Domain
{
	public interface IValidation<T>
	{
		bool Validate (T source);
	}

	public interface IAthlete
	{
		string Name { get; } 
		string FullName { get; } 
		int Age { get; } 
		DateTime DateOfBirth { get; } 
		IClub Club { get; }
		IClub RawClub { get; }
		ICrew Crew { get; } 
		void SetCrew (ICrew crew);
		int CrewId { get; }
		void PickAClub (IEnumerable<IClub> clubs);
		string Licence { get; } 
		bool IsCox { get ; } 
		int Seat { get; }
		int Points (bool sculling);
		bool HasRaw { get; }
	}

	public interface IStartPosition 
	{
		int CrewId { get; } 
		int StartNumber { get; }
	}
		
	public interface IRace 
	{
		string Name { get;}
		DateTime RaceDate { get;}
		int Identifier { get; } 
		IList<ICrew> Crews { get; } 
		IList<IStartPosition> StartPositions { get; }
		// IList<ISequence> Sequences { get; }
		// IList<IResult> Results { get; }

		// IRecipe DrawRecipe { get; } 
	}


}

