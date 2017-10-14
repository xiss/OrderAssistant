using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Windows;

namespace OrderAssistantUI
{
	class ShellWindowManager : WindowManager

	{
		protected override Window EnsureWindow(object model, object view, bool isDialog)
		{
			Window window = base.EnsureWindow(model, view, isDialog);

			window.SizeToContent = SizeToContent.Manual;
			window.Width = 300;
			window.Height = 300;

			return window;
		}


		static void Method()
		{
			ICalculator calc = new Summer();
			calc.Operate(5, 6);
			calc = new Substractor();
			calc.Operate(7, 6);
		}
	}


	interface ICalculator
	{
		int Operate(int a, int b);
	}

	class Summer : ICalculator
	{
		private int _field = 5;

		public int Operate(int a, int b)
		{
			return 2* (a + b);
		}

		public Summer()
		{
			
		}

		public void MyMethod()
		{
			
		}
	}

	class Substractor : ICalculator
	{
		public int Operate(int a, int b)
		{
			return a - b;
		}
	}
}
