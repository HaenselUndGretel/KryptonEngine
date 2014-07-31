using KryptonEngine;
using KryptonEngine.Entities;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace HanselAndGretel.Data
{
	[Serializable, XmlInclude(typeof(DirectionLight)), XmlInclude(typeof(PointLight)), XmlInclude(typeof(SpotLight))]
	public class Light : GameObject
	{		
		#region Properties

		protected float mIntensity;
		protected float mDepth;
		protected Vector3 mColor;

		protected List<Vector2> mCircleSize;

		public bool LightFading;
		public Vector3 FadingLightColor;
		public int FadingDuration;
		public Vector3 StartColor;

		public bool IsLightMoving;
		public int MoveSpeed;
		public int MaxHeight;
		public int MinHeight;
		private float DepthStandart;
		#endregion

		#region Getter & Setter

		public float Intensity { get { return mIntensity; } set { mIntensity = value; } }
		public float Depth { get { return mDepth; } set { mDepth = value; DepthStandart = value; } }
		public Vector3 LightColor { get { return mColor; } set { mColor = value; } }
		#endregion

		#region Constructor

		public Light() : base()
		{

		}

		public Light(Vector2 pPositon)
			: base(pPositon)
		{
			// 64x64 Icongroße Hitbox
			mCollisionBox = new Rectangle((int)pPositon.X, (int)pPositon.Y, 64, 64);
			mCircleSize = new List<Vector2>();
		}
		#endregion

		#region Override Methods

		public override void Update()
		{
			Fade();
			Move();
			base.Update();
		}


		#endregion

		#region Methods

		private void Fade()
		{
			if (!LightFading) return;
			
			if (FadingDuration == 0)
				FadingDuration = 1;

			float lerpFactor = ((float)Math.Sin(EngineSettings.Time.TotalGameTime.TotalMilliseconds * (1.0f / (float)FadingDuration)) + 1) / 2.0f;
			
			float X = MathHelper.Lerp(StartColor.X, FadingLightColor.X, lerpFactor);
			float Y = MathHelper.Lerp(StartColor.Y, FadingLightColor.Y, lerpFactor);
			float Z = MathHelper.Lerp(StartColor.Z, FadingLightColor.Z, lerpFactor);
			LightColor = new Vector3(X, Y, Z);

		}

		private void Move()
		{
			if (!IsLightMoving) return;

			if (MoveSpeed == 0)
				FadingDuration = 1;

			float lerpFactor = ((float)Math.Sin(EngineSettings.Time.TotalGameTime.TotalMilliseconds * (1.0f / (float)MoveSpeed)) +  1) / 2.0f;

			mDepth = (Math.Abs(MinHeight - MaxHeight) / 1000.0f) * lerpFactor + MinHeight / 1000.0f;
		}

		protected void DrawPartCircel(SpriteBatch spriteBatch,float radius, float startAngel, float endAngel, Vector2 pos)
		{
			/////////////////////////////////////////
			// Fix wenn geladen ist die liste leer //
			/////////////////////////////////////////

			foreach(Vector2 v in mCircleSize)
				spriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), v, Color.Yellow);

			for(int i = 0; i < mCircleSize.Count - 1; i++)
				DrawLine(mCircleSize[i], mCircleSize[i + 1], TextureManager.Instance.GetElementByString("pixel"), 1.0f, spriteBatch);
		}

		protected void DrawLine(Vector2 from, Vector2 to, Texture2D texture, float size, SpriteBatch spriteBatch)
		{
			double degress = Math.Atan2((to - from).Y, (to - from).X);
			float length = Vector2.Distance(from, to);
			spriteBatch.Draw(texture, from, new Rectangle(0, 0, 1, 1), Color.White, (float)degress, Vector2.Zero, new Vector2(length, size), SpriteEffects.None, 0);
		}
		#endregion
	}
}
