using Elsa;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using Newtonsoft.Json.Linq;
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
		public WorkflowExpression<string> Message
		{
			get
			{
				var defaultValue = new LiteralExpression("");
				var item = State.GetValue("Message", StringComparison.OrdinalIgnoreCase);
				if (item == null || item.Type == JTokenType.Null)
					return defaultValue != null ? defaultValue : default;

				if (item.Type == JTokenType.String)
					return new LiteralExpression(item.ToObject<string>());

				return item.ToObject<WorkflowExpression<string>>();
			}
			set => SetState(value);
		}

		protected override ActivityExecutionResult OnExecute(WorkflowExecutionContext context)
		{
			writer.WriteLine(Message);

			return base.OnExecute(context);
		}

		//protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
		//{
		//	await writer.WriteLineAsync(Message);
		//
		//	return Done();
		//}

	}
}
