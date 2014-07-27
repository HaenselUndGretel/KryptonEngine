using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KryptonEngine.AI
{
	public class Node
	{
		public Node LastNode;
		public float Cost;
		public float EstimatedCost;
		public float LowestCost;
		public Vector2 Position;

		public Node(Vector2 pPosition)
		{
			Position = pPosition;
		}

		public Node(Node pLastNode, Vector2 pPosition, float pEstimatedCost)
		{
			LastNode = pLastNode;
			Position = pPosition;
			EstimatedCost = pEstimatedCost;

			Vector2 calcCost = Position - LastNode.Position;
			if (Math.Abs(calcCost.X) == 1 && Math.Abs(calcCost.Y) == 1)
				Cost = 15 + LastNode.Cost;
			else
				Cost = 10 + LastNode.Cost;

			LowestCost = EstimatedCost + Cost;
		}

		public bool Equals(Node n)
		{
			if (n.Position == this.Position)
				return true;
			else
				return false;
		}
	}
}
