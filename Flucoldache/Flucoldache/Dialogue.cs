﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using System.IO;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Flucoldache.Battle;

namespace Flucoldache
{
	public class Dialogue : GameObj
	{
		
		static string _rootDir;
		static string _newLineSeparator = "\\";
		public static Vector2 Size = new Vector2(GameConsole.W - 2, 8);
		public static Vector2 Pos = new Vector2(1, GameConsole.H - Size.Y - 1);
		
		public bool Borderless = false;

		string[] _dialogueNames;
		string[] _dialogueLines;

		public int LineId = 0;
		
		public bool Voiceless = true;

		StringBuilder _typedText = new StringBuilder();
		Alarm _typeAlarm = new Alarm();
		float _typeSpeed = 1f / 20f;

		public bool Locked = false;

		public Dialogue(string[] dialogueNames, string[] dialogueLines)
		{
			_rootDir = "Resources/" + Strings.Localization + "/Dialogues/";
			Controls.Enabled = false;
			
			_dialogueLines = new string[dialogueLines.Length];
			Array.Copy(dialogueLines, _dialogueLines, dialogueLines.Length);
			_dialogueNames = new string[dialogueNames.Length];
			Array.Copy(dialogueNames, _dialogueNames, dialogueNames.Length);
			
		}

		public Dialogue(string fileName)
		{
			_rootDir = "Resources/" + Strings.Localization + "/Dialogues/";
			
			Controls.Enabled = false;
			
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
			if (Objects.Count<Arena>() > 0)
			{
				Borderless = true;
			}

			if (Input.KeyboardCheckPress(Controls.KeyA) || Input.KeyboardCheckPress(Controls.KeyB))
			{
				if (_dialogueLines[LineId].Length != _typedText.ToString().Length)
				{
					_typedText = new StringBuilder(_dialogueLines[LineId]);
				}
				else
				{
					if (!Locked)
					{
						LineId += 1;
						_typedText.Clear();
						if (LineId >= _dialogueLines.Length)
						{
							LineId = _dialogueLines.Length - 1;
							Controls.Enabled = true;
							Objects.Destroy(this);
							Input.KeyboardClear();
						}
					}
				}
			}

			if (_dialogueLines[LineId].Length != _typedText.ToString().Length)
			{
				_typeAlarm.Update();
				if (!_typeAlarm.Active)
				{
					_typeAlarm.Set(_typeSpeed);
				}
				if (_typeAlarm.Triggered)
				{
					if (!Voiceless)
					{
						SoundController.PlaySound(SoundController.Voice);
					}

					_typedText.Append(_dialogueLines[LineId].ElementAt(_typedText.Length));
					if (_dialogueLines[LineId].Length - 1 > _typedText.Length)
					{
						char nextCh = _dialogueLines[LineId].ElementAt(_typedText.Length);
						if (nextCh == ' ')
						{
							_typedText.Append(' ');
						}
						if (nextCh == Environment.NewLine.ElementAt(0))
						{
							_typedText.Append(Environment.NewLine);
						}
					}
				}
			}
		}

		public override void DrawEnd()
		{
			GameConsole.BackgroundColor = GameConsole.BaseBackgroundColor;
			GameConsole.ForegroundColor = GameConsole.BaseForegroundColor;

			DrawCntrl.SetTransformMatrix(Matrix.CreateTranslation(Vector3.Zero));
			GameConsole.DrawRectangle(Pos, Size);
			if (!Borderless)
			{
				GameConsole.DrawFrame(Pos - Vector2.One, Size + Vector2.One * 2);
			}

			GameConsole.DrawText(_dialogueNames[LineId], Pos + new Vector2(1, -1));
			GameConsole.DrawText(_typedText.ToString(), Pos);
			DrawCntrl.ResetTransformMatrix();
		}




	}
}
