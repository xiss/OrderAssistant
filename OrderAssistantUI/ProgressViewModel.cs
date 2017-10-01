using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Windows;

namespace OrderAssistantUI {
	[Export(typeof(ProgressViewModel))]
	public class ProgressViewModel :PropertyChangedBase, IShell
	{
		private readonly IWindowManager _windowManager;

		[ImportingConstructor]
		public ProgressViewModel(IWindowManager windowManager)
		{
			_windowManager = windowManager;
		}
	}
}