﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Monofoxe.Engine.Drawing;


namespace Monofoxe.Engine
{
	public static class DrawCntrl
	{
		private const int BUFFER_SIZE = 320000;	

		public static SpriteBatch Batch {get; private set;}
		public static GraphicsDevice Device {get; private set;}
		public static BasicEffect BasicEffect {get; private set;}

		/// <summary>
		/// List of all cameras.
		/// </summary>
		public static List<Camera> Cameras {get; private set;}

		/// <summary>
		/// Current enabled camera.
		/// </summary>
		public static Camera CurrentCamera {get; private set;} = null;

		/// <summary>
		/// Current transformation matrix.
		/// </summary>
		public static Matrix CurrentTransformMatrix {get; private set;}
		private static Stack<Matrix> _transformMatrixStack;

		
		/// <summary>
		/// Matrix for offsetting, scaling and rotating canvas contents.
		/// </summary>
		public static Matrix CanvasMatrix = Matrix.CreateTranslation(Vector3.Zero); //We need zero matrix here, or else mouse position will derp out.
		

		/// <summary>
		/// Current drawing color. Affects shapes and primitives.
		/// </summary>
		public static Color CurrentColor;

		private static DynamicVertexBuffer _vertexBuffer;
		private static DynamicIndexBuffer _indexBuffer;

		private static List<VertexPositionColorTexture> _vertices;
		private static List<short> _indices;

		/// <summary>
		/// Amount of draw calls per frame.
		/// </summary>
		public static int __drawcalls {get; private set;}

		/// <summary>
		/// Every time we want to draw primitive of a new type
		/// or switch texture, we need to empty vertex buffer
		/// and switch pipeline mode.
		/// </summary>
		private enum PipelineMode
		{
			None,               // No mode set.
			Sprites,            // Sprite batch.
			TrianglePrimitives, // Triangle list.
			OutlinePrimitives,  // Line list.
		}

		private static PipelineMode _currentPipelineMode;
		private static Texture2D _currentTexture;
		
		/// <summary>
		/// When can set surface targets inside another surfaces.
		/// </summary>
		private static Stack<RenderTarget2D> _surfaceStack;
		private static RenderTarget2D _currentSurface;

		/// <summary>
		/// Disables rendering for everything that's outside of rectangle.
		/// NOTE: To enable scissoring, enable scissor test in Rasterizer.
		/// </summary>
		public static Rectangle ScissorRectangle
		{
			set
			{
				SwitchPipelineMode(PipelineMode.None, null);
				_scissorRectangle = value;
			}
			get 
			{
				return _scissorRectangle;
			}
		}
		private static Rectangle _scissorRectangle;

		/// <summary>
		/// Rasterizer state. 
		/// NOTE: Do NOT modify object which you'll set. This will lead to errors and unexpected behaviour.
		/// </summary>
		public static RasterizerState Rasterizer
		{
			set
			{
				SwitchPipelineMode(PipelineMode.None, null); 
				_rasterizer = value;
			}
			get
			{
				return _rasterizer;
			}
		}
		private static RasterizerState _rasterizer;

		/// <summary>
		/// Sampler state. Used for interpolation and texture wrappping.
		/// NOTE: Do NOT modify object which you'll set. This will lead to errors and unexpected behaviour.
		/// </summary>
		public static SamplerState Sampler
		{
			set
			{
				SwitchPipelineMode(PipelineMode.None, null); 
				_sampler = value;
			}
			get
			{
				return _sampler;
			}
		}
		private static SamplerState _sampler;
		


		#region shapes

		private static readonly PipelineMode[] _pipelineModes = {PipelineMode.TrianglePrimitives, PipelineMode.OutlinePrimitives};
		
		// Triangle.
		private static readonly short[][] _triangleIndices = 
		{
			new short[]{0, 1, 2},
			new short[]{0, 1, 1, 2, 2, 0}
		};
		// Triangle.

		// Rectangle.
		private static readonly short[][] _rectangleIndices = 
		{
			new short[]{0, 1, 3, 1, 2, 3},
			new short[]{0, 1, 1, 2, 2, 3, 3, 0}
		};
		// Rectangle.

		// Circle.
		/// <summary>
		/// Amount of vertices in one circle. 
		/// </summary>
		public static int CircleVerticesCount 
		{
			set
			{
				_circleVectors = new List<Vector2>();
			
				var angAdd = Math.PI * 2 / value;
				
				for(var i = 0; i < value; i += 1)
				{
					_circleVectors.Add(new Vector2((float)Math.Cos(angAdd * i), (float)Math.Sin(angAdd * i)));
				}
			}

			get
			{
				return _circleVectors.Count;
			}
		}

		private static List<Vector2> _circleVectors; 
		// Circle.

		#endregion shapes

		// Primitives.
		private static List<VertexPositionColorTexture> _primitiveVertices;
		private static List<short> _primitiveIndices;
		private static PipelineMode _primitiveType;
		private static Texture2D _primitiveTexture;

		private static Vector2 _primitiveTextureOffset;
		private static Vector2 _primitiveTextureRatio;
		// Primitives.

		
		// Text.
		public static IFont CurrentFont;

		public static TextAlign HorAlign;
		public static TextAlign VerAlign;
		// Text.


