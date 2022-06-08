using System;
using System.Device.Gpio;

namespace MyCastle
{
	public class Pin : IDisposable
	{
		private readonly IGpioController controller;
		private readonly bool autoClose;

		private int[] pins;

		public Pin(IGpioController controller, int pin, bool activeLow, bool autoClose = true)
		{
			this.controller = controller;
			PinNumber = pin;
			ActiveLow = activeLow;
			this.autoClose = autoClose;
			pins = GetPins(pin);
		}

		public int PinNumber { get; }
		public bool ActiveLow { get; }

		public bool IsOpen 
		{
			get
			{
				foreach (var pin in pins)
					if (!controller.IsPinOpen(pin))
						return false;
				return true;
			}
		}

		private static int[] GetPins(int pin)
		{
			int pinCount = (pin.ToString().Length - 1) / 2;
			pinCount++;

			var pins = new int[pinCount];
			int i = 0;
			while (pin > 0)
			{
				pins[i] = pin % 100;
				pin = pin / 100;
				i++;
			}
			return pins;
		}

		public void Open(PinMode mode)
		{
			foreach(var pin in pins)
				controller.OpenPin(pin, mode);
		}

		public void Write(PinValue value)
		{
			foreach (var pin in pins)
				controller.Write(pin, value);
		}

		public void SetActive(bool active)
		{
			if (active)
				Write(ActiveLow ? PinValue.Low : PinValue.High);
			else
				Write(ActiveLow ? PinValue.High : PinValue.Low);
		}

		public bool IsActive()
		{
			foreach (var pin in pins)
			{
				var value = controller.Read(pin);
				var active = ActiveLow ? (value == PinValue.Low) : (value == PinValue.High);
				if (!active)
					return false;
			}
			return true;
		}

		public void Dispose()
		{
			if (autoClose)
				controller.ClosePin(PinNumber);
		}

	}
}
