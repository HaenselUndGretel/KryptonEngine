using KryptonEngine.Manager;
using KryptonEngine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HanselAndGretel.Data
{
	public class Wolf : Enemy
	{
		#region Properties
        private Hansel mHansel;
        private Gretel mGretel;

        private Player mTargetPlayer;
		#endregion

		#region Getter & Setter
        public Hansel hansel { set { this.mHansel = value; } }
        public Gretel gretel { set { this.mGretel = value; } }
		#endregion

		#region Constructor

		public Wolf() { }

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

           

            // Überprüfen ob Spieler ohne licht im LichtRadius ist

            // sinus bewegnung zum Anviesierten Spieler


            // im Lichtradius abfall der Bewegungs geschwindigkeit
            // erreicht den inneren minimal abstand, rückzug in sinus zur flucht distanz



            base.Update();
        }

		#endregion

		#region Methods

		#endregion
	}
}
