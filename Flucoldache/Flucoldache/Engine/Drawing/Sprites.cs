using System;
using System.Collections.Generic;
using Monofoxe.Engine.Drawing;

namespace Monofoxe.Engine.Drawing
{
	public static class Sprites
	{
		#region sprites
		public static Sprite Font;
		#endregion sprites
		
		public static void Init(Dictionary<string, Frame[]> frames)
		{
			#region sprite_constructors
			
			Font = new Sprite(frames["font"], 0, 0);
			
			#endregion sprite_constructors
		}
	}
}
