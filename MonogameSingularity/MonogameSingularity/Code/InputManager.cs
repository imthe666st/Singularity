﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Singularity.GameObjects;
using Singularity.Utilities;

namespace Singularity
{
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.Input;

	public enum MouseKeys
	{
		Left,
		Right,
		Middle,
		X1,
		X2
	}

	//TODO: multiple Gamepads
	/// <summary>
	/// Static class to handle input
	/// </summary>
	public class InputManager
	{
		#region Singleton

		public static  InputManager Instance => _instance ?? (_instance = new InputManager());
		private static InputManager _instance;

		#endregion
		
		//Mouse movement
		private Point _mouseStartPos = new Point(200,200);
		private bool _lockMouse = false;
		private bool _relock = false;
		private Vector2 _mouseMovement = new Vector2(0);
		private float _mouseSensitivity = 200f;

		//Mouse buttons
		private bool _leftMouseLastFrame = false;
		private bool _leftMouseThisFrame = false;
		private bool _rightMouseLastFrame = false;
		private bool _rightMouseThisFrame = false;
		private bool _middleMouseLastFrame = false;
		private bool _middleMouseThisFrame = false;
		private bool _x1MouseLastFrame  = false;
		private bool _x1MouseThisFrame  = false;
		private bool _x2MouseLastFrame = false;
		private bool _x2MouseThisFrame = false;

		//Mouse wheel
		private int _mouseWheelLastFrame = 0;
		private int _mouseWheelThisFrame = 0;

		//Gamepad Sticksmovement
		private GamePadDeadZone _deadZone = GamePadDeadZone.IndependentAxes;
		private Vector2 _gamepadStickLeft = new Vector2(0);
		private Vector2 _gamepadStickRight = new Vector2(0);
		private float _gamepadStickSensitivity = 100f;
		private float _gamepadStickBorderValue = 0.2f;
		public float GamepadStickBorderValue
		{
			get => _gamepadStickBorderValue;
			set
			{
				if (_gamepadStickBorderValue >= 0 || _gamepadStickBorderValue <= 1)
					_gamepadStickBorderValue = value;
				else
					throw new ArgumentOutOfRangeException(nameof(GamepadStickBorderValue), value, "Value has to be between 0 and 1");
			}
		}

		//Gamepad Trigger
		private float _gamepadTriggerLeft = 0f;
		private float _gamepadTriggerRight = 0f;

		//Gamepad Buttons
		private List<Buttons> _pressedLastFrame = new List<Buttons>();
		private List<Buttons> _pressedThisFrame = new List<Buttons>();

		//Keyboard
		private List<Keys> _pressedThisFrameKeys = new List<Keys>();
		private List<Keys> _pressedLastFrameKeys = new List<Keys>();

		/// <summary>
		/// Set Point on which the mouse is reset to to handle mouse-movement
		/// </summary>
		/// <param name="point"></param>
		public void SetMouseAnchor(Point point) => Instance._SetMouseAnchor(point);
		private void _SetMouseAnchor(Point point) => _mouseStartPos = point;

		/// <summary>
		/// Lock Mouse to position to observe movement
		/// </summary>
		public static void ActivateMouseMovement() => Instance._ActivateMouseMovement();
		private void _ActivateMouseMovement()
		{
			_lockMouse = true;
			Mouse.SetPosition(_mouseStartPos.X, _mouseStartPos.Y);
		}

		/// <summary>
		/// Unlock Mouse to stop observing
		/// </summary>
		public static void DeactivateMouseMovement() => Instance._DeactivateMouseMovement();
		private void _DeactivateMouseMovement()
		{
			_lockMouse = false;
		}

		/// <summary>
		/// Easy toggle MouseMovement observation
		/// </summary>
		/// <param name="active">true: lock mouse and observe | false: unlock and stop observing</param>
		public static void SetMouseMovement(bool active)
		{
			if(active)
				ActivateMouseMovement();
			else
				DeactivateMouseMovement();
		}

		/// <summary>
		/// Get Movement of locked mouse
		/// </summary>
		/// <returns>Percentage Mouse moved in relation to sensitivity</returns>
		public static Vector2 GetMouseMovement() => Instance._GetMouseMovement();
		private Vector2 _GetMouseMovement() => _lockMouse? (_mouseMovement / _mouseSensitivity) : new Vector2(0);

