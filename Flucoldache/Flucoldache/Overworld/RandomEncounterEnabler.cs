using System;
using System.Collections.Generic;
using System.IO;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;

namespace Flucoldache.Overworld
{
	class RandomEncounterEnabler : OverworldObj
	{
		static string _rootDir = "Resources/Arenas";
		List<string> _arenas;

		public RandomEncounterEnabler()
		{
			ArgumentName = "Table file";
		}

		
		public override void Update()
		{
			if (!EditorMode)
			{
				if (_arenas == null)
				{
					_arenas = new List<string>(File.ReadAllLines(_rootDir + '/' + Argument));
				}

				Player player = (Player)Objects.ObjFind<Player>(0);
				if (player.Pos == Pos)
				{
					player.RandomEncountersEnabled = true;
					player.RandomEncounterArenas = _arenas;
				}
			}
		}

		public override void Draw()
		{
			if (EditorMode)
			{
				GameConsole.ForegroundColor = Color.Aqua;
				GameConsole.BackgroundColor = Color.Red;
				GameConsole.DrawChar('R', Pos);
			}
		}
	}
}