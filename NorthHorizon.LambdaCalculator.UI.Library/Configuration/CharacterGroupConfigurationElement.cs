using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NorthHorizon.LambdaCalculator.UI.Configuration
{
	public class CharacterGroupConfigurationElement : ConfigurationElement
	{
		private const string NamePropertyName = "name";
		[ConfigurationProperty(NamePropertyName, IsKey = true, IsRequired = true)]
		public string Name
		{
			get { return (string)this[NamePropertyName]; }
			set { this[NamePropertyName] = value; }
		}

		private const string CharactersPropertyName = "characters";
		[ConfigurationProperty(CharactersPropertyName, IsRequired = true, IsDefaultCollection = true)]
		[ConfigurationCollection(typeof(CharacterConfigurationElementCollection), AddItemName = "character")]
		public CharacterConfigurationElementCollection Characters
		{
			get { return (CharacterConfigurationElementCollection)this[CharactersPropertyName]; }
			set { this[CharactersPropertyName] = value; }
		}
	}
}
