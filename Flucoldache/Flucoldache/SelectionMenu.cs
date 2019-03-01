using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;

namespace Flucoldache
{
	public class SelectionMenu : GameObj
	{
		public string[] Items;
		public int SelectedItem = 0;
		public bool Activated = false;
		public Vector2 Pos;
		public Vector2 Size;
		public string Name;
		public bool DisplayBorders = true;

		public SelectionMenu(string name, string[] items, Vector2 pos, Vector2 size)
		{
			Items = new string[items.Length];
			Array.Copy(items, Items, items.Length);
			Pos = pos;
			Size = size;
			Name = name;

			Controls.Enabled = false;
		}

		public override void Update()
		{
			if (!Activated)
			{
				if (Input.KeyboardCheckPress(Controls.KeyUp))
				{
					SelectedItem -= 1;
				}
				if (Input.KeyboardCheckPress(Controls.KeyDown))
				{
					SelectedItem += 1;
				}
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
				Activated = true;
			}
		}
		
		public override void Destroy()
		{
			Controls.Enabled = true;
		}



		public override void Draw()
		{
			GameConsole.ForegroundColor = GameConsole.BaseForegroundColor;
			GameConsole.BackgroundColor = GameConsole.BaseBackgroundColor;
			DrawCntrl.SetTransformMatrix(Matrix.CreateTranslation(Vector3.Zero));

			if (DisplayBorders)
			{
				GameConsole.DrawFrame(Pos - Vector2.One, Size + Vector2.One * 2);
				GameConsole.DrawRectangle(Pos, Size);
				GameConsole.DrawText(Name, Pos - Vector2.UnitY);
			}

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

		}




	}


}
