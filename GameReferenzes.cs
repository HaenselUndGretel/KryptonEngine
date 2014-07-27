using HanselAndGretel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KryptonEngine
{
	public class GameReferenzes
	{
		#region Singleton

		private static GameReferenzes instance;
		public static GameReferenzes Instance { get { if (instance == null) instance = new GameReferenzes(); return instance; ; } }
		#endregion

		#region Properties
		public static Gretel ReferenzGretel;
		public static Hansel ReferenzHansel;

		public static Player TargetPlayer;
		public static Player UntargetPlayer;
		#endregion

		public GameReferenzes()
		{ }
	}
}
