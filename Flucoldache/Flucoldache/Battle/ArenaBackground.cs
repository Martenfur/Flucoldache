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
			
		private float _animation = 0;

		private Vector2 _offset = Vector2.Zero;

		private int _timer = 0;
		private int _time = 5;

		private int _backgroundType;
		
		private float _colorIntensivity = 0.1f;

		private bool invert = false;

		private int _circleYInvert;
		private int _circleXInvert;
		private float _circleDistanceDivider;

		private int _linesMode;
		private float _linesDistanceDivider;



		public ArenaBackground()
		{
			_colorGrid = new Color[_width, _height];

			_backgroundType = GameplayController.Random.Next(3);
			InitCircle();
			InitLines();
			InitSin();
		}

		public override void Update()
		{	
			
			_timer -= 1;
			if (_timer > 0)
			{
				return;
			}
			else
			{
				_timer = _time;
			}

			switch(_backgroundType)
			{
				case 0:
					UpdateCircle();
				break;
				case 1:
					UpdateLines();
				break;
				case 2:
					UpdateSin();
				break;
			}

			if (_animation > 1)
			{
				_animation -= 1;
			}


			if (_offset.Y < 0)
			{
				_offset.Y += _height;
			}
			if (_offset.X < 0)
			{
				_offset.X += _width;
			}
			if (_offset.Y >= _height)
			{
				_offset.Y -= _height;
			}
			if (_offset.X >= _width)
			{
				_offset.X -= _width;
			}
			
		}

		public override void DrawBegin()
		{
			
			GameConsole.DrawRectangle(Vector2.Zero, new Vector2(_width, _height));

			for(var x = 0; x < _width; x += 1)
			{
				for(var y = 0; y < _height; y += 1)
				{
					var pos = new Vector2(x, y) + _offset;
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
			_animation += 0.02f;
		
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

					var sinAnim = (float)Math.Cos(_animation * Math.PI * 2 + dist / _circleDistanceDivider);
					
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
			_animation += 0.02f;
			
			
			for(var x = 0; x < _width; x += 1)
			{
				for(var y = 0; y < _height; y += 1)
				{
					var xx = x - _width / 2;
					var yy = y - _height / 2;

					var dist =  xx * xx / 4f * _circleXInvert + yy * yy * _circleYInvert;
					var sinAnim = (float)Math.Sin(_animation * Math.PI * 2 + dist / _circleDistanceDivider);
					
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
			_animation += 0.02f;
			
			_offset.Y += 0.5f;
			_offset.X += 0.5f;

			var sinAnim = (float)Math.Sin(_animation * Math.PI * 2);

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
