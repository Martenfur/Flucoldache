﻿using System.Collections.Generic;
using Flucoldache.Battle;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;

namespace Flucoldache.Overworld
{
	public class Player : OverworldObj
	{
		public List<string> RandomEncounterArenas;
		public bool RandomEncountersEnabled;
		public float RandomEncountersChance = 0.1f;
		public int RandomEncountersMaxCooldown = 22;
		public int RandomEncountersCooldown;

		public float WalkSpd = 7; // Cells/second.

		public const char Char = '@';

		private Alarm _movementAlarm = new Alarm();

		Terrain Terr;

		public static Vector2[] Rotation = {Vector2.UnitX, -Vector2.UnitY, -Vector2.UnitX, Vector2.UnitY};

		SelectionMenu _menu;
		string[] _menuOptions;
		Vector2 _menuPos = new Vector2(1, 1);
		Vector2 _menuSize = new Vector2(12, 6);


		public Player()
		{
			Depth = -9;

			_menuOptions = new string[]
			{
				Strings.Inventory,
				Strings.Potions,
				Strings.Back
			};
		}

		public Player(Vector2 pos)
		{
			Pos = pos;
			Depth = -9;

			_menuOptions = new string[]
			{
				Strings.Inventory,
				Strings.Potions,
				Strings.Back
			};
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


					if (RandomEncountersEnabled && RandomEncountersCooldown <= 0)
					{
						if (GameplayController.Random.NextDouble() <= RandomEncountersChance)
						{
							int arenaId = (int) (GameplayController.Random.NextDouble() * RandomEncounterArenas.Count);
							new ArenaAppearEffect(RandomEncounterArenas[arenaId]);
							RandomEncountersCooldown = RandomEncountersMaxCooldown;
						}
					}

					RandomEncountersCooldown -= 1;

					if (RandomEncountersCooldown < 0)
					{
						RandomEncountersCooldown = 0;
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
				if (Controls.KeyCheckPress(Microsoft.Xna.Framework.Input.Keys.Escape))
				{
					new Pause();
				}

				if (Controls.KeyCheckPress(Controls.KeyB))
				{
					_menu = new SelectionMenu(Strings.PlayerMenu, _menuOptions, _menuPos, _menuSize);
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
						case 2:
						SoundController.PlaySound(SoundController.Back);
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


				if (currentCamera.X / GameConsole.CharSize.X < Terrain.CamMinPos.X )
				{
					currentCamera.X = Terrain.CamMinPos.X * GameConsole.CharSize.X;
				}
				if (currentCamera.Y / GameConsole.CharSize.Y < Terrain.CamMinPos.Y)
				{
					currentCamera.Y = Terrain.CamMinPos.Y * GameConsole.CharSize.Y;
				}
				if (currentCamera.X / GameConsole.CharSize.X + GameConsole.W > Terrain.CamMaxPos.X)
				{
					currentCamera.X = (Terrain.CamMaxPos.X - GameConsole.W) * GameConsole.CharSize.X;
				}
				if (currentCamera.Y  / GameConsole.CharSize.Y + GameConsole.H> Terrain.CamMaxPos.Y)
				{
					currentCamera.Y = (Terrain.CamMaxPos.Y - GameConsole.H) * GameConsole.CharSize.Y;
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
				GameConsole.ForegroundColor = GameConsole.BaseForegroundColor;
				GameConsole.BackgroundColor = GameConsole.BaseBackgroundColor;
				GameConsole.DrawText("HP: " + inv.Health.ToString().PadLeft(3) + "/" + inv.MaxHealth, _menuPos + Vector2.UnitY * 4);

				GameConsole.ForegroundColor = GameConsole.HealthForegroundColor;
				GameConsole.BackgroundColor = GameConsole.HealthBackgroundColor;
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
