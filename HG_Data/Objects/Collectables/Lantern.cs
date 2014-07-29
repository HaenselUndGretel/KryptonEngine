using HanselAndGretel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel.Data
{
	public class Lantern : Collectable
	{
		public Lantern()
			:base()
		{

		}

		public Lantern(string pName)
			:base(pName)
		{

		}

		public Lantern(string pName, string pShowTextureName)
			:base(pName, pShowTextureName)
		{

		}

	}
}
