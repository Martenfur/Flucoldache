using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;

namespace Flucoldache.Overworld
{
	public class Player : GameObj
	{
		public Vector2 Pos;
		public float WalkSpd = 7; // Cells/second.

		public const char Char = '@';

		private Alarm _movementAlarm = new Alarm();

		public Player(Vector2 pos)
		{
			Pos = pos;
		}

		public override void Update()
		{
			Vector2 movement = Vector2.Zero;

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
				Pos += movement;
			}

		}

		public override void Draw()
		{
			GameConsole.ForegroundColor = Color.White;
			GameConsole.DrawChar(Char, (int)Pos.X, (int)Pos.Y);
		}

	}
}
