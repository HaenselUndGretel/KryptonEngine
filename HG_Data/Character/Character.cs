using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using KryptonEngine;
using KryptonEngine.Manager;
using KryptonEngine.Physics;
using System.Xml.Serialization;

namespace HanselAndGretel.Data
{
	public class Character : InteractiveObject
	{
		#region Properties

		#region AnimationMapping

		protected const string Anim_Idle = "idle";
		protected const string Anim_Walk = "walk";
		protected const string Anim_Addon_Walk_Up = "Up";
		protected const string Anim_Addon_Walk_Down = "Down";
		protected const string Anim_Addon_Walk_Side = "Side";
		protected const string Anim_Addon_Walk_SideUp = "SideUp";
		protected const string Anim_Addon_Walk_SideDown = "SideDown";
		protected const string Anim_Addon_Shiver = "Shiver";

		#endregion

		protected float mSpeed;
		protected float mBodyTemperature;

		//References
		protected Camera rCamera;

		#endregion

		#region Getter & Setter

		[XmlIgnoreAttribute]
		public float Speed { get { return mSpeed; } }
		[XmlIgnoreAttribute]
		public float BodyTemperature { get { return mBodyTemperature; } set { mBodyTemperature = value; } }

		#endregion

		#region Constructor

		public Character() : base() { }

		public Character(string pName)
			:base(pName)
		{

		}

		public Character(String pName, Vector2 pPosition)
			:base(pName)
		{
			this.Position = pPosition;
		}

		#endregion

		#region Override Methods

		public override void Initialize()
		{
			base.Initialize();
			mDebugColor = Color.LightYellow;
			mBodyTemperature = 1f;
		}

		#endregion

		#region Methods

		public void LoadReferences(Camera pCamera)
		{
			rCamera = pCamera;
		}

		/// <summary>
		/// Moved den Character um pDelta, prüft vorher auf Collision mit pMoveArea.
		/// </summary>
		public Vector2 Move(Vector2 pDelta, List<Rectangle> pMoveArea)
		{
			Vector2 TmpMovement = Collision.CollisionCheckedVector(CollisionBox, (int)pDelta.X, (int)pDelta.Y, pMoveArea);
			this.MoveInteractiveObject(TmpMovement);
			//this.Position += TmpMovement;
			return TmpMovement;
		}

		/// <summary>
		/// Gibt entsprechend den Bedingungen potentielles Movement zurück.
		/// </summary>
		protected Vector2 GetMovement(Vector2 pMovementDirection, float pMovementSpeedFactor = 1f)
		{
			if (pMovementDirection.Length() != 1f)
				pMovementDirection.Normalize();
			return pMovementDirection * mSpeed * mBodyTemperature * pMovementSpeedFactor * (EngineSettings.Time.ElapsedGameTime.Milliseconds / 1000f);
		}

		#region Animation

		/// <summary>
		/// Clear alle Tracks und started gelooped die "idle" Animation.
		/// </summary>
		public void AnimCutToIdle()
		{
			AnimationState.ClearTracks();
			AnimationState.SetAnimation(0, Anim_Idle, true);
		}

		/// <summary>
		/// Animiert den Character für idle und Movement.
		/// </summary>
		/// <param name="pMovement"></param>
		public void AnimBasicAnimation(Vector2 pMovement)
		{
			if (pMovement == Vector2.Zero)
			{
				SetAnimation();
				return;
			}
			string TmpAnimation = Anim_Walk;
			Vector2 TmpMovement = pMovement;
			TmpMovement.Normalize();
			//Flip?
			if (TmpMovement.X > 0)
				Flip = true;
			else if (TmpMovement.X < 0)
				Flip = false;
			//Get correct Animation
			/*
			if (TmpMovement.Y > Math.Sin(67.5)) //Hoch
				TmpAnimation += Anim_Addon_Walk_Up;
			else if (TmpMovement.Y > Math.Sin(22.5)) //Seitlich hoch
				TmpAnimation += Anim_Addon_Walk_SideUp;
			else if (TmpMovement.Y > -Math.Sin(22.5)) //Seitlich
				TmpAnimation += Anim_Addon_Walk_Side;
			else if (TmpMovement.Y > -Math.Sin(67.5)) //Seitlich runter
				TmpAnimation += Anim_Addon_Walk_SideDown;
			else //Runter
				TmpAnimation += Anim_Addon_Walk_Down;
			*/
			TmpAnimation = Anim_Idle; //
			if (mBodyTemperature < 1f)
				TmpAnimation += "";//Anim_Addon_Shiver;
			SetAnimation(TmpAnimation);
		}

		#endregion

		#endregion
	}
}
