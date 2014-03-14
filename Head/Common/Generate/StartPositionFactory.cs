using System;
using System.Collections.Generic;
using System.IO;
using Common.Logging;
using Newtonsoft.Json;
using Head.Common.Domain;
using Head.Common.Interfaces.Utils;
using Head.Common.Internal.Overrides;
using System.Linq;

namespace Head.Common.Generate
{
	public class StartPositionFactory : IFactory<IList<IStartPosition>>
	{
		readonly string _path;

		public StartPositionFactory(string path)
		{
			_path = path;
		}

		public IList<IStartPosition> Create()
		{
			var positions = new JsonOverrideFactory<StartPosition> (_path).Create ().Select (i => (IStartPosition)i).ToList ();
			return positions.Count == 0 ? null : positions;
		}
	}
}
