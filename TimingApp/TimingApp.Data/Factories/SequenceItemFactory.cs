using System;
using TimingApp.Data.Interfaces;
using TimingApp.Data.Internal.Model;
using System.Collections.Generic;

namespace TimingApp.Data.Factories
{

	public class SequenceItemFactory : IFactory<ISequenceItem>
	{
		IBoat _boat;
		DateTime _timestamp; 
		string _notes;

		public SequenceItemFactory SetBoat(IBoat boat)
		{
			_boat = boat;
			return this; 
		}

		public SequenceItemFactory SetTime(DateTime time)
		{
			_timestamp = time;
			return this; 
		}

		public SequenceItemFactory SetNotes(string notes)
		{
			_notes = notes;
			return this;
		}

		public ISequenceItem Create() 
		{

			return new SequenceItem(_boat, _timestamp, _notes);
		}
	}
}
