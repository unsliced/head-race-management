using System;
using System.Collections.Generic;
using Head.Common.Interfaces.Utils;
using Head.Common.Csv;
using Common.Logging;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using System.Linq;
using Head.Common.Internal.Categories;
using Head.Common.Interfaces.Enums;
using System.Text;

namespace Head.Common.Generate
{
	public abstract class BaseRawCreator<T, TRaw, TOverride> : BaseCreator<T, TOverride>
	{
		string _rawPath;

		protected new bool Validate()
		{
			return !String.IsNullOrEmpty (_rawPath);
		}

		protected IList<TRaw> RawUnderlying { get { return new CsvImporter<TRaw> (_rawPath).Create (); } }

		public BaseRawCreator<T, TRaw, TOverride> SetRawPath(string rawPath)
		{
			_rawPath = rawPath;
			return this;
		}
	}
}
	