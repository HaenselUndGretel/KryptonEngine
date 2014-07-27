using KryptonEngine;
using KryptonEngine.Entities;
using KryptonEngine.Manager;
using KryptonEngine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace HanselAndGretel.Data
{
	public class Wolf : Enemy
	{
		#region Properties
		private Camera mCamera;

		[XmlIgnoreAttribute]
		public const float ESCAPE_DISTANCE = 400.0f;
		[XmlIgnoreAttribute]
		public bool IsEscaping;
		[XmlIgnoreAttribute]
		public Vector2 EscapePoint;
		#endregion

		#region Getter & Setter

		[XmlIgnoreAttribute]
		public Camera Camera { set { this.mCamera = value; } get { return mCamera; } }
		[XmlIgnoreAttribute]
		public float lightWolfDistance;
		#endregion

		#region Constructor

		public Wolf() { Name = "wolf"; }

		public Wolf(string pName)
			:base(pName)
		{

		}

		#endregion

		#region OverrideMethods

		//public override void Draw(TwoDRenderer renderer)
		//{
		//	renderer.Draw(mTextures, )
		//}

		//public override void Draw(SpriteBatch spriteBatch)
		//{
		//	spriteBatch.Draw(TextureManager.Instance.GetElementByString("EnemyWolf"), mPosition, Color.White);
		//}

        public override void Update()
        {

			base.Update();

			if (GameReferenzes.UntargetPlayer == null) return;
			if (Vector2.Distance(GameReferenzes.UntargetPlayer.Position, Position) < Player.LIGHT_RADIUS)
				IsEscaping = true;

			if (Vector2.Distance(GameReferenzes.UntargetPlayer.Position, Position) > Wolf.ESCAPE_DISTANCE)
			{
				IsEscaping = false;
				EscapePoint = Vector2.Zero;
			}

			if (Path == null || CurrentPath == -1) return;


			if (IsEscaping && EscapePoint == Vector2.Zero)
				return;

			Vector2 Direction = Path[CurrentPath].Position * 16 - Position + new Vector2(EngineSettings.Randomizer.Next(0,16),EngineSettings.Randomizer.Next(0,16));
			Direction = Vector2.Normalize(Direction);

			MoveInteractiveObject(Direction * 3.0f * SlowFactor);

			if (Path[CurrentPath].Position.X == (int)(Position.X / 16)
				&& Path[CurrentPath].Position.Y == (int)(Position.Y / 16))
				CurrentPath--;

			//AttackTime += (float)EngineSettings.Time.TotalGameTime.TotalMilliseconds / 1000;
			//if (AttackTime > 1000)
			//	AttackTime -= 6000;

			//lightWolfDistance = Vector2.Distance(mUntargetPlayer.Position, Position);
			//if (lightWolfDistance < Player.LIGHT_RADIUS)
			//	mIsEscaping = true;

			//else if (lightWolfDistance > ESCAPE_DISTANCE)
			//	mIsEscaping = false;

			//float distancePlayers = Vector2.Distance(GameReferenzes.ReferenzHansel.Position, GameReferenzes.ReferenzGretel.Position);

			//if (distancePlayers < Player.LIGHT_RADIUS)
			//	mIsPlayerInLight = true;


			//Vector2 moveDirection = mTargetPlayer.Position - Position;

			//moveDirection = new Vector2(moveDirection.X, moveDirection.Y);
			//moveDirection = Vector2.Normalize(moveDirection);

			//moveDirection *= 2.0f;
			//moveDirection.Y += (float)Math.Sin((float)EngineSettings.Time.TotalGameTime.TotalMilliseconds / 1000) * 2;
			//moveDirection.X += (float)Math.Sin((float)EngineSettings.Time.TotalGameTime.TotalMilliseconds / 1000) * 2;

			//if (moveDirection.X < 1f && moveDirection.Y < 1f)
			//	mIsEscaping = false;

			//if (Vector2.Distance(Position, mUntargetPlayer.Position) > ESCAPE_DISTANCE)
			//	mIsEscaping = false;

			//if (mIsEscaping)
			//	moveDirection *= -speedFactor;

			//MoveInteractiveObject(moveDirection);

			//if (mIsPlayerInLight)
			//{
			//	lightWolfDistance = Vector2.Distance(mUntargetPlayer.Position, Position);
			//	if (lightWolfDistance < Player.LIGHT_RADIUS)
			//		mIsEscaping = true;
			//}
            // Überprüfen ob Spieler ohne licht im LichtRadius ist

            // sinus bewegnung zum Anviesierten Spieler

            // im Lichtradius abfall der Bewegungs geschwindigkeit
            // erreicht den inneren minimal abstand, rückzug in sinus zur flucht distanz

        }

		#endregion

		#region Methods
		#endregion
	}
}
