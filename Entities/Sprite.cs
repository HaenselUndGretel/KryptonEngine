﻿/**************************************************************
 * (c) Carsten Baus, Jens Richter 2014
 *************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KryptonEngine.Manager;
using System.Xml.Serialization;
using KryptonEngine.Rendering;

namespace KryptonEngine.Entities
{
    public class Sprite : GameObject
    {
        #region Properties

        protected String mTextureName;
		protected Texture2D[] mTextures;
        protected Color mTint = Color.White;
		protected int mWidth;
		protected int mHeight;

        protected Vector2 mOrigin;
		protected int mRotation = 0;
        protected SpriteEffects mEffekt = SpriteEffects.None;

        #endregion

		#region Getter & Setter

		[XmlIgnoreAttribute]
		public Texture2D[] Textures { get { return mTextures; } set { mTextures = value; } }
		public String TextureName { get { return mTextureName; } set { mTextureName = value; } }
		public int Width { get { return mWidth; } set { mWidth = value; } }
		public int Height { get { return mHeight; } set { mHeight = value; } }
		public Vector2 Origin { get { return mOrigin; } }
		public int Rotation { get { return mRotation; } set { mRotation = value; } }
		[XmlIgnoreAttribute]
		public SpriteEffects Effect { get { return mEffekt; } set { mEffekt = value; } }
		[XmlIgnoreAttribute]
		public Color Tint { set { mTint = value; } }

		#endregion

        #region Constructor

		public Sprite() : base() { }

		public Sprite(String pTextureName)
		{
			mTextureName = pTextureName;
			LoadContent();
			mWidth = mTextures[0].Width;
			mHeight = mTextures[0].Height;
			mOrigin = new Vector2(mWidth / 2, mHeight / 2);
		}

        public Sprite(Vector2 pPosition, String pTextureName, String pPathName)
            : base(pPosition)
        {
            mTextureName = pTextureName;
			LoadContent();
            
            mWidth = mTextures[0].Width;
            mHeight = mTextures[0].Height;
            mOrigin = new Vector2(mWidth / 2, mHeight / 2);

            mCollisionBox = new Rectangle((int)pPosition.X, (int)pPosition.Y, mWidth, mHeight);
        }

        public Sprite(Vector2 pPosition, String pTextureName)
            : base(pPosition)
        {
            TextureName = pTextureName;
			LoadContent();
			mWidth = mTextures[0].Width;
			mHeight = mTextures[0].Height;
            mOrigin = new Vector2(mWidth / 2, mHeight / 2);

            mCollisionBox = new Rectangle((int)pPosition.X, (int)pPosition.Y, mWidth, mHeight);
        }

        #endregion

        #region Methods

		public override void Draw(TwoDRenderer renderer)
		{
			renderer.Draw(mTextures, new Vector3(Position, NormalZ));
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(mTextures[0], new Rectangle(PositionX + (int)mOrigin.X, PositionY + (int)mOrigin.Y, mWidth, mHeight), new Rectangle(0, 0, mWidth, mHeight), mTint, MathHelper.ToRadians(mRotation), mOrigin, mEffekt, 0.0f);
			if (EngineSettings.IsDebug)
				spriteBatch.Draw(mTextures[0], new Rectangle(PositionX, PositionY, mWidth, mHeight), mDebugColor);
		}

		public override void DrawNormal(SpriteBatch spriteBatch)
		{
			if (mTextures[1] == null) return;

			spriteBatch.Draw(mTextures[1], new Rectangle(PositionX + (int)mOrigin.X, PositionY + (int)mOrigin.Y, mWidth, mHeight), new Rectangle(0, 0, mWidth, mHeight), mTint, MathHelper.ToRadians(mRotation), mOrigin, mEffekt, 0.0f);
		}

		public override void DrawAO(SpriteBatch spriteBatch)
		{
			if (mTextures[2] == null) return;

			spriteBatch.Draw(mTextures[2], new Rectangle(PositionX + (int)mOrigin.X, PositionY + (int)mOrigin.Y, mWidth, mHeight), new Rectangle(0, 0, mWidth, mHeight), mTint, MathHelper.ToRadians(mRotation), mOrigin, mEffekt, 0.0f);
		}

		public override void DrawDepth(SpriteBatch spriteBatch)
		{
			if (mTextures[3] == null) return;

			spriteBatch.Draw(mTextures[3], new Rectangle(PositionX + (int)mOrigin.X, PositionY + (int)mOrigin.Y, mWidth, mHeight), new Rectangle(0, 0, mWidth, mHeight), mTint, MathHelper.ToRadians(mRotation), mOrigin, mEffekt, 0.0f);
		}

		public override void LoadContent()
		{
			mTextures = new Texture2D[4];
			mTextures[0] = TextureManager.Instance.GetElementByString(TextureName);
			mTextures[1] = TextureManager.Instance.GetElementByString(TextureName + "Normal");
			mTextures[2] = TextureManager.Instance.GetElementByString(TextureName + "AO");
			mTextures[3] = TextureManager.Instance.GetElementByString(TextureName + "Depth");
		}

		//direkt über Textures[index], ist dann überall einheitlich
		public Texture2D GetTexture(int index)
		{
			return mTextures[index];
		}

        #endregion
    }
}
