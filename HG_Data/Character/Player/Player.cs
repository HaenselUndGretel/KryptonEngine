using KryptonEngine;
using KryptonEngine.Controls;
using KryptonEngine.Entities;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace HanselAndGretel.Data
{
	public class Player : Character
	{
		#region Properties

		[XmlIgnoreAttribute]
		public List<Activity> mHandicaps;
		[XmlIgnoreAttribute]
		public ActivityState mCurrentActivity;
		[XmlIgnoreAttribute]
		public int mCurrentState;
		protected InputHelper mInput;
		public bool Lantern;
		[XmlIgnoreAttribute]
		public bool IsLanternRaised;

		//References
		protected Player rOtherPlayer;

		#endregion

		#region Getter & Setter

		public InputHelper Input { get { return mInput; } }

		#endregion

		#region Constructor

		public Player() : base() { }

		public Player(string pName)
			:base(pName)
		{
			
		}

		#endregion

		#region Override Methods

		public override void Initialize()
		{
			base.Initialize();
			mDebugColor = Color.LimeGreen;
			mHandicaps = new List<Activity>();
			mCurrentState = 0;
			mSpeed = 200;
			Lantern = false;
		}

		public override void LoadContent()
		{
			base.LoadContent();
			InteractiveObject io = InteractiveObjectDataManager.Instance.GetElementByString(Name);
			CollisionRectList = io.CollisionRectList;
			if (CollisionRectList.Count > 0)
				this.CollisionBox = this.CollisionRectList[0];
		}

		#endregion

		#region Methods

		public void LoadReferences(Camera pCamera, Player pOtherPlayer)
		{
			base.LoadReferences(pCamera);
			rOtherPlayer = pOtherPlayer;
		}

		public void Update(bool pMayMove, float pMovementSpeedFactor, SceneData pScene)
		{
			base.Update();
			if (pMayMove && pMovementSpeedFactor > 0)
				AnimBasicAnimation(Move(ViewportCheckedVector(GetMovement(mInput.Movement, pMovementSpeedFactor)), GetBodiesForCollisionCheck(pScene)), Lantern, true, IsLanternRaised);
		}

		#region Update Movement Helper

		/// <summary>
		/// Spieler manuell entsprechend bewegen.
		/// </summary>
		/// <param name="pMovementDirection">Auszuführende Bewegungsrichtung (muss normalisiert sein)</param>
		/// <param name="pMovementSpeedFactor">Speed wird hiermit multipliziert</param>
		/// <param name="pScene">Scene für ggf Kollisionsprüfung, darf bei pIgnoreCollision = true null sein.</param>
		/// <param name="pIgnoreCollision">Keine Kollision</param>
		/// <param name="pIgnoreOtherPlayer">Wenn pIgnoreCollision = false ist auch anderen Spieler ignorieren.</param>
		public void MoveManually(Vector2 pMovementDirection, float pMovementSpeedFactor = 1f, SceneData pScene = null, bool pIgnoreCollision = true, bool pIgnoreOtherPlayer = false, bool pIgnoreMapBounds = false)
		{
			Vector2 TmpMovement;
			List<Rectangle> TmpBodies;
			if (pIgnoreCollision)
			{
				TmpMovement = GetMovement(pMovementDirection, pMovementSpeedFactor);
				TmpBodies = new List<Rectangle>();
				if (!pIgnoreOtherPlayer)
					TmpBodies.Add(rOtherPlayer.CollisionBox);
			}
			else
			{
				if (pIgnoreMapBounds)
					TmpMovement = GetMovement(pMovementDirection, pMovementSpeedFactor);
				else
					TmpMovement = ViewportCheckedVector(GetMovement(pMovementDirection, pMovementSpeedFactor));
				TmpBodies = GetBodiesForCollisionCheck(pScene);
			}
			AnimBasicAnimation(Move(TmpMovement, TmpBodies), Lantern, true, IsLanternRaised);
		}

		/// <summary>
		/// Move Player to Point (Call by Call).
		/// </summary>
		/// <param name="pTargetPoint">Point</param>
		/// <param name="pMovementSpeedFactor">Speed wird hiermit multipliziert</param>
		/// <param name="pScene">Scene für ggf Kollisionsprüfung, darf bei pIgnoreCollision = true null sein.</param>
		/// <param name="pIgnoreCollision">Keine Kollision</param>
		/// <param name="pIgnoreOtherPlayer">Wenn pIgnoreCollision = false ist auch anderen Spieler ignorieren.</param>
		public void MoveAgainstPoint(Vector2 pTargetPoint, float pMovementSpeedFactor = 1f, SceneData pScene = null, bool pIgnoreCollision = true, bool pIgnoreOtherPlayer = false, bool pAnimate = true, bool pIgnoreTemp = false)
		{
			Vector2 TmpMovementDirection = pTargetPoint - SkeletonPosition;
			TmpMovementDirection.Normalize();
			Vector2 TmpMovement;
			List<Rectangle> TmpBodies;
			if (pIgnoreCollision)
			{
				TmpMovement = ViewportCheckedVector(GetMovement(TmpMovementDirection, pMovementSpeedFactor, pIgnoreTemp));
				TmpBodies = new List<Rectangle>();
				if (!pIgnoreOtherPlayer)
					TmpBodies.Add(rOtherPlayer.CollisionBox);
			}
			else
			{
				TmpMovement = GetMovement(TmpMovementDirection, pMovementSpeedFactor, pIgnoreTemp);
				TmpBodies = GetBodiesForCollisionCheck(pScene);
			}
			if ((pTargetPoint - SkeletonPosition).Length() < TmpMovement.Length()) //Nicht über Punkt hinaus gehen.
				TmpMovement = pTargetPoint - SkeletonPosition;
			Vector2 Movement = Move(TmpMovement, TmpBodies);
			if (pAnimate)
				AnimBasicAnimation(Movement, Lantern, true, IsLanternRaised);
		}

		/// <summary>
		/// Get Scene.MoveArea + OtherPlayers CollisionBox as Rectangle-List.
		/// </summary>
		protected List<Rectangle> GetBodiesForCollisionCheck(SceneData pScene)
		{
			List<Rectangle> TmpMoveArea = new List<Rectangle>(pScene.MoveArea);
			foreach (InteractiveObject iObj in pScene.InteractiveObjects)
				TmpMoveArea.AddRange(iObj.CollisionRectList);
			TmpMoveArea.Add(rOtherPlayer.CollisionBox);
			return TmpMoveArea;
		}

		/// <summary>
		/// Vector Checked for rCamera.GameScreen & MaxScaling (/rCamera.ViewportMaxDimension).
		/// </summary>
		protected Vector2 ViewportCheckedVector(Vector2 pMovement)
		{
			if (InCameraBounds(pMovement))
			{
				return pMovement;
			}
			Vector2 TmpMovementInBounds = Vector2.Zero;
			int TmpSteps = (pMovement.X < pMovement.Y) ? (int)pMovement.Y : (int)pMovement.X;
			for (int i = TmpSteps; i > 0; i--) //Move Player step für step weniger, bis er in den Camera Viewport passt.
			{
				TmpMovementInBounds = (pMovement / TmpSteps) * i;
				if (InCameraBounds(TmpMovementInBounds))
					return TmpMovementInBounds;
			}
			return Vector2.Zero;
		}

		/// <summary>
		/// Check for rCamera.GameScreen & MaxScaling (/rCamera.ViewportMaxDimension) Collision.
		/// </summary>
		protected bool InCameraBounds(Vector2 pMovement)
		{
			if (CollisionBox.X + pMovement.X < 0 || CollisionBox.Y + pMovement.Y < 0 || CollisionBox.Right + pMovement.X > rCamera.GameScreen.Right || CollisionBox.Bottom + pMovement.Y > rCamera.GameScreen.Bottom)
				return false;
			Rectangle TmpThisPlayer = new Rectangle(PositionX + (int)pMovement.X, PositionY + (int)pMovement.Y, CollisionBox.Width, CollisionBox.Height);
			//Horizontal
			Rectangle TmpPlayerLeft;
			Rectangle TmpPlayerRight;
			if (rOtherPlayer.CollisionBox.X < SkeletonPosition.X)
			{
				TmpPlayerLeft = rOtherPlayer.CollisionBox;
				TmpPlayerRight = TmpThisPlayer;
			}
			else
			{
				TmpPlayerLeft = TmpThisPlayer;
				TmpPlayerRight = rOtherPlayer.CollisionBox;
			}
			//Vertical
			Rectangle TmpPlayerUp;
			Rectangle TmpPlayerDown;
			if (rOtherPlayer.CollisionBox.Y < SkeletonPosition.Y)
			{
				TmpPlayerUp = rOtherPlayer.CollisionBox;
				TmpPlayerDown = TmpThisPlayer;
			}
			else
			{
				TmpPlayerUp = TmpThisPlayer;
				TmpPlayerDown = rOtherPlayer.CollisionBox;
			}
			//Test
			Rectangle TmpRectangleToCheck = new Rectangle(TmpPlayerLeft.Left, TmpPlayerUp.Top, TmpPlayerRight.Right - TmpPlayerLeft.Left, TmpPlayerDown.Bottom - TmpPlayerUp.Top);
			return (TmpRectangleToCheck.Width <= rCamera.ViewportMaxDimension.X && TmpRectangleToCheck.Height <= rCamera.ViewportMaxDimension.Y) ? true : false;
		}

		#endregion

		public bool CheckForAbility(Activity pAcitvity)
		{
			if (mHandicaps.Contains(pAcitvity))
				return false;
			return true;
		}

		#endregion
	}
}
