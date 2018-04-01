using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using System.IO;
using Monofoxe.Engine;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Flucoldache
{
	class Dialogue : GameObj
	{
		
		static string _rootDir = "Resources/";
		static string _newLineSeparator = "\\";
		static Vector2 _size = new Vector2(GameConsole.W - 2, 8);
		static Vector2 _pos = new Vector2(1, GameConsole.H - _size.Y - 1);
		

		string[] _dialogueNames;
		string[] _dialogueLines;

		
		public Dialogue(string fileName)
		{
			string[] lines = File.ReadAllLines(_rootDir + fileName);

			_dialogueLines = new string[lines.Length / 2];
			_dialogueNames = new string[lines.Length / 2];

			for(var i = 0; i < lines.Length; i += 1)
			{
				if (i % 2 == 0)
				{
					_dialogueNames[i / 2] = lines[i];
					Debug.WriteLine(lines[i]);
				}
				else
				{
					_dialogueLines[(i - 1) / 2] = lines[i].Replace(_newLineSeparator, Environment.NewLine);
				}
			}
		}



		public override void DrawEnd()
		{
			GameConsole.BackgroundColor = Color.Black;
			GameConsole.ForegroundColor = Color.Gray;
			GameConsole.DrawRectangle(_pos, _size);
			GameConsole.DrawFrame(_pos - Vector2.One, _size + Vector2.One * 2);
			
			
			GameConsole.DrawText('╣' + _dialogueNames[0] + '╠', _pos + new Vector2(2, -1));
			GameConsole.DrawText(_dialogueLines[0], _pos);
		}




	}
}