		/// <summary>
		/// Set Mousesensitivity
		/// </summary>
		/// <param name="sensitivity">Pixels which the Mouse has to move for a 100% movement in each direction</param>
		public static void SetMouseSensitivity(float sensitivity) => Instance._SetMouseSensitivity(sensitivity);
		private void _SetMouseSensitivity(float sensitivity) => _mouseSensitivity = sensitivity;

		/// <summary>
		/// MouseKey down in current frame
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool IsDownMouse(MouseKeys key) => Instance._IsDownMouse(key);
		private bool _IsDownMouse(MouseKeys key)
		{
			switch (key)
			{
				case MouseKeys.Left:
					return _leftMouseThisFrame;
				case MouseKeys.Right:
					return _rightMouseThisFrame;
				case MouseKeys.Middle:
					return _middleMouseThisFrame;
				case MouseKeys.X1:
					return _x1MouseThisFrame;
				case MouseKeys.X2:
					return _x2MouseThisFrame;
				default:
					throw new ArgumentOutOfRangeException(nameof(key), key, null);
			}
		}

		/// <summary>
		/// MouseKey up in current Frame
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool IsUpMouse(MouseKeys key) => Instance._IsUpMouse(key);
		private bool _IsUpMouse(MouseKeys key) => !_IsDownMouse(key);

		/// <summary>
		/// MouseKey was pressed down in current Frame
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool IsPressedMouse(MouseKeys key) => Instance._IsPressedMouse(key);
		private bool _IsPressedMouse(MouseKeys key)
		{
			switch (key)
			{
				case MouseKeys.Left:
					return _leftMouseThisFrame && !_leftMouseLastFrame;
				case MouseKeys.Right:
					return _rightMouseThisFrame && !_rightMouseLastFrame;
				case MouseKeys.Middle:
					return _middleMouseThisFrame && !_middleMouseLastFrame;
				case MouseKeys.X1:
					return _x1MouseThisFrame && !_x1MouseLastFrame;
				case MouseKeys.X2:
					return _x2MouseThisFrame && !_x2MouseLastFrame;
				default:
					throw new ArgumentOutOfRangeException(nameof(key), key, null);
			}
		}

		/// <summary>
		/// MouseKey was released in current Frame
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool IsReleasedMouse(MouseKeys key) => Instance._IsReleasedMouse(key);
		private bool _IsReleasedMouse(MouseKeys key)
		{
			switch (key)
			{
				case MouseKeys.Left:
					return !_leftMouseThisFrame && _leftMouseLastFrame;
				case MouseKeys.Right:
					return !_rightMouseThisFrame && _rightMouseLastFrame;
				case MouseKeys.Middle:
					return !_middleMouseThisFrame && _middleMouseLastFrame;
				case MouseKeys.X1:
					return !_x1MouseThisFrame && _x1MouseLastFrame;
				case MouseKeys.X2:
					return !_x2MouseThisFrame && _x2MouseLastFrame;
				default:
					throw new ArgumentOutOfRangeException(nameof(key), key, null);
			}
		}

		/// <summary>
		/// Movement of MouseWheel since last frame
		/// </summary>
		/// <returns></returns>
		public static int GetMouseWheelMovement() => Instance._GetMouseWheelMovement();
		private int _GetMouseWheelMovement() => _mouseWheelLastFrame - _mouseWheelThisFrame;

		/// <summary>
		/// Cumulative Movement of Wheel since start of Game
		/// </summary>
		/// <returns></returns>
		public static int GetMouseWheel() => Instance._GetMouseWheel();
		private int _GetMouseWheel() => _mouseWheelThisFrame;

		/// <summary>
		/// Set Deadzone for Thumbsticks
		/// </summary>
		/// <param name="deadZone"></param>
		public static void SetDeadZone(GamePadDeadZone deadZone) => Instance._SetDeadZone(deadZone);
		private void _SetDeadZone(GamePadDeadZone deadZone)
		{
			_deadZone = deadZone;
		}

		/// <summary>
		/// Set Sensitivity of GamePadSticks. Movement will be divided by this value
		/// </summary>
		/// <param name="value"></param>
		public static void SetGamePadStickSensitivity(float value) => Instance._SetGamePadStickSensitivity(value);
		private void _SetGamePadStickSensitivity(float value) => _gamepadStickSensitivity = value;

