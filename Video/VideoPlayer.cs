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
                 case "Ga":
                     tempVideo = KryptonEngine.EngineSettings.Content.Load<Video>("video\\ga");
                     break;
                 case "Fables":
                     tempVideo = KryptonEngine.EngineSettings.Content.Load<Video>("video\\fables");
                     break;
                 case "Outro":
                     tempVideo = KryptonEngine.EngineSettings.Content.Load<Video>("video\\outro");
                     break;
             }

            player.Play(tempVideo);
        }

        public static void Draw(SpriteBatch pSpritBatch)
        {
            pSpritBatch.Draw(player.GetTexture(),Vector2.Zero,Color.White);
        }
        

    }
}
