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

namespace Monofoxe
{
	class TestObj: GameObj 
	{
		float x, y;
		double ang = 0;
		int period = 3;

		RenderTarget2D surf;

		
		Camera cam = new Camera(800, 480);
		
		Random r = new Random();

		public TestObj()
		{
			
			GameCntrl.MaxGameSpeed = 60;
			

			cam.BackgroundColor = Color.AliceBlue;

			
			cam.OffsetX = cam.W / 2;
			cam.OffsetY = cam.H / 2;

			x = cam.W / 2;
			y = cam.H / 2;

			RasterizerState rasterizerState = new RasterizerState();
			rasterizerState.CullMode = CullMode.None;
			rasterizerState.ScissorTestEnable = false;
			rasterizerState.FillMode = FillMode.Solid;
			DrawCntrl.Rasterizer = rasterizerState;
		}

		public override void Update()
		{
			
			ang += GameCntrl.Time((Math.PI * 2) / period);

			if (ang >= Math.PI * 2)
			{
				ang -= Math.PI * 2;
			}
			
			if (Input.KeyboardCheck(Keys.Left))
			{x += (5 / cam.ScaleX);}
			
			if (Input.KeyboardCheck(Keys.Right))
			{x -= (5 / cam.ScaleX);;}
			
			if (Input.KeyboardCheck(Keys.Up))
			{y += (5 / cam.ScaleX);}
			
			if (Input.KeyboardCheck(Keys.Down))
			{y -= (5 / cam.ScaleX);}
			
			if (Input.KeyboardCheck(Keys.Z))
			{
				cam.ScaleX += 0.1f;
				cam.ScaleY += 0.1f;
			}
			
			if (Input.KeyboardCheck(Keys.X))
			{
				cam.ScaleX -= 0.1f;
				cam.ScaleY -= 0.1f;
				if (cam.ScaleX <= 0)
				{
					cam.ScaleX = 0.1f;
					cam.ScaleY = 0.1f;
				}
			}
			
			if (Input.KeyboardCheck(Keys.C))
			{cam.Rotation += 5;}

			if (Input.KeyboardCheck(Keys.V))
			{cam.Rotation -= 5;}

			cam.X = x;
			cam.Y = y;

			
		}

		
		public override void Draw()
		{	
			DrawCntrl.CurrentColor = Color.Black;
			DrawCntrl.CurrentFont = Fonts.Font;
			DrawCntrl.DrawText("foxes are FLUFFERS! АИта правда. ░▒▓│┤╡╢╖╕╣║╗╝╜╛┐└┴┬├─┼╞╟╚╔╩╦╠═╬╧╨╤╥╙╘╒╓╫╪┘┌█▄▌▐▀", 32, 32);
		}

		public override void DrawGUI()
		{
		}


	}
}
