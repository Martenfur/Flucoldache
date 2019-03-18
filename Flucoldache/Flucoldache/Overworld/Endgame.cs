using System;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;

namespace Flucoldache.Overworld
{
	class Endgame : OverworldObj
	{
		Dialogue _dialogue;

		public Endgame()
		{
			ArgumentName = "none";
			Depth = -99;
		}
		
		public override void UpdateEnd()
		{
			if (_dialogue == null)
			{
				_dialogue = new Dialogue("final.txt");
				_dialogue.Voiceless = false;
			}
			else
			{
				if (_dialogue.Destroyed)
				{
					new MainMenu();
					foreach(OverworldObj obj in Objects.GetList<OverworldObj>())
					{
						Objects.Destroy(obj);
					}
					foreach(Terrain obj in Objects.GetList<Terrain>())
					{
						Objects.Destroy(obj);
					}
					foreach(Inventory obj in Objects.GetList<Inventory>())
					{
						Objects.Destroy(obj);
					}

				}
			}
		}
		public override void Draw()
		{
			DrawCntrl.SetTransformMatrix(Matrix.CreateTranslation(Vector3.Zero));
			GameConsole.BackgroundColor = GameConsole.BaseBackgroundColor;

			GameConsole.DrawRectangle(0, 0, GameConsole.W, GameConsole.H);
			DrawCntrl.ResetTransformMatrix();
		}

	}
}