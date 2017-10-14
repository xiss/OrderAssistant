using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Windows;

namespace OrderAssistantUI
{
	[Export(typeof(ProgressViewModel))]
	public class ProgressViewModel : PropertyChangedBase, IShell
	{

		private readonly IWindowManager _windowManager;
		[ImportingConstructor]
		public ProgressViewModel(IWindowManager windowManager, int progressBar1Maximum, int progressBar2Maximum, string windowTitle)
		{
			_windowManager = windowManager;
			//_labelProgress1 = labelProgress1;
			//_labelProgress2 = labelProgress2;
			_progressBar1Maximum = progressBar1Maximum;
			_progressBar2Maximum = progressBar2Maximum;
			_windowTitle = windowTitle;
		}

		private string _labelProgress1;
		public string LabelProgress1
		{
			get => _labelProgress1;
			set { _labelProgress1 = value; NotifyOfPropertyChange(() => LabelProgress1); }
		}

		private string _labelProgress2;
		public string LabelProgress2
		{
			get => _labelProgress2;
			set { _labelProgress2 = value; NotifyOfPropertyChange(() => LabelProgress2); }
		}

		private int _progressBar1Maximum;
		public int ProgressBar1Maximum
		{
			get => _progressBar1Maximum;
			set { _progressBar1Maximum = value; NotifyOfPropertyChange(() => ProgressBar1Maximum); }
		}

		private int _progressBar2Maximum;
		public int ProgressBar2Maximum
		{
			get => _progressBar2Maximum;
			set { _progressBar2Maximum = value; NotifyOfPropertyChange(() => ProgressBar2Maximum); }
		}

		private int _progressBar1Value;
		public int ProgressBar1Value
		{
			get => _progressBar1Value;
			set { _progressBar1Value = value; NotifyOfPropertyChange(() => ProgressBar1Value); }
		}

		private int _progressBar2Value;
		public int ProgressBar2Value
		{
			get => _progressBar2Value;
			set { _progressBar2Value = value; NotifyOfPropertyChange(() => ProgressBar2Value); }
		}

		private string _windowTitle;
		public string WindowTitle
		{
			get => _windowTitle;
			set { _windowTitle = value; NotifyOfPropertyChange(() => WindowTitle); }
		}

		
	}
}