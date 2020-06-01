using System;
using System.Device.Gpio;

namespace MyCastle
{
  public class Pin : IDisposable
  {
		public Pin(GpioController controller, int pin, bool activeLow = false)
		{
			Controller = controller;
			PinNumber = pin;
			ActiveLow = activeLow;
		}

		public GpioController Controller { get; }
		public int PinNumber { get; }
		public bool ActiveLow { get; }

		public void Open(PinMode mode) => Controller.OpenPin(PinNumber, mode);

		public void Write(PinValue value) => Controller.Write(PinNumber, value);

		public void SetActive(bool active)
		{
			if (active)
				Write(ActiveLow ? PinValue.Low : PinValue.High);
			else
				Write(ActiveLow ? PinValue.High : PinValue.Low);
		}

		public void Dispose()
		{
			Controller.ClosePin(PinNumber);
		}
	}
}
