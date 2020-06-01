using System;
using System.Device.Gpio;

namespace MyCastle
{
  public class Pin : IDisposable
	{
		private readonly IGpioController controller;
		private readonly bool autoClose;

		public Pin(IGpioController controller, int pin, bool activeLow, bool autoClose = true)
		{
			this.controller = controller;
			PinNumber = pin;
			ActiveLow = activeLow;
			this.autoClose = autoClose;
		}

		public int PinNumber { get; }
		public bool ActiveLow { get; }

		public bool IsOpen => controller.IsPinOpen(PinNumber);

		public void Open(PinMode mode) => controller.OpenPin(PinNumber, mode);

		public void Write(PinValue value) => controller.Write(PinNumber, value);

		public void SetActive(bool active)
		{
			if (active)
				Write(ActiveLow ? PinValue.Low : PinValue.High);
			else
				Write(ActiveLow ? PinValue.High : PinValue.Low);
		}

		public bool IsActive()
		{
			var value = controller.Read(PinNumber);
			return ActiveLow ? (value == PinValue.Low) : (value == PinValue.High);
		}

		public void Dispose()
		{
			if (autoClose)
				controller.ClosePin(PinNumber);
		}

	}
}
