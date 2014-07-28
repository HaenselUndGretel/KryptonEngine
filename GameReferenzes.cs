using HanselAndGretel.Data;
using KryptonEngine.Entities;
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

		public readonly static int RasterSize = 16;
		public readonly static float DEATH_ZONE = 20.0f;
		public static float LIGHT_RADIUS = 200.0f;

		public static Camera GameCamera;

		public static SceneData Level;
		#endregion

		public GameReferenzes()
		{ }
	}
}
