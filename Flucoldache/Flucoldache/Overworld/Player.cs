using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.IO;

namespace Flucoldache.Overworld
{
	public class Player : OverworldObj
	{
		
		public float WalkSpd = 7; // Cells/second.

		public const char Char = '@';

		private Alarm _movementAlarm = new Alarm();

		Terrain Terr;

		public Player() {}

		public Player(Vector2 pos)
		{
			Pos = pos;
		}

		public override void Update()
		{
			Terr = (Terrain)Objects.ObjFind<Terrain>(0);

			if (!EditorMode)
			{				
				Vector2 movement = Vector2.Zero;

				// Movement.
				if (Input.KeyboardCheck(GameplayController.KeyUp))
				{
					movement.Y += -1;
				}

				if (Input.KeyboardCheck(GameplayController.KeyDown))
				{
					movement.Y += 1;
				}

				if (Input.KeyboardCheck(GameplayController.KeyLeft))
				{
					movement.X += -1;
				}

				if (Input.KeyboardCheck(GameplayController.KeyRight))
				{
					movement.X += 1;
				}
				// Movement.

				_movementAlarm.Update();
				if (movement != Vector2.Zero)
				{
					if (!_movementAlarm.Active)
					{
						_movementAlarm.Set(1f / WalkSpd);
						_movementAlarm.Triggered = true;
					}
				}
				else
				{
					_movementAlarm.Reset();
				}

				if (_movementAlarm.Triggered)
				{
					if (Terr.GetTile(new Vector2(Pos.X + movement.X, Pos.Y)).IsPassable())
					{
						Pos.X += movement.X;
					}

					if (Terr.GetTile(new Vector2(Pos.X, Pos.Y + movement.Y)).IsPassable())
					{
						Pos.Y += movement.Y;
					}
				}
			}
		}

		public override void Draw()
		{
			GameConsole.ForegroundColor = Color.White;
			GameConsole.BackgroundColor = Terr.GetTile(Pos).BackgroundColor;
			GameConsole.DrawChar(Char, (int)Pos.X, (int)Pos.Y);
		}

		public override void DrawGUI()
		{
			
		}
		

	}
}
