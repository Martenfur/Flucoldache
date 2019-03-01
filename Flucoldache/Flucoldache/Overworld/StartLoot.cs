using System;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;

namespace Flucoldache.Overworld
{
	class StartLoot : OverworldObj
	{
		public StartLoot()
		{
			ArgumentName = "none";
		}
		
		public override void UpdateEnd()
		{
			if (!EditorMode)
			{
				Inventory inv = (Inventory)Objects.ObjFind<Inventory>(0);
				inv.AddPotion("cold", 999);
				inv.AddItem("book", 1);
				inv.AddItem("lab", 1);
				inv.AddItem("bread", 10);

				Objects.Destroy(this);

				GameplayController.SaveGame();
			}
		}

		public override void Draw()
		{
			if (EditorMode)
			{
				GameConsole.ForegroundColor = GameConsole.BaseBackgroundColor;
				GameConsole.BackgroundColor = Color.Pink;
				GameConsole.DrawChar('S', Pos);
			}
		}
	}
}