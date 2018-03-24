using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Flucoldache
{
	class Program
	{
		static void Main(string[] args)
		{
			
			Console.WriteLine("Foxes are fluffers. ╦════════");
			
			while(true)
			{
				char ch = Console.ReadKey(true).KeyChar;
				Console.Write(ch.ToString());
			}
		}
	}
}