		/// <summary>
		/// Value of Left Thumbstick - x and y = percentage of movement in direction
		/// </summary>
		public static Vector2 GetGamePadLeftStick() => Instance._GetGamePadLeftStick();
		private Vector2 _GetGamePadLeftStick() => _gamepadStickLeft/_gamepadStickSensitivity;

		/// <summary>
		/// Value of Right Thumbstick - x and y = percentage of movement in direction
		/// </summary>
		public static Vector2 GetGamePadRightStick() => Instance._GetGamePadRightStick();
		private Vector2 _GetGamePadRightStick() => _gamepadStickRight/_gamepadStickSensitivity;

		/// <summary>
		/// Check if Button is down in current Frame - also false if it is not supported by Device
		/// </summary>
		/// <param name="button"></param>
		/// <returns></returns>
		public static bool IsDownGamePad(Buttons button) => Instance._IsDownGamePad(button);
		private bool _IsDownGamePad(Buttons button) => _pressedThisFrame.Contains(button);

		/// <summary>
		/// Check if Button is up in current Frame - also false if it is not supported by Device
		/// </summary>
		/// <param name="button"></param>
		/// <returns></returns>
		public static bool IsUpGamePad(Buttons button) => Instance._IsUpGamePad(button);
		private bool _IsUpGamePad(Buttons button) => !_pressedThisFrame.Contains(button);

		/// <summary>
		/// Check if Button was pressed down in current Frame - also false if it is not supported by Device
		/// </summary>
		/// <param name="button"></param>
		/// <param name="consume">if true wont return pressed-status until released and pressed again</param>
		/// <returns></returns>
		public static bool IsPressedGamePad(Buttons button, bool consume = false) => Instance._IsPressedGamePad(button, consume);
		private bool _IsPressedGamePad(Buttons button, bool consume)
		{
			var val = _pressedThisFrame.Contains(button) && !_pressedLastFrame.Contains(button);
			if(consume && val) _pressedLastFrame.Add(button);
			return val;
		}

		/// <summary>
		/// Check if Button was released in current Frame - also false if it is not supported by Device
		/// </summary>
		/// <param name="button"></param>
		/// <returns></returns>
		public static bool IsReleasedGamePad(Buttons button) => Instance._IsReleasedGamePad(button);
		private bool _IsReleasedGamePad(Buttons button) =>
			!_pressedThisFrame.Contains(button) && _pressedLastFrame.Contains(button);

		/// <summary>
		/// Returns true if any button on GamePad is down this frame
		/// </summary>
		/// <returns></returns>
		public static bool IsAnyDownGamePad() => Instance._IsAnyDownGamePad();
		private bool _IsAnyDownGamePad() 
			=> this._pressedThisFrame.Count > 0;

		/// <summary>
		/// Returns true if any button on GamePad was released this frame
		/// </summary>
		/// <returns></returns>
		public static bool IsAnyReleasedGamePad() => Instance._IsAnyReleasedGamePad();
		private bool _IsAnyReleasedGamePad() 
			=> this._pressedThisFrame.Count < this._pressedLastFrame.Count;

		/// <summary>
		/// Returns true if any button on GamePad was pressed this frame
		/// </summary>
		/// <returns></returns>
		public static bool IsAnyPressedGamePad() => Instance._IsAnyPressedGamePad();
		private bool _IsAnyPressedGamePad() 
			=> this._pressedThisFrame.Count > this._pressedLastFrame.Count;

		/// <summary>
		/// Apply Vibration to Gamepad (if device supports it)
		/// </summary>
		/// <param name="leftMotor"></param>
		/// <param name="rightMotor"></param>
		public static void SetVibration(float lfMotor, float hfMotor) 
			=> Instance._SetVibration(lfMotor, hfMotor);
		private void _SetVibration(float lfMotor, float hfMotor)
		{
			var capa = GamePad.GetCapabilities(PlayerIndex.One);
			GamePad.SetVibration(PlayerIndex.One, capa.HasLeftVibrationMotor ? lfMotor : 0f,
			                     capa.HasRightVibrationMotor ? hfMotor : 0);
		}

