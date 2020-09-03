using System.Collections.Generic;

namespace MyCastle.Entities
{
	internal class SettingsData
	{
		public bool BoardActiveLow { get; set; }
		public IList<Area> Areas { get; set; }
	}
}
