using System;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;

namespace Flucoldache.Overworld
{
	class InstantDialogueTrigger : OverworldObj
	{
		public InstantDialogueTrigger()
		{
			ArgumentName = "Dialogue file";
		}

		public override void Update()
		{
			if (!EditorMode)
			{
				new Dialogue(Argument);
				Objects.Destroy(this);
			}
		}

		public override void Draw()
		{
			if (EditorMode)
			{
				GameConsole.ForegroundColor = Color.Black;
				GameConsole.BackgroundColor = Color.AliceBlue;
				GameConsole.DrawChar('I', Pos);
			}
		}
	}
}
