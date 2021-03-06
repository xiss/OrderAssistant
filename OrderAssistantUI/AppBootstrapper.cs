
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;

namespace OrderAssistantUI
{


	public class AppBootstrapper : BootstrapperBase
	{
		SimpleContainer container;

		public AppBootstrapper()
		{
			Initialize();
		}

		protected override void Configure()
		{
			container = new SimpleContainer();

			container.Singleton<IWindowManager, WindowManager>();
			container.Singleton<IEventAggregator, EventAggregator>();
			container.PerRequest<IImport, ImportViewModel>();
			container.Singleton<ImportViewModel>();
			container.Singleton<OrderAssistantEntities>();
		}

		protected override object GetInstance(Type service, string key)
		{
			return container.GetInstance(service, key);
		}

		protected override IEnumerable<object> GetAllInstances(Type service)
		{
			return container.GetAllInstances(service);
		}

		protected override void BuildUp(object instance)
		{
			container.BuildUp(instance);
		}

		protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
		{
			DisplayRootViewFor<ImportViewModel>();
		}
	}
}