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
		public readonly static float DEATH_ZONE = 50.0f;

		public readonly static float LIGHT_RADIUS = 250.0f;
		public readonly static float LIGHT_RADIUS_HIGH = 400.0f;

		public static Camera GameCamera;

		public static SceneData Level;

		public readonly static float UPDATETIME_WOLF = 800.0f;
		public readonly static float UPDATETIME_WITCH = 1000.0f;

		public static bool IsSceneSwitching = false;

		public static int SceneID;

		public static List<String> ForestTheme = new List<String>() { "MusicForest0", "MusicForest1", "MusicForest2", "MusicForest3"};
		public static List<String> HouseTheme = new List<String>() { "MusicHouse0", "MusicHouse1", "MusicHouse2", "MusicHouse3", "MusicHouse4" };
		public static List<String> MountainsTheme = new List<String>() { "MusicMountains0", "MusicMountains1", "MusicMountains2", "MusicMountains3" };
		public static List<String> MainTheme = new List<String>(){"MusicMainTheme"};
		//public static List<String> BossFightTheme = new List<String>(){ }

		public static List<String> GetBackgroundMusic()
		{
			switch(Level.BackgroundSoundSetting)
			{
				case SoundSetting.Forest: return ForestTheme;
				case SoundSetting.Inside: return HouseTheme;
				case SoundSetting.Mountain: return MountainsTheme;
				default: return ForestTheme;
			}
		}

		#endregion

		public GameReferenzes()
		{ }
	}
}
