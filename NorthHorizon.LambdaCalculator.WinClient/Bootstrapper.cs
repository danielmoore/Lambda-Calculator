using System.Windows;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.UnityExtensions;
using NorthHorizon.LambdaCalculator.UI.Modules.Calculator;
using NorthHorizon.LambdaCalculator.UI;

namespace NorthHorizon.LambdaCalculator.WinClient
{
	public class Bootstrapper : UnityBootstrapper
	{
		protected override DependencyObject CreateShell()
		{
			var shell = Container.Resolve<Shell>();

			shell.Show();

			return shell;
		}

		protected override IModuleCatalog GetModuleCatalog()
		{
			return new ModuleCatalog()
				.AddModule(typeof(UIModule))
				.AddModule(typeof(CalculatorModule));
		}
	}
}
