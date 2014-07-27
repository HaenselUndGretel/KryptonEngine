using KryptonEngine;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel.Data
{
	public class Witch : Enemy
	{
		#region Properties

		#endregion

		#region Getter & Setter

		#endregion

		#region Constructor

		public Witch() { }

		public Witch(string pName)
			:base(pName)
		{

		}

		#endregion

		#region OverrideMethods
		public override void Update()
		{
			base.Update();

			if (Path == null || CurrentPath == -1) return;

			Vector2 Direction = Path[CurrentPath].Position * 16 - Position + new Vector2(EngineSettings.Randomizer.Next(0, 16), EngineSettings.Randomizer.Next(0, 16));
			Direction = Vector2.Normalize(Direction);

			MoveInteractiveObject(Direction * 3.0f * SlowFactor);

			if (Path[CurrentPath].Position.X == (int)(Position.X / 16)
				&& Path[CurrentPath].Position.Y == (int)(Position.Y / 16))
				CurrentPath--;
		}
		#endregion

		#region Methods

		#endregion
	}
}
