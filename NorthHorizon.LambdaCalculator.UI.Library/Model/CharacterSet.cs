using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace NorthHorizon.LambdaCalculator.UI.Model
{
	public class CharacterSet
	{
		[InjectionConstructor]
		public CharacterSet(Configuration.CharacterSetConfigurationSection configSection)
		{
			var groups = new List<CharacterGroup>();

			foreach (var group in configSection.Groups)
			{
				var characters = new List<Character>();

				foreach (var character in group.Characters)
					characters.Add(new Character(character.Name, character.Value, character.Alias, character.Shortcut));

				groups.Add(new CharacterGroup(group.Name, characters));
			}

			Groups = groups;
		}
		
		public CharacterSet(IEnumerable<CharacterGroup> groups)
		{
			Groups = groups;
		}

		public IEnumerable<CharacterGroup> Groups { get; private set; }
	}
}
