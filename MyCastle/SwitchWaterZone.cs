using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyCastle
{
	public class SwitchWaterZone : Activity
	{
		protected override ActivityExecutionResult OnExecute(WorkflowExecutionContext context)
		{
			GpioController c = new GpioController(PinNumberingScheme.Board);
			using (var pin = new Pin(c, 10))
			{
				pin.Open(PinMode.Output);

				pin.Write(PinValue.High);
				Thread.Sleep(4000);

				pin.Write(PinValue.Low);
			}

			return Done();
		}
	}
}
