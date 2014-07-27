using HanselAndGretel.Data;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KryptonEngine.AI
{
	public class AIManager
	{
		#region Properties

		const float UPDATETIME = 500;
		float currentUpdateTime = 0;

		protected int[,] Map;
		protected int mRasterSize;
		protected int FieldsHeight;
		protected int FieldsWidth;

		public List<Enemy> Agents;

		//public Player TargetPlayer;
		//public Player UntargetPlayer;

		private Vector2 TargetField;
		private List<Node> OpenList;
		private List<Node> ClosedList;
		#endregion

		#region Constructor
		public AIManager(Rectangle MapSize, int EdgeSize)
		{
			mRasterSize = EdgeSize;
			FieldsWidth = MapSize.Width / EdgeSize;
			FieldsHeight = MapSize.Height / EdgeSize;
			Map = new int[FieldsWidth, FieldsHeight];

			Agents = new List<Enemy>();
			OpenList = new List<Node>();
			ClosedList = new List<Node>();
		}
		#endregion

		#region Methods

		// Fügt eine Liste von Rectangle hinzu, mit dennen Kollidiert werden kann.
		public void CalculateMoveableFields(List<Rectangle> MoveList)
		{
			Rectangle Raster = new Rectangle(0, 0, mRasterSize, mRasterSize);

			for (int y = 0; y < FieldsHeight; y++)
				for(int x = 0; x < FieldsWidth; x++)
				{
					Raster.X = x * mRasterSize;
					Raster.Y = y * mRasterSize;

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
		public void ChangeMap(Rectangle MapSize, int EdgeSize, List<Rectangle> MoveList)
		{
			mRasterSize = EdgeSize;
			FieldsWidth = MapSize.Width / EdgeSize;
			FieldsHeight = MapSize.Height / EdgeSize;
			Map = new int[FieldsWidth, FieldsHeight];

			CalculateMoveableFields(MoveList);
		}

		// Setzt die Liste Enemys die die Spieler verfolgen
		public void SetAgents(List<Enemy> AgentList)
		{
			Agents = AgentList;
		}

		public void Update()
		{
			currentUpdateTime += EngineSettings.Time.ElapsedGameTime.Milliseconds;
			if (currentUpdateTime < UPDATETIME) return;

			currentUpdateTime -= UPDATETIME;

			if (GameReferenzes.ReferenzHansel == null || GameReferenzes.ReferenzGretel == null) return;

			GameReferenzes.TargetPlayer = (GameReferenzes.ReferenzHansel.HasLamp) ? (Player)GameReferenzes.ReferenzGretel : (Player)GameReferenzes.ReferenzHansel;
			GameReferenzes.UntargetPlayer = (GameReferenzes.ReferenzHansel.HasLamp) ? (Player)GameReferenzes.ReferenzHansel : (Player)GameReferenzes.ReferenzGretel;

			TargetField = new Vector2(GameReferenzes.TargetPlayer.PositionX / mRasterSize, GameReferenzes.TargetPlayer.PositionY / mRasterSize);

			foreach(Enemy e in Agents)
			{
				OpenList.Clear();
				ClosedList.Clear();
				
				Vector2 FieldPosition = new Vector2(e.PositionX / mRasterSize, e.PositionY / mRasterSize);
				OpenList.Add(new Node(FieldPosition));

				if (e.GetType() == typeof(Witch))
				{
					CalculateWitchAI(e, FieldPosition);
					ApplyPath(e);
				}

				if(e.GetType() == typeof(Wolf))
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

			if (Vector2.Distance(GameReferenzes.ReferenzGretel.Position, e.Position) < Player.LIGHT_RADIUS)
				e.SlowFactor = 0.6f;
			else
				e.SlowFactor = 1.0f;
		}

		private void CalculateWolfAiEscape(Wolf e, Vector2 StartPos)
		{
			if(e.EscapePoint == StartPos)
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
						(int)(GameReferenzes.UntargetPlayer.PositionX + Wolf.ESCAPE_DISTANCE * Math.Cos(Angle)) / mRasterSize,
						(int)(GameReferenzes.UntargetPlayer.PositionY + Wolf.ESCAPE_DISTANCE * Math.Sin(Angle)) / mRasterSize);

					if (TargetField.X < 0) TargetField.X = 0;
					if (TargetField.Y < 0) TargetField.Y = 0;

					if (TargetField.X > FieldsWidth) TargetField.X = FieldsWidth;
					if (TargetField.Y > FieldsHeight) TargetField.Y = FieldsHeight;
				} while (Map[(int)TargetField.X, (int)TargetField.Y] == -1);
				e.EscapePoint = TargetField;

				CalculateWolfPath(StartPos,true);
			}
			ApplyPath(e);

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
			&& StartPos.X >= 0 && StartPos.Y >= 0 && StartPos.X < FieldsWidth && StartPos.Y < FieldsHeight);
		}

		private void CalculateWolfPath(Vector2 StartPos, bool escaping)
		{
			do
			{
				CheckNextNodesWolf(GetNextNode(), escaping);
			} while (ClosedList[ClosedList.Count - 1].Position != TargetField && Map[(int)TargetField.X, (int)TargetField.Y] != -1
				&& StartPos.X >= 0 && StartPos.Y >= 0 && StartPos.X < FieldsWidth && StartPos.Y < FieldsHeight);

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

					if (Map[fieldX, fieldY] != -1)
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


					if (Map[fieldX, fieldY] != -1 
						 || !(Vector2.Distance(new Vector2(fieldX,fieldY) * mRasterSize, GameReferenzes.UntargetPlayer.Position) > Player.LIGHT_RADIUS - 50.0f))
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
						if (wolfEscaping && !(Vector2.Distance(new Vector2(fieldX, fieldY) * mRasterSize, GameReferenzes.UntargetPlayer.Position) > Player.LIGHT_RADIUS - mRasterSize))
							NodeAvailable = true;
							//Console.WriteLine("Feld im Licht!");

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
				spriteBatch.Draw(pixel, new Rectangle(0, y * mRasterSize,FieldsWidth * mRasterSize, 2), Color.Black);

			for (int x = 0; x < FieldsHeight; x++)
				spriteBatch.Draw(pixel, new Rectangle(x * mRasterSize, 0, 2, FieldsHeight * mRasterSize), Color.Black);

			foreach (Node n in ClosedList)
				spriteBatch.Draw(pixel, new Rectangle((int)n.Position.X * mRasterSize, (int)n.Position.Y * mRasterSize, mRasterSize, mRasterSize), Color.Yellow * 0.8f);
			foreach (Node n in OpenList)
				spriteBatch.Draw(pixel, new Rectangle((int)n.Position.X * mRasterSize, (int)n.Position.Y * mRasterSize, mRasterSize, mRasterSize), Color.Red * 0.8f);
			foreach(Enemy e in Agents)
				if(e.Path != null)
				foreach(Node n in e.Path)
					spriteBatch.Draw(pixel, new Rectangle((int)n.Position.X * mRasterSize, (int)n.Position.Y * mRasterSize, mRasterSize, mRasterSize), Color.Green * 0.8f);
		}
		#endregion
	}
}
