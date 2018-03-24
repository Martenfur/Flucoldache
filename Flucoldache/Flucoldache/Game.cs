using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Flucoldache
{
	/// <summary>
	/// Main game class.
	/// </summary>
	public static class Game
	{
		
		/// <summary>
		/// Tells if next frame will be skipped.
		/// </summary>
		static bool _skipNextFrame = false;

		/// <summary>
		/// Delay after skipping frame.
		/// </summary>
		private static int _skipDelay = _defaultSkipDelay;

		/// <summary>
		/// Default delay after skipping frame.
		/// </summary>
		private const int _defaultSkipDelay = 100;


		/// <summary>
		/// Key pressed in current step.
		/// </summary>
		public static ConsoleKey CurrentKey = _noKey;
		
		/// <summary>
		/// This key will be treated as no input.
		/// </summary>
		private const ConsoleKey _noKey = ConsoleKey.PrintScreen;

		/// <summary>
		/// Character of key pressed in current step.
		/// </summary>
		public static char CurrentKeyChar = '\0';
		
		public static int WindowW = 96;
		public static int WindowH = 32;


		/// <summary>
		/// Main game function.
		/// </summary>
		public static void Run()
		{
			Console.CursorVisible = false;
			Console.SetWindowSize(WindowW, WindowH);
			Console.SetBufferSize(WindowW, WindowH);

			TestClass test = new TestClass();
			
			while(true)
			{
				bool delay = false;
				int delayTime = _skipDelay;

				if (!_skipNextFrame)
				{
					// Input handling.
					ConsoleKeyInfo key = Console.ReadKey(true);
					CurrentKey = key.Key;
					CurrentKeyChar = key.KeyChar;
					// Input handling.
				}
				else
				{
					// If frame being skipped, we don't wait for any user input.
					delay = true;
					_skipNextFrame = false;
					CurrentKey = _noKey;
					CurrentKeyChar = '\0';
					// If frame being skipped, we don't wait for any user input.
				}



				// Main game loop code.
				//DrawCntrl.ClearScreen();
				ObjCntrl.Update();
				// Main game loop code.



				if (delay)
				{
					Thread.Sleep(delayTime);
				}
			}
		}


		/// <summary>
		/// Checks if a certain key was pressed.
		/// </summary>
		public static bool CheckKeyboard(ConsoleKey key)
		{
			return (CurrentKey == key && CurrentKey != _noKey);
		}


		/// <summary>
		/// Tells the game to not wait for user input in the next frame.
		/// </summary>
		public static void SkipNextFrame(int delay = _defaultSkipDelay)
		{
			_skipNextFrame = true;
			_skipDelay = delay;
		}



	}
}
