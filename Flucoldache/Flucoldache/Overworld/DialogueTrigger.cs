using System;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;

namespace Flucoldache.Overworld
{
	class DialogueTrigger : OverworldObj
	{
		public DialogueTrigger()
		{
			ArgumentName = "Dialogue file";
		}

		public override void Draw()
		{
			if (EditorMode)
			{
				GameConsole.ForegroundColor = Color.Black;
				GameConsole.BackgroundColor = Color.AliceBlue;
				GameConsole.DrawChar('D', Pos);
			}
		}

		public override bool TriggerAction()
		{
			new Dialogue(Argument);
			return true;
		}
	}
}
