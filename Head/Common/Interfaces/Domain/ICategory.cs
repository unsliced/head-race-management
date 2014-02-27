using System;
using System.Collections.Generic;
using Head.Common.Interfaces.Enums;

namespace Head.Common.Domain
{
	public interface ICategory
	{
		EventType EventType { get; } 
		string Name { get;}    
		void FilterCrews(IEnumerable<ICrew> crews);
		IEnumerable<ICrew> Crews { get; } 

		//string Name { get; }
		//int EventId { get; } 
		//int Order { get; } 
		//ICategory Heavy { get; set;} 
		//Gender Gender { get; }
		//bool ApplyHandicap { get; }
		//string MastersCategory { get; } 
	}
}
