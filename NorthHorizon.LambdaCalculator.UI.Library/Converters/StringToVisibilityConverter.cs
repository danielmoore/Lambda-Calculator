using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using NorthHorizon.LambdaCalculator.UI.Extensions;
using System.Windows;

namespace NorthHorizon.LambdaCalculator.UI.Converters
{
	public class StringToVisibilityConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool isNullOrEmpty;
			if (value.As<string, bool>(string.IsNullOrEmpty, out isNullOrEmpty))
				return isNullOrEmpty ? Visibility.Collapsed : Visibility.Visible;

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
