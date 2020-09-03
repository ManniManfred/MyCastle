using System.Collections.Generic;

namespace MyCastle.Entities
{
	public class Program
	{
		public Program()
		{
		}

		public bool Active { get; set; }
		public string Name { get; set; }
		public string Cron { get; set; }
		public List<MyTask> Tasks { get; set; }
	}
}