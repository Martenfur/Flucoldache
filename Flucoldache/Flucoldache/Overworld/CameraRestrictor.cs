using System;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;

namespace Flucoldache.Overworld
{
	class CameraRestrictor : OverworldObj
	{
		bool _init = true;
		public CameraRestrictor() {}

		
		public override void Update()
		{
			if (!EditorMode && _init)
			{
				_init = false;
				if (Pos.X < Terrain.CamMinPos.X)
				{
					Terrain.CamMinPos.X = Pos.X;
				}
				if (Pos.Y < Terrain.CamMinPos.Y)
				{
					Terrain.CamMinPos.Y = Pos.Y;
				}
				if (Pos.X > Terrain.CamMaxPos.X)
				{
					Terrain.CamMaxPos.X = Pos.X;
				}
				if (Pos.Y > Terrain.CamMaxPos.Y)
				{
					Terrain.CamMaxPos.Y = Pos.Y;
				}
			}
		}

		public override void Draw()
		{
			if (EditorMode)
			{
				GameConsole.ForegroundColor = GameConsole.BaseBackgroundColor;
				GameConsole.BackgroundColor = Color.Red;
				GameConsole.DrawChar('R', Pos);
			}
		}

		
	}
}

