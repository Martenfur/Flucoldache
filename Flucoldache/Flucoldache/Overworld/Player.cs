using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using Microsoft.Xna.Framework.Input;
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

		Vector2[] _rotation = {Vector2.UnitX, -Vector2.UnitY, -Vector2.UnitX, Vector2.UnitY};

		SelectionMenu _menu;
		string[] _menuOptions = {"Инвентарь", "Зелья", "Назад"};
		Vector2 _menuPos = new Vector2(1, 1);
		Vector2 _menuSize = new Vector2(12, 3);


		public Player(Vector2 pos)
		{
			Pos = pos;
		}

		public override void Update()
		{
			Terr = (Terrain)Objects.ObjFind<Terrain>(0);
			
			


			if (!EditorMode)
			{				

			if (Controls.KeyCheckPress(Keys.A))
			{
				GameplayController.SaveGame();
			}
				Vector2 movement = Vector2.Zero;

				// Movement.
				if (Controls.KeyCheck(Controls.KeyUp))
				{
					movement.Y += -1;
				}

				if (Controls.KeyCheck(Controls.KeyDown))
				{
					movement.Y += 1;
				}

				if (Controls.KeyCheck(Controls.KeyLeft))
				{
					movement.X += -1;
				}

				if (Controls.KeyCheck(Controls.KeyRight))
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
					TryTriggeringObjects(Pos + movement);
					
					if (Terr.GetTile(new Vector2(Pos.X + movement.X, Pos.Y)).IsPassable())
					{
						Pos.X += movement.X;
					}

					if (Terr.GetTile(new Vector2(Pos.X, Pos.Y + movement.Y)).IsPassable())
					{
						Pos.Y += movement.Y;
					}
				}

				if (Controls.KeyCheckPress(Controls.KeyA))
				{
					foreach(Vector2 rot in _rotation)
					{
						TryTriggeringObjects(Pos + rot);
					}
				}

				#region Menu
				if (Controls.KeyCheckPress(Controls.KeyB))
				{
					_menu = new SelectionMenu("Меню", _menuOptions, _menuPos, _menuSize);
				}

				if (_menu != null && _menu.Activated)
				{
					Inventory inv = (Inventory)Objects.ObjFind<Inventory>(0);
					Objects.Destroy(_menu);

					switch(_menu.SelectedItem)
					{
						case 0:	
						inv.ShowItems();
						break;
						case 1:	
						inv.ShowPotions();
						break;
					}
					_menu = null;
				}
				#endregion Menu
			}
		}



		public override void Draw()
		{
			GameConsole.ForegroundColor = Color.White;
			GameConsole.BackgroundColor = Terr.GetTile(Pos).BackgroundColor;
			GameConsole.DrawChar(Char, (int)Pos.X, (int)Pos.Y);
		}

		void TryTriggeringObjects(Vector2 pos)
		{
			foreach(OverworldObj obj in Objects.GetList<OverworldObj>())
			{
				if (pos == obj.Pos)
				{
					if (obj.TriggerAction())
					{
						return;
					}
				}
			}
		}

	}
}
