using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NorthHorizon.LambdaCalculator.UI.Configuration
{
	public class CharacterGroupConfigurationElementCollection : ConfigurationElementCollection, IEnumerable<CharacterGroupConfigurationElement>
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new CharacterGroupConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((CharacterGroupConfigurationElement)element).Name;
		}

		#region IEnumerable<CharacterGroupConfigurationElement> Members

		public new IEnumerator<CharacterGroupConfigurationElement> GetEnumerator()
		{
			var enumerator = base.GetEnumerator();

			while (enumerator.MoveNext())
				yield return (CharacterGroupConfigurationElement)enumerator.Current;
		}

		#endregion
	}
}
