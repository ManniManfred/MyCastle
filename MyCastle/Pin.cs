using System;
using System.Device.Gpio;

namespace MyCastle
{
  public class Pin : IDisposable
  {
		public Pin(GpioController controller, int pin)
		{
			Controller = controller;
			PinNumber = pin;
		}

		public GpioController Controller { get; }
		public int PinNumber { get; }

		public void Open(PinMode mode) => Controller.OpenPin(PinNumber, mode);

		public void Write(PinValue value) => Controller.Write(PinNumber, value);

		public void Dispose()
		{
			Controller.ClosePin(PinNumber);
		}
	}
}
