using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace NorthHorizon.LambdaCalculator.UI.Design.Controls
{
	public class ResizableControl : ContentControl
	{
		#region Constructors

		static ResizableControl()
		{
			var self = typeof(ResizableControl);

			DefaultStyleKeyProperty.OverrideMetadata(self, new FrameworkPropertyMetadata(self));

			EventManager.RegisterClassHandler(self, Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnThumbDragDelta));
		}

		#endregion

		#region Dependency Properties

		#region GripHeight

		public static readonly DependencyProperty GripHeightProperty =
			DependencyProperty.Register("GripHeight", typeof(double), typeof(ResizableControl));
		public double GripHeight
		{
			get { return (double)GetValue(GripHeightProperty); }
			set { SetValue(GripHeightProperty, value); }
		}

		#endregion

		#endregion

		#region Event Handlers

		private static void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
		{
			((ResizableControl)sender).OnResize(e);
		}

		protected virtual void OnResize(DragDeltaEventArgs e)
		{
			var newHeight = ActualHeight + e.VerticalChange;

			Height = Math.Max(newHeight, 0);
		}

		#endregion
	}
}