		/// <summary>
		/// Initialization function for draw controller. 
		/// Should be called only once, by Game class.
		/// </summary>
		/// <param name="device">Graphics device.</param>
		public static void Init(GraphicsDevice device)
		{
			Device = device;

			Batch = new SpriteBatch(Device);
			Cameras = new List<Camera>();
			BasicEffect = new BasicEffect(Device);
			BasicEffect.VertexColorEnabled = true;
			Device.DepthStencilState = DepthStencilState.DepthRead;
			
			_vertexBuffer = new DynamicVertexBuffer(Device, typeof(VertexPositionColorTexture), BUFFER_SIZE, BufferUsage.WriteOnly);
			_indexBuffer = new DynamicIndexBuffer(Device, IndexElementSize.SixteenBits, BUFFER_SIZE, BufferUsage.WriteOnly);
			_vertices = new List<VertexPositionColorTexture>();
			_indices = new List<short>();

			_currentPipelineMode = PipelineMode.None;
			_currentTexture = null;

			Device.SetVertexBuffer(_vertexBuffer);
			Device.Indices = _indexBuffer;

			_primitiveVertices = new List<VertexPositionColorTexture>();
			_primitiveIndices = new List<short>();
			_primitiveTexture = null;
			
			_surfaceStack = new Stack<RenderTarget2D>();
			_currentSurface = null;

			CurrentColor = Color.White;
			CircleVerticesCount = 16;

			_transformMatrixStack = new Stack<Matrix>();

			HorAlign = TextAlign.Left;
			VerAlign = TextAlign.Top;
		}


		/// <summary>
		/// Performs Draw events for all objects.
		/// </summary>
		public static void Update(GameTime gameTime)
		{
			__drawcalls = 0;
			
			var depthSortedObjects = Objects.GameObjects.OrderByDescending(o => o.Depth);
			
			
			// Main draw events.
			foreach(Camera camera in Cameras)
			{
				if (camera.Enabled)
				{
					// Updating current transform matrix and camera.
					camera.UpdateTransformMatrix();
					CurrentCamera = camera;
					CurrentTransformMatrix = camera.TransformMatrix;
					BasicEffect.View = camera.TransformMatrix;
					BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, camera.W, camera.H, 0, 0, 1);
					// Updating current transform matrix and camera.

					Input.UpdateMouseWorldPosition();

					SetSurfaceTarget(camera.ViewSurface, camera.TransformMatrix);

					if (camera.ClearBackground)
					{
						Device.Clear(camera.BackgroundColor);
					}

					foreach(GameObj obj in depthSortedObjects)
					{
						if (obj.Active)
						{obj.DrawBegin();}
					}

					foreach(GameObj obj in depthSortedObjects)
					{
						if (obj.Active)
						{obj.Draw();}
					}
			
					foreach(GameObj obj in depthSortedObjects)
					{
						if (obj.Active)
						{obj.DrawEnd();}
					}
					
					ResetSurfaceTarget();
					
				}
			}
			// Main draw events.


			
			// Scales display to match canvas, but keeps aspect ratio.
			var windowManager = GameCntrl.WindowManager;

			var backbufferSize = new Vector2(
				windowManager.PreferredBackBufferWidth,
				windowManager.PreferredBackBufferHeight
			);
			float ratio,
				offsetX = 0,
				offsetY = 0;

			float backbufferRatio = windowManager.PreferredBackBufferWidth / (float)windowManager.PreferredBackBufferHeight;
			float canvasRatio = windowManager.CanvasWidth / windowManager.CanvasHeight;

			if (canvasRatio > backbufferRatio)
			{
				ratio = windowManager.PreferredBackBufferWidth / (float)windowManager.CanvasWidth;
				offsetY = (windowManager.PreferredBackBufferHeight - (windowManager.CanvasHeight * ratio)) / 2;
			}
			else
			{
				ratio = windowManager.PreferredBackBufferHeight / (float)windowManager.CanvasHeight;
				offsetX = (windowManager.PreferredBackBufferWidth - (windowManager.CanvasWidth * ratio)) / 2;
			}
					
			CanvasMatrix = Matrix.CreateScale(new Vector3(ratio, ratio, 1)) * Matrix.CreateTranslation(new Vector3(offsetX, offsetY, 0));		
			// Scales display to match canvas, but keeps aspect ratio.


			
			// Resetting camera and transform matrix.
			CurrentCamera = null;
			CurrentTransformMatrix = CanvasMatrix;//Matrix.CreateTranslation(0, 0, 0);
			BasicEffect.View = CurrentTransformMatrix;
			BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, windowManager.PreferredBackBufferWidth, windowManager.PreferredBackBufferHeight, 0, 0, 1); // TODO: Set actual backbuffer size here.
			// Resetting camera and transform matrix.
			

			// Drawing camera surfaces.
			Device.Clear(Color.TransparentBlack);
			
