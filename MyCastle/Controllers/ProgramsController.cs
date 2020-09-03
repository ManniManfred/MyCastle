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
		public void Post([FromBody] List<Entities.Program> data)
		{
			programs.SetPrograms(data);
			programs.Save();
		}

	}
}
