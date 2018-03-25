using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Flucoldache
{
	public static class GameConsole
	{
		public static Camera Camera;

		/// <summary>
		/// Width in characters.
		/// </summary>
		public static int W;

		/// <summary>
		/// Height in characters.
		/// </summary>
		public static int H;
		
		/// <summary>
		/// Console font.
		/// </summary>
		public static IFont Font;
		private static Dictionary<char, Frame> _fontChars;

		public static Vector2 CharSize;

		private static int _fillerFrame = 205;

		public static Color ForegroundColor = Color.Gray;
		public static Color BackgroundColor = Color.Black;

		private static char[] _frameBorders = {'╔', '╗', '╚', '╝', '═', '║'};	
		
		private static char[] _progressBarChars = {'░', '▒', '▓', '█'};

		public static void Init(IFont font, int w, int h)
		{
			Font = font;
			_fontChars = ((TextureFont)font).GetFrames();

			W = w;
			H = h;
			
			CharSize = Font.MeasureString(Font.DefaultCharacter.ToString());

			Camera = new Camera((int)(W * CharSize.X), (int)(H * CharSize.Y));
			//Camera.ClearBackground = false;

		}


		public static void DrawChar(char ch, int x, int y)
		{
			DrawCntrl.DrawSprite(Sprites.Font, _fillerFrame, x * CharSize.X, y * CharSize.Y, 1, 1, 0, BackgroundColor);
			DrawCntrl.DrawFrame(_fontChars[ch], new Vector2(x, y) * CharSize, Vector2.One, 0, Vector2.Zero, ForegroundColor, SpriteEffects.None);
		}

		public static void DrawText(string text, int x, int y)
		{
			DrawCntrl.CurrentFont = Font;
			string[] lines = text.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
			
			var yAdd = 0;
			foreach(string line in lines)
			{
				DrawCntrl.DrawSprite(Sprites.Font, _fillerFrame, x * CharSize.X, (y + yAdd) * CharSize.Y, line.Length, 1, 0, BackgroundColor);
				DrawCntrl.CurrentColor = ForegroundColor;
				DrawCntrl.DrawText(line, x * CharSize.X, (y + yAdd) * CharSize.Y);
				yAdd += 1;
			}
		}


		public static void DrawFrame(int x, int y, int w, int h)
		{
			if (w < 2 || h < 2)
			{
				throw(new Exception("Cannot draw a frame with size less than 2!"));
			}

			DrawChar(_frameBorders[0], x, y);
			DrawChar(_frameBorders[1], x + w - 1, y);
			DrawChar(_frameBorders[2], x, y + h - 1);
			DrawChar(_frameBorders[3], x + w - 1, y + h - 1);

			for(var i = 0; i < h; i += h - 1)
			{
				for(var ix = 1; ix < w - 1; ix += 1)
				{
					DrawChar(_frameBorders[4], x + ix, y + i);
				}
			}

			for(var i = 0; i <  w; i += w - 1)
			{
				for(var iy = 1; iy < h - 1; iy += 1)
				{
					DrawChar(_frameBorders[5], x + i, y + iy);
				}
			}
		}



		public static void DrawRectangle(int x1, int y1, int w, int h)
		{
			DrawCntrl.DrawSprite(Sprites.Font, _fillerFrame, x1 * CharSize.X, y1 * CharSize.Y, w, h, 0, BackgroundColor);
		}



		public static void DrawProgressBar(int x, int y, int w, float progress)
		{
			
			var i = 0;
			for(var progressW = w * progress; progressW > 0; progressW -= 1)
			{
				int ch = 3;
				
				if (progressW < 1)
				{
					ch = (int)(3 * progressW);
				}

				DrawChar(_progressBarChars[ch], x + i, y);
				i += 1;
			}
		}
		

	}
}
