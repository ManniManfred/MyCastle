using Elsa;
using Elsa.Attributes;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyCastle
{
	[ActivityDefinition(
		 Category = "Console",
		 DisplayName = "Write Line",
		 Description = "Write text to standard out.",
		 Icon = "fas fa-terminal",
		 Outcomes = new[] { OutcomeNames.Done }
	)]
	public class LogMessage : Activity
	{
		private readonly TextWriter writer;

		public LogMessage(TextWriter writer)
		{
			this.writer = writer;
		}

		[ActivityProperty(Hint = "The message to write.")]
		public string Message
		{
			get => GetState<string>();
			set => SetState(value);
		}

		protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
		{
			await writer.WriteLineAsync(Message);

			return Done();
		}

	}
}
