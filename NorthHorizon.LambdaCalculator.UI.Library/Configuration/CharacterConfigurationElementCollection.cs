using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NorthHorizon.LambdaCalculator.UI.Configuration
{
	public class CharacterConfigurationElementCollection : ConfigurationElementCollection, IEnumerable<CharacterConfigurationElement>
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new CharacterConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((CharacterConfigurationElement)element).Value;
		}

		#region IEnumerable<CharacterConfigurationElement> Members

		public new IEnumerator<CharacterConfigurationElement> GetEnumerator()
		{
			var enumerator = base.GetEnumerator();

			while (enumerator.MoveNext())
				yield return (CharacterConfigurationElement)enumerator.Current;
		}

		#endregion
	}
}
