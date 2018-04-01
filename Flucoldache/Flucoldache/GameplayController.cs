using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monofoxe.Engine;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Flucoldache.Overworld;
using Monofoxe.Engine.Drawing;


namespace Flucoldache
{
	/// <summary>
	/// Main gameplay controller.
	/// </summary>
	public class GameplayController : GameObj
	{
		
		public static Keys KeyUp = Keys.Up;
		public static Keys KeyDown = Keys.Down;
		public static Keys KeyLeft = Keys.Left;
		public static Keys KeyRight = Keys.Right;
		public static Keys KeyA = Keys.Z;
		public static Keys KeyB = Keys.X;

		
		public GameplayController()
		{
			GameConsole.Init(Fonts.Font, 96, 32);

			GameConsole.Camera.BackgroundColor = Color.Black;

			RasterizerState rasterizerState = new RasterizerState();
			rasterizerState.CullMode = CullMode.None; 
			rasterizerState.ScissorTestEnable = false;
			rasterizerState.FillMode = FillMode.Solid;
			DrawCntrl.Rasterizer = rasterizerState;
			DrawCntrl.Sampler = SamplerState.PointClamp;
			
			//new Terrain(96, 32);
			//new MapEditor();
			new MainMenu();
		}

		public override void Update()
		{
			
		}

	}
}
