﻿using System.Device.Gpio;

namespace MyCastle
{
	public class RealGpioController : IGpioController
	{
		private GpioController c;

		public RealGpioController()
		{
			c = new GpioController(PinNumberingScheme.Board);
		}

		public void ClosePin(int pinNumber) => c.ClosePin(pinNumber);

		public bool IsPinOpen(int pinNumber) => c.IsPinOpen(pinNumber);

		public void OpenPin(int pinNumber, PinMode mode) => c.OpenPin(pinNumber, mode);

		public PinValue Read(int pinNumber) => c.Read(pinNumber);

		public void Write(int pinNumber, PinValue value) => c.Write(pinNumber, value);
	}
}
