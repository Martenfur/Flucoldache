using System;
using System.IO;
using Flucoldache.Overworld;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;
using System.Globalization;

namespace Flucoldache
{
	public class MainMenu : GameObj
	{
		public string[] Items;
		public int SelectedItem = 0;
		public Vector2 Pos = new Vector2(1, 24);
		
		private bool _controlsBlock = false;
		private bool _newGameAnimation = false;

		private int _newGameAlarm = -1;
		private int _newGameTime = 5;
		private int _newGameAnimationStage = 0;
		
		private Color _bufferBg = GameConsole.BaseBackgroundColor;
		private Color _bufferFg = GameConsole.BaseForegroundColor;


		public MainMenu()
		{
			if (CultureInfo.CurrentUICulture.Name.Contains("ru"))
			{
				Strings.Load("ru");
			}
			else
			{
				Strings.Load("en");
			}
			//Strings.Load("en");
			
			
			Items = new string[]{
				Strings.MenuNewGame, 
				Strings.MenuLoad, 
				Strings.MenuMapEditor, 
				Strings.MenuLoadMap, 
				Strings.MenuExit
			};

			SoundController.PlaySong(SoundController.Title);
		}

		public override void Update()
		{
			if (!_controlsBlock)
			{
				if (Input.KeyboardCheckPress(Controls.KeyUp))
				{
					SelectedItem -= 1;	
					SoundController.PlaySound(SoundController.Blip);
				}
				if (Input.KeyboardCheckPress(Controls.KeyDown))
				{
					SelectedItem += 1;
					SoundController.PlaySound(SoundController.Blip);
				}

				// Wrapping cursor.
				if (SelectedItem < 0)
				{
					SelectedItem += Items.Length;
				}
				if (SelectedItem >= Items.Length)
				{
					SelectedItem -= Items.Length;
				}
				// Wrapping cursor.

				if (Input.KeyboardCheckPress(Controls.KeyA))
				{
					ItemActivate();
				}
			}




			if (_newGameAnimation)
			{

				_newGameAlarm -= 1;

				if (_newGameAlarm == 0)
				{
					switch(_newGameAnimationStage)
					{
						case 0:
							GameConsole.BaseBackgroundColor = _bufferFg;
							GameConsole.BaseForegroundColor = _bufferFg;
						break;
						case 1:
							GameConsole.BaseBackgroundColor = _bufferFg;
							GameConsole.BaseForegroundColor = _bufferBg;
						break;
						case 2:
							GameConsole.BaseBackgroundColor = _bufferBg;
							GameConsole.BaseForegroundColor = _bufferFg;
						break;
					}

					_newGameAnimationStage += 1;
					_newGameAlarm = _newGameTime;
				}

				// Shitcoding my way to victory.
				if (_newGameAnimationStage > 40 && !SoundController.NewGame.IsPlaying)
				{
					StartNewGame();
				}
			}
		}

		public override void Draw()
		{
			
			GameConsole.ForegroundColor = GameConsole.BaseForegroundColor;
			GameConsole.BackgroundColor = GameConsole.BaseBackgroundColor;

			DrawCntrl.SetTransformMatrix(Matrix.CreateTranslation(Vector3.Zero));

			GameConsole.DrawRectangle(Vector2.Zero, new Vector2(GameConsole.W, GameConsole.H));

			string itemStr;
			for(var i = 0; i < Items.Length; i += 1)
			{
				if (i == SelectedItem)
				{
					itemStr = "> " + Items[i];
				}
				else
				{
					itemStr = Items[i];
				}
				GameConsole.DrawText(itemStr, Pos + Vector2.UnitY * i);
			}
			DrawCntrl.ResetTransformMatrix();

			GameConsole.DrawText(Strings.MenuTitle, Pos + Vector2.UnitY * -2);

			GameConsole.DrawText(Strings.MenuControls, new Vector2(1, GameConsole.H - 2));
			GameConsole.DrawText(Strings.MenuCredits1, new Vector2(70, GameConsole.H - 3));
			GameConsole.DrawText(Strings.MenuCredits2, new Vector2(64, GameConsole.H - 2));
		
		}

		void StartNewGame()
		{		
			MapEditor.LoadMap(Environment.CurrentDirectory + "/Resources/Maps/mansion_room0.map", false);
			new Inventory();
			Objects.Destroy(this);
		}

		void ItemActivate()
		{
			// New game.
			if (SelectedItem == 0)
			{
				SoundController.CurrentSong.Stop();
				SoundController.PlaySound(SoundController.NewGame);
				_controlsBlock = true;
				_newGameAlarm = _newGameTime;
				_newGameAnimation = true;
			}
			// New game.

			// Load game.
			if (SelectedItem == 1)
			{
				if (File.Exists(Environment.CurrentDirectory + '/' + GameplayController.SaveDir + "/save.map"))
				{	
					GameplayController.LoadGame();
					Objects.Destroy(this);
				}
			}
			// Load game.

			// Open editor.
			if (SelectedItem == 2)
			{
				new MapEditor();
				Objects.Destroy(this);
			}
			// Open editor.

			// Load map.
			if (SelectedItem == 3)
			{
				System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
				dialog.Title = "Save map file";
				dialog.Filter = "Map File|*.map";
				dialog.ShowDialog();
				if (dialog.FileName != "")
				{
					MapEditor.LoadMap(dialog.FileName, false);	
					Inventory inv = new Inventory();
					
					Objects.Destroy(this);
				}
			}
			// Load map.

			// Exit.
			if (SelectedItem == 4)
			{
				GameCntrl.MyGame.Exit();
			}
			// Exit.
		}
	}
}
