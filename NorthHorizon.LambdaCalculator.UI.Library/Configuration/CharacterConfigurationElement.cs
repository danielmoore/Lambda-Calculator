using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NorthHorizon.LambdaCalculator.UI.Configuration
{
	public class CharacterConfigurationElement : ConfigurationElement
	{
		private const string NamePropertyName = "name";
		[ConfigurationProperty(NamePropertyName, IsRequired = false)]
		public string Name
		{
			get { return (string)this[NamePropertyName]; }
			set { this[NamePropertyName] = value; }
		}

		private const string ValuePropertyName = "value";
		[ConfigurationProperty(ValuePropertyName, IsKey = true, IsRequired = true)]
		public string Value
		{
			get { return (string)this[ValuePropertyName]; }
			set { this[ValuePropertyName] = value; }
		}

		private const string AliasPropertyName = "alias";
		[ConfigurationProperty(AliasPropertyName, IsRequired = false)]
		public string Alias
		{
			get { return (string)this[AliasPropertyName]; }
			set { this[AliasPropertyName] = value; }
		}

		private const string ShortcutPropertyName = "shortcut";
		[ConfigurationProperty(ShortcutPropertyName, IsRequired = false)]
		public string Shortcut
		{
			get { return (string)this[ShortcutPropertyName]; }
			set { this[ShortcutPropertyName] = value; }
		}
	}
}
