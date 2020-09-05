using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MyCastle.Entities
{
	public class Programs
	{
		private object lockObj = new object();

		private List<Program> programs;
		private readonly Settings settings;
		private readonly IGpioController gpio;

		public Programs(Settings settings, IGpioController gpio)
		{
			this.settings = settings;
			this.gpio = gpio;
		}

		private static JsonSerializerOptions GetOptions()
		{
			return new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = true
			};
		}

		internal void SwitchRunning(string programName, bool to)
		{
			lock (lockObj)
			{
				foreach (var p in programs)
				{
					if (p.Name == programName)
					{
						if (p.IsRunning != to)
						{
							if (p.IsRunning)
								p.Schedule?.Stop();

							if (to)
								ScheduleProgram(p, true);
						}
						break;
					}
				}
			}
		}

		internal void AssertPrograms()
		{
			if (programs != null)
				return;

			string path = GetPath();
			if (File.Exists(path))
				SetPrograms(JsonSerializer.Deserialize<List<Program>>(File.ReadAllText(path), GetOptions()));
			else
				SetPrograms(new List<Program>());
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

		internal void SetPrograms(List<Program> data)
		{
			lock (lockObj)
			{
				if (programs != null)
				{
					foreach (var p in programs)
					{
						p.Schedule?.Stop();
					}
				}

				programs = data;

				if (programs != null)
				{
					foreach (var p in GetPrograms())
					{
						if (p.Active)
						{
							ScheduleProgram(p);
						}
					}
				}
			}
		}

		private void ScheduleProgram(Program p, bool testRun = false)
		{
			Schedule schedule;
			if (!testRun)
				schedule = new Schedule(cancelToken => ExecuteProgram(cancelToken, p), p.Cron);
			else
				schedule = new Schedule(cancelToken => ExecuteProgram(cancelToken, p), s => s.OnceIn(TimeSpan.FromMilliseconds(0)));

			schedule.Start();

			p.Schedule = schedule;
		}

		private Task ExecuteProgram(CancellationToken cancelToken, Program p)
		{
			foreach (var t in p.Tasks)
			{
				if (cancelToken.IsCancellationRequested)
					return Task.CompletedTask;

				using (var pin = new Pin(gpio, t.Area, settings.BoardActiveLow))
				{
					pin.Open(PinMode.Output);
					pin.SetActive(true);

					cancelToken.WaitHandle.WaitOne(t.Duration);
				}
			}
			return Task.CompletedTask;
		}

	}
}
