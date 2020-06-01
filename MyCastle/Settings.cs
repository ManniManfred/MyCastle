
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace MyCastle
{
	public class Settings
	{
		private readonly IWebHostEnvironment env;

		private SettingsData data;
		private Dictionary<int, Area> pinToArea;


		public Settings(IWebHostEnvironment env)
		{
			this.env = env;

			LoadValves();
		}

		public bool BoardActiveLow => data.BoardActiveLow;

		private void LoadValves()
		{
			var jsonString = File.ReadAllText(Path.Combine(env.WebRootPath, "settings.json"));
			data = JsonSerializer.Deserialize<SettingsData>(jsonString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
			pinToArea = data.Areas.ToDictionary(v => v.Pin);
		}

		public IReadOnlyCollection<Area> GetAreas() => new ReadOnlyCollection<Area>(data.Areas);

		public Area GetValve(int pin)
		{
			pinToArea.TryGetValue(pin, out var result);
			return result;
		}
	}
}