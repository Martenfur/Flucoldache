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
using System.IO;

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
			LMB + P - Pick tile under cursor.
		Object mode:
			A - Edit argument.
			O - Select object.
		*/

		Keys PalettesHotkey = Keys.C;
		Keys CharsHotkey = Keys.H;
		Keys ResetCameraHotkey = Keys.Q;
		Keys TileModeHotkey = Keys.T;
		Keys CollisionModeHotkey = Keys.C;
		Keys ObjectModeHotkey = Keys.O;
		Keys ToggleSolidDisplayHotkey = Keys.L;
		Keys ObjArgumentHotkey = Keys.A;
		Keys ObjSelectHotkey = Keys.O;
		Keys SaveHotkey = Keys.S;
		Keys LoadHotkey = Keys.L;
		Keys TilePickHotkey = Keys.P;
		Keys ExitHotkey = Keys.Escape;

		public enum Mode
		{
			Tiles,
			Collision,
			Objects
		}

		Color UIFgColor = Color.Gray;
		Color UIBgColor = new Color(45, 61, 61);
		Color UIImportantColor = Color.White;

		Mode CurrentMode = Mode.Tiles;

		Terrain Terrain;
		
		Vector2 SelectionMenuPos = new Vector2(2, 2);
		Vector2 SelectionMenuSize = new Vector2(16, 16);
		bool SelectionMenuActive = false;
		int SelectionMenuMode = 0; // 0 - palettes, 1 - chars
		
		Color[][] Palettes = 
		{
			new Color[]{Color.Gray, Color.Black},
			new Color[]{Color.Black, Color.Gray},
			new Color[]{Color.White, new Color(175, 93, 35)},
		};

		int CurrentPalette = 0;
		char CurrentChar = 'a';
		
		Vector2 TileMousePos;
		Vector2 TileScreenMousePos;

		Alarm _movementAlarm = new Alarm();

		/// <summary>
		/// Blocks mouse until button release.
		/// </summary>
		bool ClickBlock = false;

		string MapFileName = "";
		
		OverworldObj CurrentObject;
		Type[] ObjectTypes = 
		{
			Type.GetType("Flucoldache.Overworld.Player"),
			Type.GetType("Flucoldache.Overworld.DialogueTrigger"),
			Type.GetType("Flucoldache.Overworld.LootContainer"),
		};
		int CurrentObjectType = 0;

		bool ObjectDragged;

		bool ObjArgumentMenuActive = false;
		Vector2 ObjArgumentMenuSize = new Vector2(GameConsole.W - 4, 1);
		Vector2 ObjArgumentMenuPos = new Vector2(2, 2);

		bool ObjSelectMenuActive = false;
		Vector2 ObjSelectMenuSize;
		Vector2 ObjSelectMenuPos = new Vector2(2, 2);

		Alarm ExitAlarm = new Alarm();

		public MapEditor()
		{
			Terrain = new Terrain(32, 16);

			DrawCntrl.Cameras[0].X = -GameConsole.CharSize.X;
			DrawCntrl.Cameras[0].Y = -GameConsole.CharSize.Y;

			ObjSelectMenuSize = new Vector2(24, ObjectTypes.Length);
			Depth = -9000;
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
					Input.KeyboardClear();
				}
				if (Input.KeyboardCheckPress(CollisionModeHotkey))
				{
					CurrentMode = Mode.Collision;
					Terrain.DisplaySolids = true;
				}
			}

			#endregion Mode selection


			#region Saving/Loading

			if (Input.KeyboardCheck(Keys.LeftControl))
			{
				if (Input.KeyboardCheckPress(SaveHotkey))
				{
					string filename = MapFileName;
					if (Input.KeyboardCheck(Keys.LeftShift) || filename == "")
					{
						System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog();
						dialog.Title = "Save map file";
						dialog.Filter = "Map File|*.map";
						dialog.ShowDialog();
						filename = dialog.FileName;
					}
			
					if (filename != "")
					{
						SaveMap(Terrain, filename);
						MapFileName = filename;
					}
				}

				if (Input.KeyboardCheckPress(LoadHotkey))
				{
					
					System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
					dialog.Title = "Save map file";
					dialog.Filter = "Map File|*.map";
					dialog.ShowDialog();
					if (dialog.FileName != "")
					{
						Terrain = LoadMap(dialog.FileName, true);
						DrawCntrl.Cameras[0].X = -GameConsole.CharSize.X;
						DrawCntrl.Cameras[0].Y = -GameConsole.CharSize.Y;
						MapFileName = dialog.FileName;
					}
				}
			}

			#endregion Saving/Loading


			#region Exiting
			ExitAlarm.Update();
			if (Input.KeyboardCheckPress(ExitHotkey))
			{
				ExitAlarm.Set(2);
			}
			if (!Input.KeyboardCheck(ExitHotkey))
			{
				ExitAlarm.Reset();
			}
			if (ExitAlarm.Triggered)
			{
				Objects.Destroy(this);
			}
			#endregion Exiting

			MoveCamera();
			
			// Menus and stuff.
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

			#region Argument menu

			if (ObjArgumentMenuActive)
			{
				foreach(char ch in Input.KeyboardString)
				{
					if (ch == (char)Keys.Back)
					{
						if (CurrentObject.Argument.Length > 0)
						{
							CurrentObject.Argument = CurrentObject.Argument.Remove(CurrentObject.Argument.Length - 1, 1);
						}
					}
					else
					{
						if (!Char.IsControl(ch))
						{
							CurrentObject.Argument += ch;
						}
					}
				}

				if (Input.KeyboardCheck(Keys.Enter) || Input.KeyboardCheck(Keys.Escape) || Input.MouseCheck(Input.MB.Left))
				{
					ClickBlock = true;
					ObjArgumentMenuActive = false;
				}

				Input.IOClear();
			}

			#endregion Argument menu

			#region Object menu

			if (ObjSelectMenuActive)
			{
				if (Input.MouseCheckPress(Input.MB.Left))
				{
					if (TileScreenMousePos.X >= ObjSelectMenuPos.X
					&& TileScreenMousePos.X < ObjSelectMenuPos.X + ObjSelectMenuSize.X
					&& TileScreenMousePos.Y >= ObjSelectMenuPos.Y
					&& TileScreenMousePos.Y < ObjSelectMenuPos.Y + ObjSelectMenuSize.Y)
					{
						CurrentObjectType = (int)(TileScreenMousePos.Y - ObjSelectMenuPos.Y);
					}
					ClickBlock = true;
					ObjSelectMenuActive = false;
					Input.MouseClear();
				}
			}

			#endregion Object menu
			// Menus and stuff.

		}

		public override void Update()
		{
			#region Tiles
			if (CurrentMode == Mode.Tiles)
			{
				if ((Input.MouseCheck(Input.MB.Left) && !Input.KeyboardCheck(Keys.LeftControl))
				|| Input.MouseCheckPress(Input.MB.Left))
				{	
					if (Terrain.InBounds(TileMousePos))
					{
						Tile tile = Terrain.GetTile(TileMousePos);

						if (Input.KeyboardCheck(TilePickHotkey))
						{
							CurrentChar = tile.Char;
							
							for(var i = 0; i < Palettes.Length; i += 1)
							{
								if (Palettes[i][0] == tile.ForegroundColor && Palettes[i][1] == tile.BackgroundColor)
								{
									CurrentPalette = i;
									break;
								}
							}
							ClickBlock = true;
						}
						else
						{
							if (!Input.KeyboardCheck(Keys.LeftAlt))
							{
								tile.Char = CurrentChar;
							}
							tile.ForegroundColor = Palettes[CurrentPalette][0];
							tile.BackgroundColor = Palettes[CurrentPalette][1];
						}
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
					
					if (Terrain.InBounds(TileMousePos))
					{
						Tile tile = Terrain.GetTile(TileMousePos);
						
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
			if (CurrentMode == Mode.Objects)
			{
				// Creating object.
				if (Input.MouseCheckPress(Input.MB.Left))
				{
					ObjectDragged = false;
					foreach(OverworldObj obj in Objects.GetList<OverworldObj>())
					{
						if (obj.Pos == TileMousePos)
						{
							CurrentObject = obj;
							ObjectDragged = true;
							break;
						}
					}
			
					if (!ObjectDragged)
					{
						OverworldObj obj = CreateObj(ObjectTypes[CurrentObjectType], TileMousePos);
						CurrentObject = obj;
						ObjectDragged = true;
					}
					
				}
				// Creating object.

				// Moving object.
				if (ObjectDragged && CurrentObject != null)
				{
					if (Input.MouseCheck(Input.MB.Left))
					{
						CurrentObject.Pos = TileMousePos;
					}
					else
					{
						ObjectDragged = false;
					}
				}
				// Moving object.

				// Deleting object.
				if (Input.MouseCheckPress(Input.MB.Right))
				{
					ObjectDragged = false;
					foreach(OverworldObj obj in Objects.GetList<OverworldObj>())
					{
						if (obj.Pos == TileMousePos)
						{
							Objects.Destroy(obj);
							CurrentObject = null;
						}
					}
				}
				// Deleting object.



				if (Input.KeyboardCheckPress(ObjArgumentHotkey) && CurrentObject != null)
				{ 
					ObjArgumentMenuActive = true;
				}

				if (Input.KeyboardCheckPress(ObjSelectHotkey))
				{ 
					ObjSelectMenuActive = true;
				}

			}
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
			if (CurrentMode == Mode.Objects)
			{
				DrawObjectOverlays();
			}


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

			if (CurrentMode == Mode.Tiles)
			{
				GameConsole.ForegroundColor = Palettes[CurrentPalette][0];
				GameConsole.BackgroundColor = Palettes[CurrentPalette][1];
				GameConsole.DrawText("Char: " + CurrentChar, caption.Length + 1, 0);
			}
			

			if (CurrentMode == Mode.Objects)
			{
				string className = ObjectTypes[CurrentObjectType].ToString().Split('.').Last();
				GameConsole.DrawText("Obj: " + className, caption.Length + 1, 0);

				if (CurrentObject != null)
				{
					className = CurrentObject.ToString().Split('.').Last();
					GameConsole.DrawText("Selected: " + className + " | " + CurrentObject.ArgumentName + ": " + CurrentObject.Argument, 1, GameConsole.H - 1);
				}
			}


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

			if (ObjArgumentMenuActive)
			{
				DrawObjArgumentMenu();
			}
			
			if (ObjSelectMenuActive)
			{
				DrawObjSelectMenu();
			}

			

			DrawCntrl.CurrentColor = Color.White;
			DrawCntrl.DrawRectangle(TileScreenMousePos * GameConsole.CharSize, TileScreenMousePos * GameConsole.CharSize + GameConsole.CharSize, true);
			
			

			DrawCntrl.ResetTransformMatrix();

	
			
		}

		public override void Destroy()
		{
			new MainMenu();
			foreach(OverworldObj obj in Objects.GetList<OverworldObj>())
			{
				Objects.Destroy(obj);
			}
			foreach(Terrain obj in Objects.GetList<Terrain>())
			{
				Objects.Destroy(obj);
			}
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

		void DrawObjArgumentMenu()
		{
			GameConsole.ForegroundColor = UIFgColor;
			GameConsole.BackgroundColor = UIBgColor;
			GameConsole.DrawFrame(ObjArgumentMenuPos - Vector2.One, ObjArgumentMenuSize + new Vector2(2, 2));
			GameConsole.DrawText(CurrentObject.ArgumentName + ":", (int)ObjArgumentMenuPos.X, (int)ObjArgumentMenuPos.Y - 1);

			GameConsole.ForegroundColor = Color.Gray;
			GameConsole.BackgroundColor = Color.Black;
			GameConsole.DrawRectangle(ObjArgumentMenuPos, ObjArgumentMenuSize);
			GameConsole.DrawText(CurrentObject.Argument, (int)ObjArgumentMenuPos.X, (int)ObjArgumentMenuPos.Y);
		}

		void DrawObjSelectMenu()
		{
			GameConsole.ForegroundColor = UIFgColor;
			GameConsole.BackgroundColor = UIBgColor;
			GameConsole.DrawFrame(ObjSelectMenuPos - Vector2.One, ObjSelectMenuSize + new Vector2(2, 2));

			string className;
			for(var i = 0; i < ObjSelectMenuSize.Y; i += 1)
			{
				className = ObjectTypes[i].ToString().Split('.').Last().PadRight((int)ObjSelectMenuSize.X);

				if (TileScreenMousePos.Y == ObjSelectMenuPos.Y + i 
				&& TileScreenMousePos.X >= ObjSelectMenuPos.X
				&& TileScreenMousePos.X < ObjSelectMenuPos.X + ObjSelectMenuSize.X)
				{
					GameConsole.ForegroundColor = UIBgColor;
					GameConsole.BackgroundColor = UIFgColor;
				}
				else
				{
					GameConsole.ForegroundColor = UIFgColor;
					GameConsole.BackgroundColor = UIBgColor;
				}

				GameConsole.DrawText(className, (int)ObjSelectMenuPos.X, (int)ObjSelectMenuPos.Y + i);
			}
		}

		void DrawObjectOverlays()
		{
			foreach(OverworldObj obj in Objects.GetList<OverworldObj>())
			{
				if (obj == CurrentObject)
				{
					DrawCntrl.CurrentColor = Color.Yellow;
				}
				else
				{
					DrawCntrl.CurrentColor = Color.Blue;
				}

				DrawCntrl.DrawRectangle(obj.Pos * GameConsole.CharSize, obj.Pos * GameConsole.CharSize + GameConsole.CharSize, true);
			}
		}



		void MoveCamera()
		{
			Vector2 movement = Vector2.Zero;

			
			if (Input.KeyboardCheckPress(ResetCameraHotkey))
			{
				DrawCntrl.Cameras[0].X = -GameConsole.CharSize.X;
				DrawCntrl.Cameras[0].Y = -GameConsole.CharSize.Y;
			}
			
			if (Input.KeyboardCheck(Controls.KeyUp))
			{
				movement.Y += -1;
			}
			if (Input.KeyboardCheck(Controls.KeyDown))
			{
				movement.Y += 1;
			}
			if (Input.KeyboardCheck(Controls.KeyLeft))
			{
				movement.X += -1;
			}
			if (Input.KeyboardCheck(Controls.KeyRight))
			{
				movement.X += 1;
			}
			
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



		OverworldObj CreateObj(Type type, Vector2 pos)
		{
			OverworldObj obj = (OverworldObj)Activator.CreateInstance(type);
			obj.Pos = pos;
			obj.EditorMode = true;
			
			return obj;
		}


		public static void SaveMap(Terrain terrain, string filename)
		{

			List<byte> bytes = new List<byte>();

			#region Saving terrain

			bytes.AddRange(BitConverter.GetBytes(terrain.TileMap.GetLength(0)));
			bytes.AddRange(BitConverter.GetBytes(terrain.TileMap.GetLength(1)));

			foreach (Tile tile in terrain.TileMap)
			{
				bytes.AddRange(BitConverter.GetBytes((int)tile.Type));
				bytes.AddRange(BitConverter.GetBytes(tile.Char));
				bytes.AddRange(BitConverter.GetBytes(tile.ForegroundColor.PackedValue));
				bytes.AddRange(BitConverter.GetBytes(tile.BackgroundColor.PackedValue));
			}

			#endregion Saving terrain


			#region Saving objects

			List<OverworldObj> objects = Objects.GetList<OverworldObj>();

			bytes.AddRange(BitConverter.GetBytes(objects.Count));

			foreach (OverworldObj obj in objects)
			{
				// Class name.
				byte[] serializedStr = Encoding.Unicode.GetBytes(obj.GetType().ToString());
				bytes.AddRange(BitConverter.GetBytes(serializedStr.Length));
				bytes.AddRange(serializedStr);

				// Position.
				bytes.AddRange(BitConverter.GetBytes((int)obj.Pos.X));
				bytes.AddRange(BitConverter.GetBytes((int)obj.Pos.Y));


				// Argument.
				serializedStr = Encoding.Unicode.GetBytes(obj.Argument);
				bytes.AddRange(BitConverter.GetBytes(serializedStr.Length));
				bytes.AddRange(serializedStr);

			}

			#endregion Saving objects

			File.WriteAllBytes(filename, bytes.ToArray());
		}

		public static Terrain LoadMap(string fileName, bool editorMode)
		{

			foreach(OverworldObj obj in Objects.GetList<OverworldObj>())
			{
				Objects.Destroy(obj);
			}
			foreach(Terrain obj in Objects.GetList<Terrain>())
			{
				Objects.Destroy(obj);
			}
			

			byte[] buffer = File.ReadAllBytes(fileName);

			int pointer = 0;

			#region Loading terrain
			
			int terrW, terrH;
			
			terrW = ReadInt(buffer, ref pointer);
			terrH = ReadInt(buffer, ref pointer);

			Terrain terrain = new Terrain(terrW, terrH);

			for(var x = 0; x < terrW; x += 1)	
			{
				for(var y = 0; y < terrH; y += 1)	
				{
					terrain.TileMap[x, y].Type = (Tile.TileType)ReadInt(buffer, ref pointer);
					terrain.TileMap[x, y].Char = ReadChar(buffer, ref pointer);
					terrain.TileMap[x, y].ForegroundColor = new Color(ReadUInt(buffer, ref pointer));
					terrain.TileMap[x, y].BackgroundColor = new Color(ReadUInt(buffer, ref pointer));
				}
			}

			#endregion Loading terrain
			

			#region Loading objects
			
			int objCount = ReadInt(buffer, ref pointer);

			for(var i = 0; i < objCount; i += 1)
			{
				int strLen = ReadInt(buffer, ref pointer); // Class string length.
				string className = ReadString(buffer, ref pointer, strLen);

				Vector2 pos = Vector2.Zero;

				pos.X = ReadInt(buffer, ref pointer);
				pos.Y = ReadInt(buffer, ref pointer);

				strLen = ReadInt(buffer, ref pointer); // Class string length.
				string argument = ReadString(buffer, ref pointer, strLen);

				OverworldObj obj = (OverworldObj)Activator.CreateInstance(Type.GetType(className));
				obj.Pos = pos;
				obj.EditorMode = editorMode;
				obj.Argument = argument;
				if (!editorMode)
				{
					obj.ProccessArgument();
				}
			}

			#endregion Loading objects
			
			return terrain;
		}

		static int ReadInt(byte[] buffer, ref int pointer)
		{
			byte[] intBuffer = new byte[4];
			Array.Copy(buffer, pointer, intBuffer, 0, intBuffer.Length);
			pointer += 4;
			return BitConverter.ToInt32(intBuffer, 0);
		}

		static uint ReadUInt(byte[] buffer, ref int pointer)
		{
			byte[] intBuffer = new byte[4];
			Array.Copy(buffer, pointer, intBuffer, 0, intBuffer.Length);
			pointer += 4;
			return BitConverter.ToUInt32(intBuffer, 0);
		}

		static char ReadChar(byte[] buffer, ref int pointer)
		{
			byte[] intBuffer = new byte[2];
			Array.Copy(buffer, pointer, intBuffer, 0, intBuffer.Length);
			pointer += 2;
			return BitConverter.ToChar(intBuffer, 0);
		}

		static string ReadString(byte[] buffer, ref int pointer, int count)
		{
			byte[] intBuffer = new byte[count];
			Array.Copy(buffer, pointer, intBuffer, 0, intBuffer.Length);
			pointer += count;
			return Encoding.Unicode.GetString(intBuffer);
		}

	}
}
