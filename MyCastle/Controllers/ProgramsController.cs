using Microsoft.AspNetCore.Mvc;
using MyCastle.Entities;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace MyCastle.Controllers
{
	[Route("api/program")]
	public class ProgramsController : Controller
	{
		private readonly Programs programs;

		public ProgramsController(Programs programs)
		{
			this.programs = programs;
		}

		[HttpGet]
		public IEnumerable<Entities.Program> Get()
		{
			return programs.GetPrograms();
		}

		[HttpPost]
		public bool Post([FromBody] List<Entities.Program> data)
		{
			programs.SetPrograms(data);
			programs.Save();
			return true;
		}

		[HttpPut("{name}")]
		public void SwitchRunning(string name, [FromBody] bool to)
		{
			programs.SwitchRunning(name, to);
		}
	}
}
