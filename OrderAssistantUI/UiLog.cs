using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using NLog;
using NLog.Targets;

namespace OrderAssistantUI
{
	[Target("UiLog")]
	public sealed class UiLog : TargetWithLayout
	{
		protected override void Write(LogEventInfo logEvent)
		{
			//TODO а что если вида еще нет?
			IoC.Get<ImportViewModel>().TextBoxLog = Layout.Render(logEvent);
		}
	}
}
