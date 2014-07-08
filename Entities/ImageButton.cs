using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KryptonEngine.Entities
{
	public class ImageButton : GameObject
	{
		#region Properties

		protected bool mIsSelected;
		protected Texture2D[] mTextures;
		protected Action mClickAction;
		#endregion

		#region Getter & Setter
		public bool IsSelected { get { return mIsSelected; } set { mIsSelected = value; } }
		#endregion

		#region Constructor
		public ImageButton() { }

		public ImageButton(String pTextureName, Vector2 pPosition, Action pAction)
			:base(pPosition)
		{
			mTextures = new Texture2D[2];
			mTextures[0] = TextureManager.Instance.GetElementByString(pTextureName + "_default");
			mTextures[1] = TextureManager.Instance.GetElementByString(pTextureName + "_hover");

			mClickAction = pAction;
		}
		#endregion

		#region Methods
		#endregion

		#region Override

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (mIsSelected)
				spriteBatch.Draw(mTextures[1], mPosition, Color.White);
			else
				spriteBatch.Draw(mTextures[0], mPosition, Color.White);
		}

		//Zum zeichnen der Menüs am Fels mit Kreide incl HudFading
		public void Draw(SpriteBatch spriteBatch, float pAlpha)
		{
			if (mIsSelected)
				spriteBatch.Draw(mTextures[1], mPosition, Color.White * pAlpha);
			else
				spriteBatch.Draw(mTextures[0], mPosition, Color.White * pAlpha);
		}

		public void IsClicked()
		{
			mClickAction();
		}
		#endregion
	}
}
