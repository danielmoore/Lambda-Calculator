using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Composite.Presentation.Commands;
using NorthHorizon.LambdaCalculator.UI.Model;
using SystemF = NorthHorizon.LambdaCalculator.Languages.SystemF.Execution;
using UntypedLambda = NorthHorizon.LambdaCalculator.Languages.UntypedLambda.Execution;

namespace NorthHorizon.LambdaCalculator.UI.Modules.Calculator
{
	public enum Language
	{
		UntypedLambda,
		SystemF
	}

	public class CalculatorViewModel : Core.ViewModel
	{
		#region Fields

		private readonly IEnumerable<Character> _flatCharacterList;

		#endregion

		#region Constructors

		public CalculatorViewModel(CharacterSet characterSet)
		{
			_script = _output = string.Empty;
			CharacterSet = characterSet;
			_flatCharacterList = characterSet.Groups.SelectMany(g => g.Characters);
			ExecuteCommand = new DelegateCommand<object>(_ => Execute());
			Languages = Enum.GetValues(typeof(Language)).OfType<Language>();
		}

		#endregion

		#region Properties

		public ICommand ExecuteCommand { get; private set; }

		public CharacterSet CharacterSet { get; private set; }

		public IEnumerable<Language> Languages { get; private set; }

		private int _scriptCaretPosition;
		public int ScriptCaretPosition
		{
			get { return _scriptCaretPosition; }
			set { SetProperty(ref _scriptCaretPosition, value, "ScriptCaretPosition"); }
		}

		private string _script;
		public string Script
		{
			get { return _script; }
			set { SetProperty(ref _script, value, "Script"); }
		}

		private string _output;
		public string Output
		{
			get { return _output; }
			private set { SetProperty(ref _output, value, "Output"); }
		}

		private Language _selectedLanguage;
		public Language SelectedLanguage
		{
			get { return _selectedLanguage; }
			set { SetProperty(ref _selectedLanguage, value, "SelectedLanguage"); }
		}

		#endregion

		#region Methods

		public void RegisterKeyBindings(InputBindingCollection bindings)
		{
			bindings.Add(new KeyBinding(ExecuteCommand, Key.F5, ModifierKeys.None));

			foreach (var c in _flatCharacterList)
			{
				var gesture = GetKeyGesture(c.Shortcut);
				if (gesture != null)
					bindings.Add(new KeyBinding
					{
						Command = CalculatorCommands.InputCharacter,
						CommandParameter = c,
						Gesture = gesture
					});
			}
		}

		private static readonly string[] KeyNames = Enum.GetNames(typeof(Key));
		private static readonly string[] ModifierKeyNames = Enum.GetNames(typeof(ModifierKeys));
		private static KeyGesture GetKeyGesture(string combo)
		{
			var modifiers = ModifierKeys.None;
			var key = Key.None;

			if (string.IsNullOrEmpty(combo.Trim()))
				return null;

			var keys = combo.Split('+').Select(s => s.Trim());
			foreach (var k in keys)
			{
				if (ModifierKeyNames.Any(s => s.Equals(k, StringComparison.CurrentCultureIgnoreCase)))
					modifiers |= (ModifierKeys)Enum.Parse(typeof(ModifierKeys), k, true);
				else if (KeyNames.Any(s => s.Equals(k, StringComparison.CurrentCultureIgnoreCase)))
				{
					if (key != Key.None)
						throw new InvalidOperationException("More than one target key specified.");

					key = (Key)Enum.Parse(typeof(Key), k, true);
				}
				else
					throw new InvalidOperationException("Invalid Key");
			}

			return key != Key.None ? new KeyGesture(key, modifiers) : null;
		}

		public void InputCharacter(Character character)
		{
			var index = ScriptCaretPosition;
			Script = Script.Insert(index, character.Value);

			ScriptCaretPosition = index + character.Value.Length;
		}


		public void OnScriptChanged()
		{
			var replace = _flatCharacterList
				.Where(c => !string.IsNullOrEmpty(c.Alias))
				.FirstOrDefault(c => Script.EndsWith(c.Alias));

			if (replace != null)
			{
				var pos = ScriptCaretPosition + 1;
				int start = pos - replace.Alias.Length, end = pos;

				Script = Script.Substring(0, start) + replace.Value + Script.Substring(end);

				ScriptCaretPosition = pos + replace.Value.Length;
			}
		}

		public void Execute()
		{
			if (string.IsNullOrEmpty(Script))
			{
				Output = "No expression to evaluate.";
				return;
			}

			switch (SelectedLanguage)
			{
				case Language.UntypedLambda:
					ExecuteUntypedLambda();
					break;
				case Language.SystemF:
					ExecuteSystemF();
					break;
				default:
					throw new NotImplementedException();
			}
		}

		public void ExecuteUntypedLambda()
		{
			try
			{
				var expr = UntypedLambda.getExpr(Script);

				if (UntypedLambda.isValid(expr))
				{
					var ret = UntypedLambda.execute(expr);
					Output = ret.ToString();
				}
				else
					Output = "Invalid Expression.";
			}
			catch
			{
				Output = "General Error.";
			}
		}

		public void ExecuteSystemF()
		{
#if DEBUG
            var tokens = SystemF.GetTokenList(Script);
#endif
			try
			{
				var expr = SystemF.GetExpression(Script);

				if (SystemF.IsValid(expr))
				{
					var type = SystemF.TypeCheckExpression(expr);
					if (type.IsErr)
						Output = ((SystemF.TypeCheckVal.Err)type).Item;
					else if (type.IsType)
					{
						Output = ((SystemF.TypeCheckVal.Type)type).Item.ToString();
						Output += Environment.NewLine;

						var result = SystemF.Execute(expr);
						Output += result.ToString();
					}
				}
				else
					Output = "Invalid Expression.";
			}
			catch
			{
				Output = "General Error.";
			}
		}

		#endregion
	}
}
