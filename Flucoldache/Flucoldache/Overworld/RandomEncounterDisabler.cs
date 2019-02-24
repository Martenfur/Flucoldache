using System;
using System.Collections.Generic;
using System.IO;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;

namespace Flucoldache.Overworld
{
	class RandomEncounterDisabler : OverworldObj
	{
		static string _rootDir = "Resources/Maps";
		List<string> _arenas;

		public RandomEncounterDisabler()
		{
			ArgumentName = "none";
		}

		
		public override void Update()
		{
			if (!EditorMode)
			{
				
				Player player = (Player)Objects.ObjFind<Player>(0);
				if (player.Pos == Pos)
				{
					player.RandomEncountersEnabled = false;
				}
			}
		}

		public override void Draw()
		{
			if (EditorMode)
			{
				GameConsole.ForegroundColor = Color.Red;
				GameConsole.BackgroundColor = Color.Aqua;
				GameConsole.DrawChar('R', Pos);
			}
		}
	}
}