using System;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;
using Flucoldache.Battle;

namespace Flucoldache.Overworld
{
	class BattleTrigger : OverworldObj
	{
		public BattleTrigger()
		{
			ArgumentName = "Arena file";
		}

		public override void Draw()
		{
			//if (EditorMode)
			{
				GameConsole.ForegroundColor = Color.White;
				GameConsole.BackgroundColor = Color.Red;
				GameConsole.DrawChar('B', Pos);
			}
		}

		public override bool TriggerAction()
		{
			new Arena(Argument);
			Objects.Destroy(this);
			return true;
		}
	}
}
