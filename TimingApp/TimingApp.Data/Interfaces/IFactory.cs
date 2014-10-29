using System;
using System.Collections.Generic;

namespace TimingApp.Data.Interfaces
{
	public interface IFactory<T>
	{
		IEnumerable<T> Create();
		void Update();
		event EventHandler ListUpdated;
	}
}

