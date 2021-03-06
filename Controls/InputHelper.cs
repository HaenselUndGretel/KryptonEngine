﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace KryptonEngine.Controls
{
	public class InputHelper
	{
		public class Input //Input um Mapping zu Button & Keys  zu speichern
		{
			public Buttons Button;
			public Keys KeyPlayer1;
			public Keys KeyPlayer2;

			public Input(Buttons pButton, Keys pKeyPlayer1, Keys pKeyPlayer2)
			{
				Button = pButton;
				KeyPlayer1 = pKeyPlayer1;
				KeyPlayer2 = pKeyPlayer2;
			}
		}

		#region Singleton

		private static InputHelper mPlayer1;
		private static InputHelper mPlayer2;
		public static InputHelper Player1
		{
			get
			{
				if (mPlayer1 == null) mPlayer1 = new InputHelper(PlayerIndex.One);
				return mPlayer1;
			}
		}
		public static InputHelper Player2
		{
			get
			{
				if (mPlayer2 == null) mPlayer2 = new InputHelper(PlayerIndex.Two);
				return mPlayer2;
			}
		}

		#endregion

		#region Properties

		private PlayerIndex mPlayer;

		private GamePadState mGamepadStateCurrent;
		private GamePadState mGamepadStateBefore;

		private static KeyboardState mKeyboardStateCurrent;
		private static KeyboardState mKeyboardStateBefore;

		public static Input mDebug = new Input(Buttons.RightStick, Keys.F7, Keys.None);

		private static Input mPause = new Input(Buttons.Start, Keys.Escape, Keys.Escape);
		public static Input mBack = new Input(Buttons.B, Keys.Escape, Keys.Back);
		public static Input mAction = new Input(Buttons.X, Keys.Space, Keys.RightControl);
		public static Input mUseItem = new Input(Buttons.A, Keys.LeftControl, Keys.RightShift);
		public static Input mSwitchItem = new Input(Buttons.Y, Keys.LeftAlt, Keys.Enter);
		private static Input mItemLeft = new Input(Buttons.LeftShoulder, Keys.Q, Keys.OemComma);
		private static Input mItemRight = new Input(Buttons.RightShoulder, Keys.E, Keys.OemPeriod);

		private static Input mMoveUp = new Input(Buttons.LeftThumbstickUp, Keys.W, Keys.Up);
		private static Input mMoveDown = new Input(Buttons.LeftThumbstickDown, Keys.S, Keys.Down);
		private static Input mMoveLeft = new Input(Buttons.LeftThumbstickLeft, Keys.A, Keys.Left);
		private static Input mMoveRight = new Input(Buttons.LeftThumbstickRight, Keys.D, Keys.Right);

		#endregion

		#region Getter & Setter

		public bool Connected { get { return mGamepadStateCurrent.IsConnected; } }

		//Movement
		public Vector2 Movement
		{
			get
			{
				Vector2 TmpMovement = Vector2.Zero;
				if (InputPressed(mMoveUp))
					--TmpMovement.Y;
				if (InputPressed(mMoveDown))
					++TmpMovement.Y;
				if (InputPressed(mMoveLeft))
					--TmpMovement.X;
				if (InputPressed(mMoveRight))
					++TmpMovement.X;
				return TmpMovement;
			}
		}

		/// <summary>
		/// Rotation des linken ThumbSticks: 1f = vollständige Drehung im Uhrzeigersinn, -1 = vollständige Drehung gegen den Uhrzeigersinn
		/// </summary>
		public float LeftStickRotation { get { return StickRoation(); } }

		//Pause
		public bool PauseJustPressed { get { return InputJustPressed(mPause); } }
		//Back
		public bool BackJustPressed { get { return InputJustPressed(mBack); } }
		//Action
		public bool ActionJustPressed { get { return InputJustPressed(mAction); } }
		public bool ActionIsPressed { get { return InputPressed(mAction); } }
		//Item
		public bool UseItemJustPressed { get { return InputJustPressed(mUseItem); } }
		public bool UseItemIsPressed { get { return InputPressed(mUseItem); } }
		public bool SwitchItemJustPressed { get { return InputJustPressed(mSwitchItem); } }

		public bool ItemLeftJustPressed { get { return InputJustPressed(mItemLeft); } }
		public bool ItemRightJustPressed { get { return InputJustPressed(mItemRight); } }

		#endregion

		#region Constructor

		InputHelper(PlayerIndex pPlayer)
		{
			mPlayer = pPlayer;
		}

		#endregion

		#region InputState Methods

		#region InputStates

		public bool InputJustPressed(Input pInput)
		{
			if (Connected)
				return ButtonJustPressed(pInput.Button);
			return KeyJustPressed(PlayerMappedKey(pInput));
		}

		private bool InputJustReleased(Input pInput)
		{
			if (Connected)
				return ButtonJustReleased(pInput.Button);
			return KeyJustReleased(PlayerMappedKey(pInput));
		}

		private bool InputPressed(Input pInput)
		{
			if (Connected)
				return ButtonPressed(pInput.Button);
			return KeyPressed(PlayerMappedKey(pInput));
		}

		private bool InputReleased(Input pInput)
		{
			if (Connected)
				return ButtonReleased(pInput.Button);
			return KeyReleased(PlayerMappedKey(pInput));
		}

		#endregion

		#region ButtonStates

		public bool ButtonJustPressed(Buttons pButton)
		{
			return (mGamepadStateBefore.IsButtonUp(pButton) && mGamepadStateCurrent.IsButtonDown(pButton)) ? true : false;
		}

		public bool ButtonJustReleased(Buttons pButton)
		{
			return (mGamepadStateBefore.IsButtonDown(pButton) && mGamepadStateCurrent.IsButtonUp(pButton)) ? true : false;
		}

		private bool ButtonPressed(Buttons pButton)
		{
			return mGamepadStateCurrent.IsButtonDown(pButton);
		}

		private bool ButtonReleased(Buttons pButton)
		{
			return mGamepadStateCurrent.IsButtonUp(pButton);
		}

		public static bool ButtonJustPressed2Player(Buttons pButton)
		{
			return (Player1.ButtonJustPressed(pButton) || Player2.ButtonJustPressed(pButton)) ? true : false;
		}

		public static bool ButtonJustReleased2Player(Buttons pButton)
		{
			return (Player1.ButtonJustReleased(pButton) || Player2.ButtonJustReleased(pButton)) ? true : false;
		}

		public static bool ButtonPressed2Player(Buttons pButton)
		{
			return (Player1.ButtonPressed(pButton) || Player2.ButtonPressed(pButton)) ? true : false;
		}

		public static bool ButtonReleased2Player(Buttons pButton)
		{
			return (Player1.ButtonReleased(pButton) && Player2.ButtonReleased(pButton)) ? true : false;
		}

		#endregion

		#region KeyboardStates

		public bool KeyJustPressed(Keys pKey)
		{
			return (mKeyboardStateBefore.IsKeyUp(pKey) && mKeyboardStateCurrent.IsKeyDown(pKey)) ? true : false;
		}

		public bool KeyJustReleased(Keys pKey)
		{
			return (mKeyboardStateBefore.IsKeyDown(pKey) && mKeyboardStateCurrent.IsKeyUp(pKey)) ? true : false;
		}

		public bool KeyPressed(Keys pKey)
		{
			return mKeyboardStateCurrent.IsKeyDown(pKey);
		}

		public bool KeyReleased(Keys pKey)
		{
			return mKeyboardStateCurrent.IsKeyUp(pKey);
		}

		#endregion

		/// <summary>
		/// Rotation des Sticks.
		/// </summary>
		/// <param name="pRightStick">true = Rechter Stick, false = linker Stick</param>
		/// <returns>1f = vollständige Drehung im Uhrzeigersinn, -1 = vollständige Drehung gegen den Uhrzeigersinn</returns>
		private float StickRoation(bool pRightStick = false)
		{
			Vector2 oldPosition;
			Vector2 newPosition;
			if (pRightStick)
			{
				oldPosition = mGamepadStateBefore.ThumbSticks.Right;
				newPosition = mGamepadStateCurrent.ThumbSticks.Right;
			}
			else
			{
				oldPosition = mGamepadStateBefore.ThumbSticks.Left;
				newPosition = mGamepadStateCurrent.ThumbSticks.Left;
			}
			oldPosition.Normalize();
			newPosition.Normalize();
			float oldAngle = (float)Math.Atan2(oldPosition.Y, oldPosition.X);
			float newAngle = (float)Math.Atan2(newPosition.Y, newPosition.X);

			float rotation = MathHelper.ToDegrees(newAngle - oldAngle) / 360;
			if (float.IsNaN(rotation) || Math.Abs(newAngle - oldAngle) > 0.5f) //Threshold gegen clipping bei 360Cut links
				rotation = 0f;
			return -rotation;
		}

		private Keys PlayerMappedKey(Input pInput) //Map Input.pKey to mPlayer
		{
			if (mPlayer == PlayerIndex.Two)
				return pInput.KeyPlayer2;
			return pInput.KeyPlayer1;
		}

		#endregion

		#region Methods

		private void UpdateInstance()
		{
			mGamepadStateBefore = mGamepadStateCurrent;
			mGamepadStateCurrent = GamePad.GetState(mPlayer, GamePadDeadZone.Circular);
		}

		public static void Update()
		{
			//Update Gamepad
			Player1.UpdateInstance();
			Player2.UpdateInstance();
			//Update Keyboard
			mKeyboardStateBefore = mKeyboardStateCurrent;
			mKeyboardStateCurrent = Keyboard.GetState();

			//Switch DebugMode
			if (mPlayer1.InputJustPressed(mDebug))
				EngineSettings.IsDebug = !EngineSettings.IsDebug;
		}

		#endregion
	}
}
