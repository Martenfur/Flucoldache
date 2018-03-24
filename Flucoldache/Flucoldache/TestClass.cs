using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flucoldache
{
	public class TestClass
	{
		public void Update()
		{
			Console.WriteLine("f" + Game.CurrentKeyChar);
			if (Game.CheckKeyboard(ConsoleKey.S))
			{
				Game.SkipNextFrame();
			}
		}
	}
}
