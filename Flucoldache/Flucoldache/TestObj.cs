using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Flucoldache;
using Flucoldache.Overworld;


namespace Monofoxe
{
	class TestObj: GameObj 
	{
		float x=2, y=2;
		double ang = 0;
		int period = 3;

		RenderTarget2D surf;

		Random r = new Random();

		public TestObj()
		{
			new Player(new Vector2(8, 8));
			
			GameConsole.Camera.BackgroundColor = Color.Black;


			RasterizerState rasterizerState = new RasterizerState();
			rasterizerState.CullMode = CullMode.None;
			rasterizerState.ScissorTestEnable = false;
			rasterizerState.FillMode = FillMode.Solid;
			DrawCntrl.Rasterizer = rasterizerState;
			DrawCntrl.Sampler = SamplerState.PointClamp;
		}

		float progress = 1;
		public override void Update()
		{
			if (Input.KeyboardCheck(Keys.Up))
			{
				y -= 1;
			}
			if (Input.KeyboardCheck(Keys.Down))
			{
				y += 1;
			}
			if (Input.KeyboardCheck(Keys.Left))
			{
				x -= 1;
				progress -= 0.002f;
				if (progress < 0)
				{progress = 0;}
			}
			if (Input.KeyboardCheck(Keys.Right))
			{
				x += 1;
				progress += 0.002f;
				if (progress > 1)
				{progress = 1;}
			}
		}
		
		public override void Draw()
		{	
			/*
			for(var ix = 0; ix < x; ix += 1)
			{
				for(var iy = 0; iy < y; iy += 1)
				{
					GameConsole.ForegroundColor = Color.White;
					GameConsole.BackgroundColor = new Color(128, ix*8, iy*8);
					GameConsole.DrawText("@", ix, iy);
					//DrawCntrl.DrawText("@", ix * 8, iy * 16);
				}	
			}
			*/

			//GameConsole.DrawText((int)(progress* 100) + "%", 16, 15);
			GameConsole.ForegroundColor = Color.Gray;
			//GameConsole.DrawFrame(15, 15, 18, 3);
			GameConsole.ForegroundColor = Color.Red;			
			//GameConsole.DrawProgressBar(16, 16, 64, progress);

			//GameConsole.DrawFrame(0, 0, (int)x, (int)y);

			//GameConsole.BackgroundColor = Color.Red;
			GameConsole.DrawText("fps: " + GameCntrl.Fps, 0, 0);
		}

		public override void DrawGUI()
		{
		}

	
	}
}
