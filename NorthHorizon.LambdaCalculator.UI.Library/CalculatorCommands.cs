using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace NorthHorizon.LambdaCalculator.UI
{
	public static class CalculatorCommands
	{
		public static readonly RoutedCommand InputCharacter =
			new RoutedUICommand("Input Character", "InputCharacter", typeof(CalculatorCommands));
	}
}
