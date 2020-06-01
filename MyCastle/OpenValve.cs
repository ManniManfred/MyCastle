using Elsa;
using Elsa.Attributes;
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
	[ActivityDefinition(
		 Category = "MyCastle",
		 DisplayName = "Ventil öffnen",
		 Description = "Öffnet das angegebene Ventil/Wasserhan.",
		 Icon = "fas fa-shower",
		 Outcomes = new[] { OutcomeNames.Done }
	)]
	public class OpenValve : Activity
	{
		private readonly Settings settings;
		private readonly IGpioController gpio;

		public OpenValve(Settings settings, IGpioController gpio)
		{
			this.settings = settings;
			this.gpio = gpio;
		}

		[ActivityProperty(Label = "Ventil", Hint = @"Das zu öffnende Ventil: 
Rasen1 = 11,
Rasen2 = 12,
Hecke = 13,
Beet1 = 15,
Beet2 = 16")]
		public int Valve
		{
			get => GetState<int>();
			set => SetState(value);
		}

		[ActivityProperty(Label = "Zeitspanne", Hint = "Zeit, wie lange das Ventil offen sein soll")]
		public TimeSpan Duration
		{
			get => GetState<TimeSpan>();
			set => SetState(value);
		}

		protected override ActivityExecutionResult OnExecute(WorkflowExecutionContext context)
		{
			var valvePin = Valve;
			var valve = settings.GetArea(valvePin);
			if (valve == null)
				return Fault($"Das angegebene Ventil \"{valvePin}\" gibt es nicht. Möglich sind: "
					+ string.Join("\r\n", settings.GetAreas().Select(v => v.Name + ": " + v.Pin)));

			if (Duration.TotalSeconds < 1.0)
				return Fault("Es muss ein Zeitspanne angegeben werden");

			using (var pin = new Pin(gpio, valvePin, settings.BoardActiveLow))
			{
				pin.Open(PinMode.Output);
				pin.SetActive(true);
				Thread.Sleep(Duration);
				pin.SetActive(false);
			}

			return Done();
		}
	}
}
