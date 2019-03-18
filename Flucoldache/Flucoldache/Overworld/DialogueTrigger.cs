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
				GameConsole.ForegroundColor = GameConsole.BaseBackgroundColor;
				GameConsole.BackgroundColor = GameConsole.BaseForegroundColor;
				GameConsole.DrawChar('D', Pos);
			}
		}

		public override bool TriggerAction()
		{
			var d = new Dialogue(Argument);
			d.Voiceless = false;
			return true;
		}
	}
}
