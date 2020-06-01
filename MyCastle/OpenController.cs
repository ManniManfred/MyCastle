using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Org.BouncyCastle.Security;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyCastle
{
	[Route("api/open")]
	public class OpenController : Controller
	{
		private readonly Settings settings;
		private readonly IGpioController gpio;

		public OpenController(Settings settings, IGpioController gpio)
		{
			this.settings = settings;
			this.gpio = gpio;
		}

		[HttpGet]
		public IEnumerable<string> Get()
		{
			var result = new List<string>();

			foreach (var area in settings.GetAreas())
			{
				var pin = new Pin(gpio, area.Pin, settings.BoardActiveLow);
				if (pin.IsOpen && pin.IsActive())
					result.Add(area.Name);
			}

			return result;
		}

		[HttpPut("{id}")]
		public void Put(int id)
		{
			var area = settings.GetArea(id);
			if (area == null)
				throw new ArgumentException($"Pin {id} is not found.");

			using (var pin = new Pin(gpio, id, settings.BoardActiveLow, false))
			{
				pin.Open(PinMode.Output);
				pin.SetActive(true);
			}
		}


		[HttpDelete("{id}")]
		public void Delete(int id)
		{
			var area = settings.GetArea(id);
			if (area == null)
				throw new ArgumentException($"Pin {id} is not found.");

			using (var pin = new Pin(gpio, id, settings.BoardActiveLow, true))
			{
				pin.Open(PinMode.Output);
				pin.SetActive(false);
			}
		}


	}
}
