using System;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;
using Flucoldache.Battle;

namespace Flucoldache.Overworld
{

	class Crowd : OverworldObj
	{
		Vector2 _spacing = new Vector2(3, 2);
		Dialogue _dialogue;
		Alarm _alarm = new Alarm();

		public Crowd()
		{
			ArgumentName = "none";
			_alarm.Active = false;
		}

		
		public override void Update()
		{
			Player player = (Player)Objects.ObjFind<Player>(0);
			if (player.Pos.Y >= Pos.Y - 2)
			{
				if (_dialogue == null)
				{
					_dialogue = new Dialogue("mansion_crowd.txt");
					_dialogue.Voiceless = false;
				}
				else
				{
					if (_dialogue.Destroyed && !_alarm.Active)
					{
						new ArenaAppearEffect("mansion_crowd.xml");
						_alarm.Set(0.5f);
						_alarm.Active = true;
					}
				}
			}

			if (_alarm.Update())
			{
				Objects.Destroy(this);
			}
		}

		public override void Draw()
		{		
			GameConsole.ForegroundColor = Color.White;
			GameConsole.BackgroundColor = GameConsole.BaseBackgroundColor;

			GameConsole.DrawChar('M', Pos);
			GameConsole.DrawChar('M', Pos + Vector2.UnitX * 3);

			for(var x = -2; x < 4; x += 1)
			{
				for(var y = 1; y < 4; y += 1)
				{
					GameConsole.DrawChar('M', Pos + new Vector2(x, y) * _spacing);
				}
			}

		}
	}

}
