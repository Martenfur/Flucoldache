using System;
using System.IO;
using Flucoldache.Overworld;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;

namespace Flucoldache
{
	public class MainMenu : GameObj
	{
		public string[] Items;
		public int SelectedItem = 0;
		public Vector2 Pos = new Vector2(1, 24);
		
		public MainMenu()
		{
			Strings.Load("en");

			Items = new string[]{
				Strings.MenuNewGame, 
				Strings.MenuLoad, 
				Strings.MenuMapEditor, 
				Strings.MenuLoadMap, 
				Strings.MenuExit
			};
		
		}

		public override void Update()
		{
			if (Input.KeyboardCheckPress(Controls.KeyUp))
			{
				SelectedItem -= 1;
			}
			if (Input.KeyboardCheckPress(Controls.KeyDown))
			{
				SelectedItem += 1;
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

		public override void Draw()
		{
			
			GameConsole.ForegroundColor = Color.Gray;
			GameConsole.BackgroundColor = Color.Black;

			DrawCntrl.SetTransformMatrix(Matrix.CreateTranslation(Vector3.Zero));
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
			GameConsole.DrawText(Strings.MenuCredits2, new Vector2(70, GameConsole.H - 2));
		
		}

		void ItemActivate()
		{
			// New game.
			if (SelectedItem == 0)
			{
				MapEditor.LoadMap(Environment.CurrentDirectory + "/Resources/Maps/mansion_room0.map", false);
				new Inventory();
				Objects.Destroy(this);
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