		/// <summary>
		/// Get Value of Left Trigger
		/// </summary>
		/// <returns></returns>
		public static float GetLeftTrigger() => Instance._GetLeftTrigger();
		private float _GetLeftTrigger() => _gamepadTriggerLeft;

		/// <summary>
		/// Get Value of Right Trigger
		/// </summary>
		/// <returns></returns>
		public static float GetRightTrigger() => Instance._GetRightTrigger();
		private float _GetRightTrigger() => _gamepadTriggerRight;

		/// <summary>
		///     Returns true, if the <seealso cref="Keys" /> have been pressed this frame, but not last one.
		///     Only returns true, if the <seealso cref="Keys" /> have been pressed this frame.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="consume">if true wont return true-pressed-status until released and pressed again</param>
		/// <returns></returns>
		public static bool IsKeyPressed(Keys key, bool consume = false) => Instance._IsKeyPressed(key, consume);
		private bool _IsKeyPressed(Keys key, bool consume)
		{
			var val = _pressedThisFrameKeys.Contains(key) && !_pressedLastFrameKeys.Contains(key);
			if(consume && val) _pressedLastFrameKeys.Add(key);
			return val;
		}

		/// <summary>
		///     Returns true, if the <seealso cref="Keys" /> have been released this frame, but not last one.
		///     Only returns true, if the <seealso cref="Keys" /> have been released this frame.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool IsKeyReleased(Keys key) => Instance._IsKeyReleased(key);
		private bool _IsKeyReleased(Keys key)
			=> !_pressedThisFrameKeys.Contains(key) && _pressedLastFrameKeys.Contains(key);

		/// <summary>
		///     Returns true, if the <seealso cref="Keys" /> have been pressed this frame.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool IsKeyDown(Keys key) => Instance._IsKeyDown(key);
		private bool _IsKeyDown(Keys key)
			=> _pressedThisFrameKeys.Contains(key);

		/// <summary>
		///	    Returns true, if the <seealso cref="Keys" /> have not been pressed this frame.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool IsKeyUp(Keys key) => Instance._IsKeyUp(key);
		private bool _IsKeyUp(Keys key)
			=> !_pressedThisFrameKeys.Contains(key);

