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
		IList<IClub> _athleteClubs; 

		public ClubCreator(IEnumerable<IAthlete> athletes)
		{
			// TODO - behave more sensibly if the club index is empty, at the moment all blanks are the first one seen, e.g. Fredensborg 
			_athleteClubs = athletes.Select (a => a.RawClub).Distinct ().ToList();
		}

		#region implemented abstract members of BaseCreator

		protected override IList<IClub> InternalCreate ()
		{
			var overrides = RawOverrides.Select (o => new Club (o)).ToList ();
			foreach(var club in overrides)
			{
				IClub athleteClub = _athleteClubs.FirstOrDefault (b => b.Index == club.Index);
				if(athleteClub != null)
					club.SetName(athleteClub.Name);
			}
			var balance = _athleteClubs.Where (cl => overrides.All (ov => ov.Index != cl.Index));
			return overrides.Select(o => (IClub)o).Union (balance).ToList ();
		}

		#endregion
	}
}
	