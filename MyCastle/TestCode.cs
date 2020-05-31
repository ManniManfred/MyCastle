using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyCastle;

namespace MyCastle
{
	public class TestCode
	{
		private void Start()
		{

			GpioController c = new GpioController(PinNumberingScheme.Board);
			using (var pin = new Pin(c, 16))
			{
				pin.Open(PinMode.Output);

				pin.Write(PinValue.High);
				Thread.Sleep(30);

				pin.Write(PinValue.Low);
			}
		}
	}
}
