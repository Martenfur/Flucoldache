using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using System.Collections.Generic;
using System.IO;
using Monofoxe.FMODAudio;

namespace Flucoldache
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		//public GraphicsDeviceManager graphics;
		public static readonly int WindowW = 768;
		public static readonly int WindowH = 512;

		public Game1()
		{
			GameCntrl.Init(this);
			Content.RootDirectory = GameCntrl.ContentDir;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content. Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			GameCntrl.MaxGameSpeed = 60.0;
			
			GameCntrl.WindowManager.CanvasWidth = WindowW;
			GameCntrl.WindowManager.CanvasHeight = WindowH;
			GameCntrl.WindowManager.SetFullScreen(false);
			GameCntrl.WindowManager.CenterWindow();
			GameCntrl.WindowManager.ApplyChanges();

			Window.TextInput += Input.TextInput;

			AudioMgr.Init("");

			base.Initialize();
		}
		
		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			GameCntrl.LoadGraphics(Content);	
			Fonts.Load(Content);
			DrawCntrl.Init(GraphicsDevice);

			SoundController.Init(Content);

			GameCntrl.Begin();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
			AudioMgr.Unload();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (Input.KeyboardCheckPress(Keys.F4))
			{
				GameCntrl.WindowManager.SetFullScreen(!GameCntrl.WindowManager.IsFullScreen);
			}
			AudioMgr.Update();
			GameCntrl.Update(gameTime);
			
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GameCntrl.UpdateFps(gameTime);
			DrawCntrl.Update(gameTime);		

			base.Draw(gameTime);
		}
	}
}
