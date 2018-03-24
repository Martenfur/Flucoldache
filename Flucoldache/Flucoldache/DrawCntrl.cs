using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flucoldache
{
	public static class DrawCntrl
	{
		private static char[] _frameBorders = {'╔', '╗', '╚', '╝', '═', '║'};	

		public static void DrawChar(
			char ch, 
			int x, 
			int y, 
			ConsoleColor foregoundColor = ConsoleColor.Gray, 
			ConsoleColor backgroundColor = ConsoleColor.Black
		)
		{
			try
			{
				Console.SetCursorPosition(x, y);
				Console.ForegroundColor = foregoundColor;
				Console.BackgroundColor = backgroundColor;
				Console.Write(ch);
			}
			catch(Exception){}
		}

		public static void DrawFrame(
			int x, 
			int y, 
			int w, 
			int h, 
			ConsoleColor foregoundColor = ConsoleColor.Gray, 
			ConsoleColor backgroundColor = ConsoleColor.Black)
		{
			DrawChar(_frameBorders[0], x, y, foregoundColor, backgroundColor);
			DrawChar(_frameBorders[1], x + w - 1, y, foregoundColor, backgroundColor);
			DrawChar(_frameBorders[2], x, y + h - 1, foregoundColor, backgroundColor);
			DrawChar(_frameBorders[3], x + w - 1, y + h - 1, foregoundColor, backgroundColor);

			
			for(var i = 0; i < h; i += h - 1)
			{
				for(var ix = 1; ix < w - 1; ix += 1)
				{
					DrawChar(_frameBorders[4], x + ix, y + i, foregoundColor, backgroundColor);
				}
			}

			for(var i = 0; i <  w; i += w - 1)
			{
				for(var iy = 1; iy < h - 1; iy += 1)
				{
					DrawChar(_frameBorders[5], x + i, y + iy, foregoundColor, backgroundColor);
				}
			}
		}

		public static void ClearScreen()
		{
			Console.Clear();
		}

	}
}
