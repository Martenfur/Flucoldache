﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using Microsoft.Xna.Framework;

namespace Flucoldache.Overworld
{
	/// <summary>
	/// Overworld terrain.
	/// </summary>
	public class Terrain : GameObj
	{
		public static int VerCamBorder = 15;
		public static int HorCamBorder = 45;
		public static Vector2 CamMinPos;
		public static Vector2 CamMaxPos;

		public Tile[,] TileMap;
		
		public bool DisplaySolids = false;

		public Tile DefaultTile;

		public Terrain(int w, int h)
		{
			SoundController.PlaySong(SoundController.Overworld);

			CamMinPos = new Vector2(1000000, 1000000);
			CamMaxPos = new Vector2(-1000000, -1000000);

			DefaultTile = new Tile();
			
			TileMap = new Tile[w, h];
			for(var x = 0; x < TileMap.GetLength(0); x += 1)
			{
				for(var y = 0; y < TileMap.GetLength(1); y += 1)
				{
					TileMap[x, y] = new Tile();
				}
			}
		}

		public void FixColors()
		{
			for(var x = 0; x < TileMap.GetLength(0); x += 1)
			{
				for(var y = 0; y < TileMap.GetLength(1); y += 1)
				{	
					if (TileMap[x, y].BackgroundColor == Color.Black && TileMap[x, y].ForegroundColor == Color.Gray)
					{
						TileMap[x, y].BackgroundColor = GameConsole.BaseBackgroundColor;
						TileMap[x, y].ForegroundColor = GameConsole.BaseForegroundColor;
					}
				}
			}
		}

		public override void DrawBegin()
		{
			int startX = Math.Max(0, (int)(DrawCntrl.CurrentCamera.X / GameConsole.CharSize.X));
			int startY = Math.Max(0, (int)(DrawCntrl.CurrentCamera.Y / GameConsole.CharSize.Y));

			for(var x = startX; x < Math.Min(startX + GameConsole.W, TileMap.GetLength(0)); x += 1)
			{
				for(var y = startY; y < Math.Min(startY + GameConsole.H, TileMap.GetLength(1)); y += 1)
				{	
					TileMap[x, y].Draw(x, y);
				}
			}

			if (DisplaySolids)
			{
				DrawCntrl.CurrentColor = Color.Red;
				for(var x = startX; x < Math.Min(startX + GameConsole.W, TileMap.GetLength(0)); x += 1)
				{
					for(var y = startY; y < Math.Min(startY + GameConsole.H, TileMap.GetLength(1)); y += 1)
					{	
						if (!TileMap[x, y].IsPassable())
						{
							DrawCntrl.DrawRectangle(new Vector2(x, y) * GameConsole.CharSize, new Vector2(x, y) * GameConsole.CharSize + GameConsole.CharSize - Vector2.One, true);
						}
					}
				}
			}
		}


		public Tile GetTile(Vector2 pos)
		{
			if (pos.X >= 0 && pos.Y >= 0 && pos.X < TileMap.GetLength(0) && pos.Y < TileMap.GetLength(1))
			{
				return TileMap[(int)pos.X, (int)pos.Y];
			}
			return DefaultTile;
		}

		public bool InBounds(Vector2 pos)
		{
			return (pos.X >= 0 && pos.Y >= 0 && pos.X < TileMap.GetLength(0) && pos.Y < TileMap.GetLength(1));
		}

		public void Resize(Vector2 newSize, Vector2 offset)
		{
			Tile[,] newTileMap = new Tile[(int)newSize.X, (int)newSize.Y];

			for(var x = 0; x < newTileMap.GetLength(0); x += 1)
			{
				for(var y = 0; y < newTileMap.GetLength(1); y += 1)
				{
					if (x >= offset.X && x < offset.X + TileMap.GetLength(0)
					&& y >= offset.Y && y < offset.Y + TileMap.GetLength(1))
					{
						newTileMap[x, y] = TileMap[x - (int)offset.X, y - (int)offset.Y];
					}
					else
					{
						newTileMap[x, y] = new Tile();
					}
				}
			}
			TileMap = newTileMap;
		}

	}
}
