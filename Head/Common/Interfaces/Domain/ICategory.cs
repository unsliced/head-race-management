using System;
using System.Collections.Generic;
using Head.Common.Interfaces.Enums;

namespace Head.Common.Domain
{
	public interface ICategory : IEquatable<ICategory>
	{
		EventType EventType { get; } 
		string Name { get;}    
		void FilterCrews(IEnumerable<ICrew> crews);
		IEnumerable<ICrew> Crews { get; } 
		int Order { get; } 
		void SetNotOffered ();
		bool Offered { get; } 

		//string Name { get; }
		//int EventId { get; } 
		//ICategory Heavy { get; set;} 
		//Gender Gender { get; }
		//bool ApplyHandicap { get; }
		//string MastersCategory { get; } 

		void SetOrdering ();
	}
}
