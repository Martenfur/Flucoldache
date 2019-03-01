using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Flucoldache.Overworld
{
	public class Tile
	{
		public enum TileType
		{
			Passable,
			Solid
		}
			
		public char Char;
		public TileType Type;
		public Color ForegroundColor;
		public Color BackgroundColor;

		public Tile(char ch = ' ', TileType type = TileType.Passable)
		{
			Char = ch;
			Type = type;
			ForegroundColor = GameConsole.BaseForegroundColor;
			BackgroundColor = GameConsole.BaseBackgroundColor;
		}

		public bool IsPassable()		
		{
			return (Type == TileType.Passable);
		}

		public void Draw(int x, int y)
		{
			GameConsole.ForegroundColor = ForegroundColor;
			GameConsole.BackgroundColor = BackgroundColor; 
			GameConsole.DrawChar(Char, x, y);
		}

	}
}
