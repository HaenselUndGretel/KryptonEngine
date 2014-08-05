using HanselAndGretel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KryptonEngine.HG_Data
{
	public class Amulet : Collectable
	{
		public Amulet()
			: base()
		{

		}

		public Amulet(string pName)
			: base(pName)
		{

		}

		public Amulet(string pName, string pShowTextureName)
			: base(pName, pShowTextureName)
		{

		}
	}
}
