using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Flucoldache.Overworld;
using Flucoldache.Battle;


namespace Flucoldache
{
	public class MainMenu : GameObj
	{
		Inventory inv = new Inventory();
			
		public MainMenu()
		{
			new Arena("test_arena.xml");
			inv.AddItem("fox", 32);
			inv.AddItem("testitem", 2);
			inv.AddItem("testitem", 1);
		
			inv.AddPotion("fox", 2);
			string[] items = {"foxes", "are", "fluffers"};

			//new SelectionMenu("Инвентарь", items, Vector2.One, new Vector2(10, 10));
		}
		public override void Update()
		{
			if (Input.KeyboardCheckPress(Keys.E))
			{
				new MapEditor();
				Objects.Destroy(this);
			}
			if (Input.KeyboardCheckPress(Keys.L))
			{
				System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
				dialog.Title = "Save map file";
				dialog.Filter = "Map File|*.map";
				dialog.ShowDialog();
				if (dialog.FileName != "")
				{
					MapEditor.LoadMap(dialog.FileName, false);	
					Objects.Destroy(this);
				}
			}

			if (Input.KeyboardCheckPress(Keys.Z))
			{
				inv.ShowItems();
			}
			if (Input.KeyboardCheckPress(Keys.X))
			{
				inv.ShowPotions();
			}
			
			

		}

		public override void Draw()
		{
			GameConsole.DrawText("Press E to open editor. Press L to load a map in gameplay mode.", 8, 8);

		}

		public void Test()
		{
			Debug.WriteLine("test");
		}
	}
}
