using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using System.Xml;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Flucoldache.Battle;

namespace Flucoldache
{
	public class Pause : GameObj
	{
				
		SelectionMenu _menu;
		string[] _menuOptions;
		Vector2 _menuPos = new Vector2(1, 1);
		Vector2 _menuSize = new Vector2(Strings.FullscreenOn.Length + 2, 3);

		public Pause()
		{
			Depth = -9;

			_menuOptions = new string[]
			{
				Strings.Continue,
				Strings.FullscreenOff,
				Strings.Exit
			};
		
			_menu = new SelectionMenu("", _menuOptions, _menuPos, _menuSize);

			if (GameCntrl.WindowManager.IsFullScreen)
			{
				_menu.Items[1] = Strings.FullscreenOn;
			}
		}

		
		public override void Update()
		{
			
			if (_menu.Activated)
			{
				switch(_menu.SelectedItem)
				{
					case 0:
						SoundController.PlaySound(SoundController.Back);
						Objects.Destroy(_menu);
						Objects.Destroy(this);
					break;

					case 1:
						if (GameCntrl.WindowManager.IsFullScreen)
						{
							_menu.Items[1] = Strings.FullscreenOff;
							GameCntrl.WindowManager.SetFullScreen(false);
						}
						else
						{
							_menu.Items[1] = Strings.FullscreenOn;
							GameCntrl.WindowManager.SetFullScreen(true);
						}
						_menu.Activated = false;
					break;

					case 2:
						GameCntrl.MyGame.Exit();
					break;
				}
			}

		}
	}
}
