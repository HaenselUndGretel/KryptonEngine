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
		protected const string Anim_LanternRaised = "raiseLantern";
		protected const string Anim_Addon_Walk_Up = "Up";
		protected const string Anim_Addon_Walk_Down = "Down";
		protected const string Anim_Addon_Walk_Side = "Side";
		protected const string Anim_Addon_Shiver = "Shiver";
		protected const string Anim_Addon_Lantern = "Lantern";

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
		protected Vector2 GetMovement(Vector2 pMovementDirection, float pMovementSpeedFactor = 1f, bool pIgnoreTemp = false)
		{
			if (pMovementDirection.Length() != 1f)
				pMovementDirection.Normalize();
			if (pIgnoreTemp)
				return pMovementDirection * mSpeed * mBodyTemperature * pMovementSpeedFactor * (EngineSettings.Time.ElapsedGameTime.Milliseconds / 1000f);
			return pMovementDirection * mSpeed * pMovementSpeedFactor * (EngineSettings.Time.ElapsedGameTime.Milliseconds / 1000f);
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
		public void AnimBasicAnimation(Vector2 pMovement, bool pLantern = false, bool CanShiver = true, bool pLanternRaised = false)
		{
			//Animation bestimmen
			Vector2 TmpMovement = pMovement;
			string TmpAnimation;
			if (TmpMovement == Vector2.Zero)
			{
				TmpAnimation = Anim_Idle;
				TmpMovement = LastMovementDirection;
			}
			else
			{
				LastMovementDirection = TmpMovement;
				TmpAnimation = Anim_Walk;
				if (pLantern)
					TmpAnimation += Anim_Addon_Lantern;
			}
			if (pLanternRaised)
				TmpAnimation = Anim_LanternRaised;
			TmpAnimation += GetRightDirectionAnimation(TmpMovement, Anim_Addon_Walk_Up, Anim_Addon_Walk_Down, Anim_Addon_Walk_Side);
			if (CanShiver && !pLantern && mBodyTemperature < 1f)
				TmpAnimation += Anim_Addon_Shiver;

			//Animation setzen
			SetAnimation(TmpAnimation);
			SetSkeletonFlipState(this, TmpMovement);
		}

		public static string GetRightDirectionAnimation(Vector2 pDirection, string pAnimUp, string pAnimDown, string pAnimSide)
		{
			if (pDirection == Vector2.Zero)
				return pAnimDown;
			pDirection.Normalize();
			string anim = "";
			
			if (pDirection.Y > Math.Sin(MathHelper.ToRadians(67.5f))) //Runter
				anim = pAnimDown;
			else if (pDirection.Y > Math.Sin(MathHelper.ToRadians(-22.5f))) //Seitlich
				anim = pAnimSide;
			else //Hoch
				anim = pAnimUp;

			return anim;
		}

		public static void SetSkeletonFlipState(SpineObject pSpineObj, Vector2 pAnimDirection)
		{
			pAnimDirection.Normalize();
			if (pAnimDirection.X > 0)
				pSpineObj.Flip = true;
			else if (pAnimDirection.X < 0)
				pSpineObj.Flip = false;
			if (pSpineObj.AnimationState.GetCurrent(0) != null && (pAnimDirection.X == 0 && (pSpineObj.AnimationState.GetCurrent(0).animation.name.Contains("Down") || pSpineObj.AnimationState.GetCurrent(0).animation.name.Contains("Up"))))
				pSpineObj.Flip = false;
		}

		#endregion

		#endregion
	}
}
