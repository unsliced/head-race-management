using System;
using System.Collections.Generic;
using Head.Common.Csv;
using System.IO;

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

        public bool RawPresent()
        {
            return File.Exists(_rawPath);
        }

		public BaseRawCreator<T, TRaw, TOverride> SetRawPath(string rawPath)
		{
			_rawPath = rawPath;
			return this;
		}
	}

}
	