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

	public class SequenceItemFactory : IFactory<IList<ISequenceItem>>
	{
		readonly string _path;

		public SequenceItemFactory(string path)
		{
			_path = path;
		}

		public IList<ISequenceItem> Create()
		{
			var items = new JsonOverrideFactory<SequenceItem> (_path).Create ().Select (i => (ISequenceItem)i).ToList ();
			return items.Count == 0 ? null : items;
		}
	}

	public class PenaltyFactory : IFactory<IList<IPenalty>>
	{
		readonly string _path;

		public PenaltyFactory(string path)
		{
			_path = path;
		}

		public IList<IPenalty> Create()
		{
			var items = new JsonOverrideFactory<Penalty> (_path).Create ().Select (i => (IPenalty)i).ToList ();
			return items.Count == 0 ? null : items;
		}
	}

	public class AdjustmentFactory : IFactory<IList<IAdjustment>>
	{
		readonly string _path;

		public AdjustmentFactory(string path)
		{
			_path = path;
		}

		public IList<IAdjustment> Create()
		{
			var items = new JsonOverrideFactory<CategoryAdjustment> (_path).Create ().Select (i => (IAdjustment)i).ToList ();
			return items.Count == 0 ? null : items;
		}
	}
}
