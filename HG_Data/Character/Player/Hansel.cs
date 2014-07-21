using KryptonEngine.Controls;
using KryptonEngine.Entities;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel.Data
{
	public class Hansel : Player
	{
		#region Constructor

		public Hansel() : base() { Initialize(); }

		public Hansel(string pName)
			:base(pName)
		{
			Initialize();
		}

		#endregion

		#region Override Methods

		public override void Initialize()
		{
			base.Initialize();
			mInput = InputHelper.Player1;
			//mCollisionBox.Width = 50;
			//mCollisionBox.Height = 50;
			mHandicaps.Add(Activity.SlipThroughRock);
		}

		public override void LoadContent()
		{
			base.LoadContent();
			SkeletonPosition = new Vector2(190, 50); //Init Position Hansel
			Position = SkeletonPosition;
		}

		#endregion
	}
}
