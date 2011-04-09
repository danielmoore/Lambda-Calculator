using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NorthHorizon.LambdaCalculator.UI.Configuration
{
	public class CharacterSetConfigurationSection : ConfigurationSection
	{
		private const string CharacterSetConfigurationSectionName = "characters";

		private const string GroupsPropertyName = "groups";
		[ConfigurationProperty(GroupsPropertyName, IsRequired = true, IsDefaultCollection = true)]
		[ConfigurationCollection(typeof(CharacterGroupConfigurationElementCollection), AddItemName = "group")]
		public CharacterGroupConfigurationElementCollection Groups
		{
			get { return (CharacterGroupConfigurationElementCollection)this[GroupsPropertyName]; }
			set { this[GroupsPropertyName] = value; }
		}

		public static CharacterSetConfigurationSection Get()
		{
			return (CharacterSetConfigurationSection)ConfigurationManager.GetSection(CharacterSetConfigurationSectionName);
		}
	}
}