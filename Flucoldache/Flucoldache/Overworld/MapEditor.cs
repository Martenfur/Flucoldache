using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Flucoldache.Overworld
{
	public class MapEditor : GameObj
	{
		/*
		Map file format:
		2b - Terrain width.
		2b - Terrain height.
		
		for(w*h)
		{
			1b - Tile type.
			1b - Tile char.
			1b - Tile color.
		}

		4b - Object amount.

		for(objAm)
		{
			1b - type
			1b - argument length
			nb - argument
		}
		*/

		/*
		Hotkeys:
		Q - Reset camera.
		Arrows - Move camera.
		Shift + Arrows - Move camera quicker.

		Ctrl + T - Tile mode.
		Ctrl + O - Object mode.
		Ctrl + C - Collision mode.

		L - Toggle solids display.

		Tile mode:
			LMB + Ctrl - Place one tile.
			LMB + Alt - Color only.
			C - Choose color set.
			H - Choose char.
		*/

		public enum Mode
		{
			Tiles,
			Collision,
			Objects
		}

		Color UIFgColor = Color.Gray;
		Color UIBgColor = new Color(45, 61, 61);

		Mode CurrentMode = Mode.Tiles;

		Terrain Terrain;
		
		Vector2 SelectionMenuPos = new Vector2(2, 2);
		Vector2 SelectionMenuSize = new Vector2(16, 16);
		bool SelectionMenuActive = false;
		int SelectionMenuMode = 0; // 0 - palettes, 1 - chars
		
		Color[][] Palettes = 
		{
			new Color[]{Color.Gray, Color.Black},
			new Color[]{Color.Black, Color.White},
			new Color[]{Color.Black, Color.White},
		};


		Keys PalettesHotkey = Keys.C;
		Keys CharsHotkey = Keys.H;
		Keys ResetCameraHotkey = Keys.Q;
		Keys TileModeHotkey = Keys.T;
		Keys CollisionModeHotkey = Keys.C;
		Keys ObjectModeHotkey = Keys.O;
		Keys ToggleSolidDisplayHotkey = Keys.L;

		int CurrentPalette = 0;
		char CurrentChar = 'a';

		
		Vector2 TileMousePos;
		Vector2 TileScreenMousePos;

		Alarm _movementAlarm = new Alarm();


		/// <summary>
		/// Blocks mouse until button release.
		/// </summary>
		bool ClickBlock = false;

		public MapEditor()
		{
			Terrain = new Terrain(32, 16);
			
		}

		public override void UpdateBegin()
		{
			TileScreenMousePos = Input.ScreenMousePos / GameConsole.CharSize;
			TileScreenMousePos.X = (float)Math.Floor(TileScreenMousePos.X);
			TileScreenMousePos.Y = (float)Math.Floor(TileScreenMousePos.Y);
			
			if (!Input.MouseCheck(Input.MB.Left))
			{
				ClickBlock = false;
			}
			if (ClickBlock)
			{
				Input.MouseClear();
			}

			#region Mode selection

			if (Input.KeyboardCheck(Keys.LeftControl))
			{
				if (Input.KeyboardCheckPress(TileModeHotkey))
				{
					CurrentMode = Mode.Tiles;
				}
				if (Input.KeyboardCheckPress(ObjectModeHotkey))
				{
					CurrentMode = Mode.Objects;
				}
				if (Input.KeyboardCheckPress(CollisionModeHotkey))
				{
					CurrentMode = Mode.Collision;
					Terrain.DisplaySolids = true;
				}
			}

			#endregion Mode selection


			MoveCamera();
			

			#region Selection menu
			if (SelectionMenuActive)
			{
				if (Input.MouseCheckPress(Input.MB.Left))
				{
					var chars = GameConsole.Font.GetGlyphs().ToArray();

					var index = 0;
					for(var y = 0; y < SelectionMenuSize.Y; y += 1)	
					{
						for(var x = 0; x < SelectionMenuSize.X; x += 1)
						{
							if (SelectionMenuMode == 0)
							{
								// Palette select.
								if (index >= Palettes.Length)
								{
									y = (int)SelectionMenuSize.Y;
									break;
								}

								if (TileScreenMousePos - SelectionMenuPos == new Vector2(x, y))
								{
									CurrentPalette = index;
									y = (int)SelectionMenuSize.Y;
									break;
								}
								// Palette select.
							}
							else
							{
								// Chars select.
								if (index >= chars.Length)
								{
									y = (int)SelectionMenuSize.Y;
									break;
								}

								if (TileScreenMousePos - SelectionMenuPos == new Vector2(x, y))
								{
									CurrentChar = chars[index].Key;
									y = (int)SelectionMenuSize.Y;
									break;
								}
								// Chars select.
							}

							index += 1;
						}
					}
					
					SelectionMenuActive = false;
					ClickBlock = true;
					Input.MouseClear();
				}

				if ((SelectionMenuMode == 0 && Input.KeyboardCheckPress(PalettesHotkey)) 
				|| (SelectionMenuMode == 1 && Input.KeyboardCheckPress(CharsHotkey)) 
				|| Input.KeyboardCheckPress(Keys.Escape))
				{
					SelectionMenuActive = false;
					Input.KeyboardClear();
				}
			}
			#endregion Selection menu



		}

		public override void Update()
		{
			#region Tiles
			if (CurrentMode == Mode.Tiles)
			{
				if ((Input.MouseCheck(Input.MB.Left) && !Input.KeyboardCheck(Keys.LeftControl))
				|| Input.MouseCheckPress(Input.MB.Left))
				{	
					Tile tile = Terrain.GetTile(TileMousePos);
					
					if (tile != null)
					{
						if (!Input.KeyboardCheck(Keys.LeftAlt))
						{
							tile.Char = CurrentChar;
						}
						tile.ForegroundColor = Palettes[CurrentPalette][0];
						tile.BackgroundColor = Palettes[CurrentPalette][1];
					}
					else
					{
						ExpandMap();
					}
				}


				if (Input.KeyboardCheckPress(PalettesHotkey))
				{
					SelectionMenuActive = true;
					SelectionMenuMode = 0;
				}

				if (Input.KeyboardCheckPress(CharsHotkey))
				{
					SelectionMenuActive = true;
					SelectionMenuMode = 1;
				}
			}
			#endregion Tiles



			#region Collision
			if (CurrentMode == Mode.Collision)
			{
				if (Input.MouseCheck(Input.MB.Left) || Input.MouseCheck(Input.MB.Right))
				{	
					Tile tile = Terrain.GetTile(TileMousePos);
					if (tile == null)
					{
						if (Input.MouseCheck(Input.MB.Left))
						{
							tile.Type = Tile.TileType.Solid;
						}
						else
						{
							tile.Type = Tile.TileType.Passable;
						}
					}
					else
					{
						ExpandMap();
					}
				}
			}

			if (Input.KeyboardCheckPress(ToggleSolidDisplayHotkey))
			{
				Terrain.DisplaySolids = !Terrain.DisplaySolids;
			}
			#endregion Collision

			
			#region Objects



			#endregion Objects
		}

		public override void DrawBegin()
		{
			TileMousePos = Input.MousePos / GameConsole.CharSize;
			TileMousePos.X = (float)Math.Floor(TileMousePos.X);
			TileMousePos.Y = (float)Math.Floor(TileMousePos.Y);
		}

		public override void DrawEnd()
		{
			DrawCntrl.CurrentColor = UIBgColor;
			DrawCntrl.DrawRectangle(0, 0, Terrain.TileMap.GetLength(0) * GameConsole.CharSize.X, Terrain.TileMap.GetLength(1) * GameConsole.CharSize.Y, true);

			DrawCntrl.SetTransformMatrix(Matrix.CreateTranslation(Vector3.Zero));

			GameConsole.ForegroundColor = UIFgColor;
			GameConsole.BackgroundColor = UIBgColor;
			GameConsole.DrawFrame(0, 0, GameConsole.W, GameConsole.H);
			
			string mode = "";
			if (CurrentMode == Mode.Tiles)
			{
				mode = "Tile mode";
			}
			if (CurrentMode == Mode.Collision)
			{
				mode = "Collision mode";
			}
			if (CurrentMode == Mode.Objects)
			{
				mode = "Object mode";
			}


			string caption = mode 
			+ " | mx:" + TileMousePos.X 
			+ " my:" + TileMousePos.Y 
			+ " | fps:" + GameCntrl.Fps 
			+ " | map w:" + Terrain.TileMap.GetLength(0)
			+ " map h:" + Terrain.TileMap.GetLength(1) 
			+ " | ";

			GameConsole.DrawText(caption, 1, 0);
			GameConsole.ForegroundColor = Palettes[CurrentPalette][0];
			GameConsole.BackgroundColor = Palettes[CurrentPalette][1];
			GameConsole.DrawText("Char: " + CurrentChar, caption.Length + 1, 0);
			

			if (SelectionMenuActive)
			{
				if (SelectionMenuMode == 0)
				{
					DrawPaletteSelectionMenu();
				}
				else
				{
					DrawCharSelectionMenu();
				}
			}
			
			DrawCntrl.CurrentColor = Color.White;
			DrawCntrl.DrawRectangle(TileScreenMousePos * GameConsole.CharSize, TileScreenMousePos * GameConsole.CharSize + GameConsole.CharSize, true);
			
			DrawCntrl.ResetTransformMatrix();

	
			
		}

		void DrawPaletteSelectionMenu()
		{
			GameConsole.ForegroundColor = UIFgColor;
			GameConsole.BackgroundColor = UIBgColor;
			GameConsole.DrawFrame(SelectionMenuPos - Vector2.One, SelectionMenuSize + new Vector2(2, 2));
			GameConsole.DrawRectangle(SelectionMenuPos, SelectionMenuSize);
			GameConsole.DrawText("Palettes:", (int)SelectionMenuPos.X, (int)SelectionMenuPos.Y - 1);

			for(var y = 0; y < SelectionMenuSize.Y; y += 1)	
			{
				for(var x = 0; x < SelectionMenuSize.X; x += 1)
				{
					int paletteId = y * (int)SelectionMenuSize.X + x;
					if (paletteId >= Palettes.Length)
					{
						return;
					}
					
					GameConsole.ForegroundColor = Palettes[paletteId][0];
					GameConsole.BackgroundColor = Palettes[paletteId][1];
					GameConsole.DrawChar('.', SelectionMenuPos + new Vector2(x, y));
				}
			}
		}

		void DrawCharSelectionMenu()
		{
			GameConsole.ForegroundColor = UIFgColor;
			GameConsole.BackgroundColor = UIBgColor;
			GameConsole.DrawFrame(SelectionMenuPos - Vector2.One, SelectionMenuSize + new Vector2(2, 2));
			GameConsole.DrawRectangle(SelectionMenuPos, SelectionMenuSize);
			GameConsole.DrawText("Characters:", (int)SelectionMenuPos.X, (int)SelectionMenuPos.Y - 1);

			var chars = GameConsole.Font.GetGlyphs().ToArray();

			for(var y = 0; y < SelectionMenuSize.Y; y += 1)	
			{
				for(var x = 0; x < SelectionMenuSize.X; x += 1)
				{
					int charId = y * (int)SelectionMenuSize.X + x;
					if (charId >= chars.Length)
					{
						return;
					}
					
					GameConsole.DrawChar(chars[charId].Key, SelectionMenuPos + new Vector2(x, y));
				}
			}
		}



		void MoveCamera()
		{
			Vector2 movement = Vector2.Zero;

			
			if (Input.KeyboardCheckPress(ResetCameraHotkey))
			{
				DrawCntrl.Cameras[0].X = 0;
				DrawCntrl.Cameras[0].Y = 0;
			}
			/*
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
			*/
			_movementAlarm.Update();
			if (movement != Vector2.Zero)
			{
				if (!_movementAlarm.Active)
				{
					if (Input.KeyboardCheck(Keys.LeftShift))
					{
						_movementAlarm.Set(1f / 64f);
					}
					else
					{
						_movementAlarm.Set(1f / 10f);
					}
					_movementAlarm.Triggered = true;
				}
			}
			else
			{
				_movementAlarm.Reset();
			}
			
			if (_movementAlarm.Triggered)
			{
				DrawCntrl.Cameras[0].X += movement.X * GameConsole.CharSize.X;
				DrawCntrl.Cameras[0].Y += movement.Y * GameConsole.CharSize.Y;
			}

			if (Input.KeyboardCheck(Keys.LeftShift))
			{
				DrawCntrl.Cameras[0].X += Input.MouseWheelVal * GameConsole.CharSize.X;
			}
			else
			{
				DrawCntrl.Cameras[0].Y += Input.MouseWheelVal * GameConsole.CharSize.Y;
			}
		}

		void ExpandMap()
		{
			Vector2 newOffset = -TileMousePos;
			Vector2 newSize = new Vector2(Terrain.TileMap.GetLength(0), Terrain.TileMap.GetLength(1));

			if (TileMousePos.X >= 0)
			{
				newOffset.X = 0;
				if (TileMousePos.X > Terrain.TileMap.GetLength(0) - 1)
				{
					newSize.X = TileMousePos.X + 1;
				}
			}
			else
			{
				newSize.X -= TileMousePos.X;
			}

			if (TileMousePos.Y >= 0)
			{
				newOffset.Y = 0;
				if (TileMousePos.Y > Terrain.TileMap.GetLength(1) - 1)
				{
					newSize.Y = TileMousePos.Y + 1;
				}
			}
			else
			{
				newSize.Y -= TileMousePos.Y;
			}

			Terrain.Resize(newSize, newOffset);

			DrawCntrl.Cameras[0].X += newOffset.X * GameConsole.CharSize.X;
			DrawCntrl.Cameras[0].Y += newOffset.Y * GameConsole.CharSize.Y;

			foreach(OverworldObj obj in Objects.GetList<OverworldObj>())
			{
				obj.Pos += newOffset;
			}

		}

	}
}
