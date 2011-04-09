using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NorthHorizon.LambdaCalculator.UI.Model
{
	public class Character
	{
		public Character(string name, string value, string alias, string shortcut)
		{
			Name = name;
			Value = value;
			Alias = alias;
			Shortcut = shortcut;
		}

		public string Name { get; private set; }
		public string Value { get; private set; }
		public string Alias { get; private set; }
		public string Shortcut { get; private set; }
	}
}
