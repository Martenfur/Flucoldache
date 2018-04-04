using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using System.IO;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Flucoldache
{
	class Dialogue : GameObj
	{
		
		static string _rootDir = "Resources/Dialogues/";
		static string _newLineSeparator = "\\";
		public static Vector2 Size = new Vector2(GameConsole.W - 2, 8);
		public static Vector2 Pos = new Vector2(1, GameConsole.H - Size.Y - 1);
		

		string[] _dialogueNames;
		string[] _dialogueLines;

		public int LineId;
		
		public Dialogue(string fileName)
		{
			Controls.Enabled = false;
			
			LineId = 0;

			// Loading dialogue file.
			string[] lines = File.ReadAllLines(_rootDir + fileName);

			_dialogueLines = new string[lines.Length / 2];
			_dialogueNames = new string[lines.Length / 2];

			for(var i = 0; i < lines.Length; i += 1)
			{
				if (i % 2 == 0)
				{
					_dialogueNames[i / 2] = lines[i];
				}
				else
				{
					_dialogueLines[(i - 1) / 2] = lines[i].Replace(_newLineSeparator, Environment.NewLine);
				}
			}
			// Loading dialogue file.
		}


		public override void Update()
		{
			if (Input.KeyboardCheckPress(Controls.KeyA) || Input.KeyboardCheckPress(Controls.KeyB))
			{
				LineId += 1;
				if (LineId >= _dialogueLines.Length)
				{
					LineId = _dialogueLines.Length - 1;
					Controls.Enabled = true;
					Objects.Destroy(this);
				}
			}
		}

		public override void DrawEnd()
		{
			GameConsole.BackgroundColor = Color.Black;
			GameConsole.ForegroundColor = Color.Gray;
			GameConsole.DrawRectangle(Pos, Size);
			GameConsole.DrawFrame(Pos - Vector2.One, Size + Vector2.One * 2);
			
			
			GameConsole.DrawText('╣' + _dialogueNames[LineId] + '╠', Pos + new Vector2(1, -1));
			GameConsole.DrawText(_dialogueLines[LineId], Pos);
		}




	}
}
