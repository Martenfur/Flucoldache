using Monofoxe.Engine;

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
				var d = new Dialogue(Argument);
				d.Voiceless = false;
				Objects.Destroy(this);
			}
		}

		public override void Draw()
		{
			if (EditorMode)
			{
				GameConsole.ForegroundColor = GameConsole.BaseBackgroundColor;
				GameConsole.BackgroundColor = GameConsole.BaseForegroundColor;
				GameConsole.DrawChar('I', Pos);
			}
		}
	}
}
