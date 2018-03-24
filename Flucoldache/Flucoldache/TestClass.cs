using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flucoldache
{
	public class TestClass : GameObj
	{
		int x = 0;
		int y = 0;

		public override void Update()
		{
			DrawCntrl.DrawChar('^', x, y, ConsoleColor.Green, ConsoleColor.DarkGreen);

			if (Game.CheckKeyboard(ConsoleKey.A))
			{
				x -= 1;
			}

			if (Game.CheckKeyboard(ConsoleKey.D))
			{
				x += 1;
			}

			if (Game.CheckKeyboard(ConsoleKey.W))
			{
				y -= 1;
			}

			if (Game.CheckKeyboard(ConsoleKey.S))
			{
				y += 1;
			}

			DrawCntrl.DrawChar('@', x, y, ConsoleColor.Gray, ConsoleColor.Black);
			
		}
	}
}
