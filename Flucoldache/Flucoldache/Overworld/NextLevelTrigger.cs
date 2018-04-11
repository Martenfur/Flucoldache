using System;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;

namespace Flucoldache.Overworld
{
	class NextLevelTrigger : OverworldObj
	{
		static string _rootDir = "Resources/Maps";
		public NextLevelTrigger()
		{
			ArgumentName = "Map file";
		}

		public override void Draw()
		{
			if (EditorMode)
			{
				GameConsole.ForegroundColor = Color.Yellow;
				GameConsole.BackgroundColor = Color.Violet;
				GameConsole.DrawChar('>', Pos);
			}
		}

		public override bool TriggerAction()
		{
			MapEditor.LoadMap(_rootDir + '/' + Argument, false);
			return true;
		}
	}
}
