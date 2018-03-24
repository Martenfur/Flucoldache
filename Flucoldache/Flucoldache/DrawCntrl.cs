using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flucoldache
{
	public static class DrawCntrl
	{
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

		public static void ClearScreen()
		{
			Console.Clear();
		}

	}
}
