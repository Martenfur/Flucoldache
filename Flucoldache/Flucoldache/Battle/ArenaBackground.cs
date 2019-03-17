using System;
using Microsoft.Xna.Framework;
using Monofoxe.Engine;

namespace Flucoldache.Battle
{
	public class ArenaBackground : GameObj
	{
		private int _width = GameConsole.W;
		private int _height = GameConsole.H - (int)Dialogue.Size.Y;

		private Color[,] _colorGrid;
			
		private float anim = 0;

		Vector2 offset = Vector2.Zero;

		int timer = 0;
		int time = 5;

		float _colorIntensivity = 0.1f;

		bool invert = false;

		int _circleYInvert;
		int _circleXInvert;
		float _circleDistanceDivider;

		int _linesMode;
		float _linesDistanceDivider;


		public ArenaBackground()
		{
			_colorGrid = new Color[_width, _height];
			InitCircle();
			InitLines();
			InitSin();
		}
		public override void Update()
		{	
			
			if (Input.KeyboardCheckPress(Microsoft.Xna.Framework.Input.Keys.G))
			{
				InitCircle();
				InitLines();
				InitSin();
			}

			timer -= 1;
			if (timer > 0)
			{
				return;
			}
			else
			{
				timer = time;
			}

			//UpdateCircle();
			//UpdateLines();
			UpdateSin();

			if (anim > 1)
			{
				anim -= 1;
			}


			if (offset.Y < 0)
			{
				offset.Y += _height;
			}
			if (offset.X < 0)
			{
				offset.X += _width;
			}
			if (offset.Y >= _height)
			{
				offset.Y -= _height;
			}
			if (offset.X >= _width)
			{
				offset.X -= _width;
			}
			
		}

		public override void DrawBegin()
		{
			
			GameConsole.DrawRectangle(Vector2.Zero, new Vector2(_width, _height));

			for(var x = 0; x < _width; x += 1)
			{
				for(var y = 0; y < _height; y += 1)
				{
					var pos = new Vector2(x, y) + offset;
					if (pos.X < 0)
					{
						pos.X += _width;
					}
					if (pos.Y < 0)
					{
						pos.Y += _height;
					}
					if (pos.X >= _width)
					{
						pos.X -= _width;
					}
					if (pos.Y >= _height)
					{
						pos.Y -= _height;
					}

					GameConsole.BackgroundColor = _colorGrid[x, y];
					GameConsole.DrawChar(' ', pos);
				}
			}
		}


		private void InitLines()
		{
			_linesMode = GameplayController.Random.Next(3);

			_linesDistanceDivider = MathHelper.Lerp(16, 48, (float)GameplayController.Random.NextDouble());
		}
		

		private void InitCircle()
		{
			if (GameplayController.Random.NextDouble() > 0.5)
			{
				_circleXInvert = 1;
			}
			else
			{
				_circleXInvert = -1;
			}

			if (GameplayController.Random.NextDouble() > 0.5)
			{
				_circleYInvert = 1;
			}
			else
			{
				_circleYInvert = -1;
			}

			_circleDistanceDivider = MathHelper.Lerp(16, 48, (float)GameplayController.Random.NextDouble());
		}
		
		
		private void InitSin()
		{
		
		}
		

		private void UpdateLines()
		{
			anim += 0.02f;
		
			for(var x = 0; x < _width; x += 1)
			{
				for(var y = 0; y < _height; y += 1)
				{
					var xx = x - _width / 2;
					var yy = y - _height / 2;


					float dist = 0;
					
					switch(_linesMode)
					{
						case 0:
							dist = yy * xx;
						break;
						case 1:
							dist = xx * xx;
						break;
						case 2:
							dist = yy * yy;
						break;
					}

					var sinAnim = (float)Math.Cos(anim * Math.PI * 2 + dist / _circleDistanceDivider);
					
					_colorGrid[x, y] = Color.Lerp(
						GameConsole.BaseBackgroundColor, 
						GameConsole.BaseForegroundColor, 
						_colorIntensivity + sinAnim * _colorIntensivity
					);
				}
			}
			
		}


		private void UpdateCircle()
		{
			anim += 0.02f;
			
			
			for(var x = 0; x < _width; x += 1)
			{
				for(var y = 0; y < _height; y += 1)
				{
					var xx = x - _width / 2;
					var yy = y - _height / 2;

					var dist =  xx * xx / 4f * _circleXInvert + yy * yy * _circleYInvert;
					var sinAnim = (float)Math.Sin(anim * Math.PI * 2 + dist / _circleDistanceDivider);
					
					_colorGrid[x, y] = Color.Lerp(
						GameConsole.BaseBackgroundColor, 
						GameConsole.BaseForegroundColor, 
						_colorIntensivity + sinAnim * _colorIntensivity
					);
				}
			}
			
		}

		private void UpdateSin()
		{
			anim += 0.02f;
			
			offset.Y += 0.5f;
			offset.X += 0.5f;

			var sinAnim = (float)Math.Sin(anim * Math.PI * 2);

			for(var x = 0; x < _width; x += 1)
			{
				for(var y = 0; y < _height; y += 1)
				{
					var l = (float)(Math.Sin((y / (float)_height) * Math.PI * 2 * 2) + 1) * 0.5f;
					var l1 = (float)(Math.Sin((x * 2 / (float)_width) * Math.PI * 2 * 2) + 1) * 0.5f;
					

					float value = MathHelper.Lerp(l, l1, l);

					if (!invert)
					{
						value = 1 - value;
					}

					_colorGrid[x, y] = Color.Lerp(
						GameConsole.BaseBackgroundColor, 
						GameConsole.BaseForegroundColor, 
						value * (_colorIntensivity + sinAnim * _colorIntensivity)
					);
				}
			}
		}
	}
}
