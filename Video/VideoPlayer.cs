using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;

namespace KryptonEngine
{
   public static class CutScenePlayer
    {
        static Video Ga = null;
        static Video Fables = null;
        static Video Outro = null;

        public static VideoPlayer player;

        static public void Play(string scene)
        {
             Video tempVideo = null;
             player = new VideoPlayer();
             switch (scene)
             {
                 case "Intro":
                     tempVideo = KryptonEngine.EngineSettings.Content.Load<Video>("video\\Intro");
                     break;
                 case "Outro":
                     tempVideo = KryptonEngine.EngineSettings.Content.Load<Video>("video\\Outro");
                     break;
             }

            player.Play(tempVideo);
        }

        public static void Draw(SpriteBatch pSpritBatch)
        {



            pSpritBatch.Draw(player.GetTexture(), new Rectangle(0,0,1280,720),null, Color.White);
        }
        

    }
}
