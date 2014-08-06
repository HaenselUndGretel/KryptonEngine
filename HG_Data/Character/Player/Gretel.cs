using KryptonEngine.Controls;
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
		#region Constructor

		public Gretel() : base("gretel") { Initialize(); }

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
			mHandicaps.Add(Activity.JumpOverGap);
		}

		public override void LoadContent()
		{
			base.LoadContent();
			SkeletonPosition = new Vector2(632, 238); //Init Position Gretel
			Position = SkeletonPosition;
		}

		#endregion
	}
}
