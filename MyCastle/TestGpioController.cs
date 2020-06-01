using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading.Tasks;

namespace MyCastle
{
  public class TestGpioController : IGpioController
	{
		private Dictionary<int, PinValue> values = new Dictionary<int, PinValue>();

		public void ClosePin(int pinNumber)
		{
			values.Remove(pinNumber);
		}

		public bool IsPinOpen(int pinNumber)
		{
			return values.ContainsKey(pinNumber);
		}

		public void OpenPin(int pinNumber, PinMode mode)
		{
			values[pinNumber] = PinValue.Low;
		}

		public PinValue Read(int pinNumber)
		{
			return values[pinNumber];
		}

		public void Write(int pinNumber, PinValue value)
		{
			values[pinNumber] = value;
		}
	}
}
