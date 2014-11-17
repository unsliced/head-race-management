using System;
using System.Collections.Generic;

namespace TimingApp.Data.Interfaces
{
	public interface IFactory<T>
	{
		T Create();
	}
}

