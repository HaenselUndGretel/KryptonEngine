using HanselAndGretel.Data;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KryptonEngine.HG_Data
{
	public class AmbientLight : Light
	{
		#region Properties
		#endregion

		#region Constructir
		public AmbientLight()
		{
			mColor = new Vector3(100.0f / 255.0f, 100.0f / 255.0f, 125.0f / 255.0f);
			mIntensity = 1f;
		}
		#endregion

		#region Override Methods
		#endregion

		#region Methods
		#endregion

	}
}
