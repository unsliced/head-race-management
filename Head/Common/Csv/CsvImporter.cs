using System;
using Head.Common.Interfaces.Utils;
using System.Collections.Generic;
using System.IO;
using FileHelpers;
using Common.Logging;

namespace Head.Common.Csv
{
	public abstract class CsvImporter {
        protected static readonly ILog Logger = LogManager.GetCurrentClassLogger();
        public static string CsvPath { get; set; }
    }

    public class CsvImporter<T> : CsvImporter, IFactory<IList<T>>
	{
		readonly string _path;

		public CsvImporter(string path)
		{
            if (!string.IsNullOrEmpty(CsvPath))
                path = CsvPath + path;

			if(!File.Exists(path))
				throw new InvalidOperationException(String.Format("The file {0} does not exist", path));

            Logger.DebugFormat("File exists: {0}", path);

			_path = path;
		}

		#region IFactory implementation

		public IList<T> Create ()
		{
			try
			{
				FileHelperEngine engine = new FileHelperEngine(typeof(T)); 
				T[] res = engine.ReadFile(_path) as T[];
				return new List<T>(res);
			} catch(Exception ex)
			{
                Logger.ErrorFormat("Problem in {1} reading objects: {0}", ex.Message, _path); 
				throw;
			}
		}

		#endregion

	}
}

