using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Monofoxe.Engine.Drawing
{
	public static class Fonts
	{
		private static ContentManager _content;
		

		static string ascii = " !" + '"' + @"#$%&'()*+,-./0123456789:;<=>?@"
		+ "ABCDEFGHIJKLMNOPQRSTUVWXYZ" 
		+ @"[\]^_`" 
		+ "abcdefghijklmnopqrstuvwxyz" 
		+ "{|}~╳" 
		+ "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ"
		+ "абвгдежзийклмнопрстуфхцчшщъыьэюяЁё"
		+ "░▒▓│┤╡╢╖╕╣║╗╝╜╛┐└┴┬├─┼╞╟╚╔╩╦╠═╬╧╨╤╥╙╘╒╓╫╪┘┌█▄▌▐▀";
		
		public static IFont Font;
		
		public static void Load(ContentManager content)
		{Debug.WriteLine(ascii.Length);
			Font = new TextureFont(Sprites.Font, 0, 0, ascii, true);
		}

		public static void Unload()
		{
			_content.Unload();
		}

	}
}
