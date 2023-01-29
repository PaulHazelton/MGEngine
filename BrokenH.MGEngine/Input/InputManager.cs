using System;
using Microsoft.Xna.Framework.Input;
using BrokenH.MGEngine.Input.Interfaces;

namespace BrokenH.MGEngine.Input
{
	public static class InputManager
	{
		private static KeyboardState s_oldKeyState;
		private static MouseState s_oldMouseState;

		private static ICommandMapper? _commandMapper;
		private static IInputHandler? _inputHandler;

		public static int[] Commands = Array.Empty<int>();


		public static void Initialize(IInputHandler inputHandler, ICommandMapper commandMapper, Type commandEnum)
		{
			InputManager._inputHandler = inputHandler;
			InputManager._commandMapper = commandMapper;
			Commands = (int[])Enum.GetValues(commandEnum);
		}

		public static void Update()
		{
			if (_inputHandler == null || _commandMapper == null || Commands == null)
				return;

			// Keyboard --------
			KeyboardState keyState = Keyboard.GetState();

			// For each command, if it was up last update, and down this update, trigger event
			foreach (int command in Commands)
			{
				if (keyState.IsKeyDown(_commandMapper.GetKeys(command)) && s_oldKeyState.IsKeyUp(_commandMapper.GetKeys(command)))
					_inputHandler.CommandEvent(command, ButtonState.Pressed);
				if (keyState.IsKeyUp(_commandMapper.GetKeys(command)) && s_oldKeyState.IsKeyDown(_commandMapper.GetKeys(command)))
					_inputHandler.CommandEvent(command, ButtonState.Released);
			}

			// For each key, if it was up last update, and down this update, trigger event
			foreach (Keys key in s_oldKeyState.GetPressedKeys())
			{
				if (keyState.IsKeyUp(key))
					_inputHandler.KeyEvent(key, ButtonState.Released);
			}
			foreach (Keys key in keyState.GetPressedKeys())
			{
				if (s_oldKeyState.IsKeyUp(key))
					_inputHandler.KeyEvent(key, ButtonState.Pressed);
			}

			s_oldKeyState = keyState;

			// Mouse --------
			MouseState mouseState = Mouse.GetState();

			// Left click
			if (mouseState.LeftButton == ButtonState.Pressed && s_oldMouseState.LeftButton == ButtonState.Released)
				_inputHandler.MouseEvent(MouseButton.Left, ButtonState.Pressed);
			if (mouseState.LeftButton == ButtonState.Released && s_oldMouseState.LeftButton == ButtonState.Pressed)
				_inputHandler.MouseEvent(MouseButton.Left, ButtonState.Released);

			// Middle click
			if (mouseState.MiddleButton == ButtonState.Pressed && s_oldMouseState.MiddleButton == ButtonState.Released)
				_inputHandler.MouseEvent(MouseButton.Middle, ButtonState.Pressed);
			if (mouseState.MiddleButton == ButtonState.Released && s_oldMouseState.MiddleButton == ButtonState.Pressed)
				_inputHandler.MouseEvent(MouseButton.Middle, ButtonState.Released);

			// Right click
			if (mouseState.RightButton == ButtonState.Pressed && s_oldMouseState.RightButton == ButtonState.Released)
				_inputHandler.MouseEvent(MouseButton.Right, ButtonState.Pressed);
			if (mouseState.RightButton == ButtonState.Released && s_oldMouseState.RightButton == ButtonState.Pressed)
				_inputHandler.MouseEvent(MouseButton.Right, ButtonState.Released);

			// Scroll
			if (mouseState.ScrollWheelValue != s_oldMouseState.ScrollWheelValue)
				_inputHandler.ScrollEvent(ScrollDirection.Vertical, mouseState.ScrollWheelValue - s_oldMouseState.ScrollWheelValue);
			if (mouseState.HorizontalScrollWheelValue != s_oldMouseState.HorizontalScrollWheelValue)
				_inputHandler.ScrollEvent(ScrollDirection.Horizontal, mouseState.HorizontalScrollWheelValue - s_oldMouseState.HorizontalScrollWheelValue);

			s_oldMouseState = mouseState;
		}

		public static bool IsCommandActive(int command)
		{
			_ = _commandMapper ?? throw new InvalidOperationException($"{nameof(Initialize)} must be called before {nameof(IsCommandActive)}");
			return Keyboard.GetState().IsKeyDown(_commandMapper.GetKeys(command));
		}
	}

	public static class KeyboardStateExtensions
	{
		public static bool IsKeyDown(this KeyboardState ks, (Keys, Keys) keys)
		{
			return ks.IsKeyDown(keys.Item1) | ks.IsKeyDown(keys.Item2);
		}
		public static bool IsKeyUp(this KeyboardState ks, (Keys, Keys) keys)
		{
			return ks.IsKeyUp(keys.Item1) & ks.IsKeyUp(keys.Item2);
		}
	}
}