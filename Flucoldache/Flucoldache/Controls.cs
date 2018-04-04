using System;
using Monofoxe.Engine;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;


namespace Flucoldache
{
	/// <summary>
	/// Main "console" controls.
	/// Mainly used to have convenient controls disabling. 
	/// If you want to bypass it, use Input class.
	/// </summary>
	public static class Controls
	{
		public static Keys KeyUp = Keys.Up;
		public static Keys KeyDown = Keys.Down;
		public static Keys KeyLeft = Keys.Left;
		public static Keys KeyRight = Keys.Right;
		public static Keys KeyA = Keys.Z;
		public static Keys KeyB = Keys.X;
		public static Keys KeyStart = Keys.Enter;

		public static bool Enabled = true;

		public static bool KeyCheck(Keys key)
		{	
			return (Input.KeyboardCheck(key) && Enabled);
		}
		
		public static bool KeyCheckPress(Keys key)
		{
			return (Input.KeyboardCheckPress(key) && Enabled);
		}
		
		public static bool KeyCheckRelease(Keys key)
		{
			return (Input.KeyboardCheckRelease(key) && Enabled);
		}
	}
}
