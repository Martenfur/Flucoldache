using System;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;

namespace Flucoldache.Overworld
{

	class GateTrigger : OverworldObj
	{
		char myChar = '\\';
		public GateTrigger()
		{
			ArgumentName = "none";
		}

		public override void Draw()
		{		
			GameConsole.ForegroundColor = Color.Yellow;
			GameConsole.BackgroundColor = Color.Black;
			GameConsole.DrawChar(myChar, Pos);
		}

		public override bool TriggerAction()
		{
			new Dialogue(new string[]{""}, new string[]{"Вы повернули рычаг, и ворота со скрежетом открылись."});
			Terrain terr = (Terrain)Objects.ObjFind<Terrain>(0);
			for(var x = 1; x <= 4; x += 1)
			{
				for(var y = 0; y < 4; y += 1)
				{
					Tile tile = terr.GetTile(Pos + new Vector2(x, y));
					tile.Char = ' ';
					tile.Type = Tile.TileType.Passable;
				}
			}
			
			Tile tile1 = terr.GetTile(Pos);
			tile1.Char = '/';
			tile1.BackgroundColor = Color.Black;
			tile1.ForegroundColor = Color.Yellow;

			Objects.Destroy(this);
						
			return true;
		
		}
	}

}
