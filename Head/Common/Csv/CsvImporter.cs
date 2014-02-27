using System;
using Head.Common.Interfaces.Utils;
using System.Collections.Generic;
using System.IO;
// using Common.Logging;
using FileHelpers;

namespace Head.Common.Csv
{
	public abstract class CsvImporter {
		// protected static readonly ILog Logger = LogManager.GetLogger(typeof(CsvImporter));
	}

	public class CsvImporter<T> : CsvImporter, IFactory<IList<T>>
	{
		readonly string _path;

		public CsvImporter(string path)
		{
			if(!File.Exists(path))
				throw new InvalidOperationException(String.Format("The file {0} does not exist", path));

			Console.WriteLine("File exists: {0}", path);

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
				Console.WriteLine("Problem reading objects: {0}", ex.Message); // Logger.ErrorFormat
				throw;
			}
		}

		#endregion

	}
}

