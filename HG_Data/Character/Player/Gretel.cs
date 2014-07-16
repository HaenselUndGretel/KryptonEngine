﻿using KryptonEngine.Controls;
using KryptonEngine.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel.Data
{
	public class Gretel : Player
	{
		#region Properties

		#endregion

		#region Getter & Setter

		#endregion

		#region Constructor

		public Gretel(string pName)
			:base(pName)
		{
			Initialize();
		}

		#endregion

		#region OverrideMethods

		public override void Initialize()
		{
			base.Initialize();
			mInput = InputHelper.Player2;
			mCollisionBox.Width = 50;
			mCollisionBox.Height = 50;
			mHandicaps.Add(Activity.JumpOverGap);
		}

		#endregion

		#region Methods

		public void TryToGrabItem()
		{
			
		}

		#endregion
	}
}
