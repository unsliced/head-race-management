using System;
using System.Collections.Generic;
using System.IO;
using Common.Logging;
using Newtonsoft.Json;
using Head.Common.Interfaces.Utils;
using Newtonsoft.Json.Converters;

namespace Head.Common.Generate
{
    public abstract class JsonOverrideFactory
	{
		protected static readonly ILog Logger = LogManager.GetCurrentClassLogger();
        public static string JsonPath { get; set; }

    }

    public class JsonOverrideFactory<T> : JsonOverrideFactory, IFactory<IList<T>>
	{
		string _path; 

		public JsonOverrideFactory(string path)
		{
            if (!string.IsNullOrEmpty(JsonPath))
                path = JsonPath + path;

            _path = path;
		}

		public IList<T> Create()
		{
			if(String.IsNullOrEmpty(_path))
				Logger.ErrorFormat("Path is empty");

			bool exists = File.Exists(_path);
			if(!exists)
				throw new InvalidOperationException("Invalid path: " + _path);

			Logger.DebugFormat("File exists: {0}", _path);

			string json = File.ReadAllText(_path);

            Logger.DebugFormat("Read in {0} characters", json.Length);

			List<T> result = null;
			var dtc = new IsoDateTimeConverter 
			{ 
				DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.f" 
			};
			try
			{
				result = JsonConvert.DeserializeObject<List<T>>(json, dtc);
			} catch(Exception ex)
			{
				Logger.ErrorFormat ("Failed to parse {0} for types {1}: {2}", _path, typeof(T).GetType(), ex.Message); 
				throw;
			}

			return result;
		}
	}

}

