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

		[ActivityProperty(Label = "Ventil", Hint = @"Das zu öffnende Ventil: 
Rasen1 = 11,
Rasen2 = 12,
Hecke = 13,
Beet1 = 15,
Beet2 = 16")]
		public ValveEnum Valve
		{
			get => GetState<ValveEnum>();
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
			var valve = Valve;
			if (!Enum.IsDefined(typeof(ValveEnum), valve))
				return Fault($"Das angegebene Ventil \"{valve}\" gibt es nicht. Möglich sind: "
					+ string.Join("\r\n", Enum.GetNames(typeof(ValveEnum)).Select(n => n + ": " + (int)Enum.Parse(typeof(ValveEnum), n))));

			if (Duration.TotalSeconds < 1.0)
				return Fault("Es muss ein Zeitspanne angegeben werden");

			GpioController c = new GpioController(PinNumberingScheme.Board);
			using (var pin = new Pin(c, (int)valve))
			{
				pin.Open(PinMode.Output);

				pin.Write(PinValue.High);
				Thread.Sleep(Duration);

				pin.Write(PinValue.Low);
			}

			return Done();
		}
	}
}
