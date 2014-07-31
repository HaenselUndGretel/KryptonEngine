using KryptonEngine;
using KryptonEngine.AI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace HanselAndGretel.Data
{
	public class Enemy : Character
	{
		#region Properties
		[XmlIgnoreAttribute]
		public float CurrentAiUpdateTime = 0.0f;
		[XmlIgnoreAttribute]
		public Vector2 Destination = Vector2.Zero;
		[XmlIgnoreAttribute]
		public List<Node> Path;
		[XmlIgnoreAttribute]
		public int CurrentPath = 0;
		[XmlIgnoreAttribute]
		public float SlowFactor = 1.0f;
		[XmlIgnoreAttribute]
		public bool IsAiActive = false;

		protected const float SOUND_COOLDOWN = 15000.0f;
		protected float mSoundCountdown = 15000;
		#endregion

		#region Getter & Setter

		#endregion

		#region Constructor

		public Enemy() : base() { }

		public Enemy(string pName)
			:base(pName)
		{

		}


		#endregion

		#region OverrideMethods

		public override void Update()
		{
			base.Update();

		}
		#endregion

		#region Methods

		#endregion
	}
}
