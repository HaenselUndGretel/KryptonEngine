﻿using KryptonEngine.Entities;
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

		public Vector2 PositionHansel;
		public Vector2 PositionGretel;

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
			PositionHansel = pSavegame.PositionHansel;
			PositionGretel = pSavegame.PositionGretel;
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
			Scenes = new SceneData[1]; //ToDo: Anzahl Scenes setzen !---!---!---!---!
			for (int i = 0; i < Scenes.Length; i++)
				Scenes[i] = new SceneData(); //Scenes initialisieren
		}

		public static Savegame Load(Hansel pHansel, Gretel pGretel) //Muss static sein damit das Savegame als solches gesetzt werden kann.
		{
			Savegame TmpSavegame;
			FileInfo file = new FileInfo(Savegame.SavegamePath);
			if (!file.Exists)
			{
				TmpSavegame = new Savegame();
				TmpSavegame.Reset();
				pHansel.Position = new Vector2(190, 50); //Init Position Hansel
				pGretel.Position = new Vector2(250, 50); //Init Position Gretel
				Savegame.Save(TmpSavegame, pHansel, pGretel);
				TmpSavegame.LoadContent();
				TmpSavegame.Scenes[TmpSavegame.SceneId].SetupRenderList(pHansel, pGretel);
				return TmpSavegame;
			}
			xmlReader = new StreamReader(Savegame.SavegamePath);
			TmpSavegame = (Savegame)SavegameSerializer.Deserialize(xmlReader); //Savegame aus File laden
			xmlReader.Close();
			TmpSavegame.LoadContent();
			pHansel.Position = TmpSavegame.PositionHansel;
			pGretel.Position = TmpSavegame.PositionGretel;
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
			pSavegame.PositionHansel = pHansel.Position;
			pSavegame.PositionGretel = pGretel.Position;
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
		}

		#endregion
	}
}
