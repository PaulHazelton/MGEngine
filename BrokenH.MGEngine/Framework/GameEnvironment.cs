using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BrokenH.MGEngine.Input;
using BrokenH.MGEngine.Input.Interfaces;

namespace BrokenH.MGEngine.Framework
{
	public abstract class GameEnvironment : IInputHandler, IDisposable
	{
		// Parent
		protected GameManager GameManager { get; private set; }

		// Services
		public IServiceProvider ServiceProvider { get; private set; }

		// Graphics Data
		public ContentManager ContentManager { get; private set; }
		public GraphicsDevice GraphicsDevice { get; set; }
		protected int ScreenWidth { get; private set; }
		protected int ScreenHeight { get; private set; }
		protected Color BackgroundColor { get; set; }

		// Constructors
		protected GameEnvironment(GameManager manager, GraphicsDevice device, IServiceProvider serviceProvider)
		{
			GameManager = manager;
			GraphicsDevice = device;
			ServiceProvider = serviceProvider;

			ContentManager = new ContentManager(serviceProvider, "Content");
			ScreenWidth = GraphicsDevice.Viewport.Width;
			ScreenHeight = GraphicsDevice.Viewport.Height;
		}
		public virtual void Dispose()
		{
			ContentManager.Dispose();
		}

		// Event handling
		public abstract void CommandEvent(int command, ButtonState eventType);
		public abstract void KeyEvent(Keys key, ButtonState eventType);
		public abstract void MouseEvent(MouseButton button, ButtonState state);
		public abstract void ScrollEvent(ScrollDirection direction, int distance);
		public void WindowSizeChanged(int w, int h)
		{
			ScreenWidth = w;
			ScreenHeight = h;
			UpdateSize();
		}

		protected abstract void UpdateSize();

		// Updating and drawing
		public abstract void Update(GameTime gameTime);
		public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
	}
}