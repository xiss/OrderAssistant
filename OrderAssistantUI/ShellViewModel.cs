using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Windows;

namespace OrderAssistantUI {
	[Export(typeof(ShellViewModel))]
	public class ShellViewModel : PropertyChangedBase, IShell
	{
		private readonly IWindowManager _windowManager;

		[ImportingConstructor]
		public ShellViewModel(IWindowManager windowManager)
		{
			_windowManager = windowManager;
		}
		public void OpenWindow()
		{
			dynamic settings = new ExpandoObject();
			settings.WindowStartupLocation = WindowStartupLocation.Manual;
			//_windowManager.ShowDialog(new ProgressViewModel(_windowManager, "sdfhsdhfshdf","asd",12,23,"asdasdsd"));
			_windowManager.ShowDialog(IoC.Get<ProgressViewModel>());
		}

		public void Import()
		{
			OrderAssistantUI.Import.ImportOrderStocksAndTraffic(_windowManager);
		}
	}
}