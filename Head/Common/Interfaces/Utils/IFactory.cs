using System;

namespace Head.Common.Interfaces.Utils
{
	public interface IFactory<T>
	{
		T Create();
	}
}

