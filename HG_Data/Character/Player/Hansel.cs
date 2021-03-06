﻿using KryptonEngine.Controls;
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
	public class Hansel : Player
	{

		#region Constructor

		public Hansel() : base("hansel") { Initialize(); }

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
			mHandicaps.Add(Activity.SlipThroughRock);
		}

		public override void LoadContent()
		{
			base.LoadContent();
			SkeletonPosition = new Vector2(709, 246); //Init Position Hansel
			Position = SkeletonPosition;
		}

		#endregion
	}
}
