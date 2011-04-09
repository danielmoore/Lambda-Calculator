using System.Windows.Controls;
using System.Windows.Input;
using NorthHorizon.LambdaCalculator.UI.Model;

namespace NorthHorizon.LambdaCalculator.UI.Modules.Calculator
{
	/// <summary>
	/// Interaction logic for CalculatorView.xaml
	/// </summary>
	public partial class CalculatorView : UserControl
	{
		private readonly CalculatorViewModel _viewModel;

		public CalculatorView()
		{
			InitializeComponent();
		}

		public CalculatorView(CalculatorViewModel viewModel)
		{
			DataContext = _viewModel = viewModel;
			viewModel.RegisterKeyBindings(InputBindings);

			InitializeComponent();
		}

		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			base.OnMouseDoubleClick(e);
		}

		private void OnInputCharacterExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			_viewModel.InputCharacter(e.Parameter as Character);
		}

		private void ExtendedTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			_viewModel.OnScriptChanged();
		}
	}
}
