using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyCastle.Entities
{
	public class Programs
	{
		private List<Program> programs;

		private static JsonSerializerOptions GetOptions()
		{
			return new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = true
			};
		}

		private void AssertPrograms()
		{
			if (programs != null)
				return;

			string path = GetPath();
			if (File.Exists(path))
				programs = JsonSerializer.Deserialize<List<Program>>(File.ReadAllText(path), GetOptions());
			else
				programs = new List<Program>();
		}

	internal void SetPrograms(List<Program> data)
	{
			programs = data;
	}

	private static string GetPath()
		{
			var dir = Path.GetDirectoryName(typeof(Programs).Assembly.Location);
			var path = Path.Combine(dir, "programs.json");
			return path;
		}

		public void Save()
		{
			var json = JsonSerializer.Serialize(programs, GetOptions());
			File.WriteAllText(GetPath(), json);
		}

		public IEnumerable<Program> GetPrograms()
		{
			AssertPrograms();
			return programs;
		}

		public void AddProgram(Program program)
		{
			AssertPrograms();
			programs.Add(program);
		}

		public void RemoveProgram(Program program)
		{
			AssertPrograms();
			programs.Remove(program);
		}

		public void UpdateProgram(Program program)
		{

		}
	}
}
