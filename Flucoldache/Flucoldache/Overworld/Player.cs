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

		public static Vector2[] Rotation = {Vector2.UnitX, -Vector2.UnitY, -Vector2.UnitX, Vector2.UnitY};

		SelectionMenu _menu;
		string[] _menuOptions = {"Инвентарь", "Зелья", "Назад"};
		Vector2 _menuPos = new Vector2(1, 1);
		Vector2 _menuSize = new Vector2(12, 6);


		public Player(Vector2 pos)
		{
			Pos = pos;
			Depth = -90000;
		}

		public override void Update()
		{
			Terr = (Terrain)Objects.ObjFind<Terrain>(0);
			
			


			if (!EditorMode)
			{				

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
					foreach(Vector2 rot in Rotation)
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


				#region Camera

				Camera currentCamera = DrawCntrl.Cameras[0];

				Vector2 screenPos = (Pos - new Vector2(Terrain.HorCamBorder, Terrain.VerCamBorder)) * GameConsole.CharSize - new Vector2(currentCamera.X, currentCamera.Y);
				Vector2 screenSize = new Vector2(GameConsole.W - 1 - Terrain.HorCamBorder * 2, GameConsole.H - 1 - Terrain.VerCamBorder * 2) * GameConsole.CharSize;

				if (screenPos.X < 0)
				{
					currentCamera.X += screenPos.X;
				}
				if (screenPos.Y < 0)
				{
					currentCamera.Y += screenPos.Y;
				}
				if (screenPos.X >= screenSize.X)
				{
					currentCamera.X += screenPos.X - screenSize.X;
				}
				if (screenPos.Y >= screenSize.Y)
				{
					currentCamera.Y += screenPos.Y - screenSize.Y;
				}


				#endregion Camera 
			}
		}



		public override void Draw()
		{
			GameConsole.ForegroundColor = Color.White;
			GameConsole.BackgroundColor = Terr.GetTile(Pos).BackgroundColor;
			GameConsole.DrawChar(Char, (int)Pos.X, (int)Pos.Y);
		}

		public override void DrawEnd()
		{
			base.DrawEnd();

			if (_menu != null)
			{
				DrawCntrl.SetTransformMatrix(Matrix.CreateTranslation(Vector3.Zero));
				Inventory inv = (Inventory)Objects.ObjFind<Inventory>(0);
				GameConsole.ForegroundColor = Color.Gray;
				GameConsole.BackgroundColor = Color.Black;
				GameConsole.DrawText("HP: " + inv.Health.ToString().PadLeft(3) + "/" + inv.MaxHealth, _menuPos + Vector2.UnitY * 4);

				GameConsole.ForegroundColor = Color.Red;
				GameConsole.BackgroundColor = new Color(32, 0, 0);
				GameConsole.DrawRectangle((int)_menuPos.X, (int)_menuPos.Y + 5, (int)_menuSize.X, 1);
				GameConsole.DrawProgressBar((int)_menuPos.X, (int)_menuPos.Y + 5, (int)_menuSize.X, ((float)inv.Health) / ((float)inv.MaxHealth));
				DrawCntrl.ResetTransformMatrix();
			}
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