			Batch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, CurrentTransformMatrix);
			foreach(Camera camera in Cameras)
			{
				if (camera.Autodraw && camera.Enabled)
				{Batch.Draw(camera.ViewSurface, new Vector2(camera.PortX, camera.PortY), Color.White);}
			}
			Batch.End();
			// Drawing camera surfaces.

			
			// Drawing GUI stuff.
			_currentPipelineMode = PipelineMode.None;
			
			foreach(GameObj obj in depthSortedObjects)
			{
				if (obj.Active)
				{obj.DrawGUI();}
			}
			
			if (_currentPipelineMode == PipelineMode.Sprites) // If there's something left in batch or vertex buffer, we should draw it.
			{Batch.End();}
			DrawVertices();
			// Drawing GUI stuff.


			_currentPipelineMode = PipelineMode.None;

			// Safety checks.
			if (_surfaceStack.Count != 0)
			{
				throw(new InvalidOperationException("Unbalanced surface stack! Did you forgot to reset a surface somewhere?"));
			}

			if (_transformMatrixStack.Count != 0)
			{
				throw(new InvalidOperationException("Unbalanced matrix stack! Did you forgot to reset a matrix somewhere?"));
			}
			// Safety checks.

			//Debug.WriteLine("CALLS: " + __drawcalls);
		}



		#region pipeline

		/// <summary>
		/// Switches graphics pipeline mode.
		/// </summary>
		/// <param name="mode"></param>
		private static void SwitchPipelineMode(PipelineMode mode, Texture2D texture)
		{
			if (mode != _currentPipelineMode || texture != _currentTexture)
			{
				if (_currentPipelineMode != PipelineMode.None)
				{
					if (_currentPipelineMode == PipelineMode.Sprites)
					{
						Batch.End();
					}
					else
					{
						DrawVertices();
					}
				}

				if (mode == PipelineMode.Sprites)
				{
					Device.ScissorRectangle = _scissorRectangle;
					
					Batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, _sampler, null, _rasterizer, null, CurrentTransformMatrix);
				}
				_currentPipelineMode = mode;
				_currentTexture = texture;
			}
		}



		/// <summary>
		/// Adds vertices and indices to global vertex and index list.
		/// If current and suggested pipeline modes are different, draws accumulated vertices first.
		/// </summary>
		/// <param name="mode">Suggested pipeline mode.</param>
		/// <param name="vertices">List of vertices.</param>
		/// <param name="indices">List of indices.</param>
		private static void AddVertices(PipelineMode mode, Texture2D texture, List<VertexPositionColorTexture> vertices, short[] indices)
		{
			if (_indices.Count + indices.Length >= BUFFER_SIZE)
			{
				DrawVertices(); // If buffer overflows, we need to empty it.
			}

			SwitchPipelineMode(mode, texture);

			short[] indexesCopy= new short[indices.Length];
			Array.Copy(indices, indexesCopy, indices.Length); // We must copy an array to prevent modifying original.

			for(var i = 0; i < indices.Length; i += 1)
			{
				indexesCopy[i] += (short)_vertices.Count; // We need to offset each index because of single buffer for everything.
			} 

			_vertices.AddRange(vertices);
			_indices.AddRange(indexesCopy);
		}



		/// <summary>
		/// Draws vertices from vertex buffer and empties it.
		/// </summary>
		private static void DrawVertices()
		{
			BasicEffect.View = CurrentTransformMatrix;
			
			__drawcalls += 1;

			if (_vertices.Count > 0)
			{
				BasicEffect.Texture = _currentTexture;
				BasicEffect.TextureEnabled = (_currentTexture != null);


				PrimitiveType type;
				int prCount;

				if (_currentPipelineMode == PipelineMode.OutlinePrimitives)
				{
					type = PrimitiveType.LineList;
					prCount = _indices.Count / 2;
				}
				else
				{
					type = PrimitiveType.TriangleList;
					prCount = _indices.Count / 3;
				}
				
				// Passing primitive data to the buffers.
				_vertexBuffer.SetData(_vertices.ToArray(), 0, _vertices.Count, SetDataOptions.None);
				_indexBuffer.SetData(_indices.ToArray(), 0, _indices.Count);
				// Passing primitive data to the buffers.
				
				if (_rasterizer != null)
				{
					Device.RasterizerState = _rasterizer;
				}

				if (_sampler != null)
				{
					Device.SamplerStates[0] = _sampler;
				}
				
				Device.ScissorRectangle = _scissorRectangle;
				
				foreach(EffectPass pass in BasicEffect.CurrentTechnique.Passes)
				{
					pass.Apply();
					Device.DrawIndexedPrimitives(type, 0, 0, prCount);
				}

				_vertices.Clear();
				_indices.Clear();
				
			}
		}

		#endregion pipeline

		
		
		#region surfaces and matrixes

		/// <summary>
		/// Sets surface as a render target.
		/// </summary>
		/// <param name="surf">Target surface.</param>
		public static void SetSurfaceTarget(RenderTarget2D surf)
		{
			SetSurfaceTarget(surf, Matrix.CreateTranslation(Vector3.Zero));
		}

		/// <summary>
		/// Sets surface as a render target.
		/// </summary>
		/// <param name="surf">Target surface.</param>
		/// <param name="matrix">Surface transformation matrix.</param>
		public static void SetSurfaceTarget(RenderTarget2D surf, Matrix matrix)
		{
			SetTransformMatrix(matrix);

			_surfaceStack.Push(_currentSurface);
			_currentSurface = surf;

			Device.SetRenderTarget(_currentSurface);
		}



		/// <summary>
		/// Resets render target to a previous surface.
		/// </summary>
		public static void ResetSurfaceTarget()
		{
			ResetTransformMatrix();

			if (_surfaceStack.Count == 0)
			{
				throw(new InvalidOperationException("Surface stack is empty! Did you forgot to set a surface somewhere?"));
			}
			_currentSurface = _surfaceStack.Pop();

			Device.SetRenderTarget(_currentSurface);
		}



		/// <summary>
		/// Sets new transform matrix.
		/// </summary>
		/// <param name="matrix">Transform matrix.</param>
		public static void SetTransformMatrix(Matrix matrix)
		{
			SwitchPipelineMode(PipelineMode.None, null);
			_transformMatrixStack.Push(CurrentTransformMatrix);
			CurrentTransformMatrix = matrix;
		}



		/// <summary>
		/// Sets new transform matrix multiplied by current transform matrix.
		/// </summary>
		/// <param name="matrix"></param>
		public static void AddTransformMatrix(Matrix matrix)
		{
			SwitchPipelineMode(PipelineMode.None, null);
			_transformMatrixStack.Push(CurrentTransformMatrix);
			CurrentTransformMatrix = matrix * CurrentTransformMatrix;
		}



		/// <summary>
		/// Resets to a previous transform matrix.
		/// </summary>
		public static void ResetTransformMatrix()
		{
			if (_transformMatrixStack.Count == 0)
			{
				throw(new InvalidOperationException("Matrix stack is empty! Did you forgot to set a matrix somewhere?"));
			}

			SwitchPipelineMode(PipelineMode.None, null); 
			CurrentTransformMatrix = _transformMatrixStack.Pop();
		}

		#endregion surfaces and matrixes



		#region sprites
		

		public static void DrawFrame(Frame frame, Vector2 pos, Vector2 scale, float rotation, Vector2 offset, Color color, SpriteEffects effect)
		{
			SwitchPipelineMode(PipelineMode.Sprites, null);

			Batch.Draw(
				frame.Texture, 
				pos, 
				frame.TexturePosition, 
				color, 
				MathHelper.ToRadians(rotation), 
				offset - frame.Origin,
				scale,
				effect, 
				0
			);
		}


		private static int CalculateSpriteFrame(Sprite sprite, float frame)
		{
			return Math.Max(0, Math.Min(sprite.Frames.Count() - 1, (int)frame));
		}

		// Vectors.

		public static void DrawSprite(Sprite sprite, Vector2 pos)
		{
			DrawFrame(sprite.Frames[0], pos, Vector2.One, 0, sprite.Origin, CurrentColor, SpriteEffects.None);
		}
		
		public static void DrawSprite(Sprite sprite, float frameId, Vector2 pos)
		{			
			int frame = CalculateSpriteFrame(sprite, frameId);
			
			DrawFrame(sprite.Frames[frame], pos, Vector2.One, 0, sprite.Origin, CurrentColor, SpriteEffects.None);
		}


		public static void DrawSprite(Sprite sprite, float frameId, Vector2 pos, Vector2 scale, float rotation, Color color)
		{
			SwitchPipelineMode(PipelineMode.Sprites, null);
			
			int frame = CalculateSpriteFrame(sprite, frameId);
			
			SpriteEffects mirroring = SpriteEffects.None;

			// Proper negative scaling.
			Vector2 offset = Vector2.Zero;

			if (scale.X < 0)
			{
				mirroring = mirroring | SpriteEffects.FlipHorizontally;
				scale.X *= -1;
				offset.X = sprite.W;
			}

			if (scale.Y < 0)
			{
				mirroring = mirroring | SpriteEffects.FlipVertically;
				scale.Y *= -1;
				offset.Y = sprite.H;
			}
			// Proper negative scaling.

			DrawFrame(sprite.Frames[frame], pos, scale, rotation, sprite.Origin + offset, color, mirroring);
		}
		
		// Vectors.
		
		// Floats.

		public static void DrawSprite(Sprite sprite, float x, float y)
		{
			DrawSprite(sprite, new Vector2(x, y));
		}
		
		public static void DrawSprite(Sprite sprite, float frameId, float x, float y)
		{
			DrawSprite(sprite, frameId, new Vector2(x, y));
		}
		
		public static void DrawSprite(Sprite sprite, float frameId, float x, float y, float scaleX, float scaleY, float rotation, Color color)
		{
			DrawSprite(sprite, frameId, new Vector2(x, y), new Vector2(scaleX, scaleY), rotation, color);
		}

		// Floats.

		// Rectangles.

		public static void DrawSprite(Sprite sprite, float frameId, Rectangle posRect)
		{
			SwitchPipelineMode(PipelineMode.Sprites, null);
			
			int frame = CalculateSpriteFrame(sprite, frameId);
			
			Batch.Draw(sprite.Frames[frame].Texture, posRect, sprite.Frames[frame].TexturePosition, CurrentColor);
		}
		
		public static void DrawSprite(Sprite sprite, float frameId, Rectangle posRect, float rotation, Color color)
		{
			SwitchPipelineMode(PipelineMode.Sprites, null);
			
			int frame = CalculateSpriteFrame(sprite, frameId);
			
			Batch.Draw(
				sprite.Frames[frame].Texture, 
				posRect, 
				sprite.Frames[frame].TexturePosition, 
				color, 
				rotation, 
				sprite.Origin - sprite.Frames[frame].Origin,
				SpriteEffects.None, 
				0
			);
		}

		public static void DrawSprite(Sprite sprite, float frameId, Rectangle posRect, Rectangle scissorRect)
		{
			SwitchPipelineMode(PipelineMode.Sprites, null);
			
			int frame = CalculateSpriteFrame(sprite, frameId);
			
			scissorRect.X += sprite.Frames[frame].TexturePosition.X;
			scissorRect.Y += sprite.Frames[frame].TexturePosition.Y;

			Batch.Draw(sprite.Frames[frame].Texture, posRect, scissorRect, CurrentColor);
		}
		
		public static void DrawSprite(Sprite sprite, float frameId, Rectangle posRect, Rectangle scissorRect, float rotation, Color color)
		{
			SwitchPipelineMode(PipelineMode.Sprites, null);
			
			int frame = CalculateSpriteFrame(sprite, frameId);
			
			scissorRect.X += sprite.Frames[frame].TexturePosition.X;
			scissorRect.Y += sprite.Frames[frame].TexturePosition.Y;

			Batch.Draw(
				sprite.Frames[frame].Texture, 
				posRect, 
				sprite.Frames[frame].TexturePosition, 
				color, 
				rotation, 
				sprite.Origin - sprite.Frames[frame].Origin,
				SpriteEffects.None, 
				0
			);
		}

		// Rectangles.
		
		#endregion sprites



		#region shapes


		#region line overloads
		
		/// <summary>
		/// Draws a line.
		/// </summary>
		public static void DrawLine(Vector2 p1, Vector2 p2)
		{
			DrawLine(p1.X, p1.Y, p2.X, p2.Y, CurrentColor, CurrentColor);
		}

		/// <summary>
		/// Draws a line with specified colors.
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		public static void DrawLine(Vector2 p1, Vector2 p2, Color c1, Color c2)
		{
			DrawLine(p1.X, p1.Y, p2.X, p2.Y, c1, c2);
		}

		/// <summary>
		/// Draws a line with specified width and colors.
		/// </summary>
		public static void DrawLine(Vector2 p1, Vector2 p2, float w, Color c1, Color c2)
		{
			DrawLine(p1.X, p1.Y, p2.X, p2.Y, w, c1, c2);
		}

		/// <summary>
		/// Draws a line.
		/// </summary>
		public static void DrawLine(float x1, float y1, float x2, float y2)
		{
			DrawLine(x1, y1, x2, y2, CurrentColor, CurrentColor);
		}

		/// <summary>
		/// Draws a line with specified width.
		/// </summary>
		public static void DrawLine(float x1, float y1, float x2, float y2, float w)
		{
			DrawLine(x1, y1, x2, y2, CurrentColor, CurrentColor);
		}

		#endregion line overloads

		/// <summary>
		/// Draws a line with specified colors.
		/// </summary>
		/// <param name="c1">First color.</param>
		/// <param name="c2">Second color.</param>
		public static void DrawLine(float x1, float y1, float x2, float y2, Color c1, Color c2)
		{
			var vertices = new List<VertexPositionColorTexture>();

			vertices.Add(new VertexPositionColorTexture(new Vector3(x1, y1, 0), c1, Vector2.Zero));
			vertices.Add(new VertexPositionColorTexture(new Vector3(x2, y2, 0), c2, Vector2.Zero));
			
			AddVertices(PipelineMode.OutlinePrimitives, null, vertices, new short[]{0, 1});
		}

		/// <summary>
		/// Draws a line with specified width and colors.
		/// </summary>
		public static void DrawLine(float x1, float y1, float x2, float y2, float w, Color c1, Color c2)
		{
			var vertices = new List<VertexPositionColorTexture>();

			Vector3 normal = new Vector3(y2 - y1, x1 - x2, 0);
			normal.Normalize(); // The result is a unit vector rotated by 90 degrees.
			normal *= w / 2;
			
			vertices.Add(new VertexPositionColorTexture(new Vector3(x1, y1, 0) + normal, c1, Vector2.Zero));
			vertices.Add(new VertexPositionColorTexture(new Vector3(x1, y1, 0) - normal, c1, Vector2.Zero));
			vertices.Add(new VertexPositionColorTexture(new Vector3(x2, y2, 0) - normal, c2, Vector2.Zero));
			vertices.Add(new VertexPositionColorTexture(new Vector3(x2, y2, 0) + normal, c2, Vector2.Zero));
			
			AddVertices(PipelineMode.TrianglePrimitives, null, vertices, _rectangleIndices[1]); // Thick line is in fact just a rotated rectangle.
		}


		#region triangle overloads

		/// <summary>
		/// Draws a triangle.
		/// </summary>
		/// <param name="p1">First point.</param>
		/// <param name="p2">Second point.</param>
		/// <param name="p3">Third point.</param>
		/// <param name="isOutline">Is shape outline.</param>
		public static void DrawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, bool isOutline)
		{
			DrawTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, isOutline, CurrentColor, CurrentColor, CurrentColor);
		}

		/// <summary>
		/// Draws a triangle with specified colors.
		/// </summary>
		/// <param name="p1">First point.</param>
		/// <param name="p2">Second point.</param>
		/// <param name="p3">Third point.</param>
		/// <param name="isOutline">Is shape outline.</param>
		/// <param name="c1">1st color.</param>
		/// <param name="c2">2nd color.</param>
		/// <param name="c3">3rd color.</param>
		public static void DrawTriangle(Vector2 p1, Vector2 p2, Vector2 p3, bool isOutline, Color c1, Color c2, Color c3)
		{
			DrawTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, isOutline, c1, c2, c3);
		}
		
		/// <summary>
		/// Draws a triangle.
		/// </summary>
		/// <param name="isOutline"></param>
		public static void DrawTriangle(float x1, float y1, float x2, float y2, float x3, float y3, bool isOutline)
		{
			DrawTriangle(x1, y1, x2, y2, x3, y3, isOutline, CurrentColor, CurrentColor, CurrentColor);
		}

		#endregion triangle overloads

		/// <summary>
		/// Draw a triangle with specified colors.
		/// </summary>
		/// <param name="isOutline">Is shape outline.</param>
		/// <param name="c1">1st color.</param>
		/// <param name="c2">2nd color.</param>
		/// <param name="c3">3rd color.</param>
		public static void DrawTriangle(float x1, float y1, float x2, float y2, float x3, float y3, bool isOutline, Color c1, Color c2, Color c3)
		{
			int isOutlineInt = Convert.ToInt32(isOutline); // We need to convert true/false to 1/0 to be able to get different sets of values from arrays. 
			
			var vertices = new List<VertexPositionColorTexture>();
			vertices.Add(new VertexPositionColorTexture(new Vector3(x1, y1, 0), c1, Vector2.Zero));
			vertices.Add(new VertexPositionColorTexture(new Vector3(x2, y2, 0), c2, Vector2.Zero));
			vertices.Add(new VertexPositionColorTexture(new Vector3(x3, y3, 0), c3, Vector2.Zero));
			
			AddVertices(_pipelineModes[isOutlineInt], null, vertices, _triangleIndices[isOutlineInt]);
		}



		#region rectangle overloads

		/// <summary>
		/// Draws a rectangle.
		/// </summary>
		/// <param name="p1">First point.</param>
		/// <param name="p2">Second point.</param>
		/// <param name="isOutline">Is shape outline.</param>
		public static void DrawRectangle(Vector2 p1, Vector2 p2, bool isOutline)
		{
			DrawRectangle(p1.X, p1.Y, p2.X, p2.Y, isOutline, CurrentColor, CurrentColor, CurrentColor, CurrentColor);
		}

		/// <summary>
		/// Draws a rectangle with specified colors for each corner.
		/// </summary>
		/// <param name="p1">First point.</param>
		/// <param name="p2">Second point.</param>
		/// <param name="isOutline">Is shape outline.</param>
		/// <param name="c1">1st color.</param>
		/// <param name="c2">2nd color.</param>
		/// <param name="c3">3rd color.</param>
		/// <param name="c4">4th color.</param>
		public static void DrawRectangle(Vector2 p1, Vector2 p2, bool isOutline, Color c1, Color c2, Color c3, Color c4)
		{
			DrawRectangle(p1.X, p1.Y, p2.X, p2.Y, isOutline, c1, c2, c3, c4);
		}

		/// <summary>
		/// Draws a rectangle.
		/// </summary>
		/// <param name="isOutline">Is shape outline</param>
		public static void DrawRectangle(float x1, float y1, float x2, float y2, bool isOutline)
		{
			DrawRectangle(x1, y1, x2, y2, isOutline, CurrentColor, CurrentColor, CurrentColor, CurrentColor);
		}

		#endregion rectangle overloads

		/// <summary>
		/// Draws a rectangle with specified colors for each corner.
		/// </summary>
		/// <param name="isOutline">Is shape outline.</param>
		/// <param name="c1">1st color.</param>
		/// <param name="c2">2nd color.</param>
		/// <param name="c3">3rd color.</param>
		/// <param name="c4">4th color.</param>
		public static void DrawRectangle(float x1, float y1, float x2, float y2, bool isOutline, Color c1, Color c2, Color c3, Color c4)
		{
			int isOutlineInt = Convert.ToInt32(isOutline); // We need to convert true/false to 1/0 to be able to get different sets of values from arrays. 

			var vertices = new List<VertexPositionColorTexture>();
			
			vertices.Add(new VertexPositionColorTexture(new Vector3(x1, y1, 0), c1, Vector2.Zero));
			vertices.Add(new VertexPositionColorTexture(new Vector3(x2, y1, 0), c2, Vector2.Zero));
			vertices.Add(new VertexPositionColorTexture(new Vector3(x2, y2, 0), c3, Vector2.Zero));
			vertices.Add(new VertexPositionColorTexture(new Vector3(x1, y2, 0), c4, Vector2.Zero));
			
			AddVertices(_pipelineModes[isOutlineInt], null, vertices, _rectangleIndices[isOutlineInt]);
		}


		#region circle overloads

		/// <summary>
		/// Draws a circle.
		/// </summary>
		/// <param name="r">Radius.</param>
		/// <param name="isOutline">Is shape outline.</param>
		public static void DrawCircle(Vector2 p, float r, bool isOutline)
		{
			DrawCircle(p.X, p.Y, r, isOutline);
		}

		#endregion circle overloads

		/// <summary>
		/// Draws a circle.
		/// </summary>
		/// <param name="p">Center point.</param>
		/// <param name="r">Radius.</param>
		/// <param name="isOutline">Is shape outline.</param>
		public static void DrawCircle(float x, float y, float r, bool isOutline)
		{
			short[] indexArray;
			PipelineMode prType;
			if (isOutline)
			{
				indexArray = new short[CircleVerticesCount * 2];
				prType = PipelineMode.OutlinePrimitives;
				
				for(var i = 0; i< CircleVerticesCount - 1; i += 1)
				{
					indexArray[i * 2] = (short)i;
					indexArray[i * 2 + 1] = (short)(i + 1);
				}
				indexArray[(CircleVerticesCount - 1) * 2] = (short)(CircleVerticesCount - 1);
				indexArray[(CircleVerticesCount - 1) * 2 + 1] = 0;
			}
			else
			{
				indexArray = new short[CircleVerticesCount * 3];
				prType = PipelineMode.TrianglePrimitives;

				for(var i = 0; i < CircleVerticesCount - 1; i += 1)
				{
					indexArray[i * 3] = 0;
					indexArray[i * 3] = (short)i;
					indexArray[i * 3 + 1] = (short)(i + 1);
				}

			}

			var vertices = new List<VertexPositionColorTexture>();
			
			for(var i = 0; i < CircleVerticesCount; i += 1)
			{vertices.Add(new VertexPositionColorTexture(new Vector3(x + r * _circleVectors[i].X, y + r * _circleVectors[i].Y, 0), CurrentColor, Vector2.Zero));}
			AddVertices(prType, null, vertices, indexArray);
		}

		#endregion shapes



		#region primitives
		
		/// <summary>
		/// Sets texture for a primitive.
		/// </summary>
		/// <param name="texture"></param>
		public static void PrimitiveSetTexture(Texture2D texture)
		{
			_primitiveTexture = texture;
			_primitiveTextureOffset = Vector2.Zero;
			_primitiveTextureRatio = Vector2.One;
		}

		/// <summary>
		/// Sets texture for a primitive.
		/// </summary>
		/// <param name="texture"></param>
		public static void PrimitiveSetTexture(Sprite sprite, float frameId)
		{
			Frame frame = sprite.Frames[(int)frameId];

			_primitiveTexture = frame.Texture;
			_primitiveTextureOffset = new Vector2(
				frame.TexturePosition.X / (float)frame.Texture.Width, 
				frame.TexturePosition.Y / (float)frame.Texture.Height
			);
			
			_primitiveTextureRatio = new Vector2(
				frame.TexturePosition.Width / (float)frame.Texture.Width, 
				frame.TexturePosition.Height / (float)frame.Texture.Height
			);
		}



		#region AddVertex overloads

		public static void PrimitiveAddVertex(Vector2 pos)
		{
			PrimitiveAddVertex(pos.X, pos.Y, CurrentColor, Vector2.Zero);
		}

		public static void PrimitiveAddVertex(Vector2 pos, Color color)
		{
			PrimitiveAddVertex(pos.X, pos.Y, color, Vector2.Zero);
		}

		public static void PrimitiveAddVertex(Vector2 pos, Vector2 texturePos)
		{
			PrimitiveAddVertex(pos.X, pos.Y, CurrentColor, texturePos);
		}

		public static void PrimitiveAddVertex(Vector2 pos, Color color, Vector2 texturePos)
		{
			_primitiveVertices.Add(new VertexPositionColorTexture(new Vector3(pos.X, pos.Y, 0), color, texturePos));
		}

		public static void PrimitiveAddVertex(float x, float y)
		{
			PrimitiveAddVertex(x, y, CurrentColor, Vector2.Zero);
		}

		public static void PrimitiveAddVertex(float x, float y, Color color)
		{
			PrimitiveAddVertex(x, y, color, Vector2.Zero);
		}
	
		public static void PrimitiveAddVertex(float x, float y, Vector2 texturePos)
		{
			PrimitiveAddVertex(x, y, CurrentColor, texturePos);
		}
		
		#endregion AddVertex overloads

		public static void PrimitiveAddVertex(float x, float y, Color color, Vector2 texturePos)
		{
			/*
			 * Since we may work with sprites, which are only little parts of whole texture atlass,
			 * we need to convert local sprite coordinates to global atlass coordinates.
			 */
			Vector2 atlassPos = _primitiveTextureOffset + texturePos * _primitiveTextureRatio;
			_primitiveVertices.Add(new VertexPositionColorTexture(new Vector3(x, y, 0), color, atlassPos));
		}



		/// <summary>
		/// Sets indexes according to trianglestrip pattern.
		/// </summary>
		public static void PrimitiveSetTriangleStripIndices()
		{
			// 0 - 2 - 4
			//  \ / \ /
			//   1 - 3
			
			_primitiveType = PipelineMode.TrianglePrimitives;

			for(var i = 0; i < _primitiveVertices.Count - 2; i += 1)
			{
				_primitiveIndices.Add((short)i);
				_primitiveIndices.Add((short)(i + 1));
				_primitiveIndices.Add((short)(i + 2));
			}
		}
		
		/// <summary>
		/// Sets indexes according to trianglefan pattern.
		/// </summary>
		public static void PrimitiveSetTriangleFanIndices()
		{
			//   1
			//  / \
			// 0 - 2 
			//  \ / 
			//   3 

			_primitiveType = PipelineMode.TrianglePrimitives;

			for(var i = 0; i < _primitiveVertices.Count - 1; i += 1)
			{
				_primitiveIndices.Add(0);
				_primitiveIndices.Add((short)i);
				_primitiveIndices.Add((short)(i + 1));
			}
		}
		
		/// <summary>
		/// Sets indexes according to mesh pattern.
		/// IMPORTANT: Make sure there's enough vertices for width and height of the mesh.
		/// </summary>
		/// <param name="w">Width of the mesh.</param>
		/// <param name="h">Height of the mesh.</param>
		public static void PrimitiveSetMeshIndices(int w, int h)
		{
			// 0 - 1 - 2
			// | / | / |
			// 3 - 4 - 5
			// | / | / |
			// 6 - 7 - 8

			_primitiveType = PipelineMode.TrianglePrimitives;

			var offset = 0; // Basically, equals w * y.

			for(var y = 0; y < h - 1; y += 1)
			{
				for(var x = 0; x < w - 1; x += 1)
				{
					_primitiveIndices.Add((short)(x + offset));
					_primitiveIndices.Add((short)(x + 1 + offset));
					_primitiveIndices.Add((short)(x + w + offset));

					_primitiveIndices.Add((short)(x + w + offset));
					_primitiveIndices.Add((short)(x + w + 1 + offset));
					_primitiveIndices.Add((short)(x + 1 + offset));
				}
				offset += w;
			}
		}

		/// <summary>
		/// Sets indexes according to line strip pattern.
		/// </summary>
		/// <param name="loop">Tells is first and last vertix will have a line between them.</param>
		public static void PrimitiveSetLineStripIndices(bool loop)
		{
			// 0 - 1 - 2 - 3
			
			_primitiveType = PipelineMode.OutlinePrimitives;

			for(var i = 0; i < _primitiveVertices.Count - 1; i += 1)
			{
				_primitiveIndices.Add((short)i);
				_primitiveIndices.Add((short)(i + 1));
			}
			if (loop)
			{
				_primitiveIndices.Add((short)(_primitiveVertices.Count - 1));
				_primitiveIndices.Add(0);
			}
		}



		/// <summary>
		/// Sets user-defined array of indices for alist of lines.
		/// </summary>
		/// <param name="indices">Array of indices.</param>
		public static void PrimitiveSetCustomLineIndices(short[] indices)
		{ 
			_primitiveType = PipelineMode.OutlinePrimitives;
			_primitiveIndices = indices.ToList();
		}
		
		/// <summary>
		/// Sets user-defined list of indices for list of lines.
		/// </summary>
		/// <param name="indices">List of indices.</param>
		public static void PrimitiveSetCustomLineIndices(List<short> indices)
		{
			_primitiveType = PipelineMode.OutlinePrimitives;
			_primitiveIndices = indices;
		}
		
		/// <summary>
		/// Sets user-defined array of indices for list of triangles.
		/// </summary>
		/// <param name="indices">Array of indices.</param>
		public static void PrimitiveSetCustomTriangleIndices(short[] indices)
		{
			_primitiveType = PipelineMode.TrianglePrimitives;
			_primitiveIndices = indices.ToList();
		}
		
		/// <summary>
		/// Sets user-defined list of indices for list of triangles.
		/// </summary>
		/// <param name="indices">List of indices.</param>
		public static void PrimitiveSetCustomTriangleIndices(List<short> indices)
		{
			_primitiveType = PipelineMode.TrianglePrimitives;
			_primitiveIndices = indices;
		}


		/// <summary>
		/// Begins primitive drawing. 
		/// It's more of a safety check for junk primitives.
		/// </summary>
		public static void PrimitiveBegin()
		{
			if (_primitiveVertices.Count != 0 || _primitiveIndices.Count != 0)
			{throw(new Exception("Junk primitive data detected! Did you set index data wrong or forgot PrimitiveEnd somewhere?"));}
		}

		/// <summary>
		/// Ends drawing of a primitive.
		/// </summary>
		public static void PrimitiveEnd()
		{
			
			AddVertices(_primitiveType, _primitiveTexture, _primitiveVertices, _primitiveIndices.ToArray());

			_primitiveVertices.Clear();
			_primitiveIndices.Clear();
			_primitiveTexture = null;
		}

		#endregion primitives



		#region text

		/// <summary>
		/// Draws text in specified coordinates.
		/// </summary>
		public static void DrawText(string text, float x, float y) => DrawText(text, new Vector2(x, y));

		/// <summary>
		/// Draws text in specified coordinates.
		/// </summary>
		public static void DrawText(string text, Vector2 pos)
		{
			SwitchPipelineMode(PipelineMode.Sprites, null);
			
			CurrentFont.Draw(Batch, text, pos, HorAlign, VerAlign);
		}

		/// <summary>
		/// Draws text in specified coordinates with rotation, scale and origin.
		/// </summary>
		public static void DrawText(string text, Vector2 pos, Vector2 scale, Vector2 origin, float rot = 0) 
		=> DrawText(text, pos.X, pos.Y, scale.X, scale.Y, origin.X, origin.Y, rot);

		/// <summary>
		/// Draws text in specified coordinates with rotation, scale and origin.
		/// </summary>
		public static void DrawText(string text, float x, float y, float scaleX, float scaleY, float originX = 0, float originY = 0, float rot = 0)
		{
			Matrix transformMatrix = Matrix.CreateTranslation(new Vector3(-originX, -originY, 0)) * // Origin.
		                    Matrix.CreateRotationZ(MathHelper.ToRadians(-rot)) *		              // Rotation.
		                    Matrix.CreateScale(new Vector3(scaleX, scaleY, 1)) *	                // Scale.
		                    Matrix.CreateTranslation(new Vector3(x, y, 0));                       // Position.
												
			AddTransformMatrix(transformMatrix);
			SwitchPipelineMode(PipelineMode.Sprites, null);
			CurrentFont.Draw(Batch, text, Vector2.Zero, HorAlign, VerAlign);
			ResetTransformMatrix();
		}

		#endregion text




		public static void DrawSurface(RenderTarget2D surf, float x, float y, Color color)
		{
			SwitchPipelineMode(PipelineMode.Sprites, null);
			Batch.Draw(surf, new Vector2(x, y), color);
		}


	}
}