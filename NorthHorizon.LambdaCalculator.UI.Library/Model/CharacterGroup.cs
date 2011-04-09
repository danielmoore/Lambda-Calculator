using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NorthHorizon.LambdaCalculator.UI.Model
{
	public class CharacterGroup
	{
		public CharacterGroup(string name, IEnumerable<Character> characters)
		{
			Name = name;
			Characters = characters;
		}

		public string Name { get; private set; }

		public IEnumerable<Character> Characters { get; private set; }
	}
}
