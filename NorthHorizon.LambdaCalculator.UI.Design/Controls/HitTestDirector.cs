using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NorthHorizon.LambdaCalculator.UI.Design.Controls
{
	public class HitTestDirector : ContentControl
	{
		public static readonly DependencyProperty TargetElementProperty =
			DependencyProperty.Register("TargetElement", typeof(UIElement), typeof(HitTestDirector));
		public UIElement TargetElement
		{
			get { return (UIElement)GetValue(TargetElementProperty); }
			set { SetValue(TargetElementProperty, value); }
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return VisualTreeHelper.HitTest(TargetElement, Mouse.GetPosition(TargetElement));
		}

		protected override void OnInitialized(EventArgs e)
		{
			var hitTestable = Content as UIElement;
			if (hitTestable != null)
				hitTestable.IsHitTestVisible = false;

			base.OnInitialized(e);
		}
	}
}
