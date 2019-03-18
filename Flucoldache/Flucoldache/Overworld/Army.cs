using System;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;
using Flucoldache.Battle;

namespace Flucoldache.Overworld
{

	class Army : OverworldObj
	{
		Vector2 _spacing = new Vector2(3, 2);
		Dialogue _dialogue;
		Alarm _alarm = new Alarm();

		public Army()
		{
			ArgumentName = "none";
			_alarm.Active = false;
		}

		
		public override void Update()
		{
			Player player = (Player)Objects.ObjFind<Player>(0);
			if (player.Pos.X >= Pos.X - 4)
			{
				if (_dialogue == null)
				{
					_dialogue = new Dialogue("knights.txt");
					_dialogue.Voiceless = false;
				}
				else
				{
					if (_dialogue.Destroyed && !_alarm.Active)
					{
						new ArenaAppearEffect("final.xml");
						_alarm.Set(0.75f);
						_alarm.Active = true;
					}
				}
			}

			if (_alarm.Update())
			{
				new Endgame();
				Objects.Destroy(this);
			}
		}

		public override void Draw()
		{		
			GameConsole.ForegroundColor = GameConsole.BaseBackgroundColor;
			GameConsole.BackgroundColor = Color.LightGray;

			GameConsole.DrawChar('T', Pos);

			for(var x = 1; x < 4; x += 1)
			{
				for(var y = -1; y <= 1; y += 1)
				{
					GameConsole.DrawChar('T', Pos + new Vector2(x, y) * _spacing);
				}
			}

		}
	}

}
