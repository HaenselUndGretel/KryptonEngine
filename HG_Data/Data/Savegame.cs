using KryptonEngine.Entities;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace HanselAndGretel.Data
{
	public class Savegame
	{
		#region Properties

		public List<Collectable> Collectables;

		public SceneData[] Scenes;

		public Hansel hansel;
		public Gretel gretel;

		/// <summary>
		/// Saved Scene to start from.
		/// </summary>
		public int SceneId;

		[XmlIgnoreAttribute]
		protected static string ScenePath;
		[XmlIgnoreAttribute]
		protected static string SavegamePath;
		[XmlIgnoreAttribute]
		protected static XmlSerializer SceneSerializer;
		[XmlIgnoreAttribute]
		protected static XmlSerializer SavegameSerializer;
		[XmlIgnoreAttribute]
		protected static StreamReader xmlReader;
		[XmlIgnoreAttribute]
		protected static StreamWriter xmlWriter;

		#endregion

		#region Constructor

		public Savegame()
		{
			Initialize();
		}

		public Savegame(Savegame pSavegame)
		{
			Initialize();
			Collectables = pSavegame.Collectables;
			hansel = pSavegame.hansel;
			gretel = pSavegame.gretel;
			SceneId = pSavegame.SceneId;
			Scenes = pSavegame.Scenes;
		}

		#endregion

		#region Methods

		public void Initialize()
		{
			ScenePath = Environment.CurrentDirectory + @"\Content\hug";
			SavegamePath = Environment.CurrentDirectory + @"\save.hugs"; //Hänsel Und Gretel Savegame
			SceneSerializer = new XmlSerializer(typeof(SceneData));
			SavegameSerializer = new XmlSerializer(typeof(Savegame));
			Collectables = new List<Collectable>();
			SceneId = 0;
			Scenes = new SceneData[19]; //ToDo: Anzahl Scenes setzen !---!---!---!---!
			for (int i = 0; i < Scenes.Length; i++)
				Scenes[i] = new SceneData(); //Scenes initialisieren
			hansel = new Hansel();
			gretel = new Gretel();
		}

		public static Savegame Load(Hansel pHansel, Gretel pGretel) //Muss static sein damit das Savegame als solches gesetzt werden kann.
		{
			Savegame TmpSavegame;
			FileInfo file = new FileInfo(Savegame.SavegamePath);
			if (!file.Exists)
			{
				//Build Default Savegame
				TmpSavegame = new Savegame();
				TmpSavegame.Reset();
				CopyPlayerPositions(pHansel, pGretel, TmpSavegame);
				//Save new Savegame to File
				Savegame.Save(TmpSavegame, pHansel, pGretel);
				//Setup Savegame
				TmpSavegame.LoadContent();
				TmpSavegame.Scenes[TmpSavegame.SceneId].SetupRenderList(pHansel, pGretel);
				return TmpSavegame;
			}
			//Get Savegame from File
			xmlReader = new StreamReader(Savegame.SavegamePath);
			TmpSavegame = (Savegame)SavegameSerializer.Deserialize(xmlReader); //Savegame aus File laden
			xmlReader.Close();
			//SetupSavegame
			CopyPlayerPositions(pHansel, pGretel, TmpSavegame);
			TmpSavegame.LoadContent();
			TmpSavegame.Scenes[TmpSavegame.SceneId].SetupRenderList(pHansel, pGretel);
			return TmpSavegame;
		}

		/// <summary>
		/// Für den Editor und für Reset(): Lädt die Scenes in ScenePath in Scenes[].
		/// </summary>
		/// <param name="pLevelId">000 - 999</param>
		public void LoadLevel(int pLevelId)
		{
			Scenes[pLevelId].ResetLevel();
			FileInfo file = new FileInfo(ScenePath + "\\" + LevelNameFromId(pLevelId) + ".hug");
			if (!file.Exists)
				throw new FileNotFoundException("Die Scene " + LevelNameFromId(pLevelId).ToString() + " existiert nicht! WIESO?!?");
			xmlReader = new StreamReader(file.FullName);
			Scenes[pLevelId] = (SceneData)SceneSerializer.Deserialize(xmlReader); //sData File in SpineData Object umwandeln
			xmlReader.Close();
			Scenes[pLevelId].LoadContent(LevelNameFromId(pLevelId));
		}

		/// <summary>
		/// Speichert pSavegame an pSavegame.SavegamePath.
		/// </summary>
		/// <param name="pSavegame">Savegame, das gesaved werden soll.</param>
		public static void Save(Savegame pSavegame, Hansel pHansel, Gretel pGretel) //Muss static sein damit das Savegame als solches serialisiert werden kann.
		{
			pSavegame.hansel = pHansel;
			pSavegame.gretel = pGretel;
			xmlWriter = new StreamWriter(Savegame.SavegamePath);
			SavegameSerializer.Serialize(xmlWriter, pSavegame); //Savegame in File schreiben
			xmlWriter.Close();
		}

		/// <summary>
		/// Für den Editor: Speichert eine Scene als pLevelId.hug
		/// </summary>
		/// <param name="pLevelId">000 - 999</param>
		public void SaveLevel(int pLevelId)
		{
			xmlWriter = new StreamWriter(ScenePath + "\\" + LevelNameFromId(pLevelId) + ".hug");
			SceneSerializer.Serialize(xmlWriter, Scenes[pLevelId]);
			xmlWriter.Close();
		}

		public void Reset()
		{
			Initialize(); //Flush Savegame mit default Werten
			for (int i = 0; i < Scenes.Length; i++)
				LoadLevel(i); //Scenes neu laden
			LoadContent();
		}

		protected string LevelNameFromId(int pLevelId)
		{
			string LevelName = "";
			for (int i = 0; i < 3 - pLevelId.ToString().Length; i++)
				LevelName += "0";
			LevelName += pLevelId.ToString();
			return LevelName;
		}

		public void LoadContent()
		{
			for (int i = 0; i < Scenes.Length; i++)
			{
				Scenes[i].LoadContent(LevelNameFromId(i));
			}
			foreach (Collectable col in Collectables)
				col.LoadContent();
			hansel.LoadContent();
			gretel.LoadContent();
		}

		protected static void CopyPlayerPositions(Hansel pHansel, Gretel pGretel, Savegame pSavegame)
		{
			pHansel.SkeletonPosition = pSavegame.hansel.Position;
			pHansel.Position = pSavegame.hansel.Position;
			pHansel.CollisionRectList = pSavegame.hansel.CollisionRectList;
			pGretel.SkeletonPosition = pSavegame.gretel.Position;
			pGretel.Position = pSavegame.gretel.Position;
			pGretel.CollisionRectList = pSavegame.gretel.CollisionRectList;
			pHansel.ApplySettings();
			pGretel.ApplySettings();
		}

		public static void Delete()
		{
			FileInfo file = new FileInfo(Savegame.SavegamePath);
			if (file.Exists)
				file.Delete();
		}

		#endregion
	}
}
