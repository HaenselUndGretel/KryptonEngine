using KryptonEngine;
using KryptonEngine.Entities;
using KryptonEngine.FModAudio;
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
		private float mWolfSpeed = 6.0f;
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

        public override void Update()
        {

			base.Update();

			if (GameReferenzes.UntargetPlayer == null) return;
			if (Vector2.Distance(GameReferenzes.UntargetPlayer.Position, Position) < GameReferenzes.LIGHT_RADIUS)
				IsEscaping = true;

			if (Vector2.Distance(GameReferenzes.UntargetPlayer.Position, Position) > Wolf.ESCAPE_DISTANCE)
			{
				IsEscaping = false;
				EscapePoint = Vector2.Zero;
			}

			if(Vector2.Distance(GameReferenzes.UntargetPlayer.Position, Position) < Wolf.ESCAPE_DISTANCE)
			{
				mSoundCountdown += EngineSettings.Time.ElapsedGameTime.Milliseconds;

				if (mSoundCountdown > SOUND_COOLDOWN)
				{
					int number = EngineSettings.Randomizer.Next(1, 5);
					FmodMediaPlayer.Instance.AddSong("SfxWolf" + number, 0.9f);
					mSoundCountdown -= SOUND_COOLDOWN;
				}
			}

			if (Path == null || CurrentPath == -1) return;

			if (IsEscaping && EscapePoint == Vector2.Zero)
				return;

			Vector2 Direction = Path[CurrentPath].Position * 16 - Position + new Vector2(EngineSettings.Randomizer.Next(0,16),EngineSettings.Randomizer.Next(0,16));
			Direction = Vector2.Normalize(Direction);

			MoveInteractiveObject(Direction * mWolfSpeed *SlowFactor);

			if (Path[CurrentPath].Position.X == (int)(Position.X / 16)
				&& Path[CurrentPath].Position.Y == (int)(Position.Y / 16))
				CurrentPath--;
        }

		#endregion

		#region Methods
		#endregion
	}
}
