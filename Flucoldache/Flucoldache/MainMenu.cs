using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Flucoldache.Overworld;

namespace Flucoldache
{
	public class MainMenu : GameObj
	{
		
		public MainMenu()
		{
			
			new Dialogue("test_dialogue.txt");
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

		}

		public override void Draw()
		{
			GameConsole.DrawText("Press E to open editor. Press L to load a map in gameplay mode.", 8, 8);

		}
	}
}
