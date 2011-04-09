using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace NorthHorizon.LambdaCalculator.UI.Core
{
	public abstract class BindableBase : INotifyPropertyChanged
	{
		private bool _isPropertyChangedSuppressed;
		private bool _hasPropertyChangedWhileSuppressed;

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		protected bool IsPropertyChangedSuppressed
		{
			get { return _isPropertyChangedSuppressed; }
		}

		protected bool SetProperty<T>(ref T backingStore, T value, string propertyName)
		{
			if (EqualityComparer<T>.Default.Equals(backingStore, value))
				return false;

			backingStore = value;

			if (_isPropertyChangedSuppressed)
				_hasPropertyChangedWhileSuppressed = true;
			else
				OnPropertyChanged(propertyName);

			return true;
		}

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		protected void SuppressPropertyChanged()
		{
			if (_isPropertyChangedSuppressed)
				throw new InvalidOperationException();

			_isPropertyChangedSuppressed = true;
		}

		protected void ResumePropertyChanged(bool raiseIfChanged)
		{
			if (!_isPropertyChangedSuppressed)
				throw new InvalidOperationException();

			if (_hasPropertyChangedWhileSuppressed)
				OnPropertyChanged(string.Empty);

			_isPropertyChangedSuppressed = _hasPropertyChangedWhileSuppressed = false;
		}
	}
}
