using System;
using System.Text.Json.Serialization;

namespace MyCastle.Entities
{
	public class MyTask
  {
		public int Area { get; set; }

		[JsonConverter(typeof(JsonTimeSpanConverter))]
		public TimeSpan Duration { get; set; }
  }
}