		/// <summary>
		/// Update all values - called each Frame
		/// </summary>
		public static void Update(SingularityGame game) => Instance._Update(game);
		private void _Update(SingularityGame game)
		{

			//GAMEPAD
			var capa = GamePad.GetCapabilities(PlayerIndex.One);

			if (capa.IsConnected)
			{
				var state = GamePad.GetState(PlayerIndex.One, _deadZone);

				//Switch pressedLists and clear
				var helper = _pressedLastFrame;
				helper.Clear();
				_pressedLastFrame = _pressedThisFrame;

				//Thumbstick movement
				if (capa.HasLeftXThumbStick && capa.HasLeftYThumbStick)
				{
					_gamepadStickLeft = state.ThumbSticks.Left;

					if (_gamepadStickLeft.Y > _gamepadStickBorderValue)
						helper.Add(Buttons.LeftThumbstickUp);
					if(_gamepadStickLeft.Y < -_gamepadStickBorderValue)
						helper.Add(Buttons.LeftThumbstickDown);
					if(_gamepadStickLeft.X < -_gamepadStickBorderValue)
						helper.Add(Buttons.LeftThumbstickLeft);
					if(_gamepadStickLeft.X > _gamepadStickBorderValue)
						helper.Add(Buttons.LeftThumbstickRight);
				}

				if (capa.HasRightXThumbStick && capa.HasRightYThumbStick)
				{
					_gamepadStickRight = state.ThumbSticks.Right;

					if(_gamepadStickRight.Y > _gamepadStickBorderValue)
						helper.Add(Buttons.RightThumbstickUp);
					if(_gamepadStickRight.Y < -_gamepadStickBorderValue)
						helper.Add(Buttons.RightThumbstickDown);
					if(_gamepadStickRight.X < -_gamepadStickBorderValue)
						helper.Add(Buttons.RightThumbstickLeft);
					if(_gamepadStickRight.X > _gamepadStickBorderValue)
						helper.Add(Buttons.RightThumbstickRight);
				}



				//Buttons
				if (capa.HasAButton && state.Buttons.A == ButtonState.Pressed)
					helper.Add(Buttons.A);
				if (capa.HasBButton && state.Buttons.B == ButtonState.Pressed)
					helper.Add(Buttons.B);
				if (capa.HasXButton && state.Buttons.X == ButtonState.Pressed)
					helper.Add(Buttons.X);
				if(capa.HasYButton && state.Buttons.Y == ButtonState.Pressed)
					helper.Add(Buttons.Y);
				if (capa.HasBackButton && state.Buttons.Back == ButtonState.Pressed)
					helper.Add(Buttons.Back);
				if(capa.HasStartButton && state.Buttons.Start == ButtonState.Pressed)
					helper.Add(Buttons.Start);
				if(capa.HasBigButton && state.Buttons.BigButton == ButtonState.Pressed)
					helper.Add(Buttons.BigButton);
				if(capa.HasLeftShoulderButton && state.Buttons.LeftShoulder == ButtonState.Pressed)
					helper.Add(Buttons.LeftShoulder);
				if(capa.HasRightShoulderButton && state.Buttons.RightShoulder == ButtonState.Pressed)
					helper.Add(Buttons.RightShoulder);
				if(capa.HasLeftStickButton && state.Buttons.LeftStick == ButtonState.Pressed)
					helper.Add(Buttons.LeftStick);
				if(capa.HasRightStickButton && state.Buttons.RightStick == ButtonState.Pressed)
					helper.Add(Buttons.RightStick);
				
				//Dpad
				if(capa.HasDPadUpButton && state.DPad.Up == ButtonState.Pressed)
					helper.Add(Buttons.DPadUp);
				if(capa.HasDPadDownButton && state.DPad.Down == ButtonState.Pressed)
					helper.Add(Buttons.DPadDown);
				if(capa.HasDPadLeftButton && state.DPad.Left == ButtonState.Pressed)
					helper.Add(Buttons.DPadLeft);
				if(capa.HasDPadRightButton && state.DPad.Right == ButtonState.Pressed)
					helper.Add(Buttons.DPadRight);

				//Trigger
				if (capa.HasLeftTrigger)
					_gamepadTriggerLeft = state.Triggers.Left;
				if (capa.HasRightTrigger)
					_gamepadTriggerRight = state.Triggers.Right;

				if(Math.Abs(_gamepadTriggerLeft) > 0.01f)
					helper.Add(Buttons.LeftTrigger);
				if(Math.Abs(_gamepadTriggerRight) > 0.01f)
					helper.Add(Buttons.RightTrigger);

				_pressedThisFrame = helper;
			}

			//Mouse
			if (!game.IsActive && _lockMouse)
			{
				_relock = true;
				this._DeactivateMouseMovement();
			}

			if (game.IsActive && _relock)
			{
				_relock = false;
				this._ActivateMouseMovement();
			}

			var mouseState = Mouse.GetState();
			if (_lockMouse)
			{
				
				var moved = mouseState.Position - _mouseStartPos;
				var x = moved.X;
				var y = moved.Y;

				_mouseMovement = new Vector2(x,y);

				Mouse.SetPosition(_mouseStartPos.X, _mouseStartPos.Y);
			}

			//Buttons
			_leftMouseLastFrame   = _leftMouseThisFrame;
			_rightMouseLastFrame  = _rightMouseThisFrame;
			_middleMouseLastFrame = _middleMouseThisFrame;
			_x1MouseLastFrame     = _x1MouseThisFrame;
			_x2MouseLastFrame     = _x2MouseThisFrame;

			_leftMouseThisFrame   = mouseState.LeftButton   == ButtonState.Pressed;
			_rightMouseThisFrame  = mouseState.RightButton  == ButtonState.Pressed;
			_middleMouseThisFrame = mouseState.MiddleButton == ButtonState.Pressed;
			_x1MouseThisFrame     = mouseState.XButton1     == ButtonState.Pressed;
			_x2MouseThisFrame     = mouseState.XButton2     == ButtonState.Pressed;

			//Wheel
			_mouseWheelLastFrame = _mouseWheelThisFrame;
			_mouseWheelThisFrame = mouseState.ScrollWheelValue;


			//Keyboard
			var ks = Keyboard.GetState();
			_pressedLastFrameKeys = _pressedThisFrameKeys;
			_pressedThisFrameKeys = ks.GetPressedKeys().ToList();
		}
	}
}
