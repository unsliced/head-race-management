using System.Collections.Generic;
using Head.Common.Domain;
using Head.Common.BritishRowing;
using Head.Common.Internal.Overrides;
using System.Linq;
using Head.Common.Internal.JsonObjects;

namespace Head.Common.Generate
{

    public class AthleteCreator : BaseRawCreator<IAthlete, RawCompetitor, AthleteOverride>
	{
		#region implemented abstract members of BaseCreator

		public AthleteCreator()
		{
		}

		protected override IList<IAthlete> InternalCreate ()
		{
			var scratched = RawOverrides.Where (o => !RawUnderlying.Select (u => u.CrewId).Contains (o.CrewId)).Select (o => new Athlete (null, o));

			return RawUnderlying.Select (a => new Athlete (a, RawOverrides.FirstOrDefault(ov => ov.CrewId == a.CrewId && ov.Position == a.Position)) as IAthlete).Union(scratched).ToList ();
		}

		#endregion
	}
}
	