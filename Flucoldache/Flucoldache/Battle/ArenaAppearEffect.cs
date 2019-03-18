using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;
using Flucoldache.Overworld;

namespace Flucoldache.Battle
{
	public class ArenaAppearEffect : GameObj
	{
		Alarm _alarm = new Alarm();
		string _arenaArg;
		int _mode = 0;
		bool _drawWhiteScreen = true;
		float _alarmTime = 0.15f;

		public ArenaAppearEffect(string arenaArg)
		{
			SoundController.CurrentSong.Stop();
			_alarm.Set(_alarmTime);
			_arenaArg = arenaArg;
			Controls.Enabled = false;
			
			SoundController.PlaySound(SoundController.ArenaAppearEffect);
			Depth = -990000;
		}

		public override void Update()
		{
			base.Update();
			
			if (_alarm.Update())
			{
				_alarm.Set(_alarmTime);
				_mode += 1;
				
				if (_mode > 2)
				{
					new Arena(_arenaArg);
					Objects.Destroy(this);
					Controls.Enabled = true;
				}
				else
				{
					_drawWhiteScreen = !_drawWhiteScreen;
					SoundController.PlaySound(SoundController.ArenaAppearEffect);
				}
			}
		}

		public override void DrawEnd()
		{
			if (_drawWhiteScreen)
			{
				DrawCntrl.SetTransformMatrix(Matrix.CreateTranslation(Vector3.Zero));
				GameConsole.ForegroundColor = Color.White;
				GameConsole.BackgroundColor = GameConsole.BaseBackgroundColor;

				GameConsole.DrawRectangle(0, 0, GameConsole.W, GameConsole.H);
				DrawCntrl.ResetTransformMatrix();

				Player player = (Player)Objects.ObjFind<Player>(0);
				GameConsole.DrawChar('@', player.Pos);
			}
			
		}

	}
}
