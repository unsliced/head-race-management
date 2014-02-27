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
using Head.Common.Internal.JsonObjects;

namespace Head.Common.Generate
{
	public abstract class BaseCreator 
	{
		protected static readonly ILog Logger = LogManager.GetCurrentClassLogger();
	}

	public abstract class BaseCreator<T, TOverride> : BaseCreator, IFactory<IList<T>>
	{
		Lazy<IList<TOverride>> _lazyOverrides = new Lazy<IList<TOverride>>(() => new List<TOverride>());

		protected abstract IList<T> InternalCreate ();
		protected IList<TOverride> RawOverrides { get { return _lazyOverrides.Value; } } 

		protected virtual bool Validate()
		{
			return true;
		}

		public IList<T> Create()
		{
			if (!Validate ())
				throw new InvalidOperationException (String.Format("Cannot validate the objects in the {0} list creator.", typeof(T).ToString ()));
			return InternalCreate ();
		}

		public BaseCreator<T, TOverride> SetOverrideFactory(string factoryPath)
		{
			var overrideFactory = new JsonOverrideFactory<TOverride>(factoryPath);
			_lazyOverrides = new Lazy<IList<TOverride>> (overrideFactory.Create);
			return this;
		}
	}

	public class ClubCreator : BaseCreator<IClub, ClubDetails> 
	{
		public ClubCreator()
		{
		}

		#region implemented abstract members of BaseCreator

		protected override IList<IClub> InternalCreate ()
		{
			return RawOverrides.Select (o => new Club (o) as IClub).ToList ();
		}

		#endregion
	}
}
	