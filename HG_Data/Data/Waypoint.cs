﻿using KryptonEngine.Entities;
using KryptonEngine.Interface;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace HanselAndGretel.Data
{
	public class Waypoint : GameObject
	{
		#region Properties

		protected int mDestinationSceneId;
		protected int mDesinationWaypointId;
		protected bool mOneWay;
		protected const float mLeaveSpeed = 1;
		protected Vector2 mMovementOnEnter;

		#endregion

		#region Getter & Setter

		public int DestinationScene { get { return mDestinationSceneId; } set { mDestinationSceneId = value; } }
		public int DestinationWaypoint { get { return mDesinationWaypointId; } set { mDesinationWaypointId = value; } }

		/// <summary>
		/// Wenn True: Dieser Waypoint kann nur Betreten aber nicht Verlassen werden.
		/// </summary>
		public bool OneWay { get { return mOneWay; } set { mOneWay = value; } }

		/// <summary>
		/// Bewegung beim Betreten des Wegpunkts.
		/// </summary>
		public Vector2 MovementOnEnter { get { return mMovementOnEnter; } set { mMovementOnEnter = value; } }

		#endregion

		#region Constructor

		public Waypoint()
		{
			Initialize();
		}

		#endregion

		#region Override Methods

		public override void Initialize()
		{
			mDebugColor = Color.DarkGreen;
			mOneWay = false;
			mMovementOnEnter = new Vector2(-1f, 0);
		}

		// Wird nur im Editor gezeichnet
		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(TextureManager.Instance.GetElementByString("IconMoveArea"), CollisionBox, Color.White);
		}

		/// <summary>
		/// Wird für die Infobox im Editor benötigt.
		/// </summary>
		/// <returns></returns>
		public override string GetInfo()
		{
			String tmpInfo;

			tmpInfo = base.GetInfo();
			tmpInfo += "\nZiel Scene: " + mDestinationSceneId;
			tmpInfo += "\nZiel Waypoint: " + mDesinationWaypointId;
			tmpInfo += "\nOneway:" + mOneWay;

			String leave = "";
			if (mMovementOnEnter.X > 0)
				leave = "\nVerlassen : Osten";
			else if(mMovementOnEnter.X < 0)
				leave = "\nVerlassen : Westen";
			else if(mMovementOnEnter.Y > 0)
				leave = "\nVerlassen : Sueden";
			else if (mMovementOnEnter.Y < 0)
				leave = "\nVerlassen : Norden";

			tmpInfo += leave;

			return tmpInfo;
		}

		#endregion

		#region Methods

		#endregion
	}
}
