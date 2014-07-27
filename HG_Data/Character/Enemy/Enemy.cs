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
		public Vector2 Destination = Vector2.Zero;
		[XmlIgnoreAttribute]
		public List<Node> Path;
		[XmlIgnoreAttribute]
		public int CurrentPath = 0;
		[XmlIgnoreAttribute]
		public float SlowFactor = 1.0f;
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

		#endregion

		#region Methods

		#endregion
	}
}
