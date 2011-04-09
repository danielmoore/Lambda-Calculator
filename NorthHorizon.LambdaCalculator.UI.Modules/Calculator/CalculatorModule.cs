using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Regions;
using Microsoft.Practices.Unity;

namespace NorthHorizon.LambdaCalculator.UI.Modules.Calculator
{
	[ModuleDependency("UIModule")]
	public class CalculatorModule : IModule
	{
		private IUnityContainer _unityContainer;
		private IRegionManager _regionManager;

		public CalculatorModule(IUnityContainer unityContainer, IRegionManager regionManager)
		{
			_unityContainer = unityContainer;
			_regionManager = regionManager;
		}

		#region IModule Members

		public void Initialize()
		{
			_regionManager.RegisterViewWithRegion(Regions.CalculatorRegion, typeof(CalculatorView));
		}

		#endregion
	}
}
