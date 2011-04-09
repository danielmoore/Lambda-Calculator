using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NorthHorizon.LambdaCalculator.UI.Extensions
{
	public static class ObjectExtensions
	{
		public static bool As<T>(this object obj, Action<T> action) where T : class
		{
			var target = obj as T;
			if (target == null)
				return false;

			action(target);
			return true;
		}

		public static TResult As<T, TResult>(this object obj, Func<T, TResult> func) where T : class
		{
			var target = obj as T;
			if (target == null)
				return default(TResult);

			return func(target);
		}

		public static bool As<T, TResult>(this object obj, Func<T, TResult> func, out TResult result) where T : class
		{
			var target = obj as T;
			if (target == null)
			{
				result = default(TResult);
				return false;
			}

			result = func(target);
			return true;
		}
	}
}
