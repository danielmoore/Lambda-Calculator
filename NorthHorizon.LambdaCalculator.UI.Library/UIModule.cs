using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Unity;
using NorthHorizon.LambdaCalculator.UI.Model;

namespace NorthHorizon.LambdaCalculator.UI
{
	public class UIModule : IModule
	{
		private readonly IUnityContainer _unityContainer;

		public UIModule(IUnityContainer unityContainer)
		{
			_unityContainer = unityContainer;
		}

		#region IModule Members

		public void Initialize()
		{
			_unityContainer.RegisterInstance(Configuration.CharacterSetConfigurationSection.Get());
			_unityContainer.RegisterType<CharacterSet>(new ContainerControlledLifetimeManager());
		}

		#endregion
	}
}
