using HanselAndGretel.Data;
using KryptonEngine.Entities;
using KryptonEngine.FModAudio;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KryptonEngine.AI
{
	enum EnemyType
	{
		Wolf,
		Witch
	}
	public class AIManager
	{
		#region Singleton
		public static AIManager Instance { get { if (instance == null) instance = new AIManager(); return instance; } }
		private static AIManager instance;
		#endregion

		#region Properties

		private EnemyType type;

		protected int[,] Map;
		protected int FieldsHeight;
		protected int FieldsWidth;

		public List<Enemy> Agents;

		public List<InteractiveObject> MoveAreaInteractiveObjects;

		private Vector2 TargetField;
		private List<Node> OpenList;
		private List<Node> ClosedList;
		#endregion

		#region Constructor
		public AIManager()
		{
			Initialize();
		}

		public void Initialize()
		{
			Agents = new List<Enemy>();
			OpenList = new List<Node>();
			ClosedList = new List<Node>();
			MoveAreaInteractiveObjects = new List<InteractiveObject>();
		}
		#endregion

		#region Methods

		// Fügt eine Liste von Rectangle hinzu, mit dennen Kollidiert werden kann.
		public void CalculateMoveableFields(List<Rectangle> MoveList)
		{
			Rectangle Raster = new Rectangle(0, 0, GameReferenzes.RasterSize, GameReferenzes.RasterSize);

			for (int y = 0; y < FieldsHeight; y++)
				for(int x = 0; x < FieldsWidth; x++)
				{
					Raster.X = x * GameReferenzes.RasterSize;
					Raster.Y = y * GameReferenzes.RasterSize;

					foreach(Rectangle r in MoveList)
					{
						if(r.Intersects(Raster))
						{
							Map[x, y] = -1;
							continue;
						}
					}
				}
		}

		// Neuberechnung der Map nachdem die Scene gewechselt wurde
		public void ChangeMap(Rectangle MapSize, List<Rectangle> MoveList)
		{
			FieldsWidth = MapSize.Width / GameReferenzes.RasterSize;
			FieldsHeight = MapSize.Height / GameReferenzes.RasterSize;
			Map = new int[FieldsWidth, FieldsHeight];

			CalculateMoveableFields(MoveList);
		}

		// Setzt die Liste Enemys die die Spieler verfolgen
		public void SetAgents(List<Enemy> AgentList)
		{
			Agents = AgentList;
		}

		public void SetInterActiveObjects(List<InteractiveObject> iObj)
		{
			MoveAreaInteractiveObjects = iObj;
		}

		private void SetCollisionInteractiveObject()
		{
			Rectangle Raster = new Rectangle(0, 0, GameReferenzes.RasterSize, GameReferenzes.RasterSize);

			for (int y = 0; y < FieldsHeight; y++)
				for (int x = 0; x < FieldsWidth; x++)
				{
					Raster.X = x * GameReferenzes.RasterSize;
					Raster.Y = y * GameReferenzes.RasterSize;

				foreach(InteractiveObject iObj in MoveAreaInteractiveObjects)
					foreach(Rectangle r in iObj.CollisionRectList)
					{
						if (r.Intersects(Raster))
							{
								Map[x, y] = -2;
								continue;
							}
						}
				}

		}

		public void Update()
		{

			for (int y = 0; y < FieldsHeight; y++)
				for (int x = 0; x < FieldsWidth; x++)
					if (Map[x, y] == -2)
						Map[x, y] = 0;
			SetCollisionInteractiveObject();

			if (GameReferenzes.ReferenzHansel == null || GameReferenzes.ReferenzGretel == null || GameReferenzes.IsSceneSwitching) return;

			GameReferenzes.TargetPlayer = (GameReferenzes.ReferenzHansel.Lantern) ? (Player)GameReferenzes.ReferenzGretel : (Player)GameReferenzes.ReferenzHansel;
			GameReferenzes.UntargetPlayer = (GameReferenzes.ReferenzHansel.Lantern) ? (Player)GameReferenzes.ReferenzHansel : (Player)GameReferenzes.ReferenzGretel;

			TargetField = new Vector2(GameReferenzes.TargetPlayer.PositionX / GameReferenzes.RasterSize, GameReferenzes.TargetPlayer.PositionY / GameReferenzes.RasterSize);

			foreach(Enemy e in Agents)
			{
				type = (e.GetType() == typeof(Wolf)) ? EnemyType.Wolf : EnemyType.Witch;
				if(!e.IsAiActive)
				{
					int x = (int)(EngineSettings.VirtualResWidth / 2 - (EngineSettings.VirtualResWidth / GameReferenzes.GameCamera.Scale) / 2 + GameReferenzes.GameCamera.Position.X - EngineSettings.VirtualResWidth / 2);
					if (x < 0) x = 0;
					int y = (int)(EngineSettings.VirtualResHeight / 2 - (EngineSettings.VirtualResHeight / GameReferenzes.GameCamera.Scale) / 2 + GameReferenzes.GameCamera.Position.Y - EngineSettings.VirtualResHeight / 2);
					if(y < 0) y = 0;
					Rectangle ScreenView = new Rectangle(x, y, (int)(EngineSettings.VirtualResWidth / GameReferenzes.GameCamera.Scale), (int)(EngineSettings.VirtualResHeight / GameReferenzes.GameCamera.Scale));
					if (ScreenView.Contains(e.PositionX, e.PositionY))
					{
						int charackterSound = EngineSettings.Randomizer.Next(0,2);
						int number = EngineSettings.Randomizer.Next(1,5);

						String SongName;
						SongName = (charackterSound == 0)? "hansel_fear_0" + number.ToString() : "gretel_fear_0" + number.ToString();

						FmodMediaPlayer.Instance.AddSong(SongName, 0.7f);

						if(type == EnemyType.Wolf)
							e.IsAiActive = true;

						switch(type)
						{
							case EnemyType.Witch:
								FmodMediaPlayer.Instance.FadeBackgroundChannelIn(2);
								break;
							case EnemyType.Wolf:
								FmodMediaPlayer.Instance.FadeBackgroundChannelIn(3);
								break;
						}
					}
				}

				if (!e.IsAiActive) continue;

				e.CurrentAiUpdateTime += EngineSettings.Time.ElapsedGameTime.Milliseconds;
				if (EnemyType.Witch == type && e.CurrentAiUpdateTime < GameReferenzes.UPDATETIME_WOLF) continue;
				if (EnemyType.Wolf == type && e.CurrentAiUpdateTime < GameReferenzes.UPDATETIME_WITCH) continue;

				switch(type)
				{
					case EnemyType.Witch: e.CurrentAiUpdateTime -= GameReferenzes.UPDATETIME_WITCH;
						break;
					case EnemyType.Wolf: e.CurrentAiUpdateTime -= GameReferenzes.UPDATETIME_WOLF;
						break;
				}
				
				UpdateEnemy(e);
			}
		}

		private void UpdateEnemy(Enemy e)
		{

			OpenList.Clear();
			ClosedList.Clear();

			Vector2 FieldPosition = new Vector2(e.PositionX / GameReferenzes.RasterSize, e.PositionY / GameReferenzes.RasterSize);
			OpenList.Add(new Node(FieldPosition));

			if (e.GetType() == typeof(Witch))
			{
				CalculateWitchAI(e, FieldPosition);
				ApplyPath(e);
			}

			if (e.GetType() == typeof(Wolf))
			{
				Wolf w = (Wolf)e;
				if (w.IsEscaping)
					CalculateWolfAiEscape(w, FieldPosition);
				else
				{
					CalculateWolfAiAttack(w, FieldPosition);
					ApplyPath(w);
				}
			}
		}

		private void ApplyPath(Enemy e)
		{
			if (ClosedList.Count == 0) return;

			e.Path = CreatePath();
			e.CurrentPath = e.Path.Count - 1;
		}

		private void CalculateWitchAI(Enemy e, Vector2 StartPos)
		{
			CalculateWitchPath(StartPos);

			if (Vector2.Distance(GameReferenzes.UntargetPlayer.Position, e.Position) < GameReferenzes.LIGHT_RADIUS)
				e.SlowFactor = 0.5f;
			else
				e.SlowFactor = 1.0f;
		}

		private void CalculateWolfAiEscape(Wolf e, Vector2 StartPos)
		{
			if(e.EscapePoint == StartPos
				|| e.CurrentPath == 0)
			{
				e.EscapePoint = Vector2.Zero;
				e.IsEscaping = false;
				return;
			}

			if(e.EscapePoint == Vector2.Zero)
			{
				do
				{
				float Angle = EngineSettings.Randomizer.Next() * 360;
					TargetField = new Vector2(
						(int)(GameReferenzes.UntargetPlayer.PositionX + Wolf.ESCAPE_DISTANCE * Math.Cos(Angle)) / GameReferenzes.RasterSize,
						(int)(GameReferenzes.UntargetPlayer.PositionY + Wolf.ESCAPE_DISTANCE * Math.Sin(Angle)) / GameReferenzes.RasterSize);

					if (TargetField.X < 0) TargetField.X = 0;
					if (TargetField.Y < 0) TargetField.Y = 0;

					if (TargetField.X >= FieldsWidth) TargetField.X = FieldsWidth - 1;
					if (TargetField.Y >= FieldsHeight) TargetField.Y = FieldsHeight - 1;
				} while (Map[(int)TargetField.X, (int)TargetField.Y] == -1 || Map[(int)TargetField.X, (int)TargetField.Y] == -2 );
				e.EscapePoint = TargetField;

				CalculateWolfPath(StartPos,true);
				ApplyPath(e);
				return;
			}

		}

		private void CalculateWolfAiAttack(Wolf e, Vector2 StartPos)
		{
			CalculateWolfPath(StartPos,false);
		}

		private void CalculateWitchPath(Vector2 StartPos)
		{
			do
			{
				CheckNextNodesWitch(GetNextNode());
			} while (ClosedList[ClosedList.Count - 1].Position != TargetField && Map[(int)TargetField.X, (int)TargetField.Y] != -1
			&& Map[(int)TargetField.X, (int)TargetField.Y] != -2
			&& StartPos.X >= 0 && StartPos.Y >= 0 && StartPos.X < FieldsWidth && StartPos.Y < FieldsHeight
			&& OpenList.Count != 0 && ClosedList.Count < 500);
		}

		private void CalculateWolfPath(Vector2 StartPos, bool escaping)
		{
			do
			{
				CheckNextNodesWolf(GetNextNode(), escaping);
			} while (ClosedList[ClosedList.Count - 1].Position != TargetField && Map[(int)TargetField.X, (int)TargetField.Y] != -1
			&& Map[(int)TargetField.X, (int)TargetField.Y] != -2
			&& StartPos.X >= 0 && StartPos.Y >= 0 && StartPos.X < FieldsWidth && StartPos.Y < FieldsHeight
			&& OpenList.Count != 0 && ClosedList.Count < 500);

		}
		
		private List<Node> CreatePath()
		{
			List<Node> Path = new List<Node>();
			Path.Add(ClosedList[ClosedList.Count - 1]);

			while (Path[Path.Count - 1].LastNode != null)
				Path.Add(Path[Path.Count - 1].LastNode);
			if (Path.Count > 0)
				Path.RemoveAt(Path.Count - 1);
			if (Path.Count > 0)
				Path.RemoveAt(Path.Count - 1);
			return Path;
		}

		private void CheckNextNodesWitch(Node n)
		{
			if (n == null) return;

			for(int y = -1; y < 2; y++)
				for(int x = -1; x < 2; x++)
				{
					int fieldX = (int)n.Position.X + x;
					int fieldY = (int)n.Position.Y + y;
					if (fieldY < 0 || fieldX < 0
						|| fieldY >= FieldsHeight || fieldX >= FieldsWidth)
						continue;

					float estimatedCost = (Math.Abs(fieldX - (int)TargetField.X) + Math.Abs(fieldY - (int)TargetField.Y)) * 10;
					Node newNode = new Node(n, new Vector2(fieldX, fieldY), estimatedCost);

					if (Map[fieldX, fieldY] != -1 && Map[fieldX, fieldY] != -2)
					{
						bool NodeAvailable = false;
						foreach(Node nodes in OpenList)
							if (newNode.Equals(nodes))
							{
								NodeAvailable = true;
								break;
							}
						foreach (Node nodes in ClosedList)
							if (newNode.Equals(nodes))
							{
								NodeAvailable = true;
								break;
							}

						if(!NodeAvailable)
							OpenList.Add(newNode);
					}
				}
			SwitchNode(n);
		}

		private void CheckNextNodesWolf(Node n, bool wolfEscaping)
		{
			if (n == null) return;

			for (int y = -1; y < 2; y++)
				for (int x = -1; x < 2; x++)
				{
					int fieldX = (int)n.Position.X + x;
					int fieldY = (int)n.Position.Y + y;
					if (fieldY < 0 || fieldX < 0
						|| fieldY >= FieldsHeight || fieldX >= FieldsWidth)
						continue;

					float estimatedCost = (Math.Abs(fieldX - (int)TargetField.X) + Math.Abs(fieldY - (int)TargetField.Y)) * 10;
					Node newNode = new Node(n, new Vector2(fieldX, fieldY), estimatedCost);


					if (Map[fieldX, fieldY] != -1 && Map[fieldX, fieldY] != -2
						 || !(Vector2.Distance(new Vector2(fieldX, fieldY) * GameReferenzes.RasterSize, GameReferenzes.UntargetPlayer.Position) > GameReferenzes.LIGHT_RADIUS - 50.0f))
					{
						bool NodeAvailable = false;
						foreach (Node nodes in OpenList)
							if (newNode.Equals(nodes))
							{
								NodeAvailable = true;
								break;
							}
						foreach (Node nodes in ClosedList)
							if (newNode.Equals(nodes))
							{
								NodeAvailable = true;
								break;
							}
						if (wolfEscaping && !(Vector2.Distance(new Vector2(fieldX, fieldY) * GameReferenzes.RasterSize, GameReferenzes.UntargetPlayer.Position) > GameReferenzes.LIGHT_RADIUS - GameReferenzes.RasterSize))
							NodeAvailable = true;

						if (!NodeAvailable)
							OpenList.Add(newNode);
					}
				}
			SwitchNode(n);
		}

		// Gibt den nächsten Node mit den geringsten Kosten zurück
		private Node GetNextNode()
		{
			if (OpenList.Count == 0) return null;

			Node returnNode = OpenList[0];
			foreach (Node n in OpenList)
				if (n.LowestCost < returnNode.LowestCost)
					returnNode = n;
			return returnNode;
		}

		// Löscht aus der OpenList die Node und schiebt sie in die ClosedList
		private void SwitchNode(Node n)
		{
			ClosedList.Add(n);
			OpenList.Remove(n);
		}

		public void DrawDebugAiGrid(SpriteBatch spriteBatch)
		{
			Texture2D pixel = TextureManager.Instance.GetElementByString("pixel");
			for(int y = 0; y < FieldsHeight; y++)
				spriteBatch.Draw(pixel, new Rectangle(0, y * GameReferenzes.RasterSize,FieldsWidth * GameReferenzes.RasterSize, 2), Color.Black);

			for (int x = 0; x < FieldsWidth; x++)
				spriteBatch.Draw(pixel, new Rectangle(x * GameReferenzes.RasterSize, 0, 2, FieldsHeight * GameReferenzes.RasterSize), Color.Black);

			foreach (Node n in ClosedList)
				spriteBatch.Draw(pixel, new Rectangle((int)n.Position.X * GameReferenzes.RasterSize, (int)n.Position.Y * GameReferenzes.RasterSize, GameReferenzes.RasterSize, GameReferenzes.RasterSize), Color.Yellow * 0.8f);
			foreach (Node n in OpenList)
				spriteBatch.Draw(pixel, new Rectangle((int)n.Position.X * GameReferenzes.RasterSize, (int)n.Position.Y * GameReferenzes.RasterSize, GameReferenzes.RasterSize, GameReferenzes.RasterSize), Color.Red * 0.8f);
			foreach(Enemy e in Agents)
				if(e.Path != null)
				foreach(Node n in e.Path)
					spriteBatch.Draw(pixel, new Rectangle((int)n.Position.X * GameReferenzes.RasterSize, (int)n.Position.Y * GameReferenzes.RasterSize, GameReferenzes.RasterSize, GameReferenzes.RasterSize), Color.Green * 0.8f);
		}

		public void ClearAgents()
		{
			List<Enemy> delete = new List<Enemy>();
			foreach(Enemy e in Agents)
			{
				e.IsAiActive = false;
				if (e.GetType() == typeof(Witch))
					delete.Add(e);
			}

			foreach (Enemy e in delete)
				Agents.Remove(e);
		}
		#endregion
	}
}
