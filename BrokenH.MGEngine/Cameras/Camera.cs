using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BrokenH.MGEngine.Cameras
{
	public class Camera
	{
		// Camera Types
		public enum CameraSmoothingMode
		{
			Fixed,
			Lerp,
		}

		// Dependencies
		private Random _randomGenerator;

		// Camera result
		public Matrix View { get; private set; }
		public Matrix SimView { get; private set; }

		// Controlling the camera properties
		public Vector2 TargetPosition { get; set; }
		public float TargetRotation { get; set; }
		public float TargetScale { get; set; }
		public Vector2 ScreenCenter { get; private set; }
		public CameraSmoothingMode SmoothingMode { get; set; }
		public float CameraSpeed { get; set; }

		// Private details
		private int _screenWidth;
		private int _screenHeight;
		private Rectangle? _clampRectangle;
		public CameraShaker Shaker { get; set; }

		// Public details
		public Vector2 Position { get; private set; }
		public float Rotation { get; private set; }
		public float Scale { get; private set; }


		// Constructor
		public Camera(int screenX, int screenY)
		{
			_screenWidth = screenX;
			_screenHeight = screenY;

			_randomGenerator = new();
			Shaker = new();

			// Set defaults
			Position = Vector2.Zero;
			Rotation = 0f;
			Scale = 1f;

			TargetPosition = Vector2.Zero;
			TargetRotation = 0f;
			TargetScale = 1f;
			SmoothingMode = CameraSmoothingMode.Lerp;
			CameraSpeed = 6.0f;

			ScreenCenter = new Vector2(screenX / 2, screenY / 2);
			View = Matrix.Identity;
			UpdateView();
		}

		// Update (for smoothing)
		public void Update(GameTime deltaTime)
		{
			float progress = (float)(CameraSpeed * deltaTime.ElapsedGameTime.TotalSeconds);
			switch (SmoothingMode)
			{
				case CameraSmoothingMode.Fixed:
					Position = TargetPosition;
					Rotation = TargetRotation;
					Scale = TargetScale;
					break;
				case CameraSmoothingMode.Lerp:
					Position = Vector2.Lerp(Position, TargetPosition, progress);
					Rotation = MathHelper.Lerp(Rotation, TargetRotation, progress);
					Scale = MathHelper.Lerp(Scale, TargetScale, progress);
					break;
			}

			Shaker.Update(deltaTime);
			UpdateView();
		}

		// Alter camera settings
		public void ClampToRectangle(Rectangle rectangle)
		{
			_clampRectangle = rectangle;
		}
		public void SetZoomTarget(Rectangle target)
		{
			TargetPosition = new Vector2(target.X + (target.Width / 2), target.Y + (target.Height / 2));
			var s1 = (float)_screenWidth / target.Width;
			var s2 = (float)_screenHeight / target.Height;
			TargetScale = MathHelper.Min(s1, s2);
		}

		// Positioning the camera
		public void MoveCamera(Vector2 position) => MoveCamera(position, Rotation, Scale);
		public void MoveCamera(Vector2 position, float rotation) => MoveCamera(position, rotation, Scale);
		public void MoveCamera(Vector2 position, float rotation, float scale)
		{
			TargetPosition = position;
			TargetRotation = rotation;
			TargetScale = scale;
			UpdateView();
		}

		public void SnapPosition(Vector2 position)
		{
			TargetPosition = position;
			Position = position;
		}
		public void SnapScale(float scale)
		{
			TargetScale = scale;
			Scale = scale;
		}

		public void SetPosition(Vector2 position)
		{
			TargetPosition = position;
			UpdateView();
		}
		public void SetRotation(float rotation)
		{
			TargetRotation = rotation;
			UpdateView();
		}
		public void SetScale(float scale)
		{
			TargetScale = scale;
			UpdateView();
		}

		public void UpdateScreenSize(int screenX, int screenY)
		{
			_screenWidth = screenX;
			_screenHeight = screenY;
			ScreenCenter = new Vector2(screenX / 2, screenY / 2);
			UpdateView();
		}

		// Utilities
		public Vector2 GetMouseWorld()
		{
			return ConvertUnits.ToSimUnits(Vector2.Transform(
				Mouse.GetState().Position.ToVector2(),
				Matrix.Invert(View)
			));
		}

		// Privates
		private void UpdateView()
		{
			Clamp();
			ApplyShake();

			View
				= Matrix.CreateTranslation(new Vector3(-Position, 0f))
				* Matrix.CreateRotationZ(-Rotation)
				* Matrix.CreateScale(Scale, Scale, 1)
				* Matrix.CreateTranslation(new Vector3(ScreenCenter, 0f));

			SimView
				= Matrix.CreateTranslation(new Vector3(ConvertUnits.ToSimUnits(-Position), 0))
				* Matrix.CreateRotationZ(-Rotation)
				* Matrix.CreateScale(Scale, Scale, 1);
		}

		/// <summary>
		/// Limits the position and target position of the camera to be not only contained in the target rectangle, but also limits the camera from showing area outside the clamp rectangle. (If the clamp rectangle is smaller than the screen, out of bounds area will be shown.)
		/// </summary>
		private void Clamp()
		{
			if (!_clampRectangle.HasValue) return;
			int sw = (int)(_screenWidth / Scale);	// Scaled screen width
			int sh = (int)(_screenHeight / Scale);	// Scaled screen height

			Rectangle cr = _clampRectangle.Value;
			Vector2 pos = new();
			float hw = sw / 2;
			float hh = sh / 2;

			if (cr.Width < sw)
				pos.X = cr.Width / 2;
			else
				pos.X = MathHelper.Clamp(TargetPosition.X, cr.X + hw, cr.X + cr.Width - hw);
			if (cr.Height < sh)
				pos.Y = cr.Height / 2;
			else
				pos.Y = MathHelper.Clamp(TargetPosition.Y, cr.Y + hh, cr.Y + cr.Height - hh);

			TargetPosition = pos;

			if (cr.Width < sw)
				pos.X = cr.Width / 2;
			else
				pos.X = MathHelper.Clamp(Position.X, cr.X + hw, cr.X + cr.Width - hw);
			if (cr.Height < sh)
				pos.Y = cr.Height / 2;
			else
				pos.Y = MathHelper.Clamp(Position.Y, cr.Y + hh, cr.Y + cr.Height - hh);

			Position = pos;
		}

		private void ApplyShake()
		{
			Position += Shaker.PositionOffset;
			Rotation += Shaker.RotationOffset;
		}
	}
}