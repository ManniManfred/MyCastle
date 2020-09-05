using FluentScheduler;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Text.Json.Serialization;
using System.Threading;

namespace MyCastle.Entities
{
	public class Program
	{

		public Program()
		{
		}

		[JsonIgnore]
		internal Schedule Schedule { get; set; }

		[JsonIgnore]
		public bool Running => Schedule?.Running ?? false;

		public bool JobRunning => Schedule?.JobRunning ?? false;

		public bool Active { get; set; }
		public string Name { get; set; }
		public string Cron { get; set; }
		public List<MyTask> Tasks { get; set; }

	}
}