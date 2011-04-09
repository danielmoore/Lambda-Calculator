using System;
using System.Windows;
using System.Windows.Controls;

namespace NorthHorizon.LambdaCalculator.UI.Design.Controls
{
	public class ExtendedTextBox : TextBox
	{
		private bool _suppressPropertyChangeEvents;

		public static readonly DependencyProperty SelectionStartProperty =
			DependencyProperty.Register("SelectionStart", typeof(int), typeof(ExtendedTextBox),
			new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectionStartChanged));
		public new int SelectionStart
		{
			get { return (int)GetValue(SelectionStartProperty); }
			set { SetValue(SelectionStartProperty, value); }
		}

		private static void OnSelectionStartChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			((ExtendedTextBox)sender).OnSelectionStartChanged(e);
		}

		private void OnSelectionStartChanged(DependencyPropertyChangedEventArgs e)
		{
			if (SuppressPropertyChangedEvents())
				try
				{
					base.SelectionStart = (int)e.NewValue;
				}
				finally
				{
					ResumePropertyChangedEvents();
				}
		}

		protected override void OnSelectionChanged(RoutedEventArgs e)
		{
			if (SuppressPropertyChangedEvents())
				try
				{
					SelectionStart = base.SelectionStart;
				}
				finally
				{
					ResumePropertyChangedEvents();
				}
		}

		private bool SuppressPropertyChangedEvents()
		{
			return _suppressPropertyChangeEvents ? false : _suppressPropertyChangeEvents = true;
		}

		private void ResumePropertyChangedEvents()
		{
			if (!_suppressPropertyChangeEvents)
				throw new InvalidOperationException();

			_suppressPropertyChangeEvents = false;
		}
	}
}
