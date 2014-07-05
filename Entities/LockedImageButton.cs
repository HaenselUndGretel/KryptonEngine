using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KryptonEngine.Entities
{
	public class LockedImageButton : ImageButton
	{
		#region Properties

		protected bool mIsUnlocked;
		#endregion

		#region Getter & Setter
		public bool IsUnlocked { get { return mIsUnlocked; } set { mIsUnlocked = value; } }
		#endregion

		#region Constructor
		public LockedImageButton(String pTextureName, Vector2 pPosition, Action pAction)
		{
			mPosition = pPosition;

			mTextures = new Texture2D[3];
			mTextures[0] = TextureManager.Instance.GetElementByString(pTextureName + "_default");
			mTextures[1] = TextureManager.Instance.GetElementByString(pTextureName + "_hover");
			mTextures[2] = TextureManager.Instance.GetElementByString(pTextureName + "_locked");

			mClickAction = pAction;
		}
		#endregion

		#region Methods
		#endregion

		#region Override

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!mIsUnlocked)
				spriteBatch.Draw(mTextures[2], mPosition, Color.White);
			else if (mIsSelected)
				spriteBatch.Draw(mTextures[1], mPosition, Color.White);
			else
				spriteBatch.Draw(mTextures[0], mPosition, Color.White);
		}

		public void IsClicked()
		{
			mClickAction();
		}
		#endregion
	}
}
