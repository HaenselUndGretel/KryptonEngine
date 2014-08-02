using KryptonEngine.Rendering.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KryptonEngine.Rendering.Shader
{
    class Hatching
    {
        #region Properties

        Effect HatchingShader;

        Texture2D[] mHatchingTextures;

        Texture2D mLightMap;

        RenderTarget2D mHatchingTarget;

        private Vector2 mResolution;

        #endregion


        #region Constructor

        public Hatching(Texture2D pLightMap)
        {
            mHatchingTarget = new RenderTarget2D(KryptonEngine.EngineSettings.Graphics.GraphicsDevice, KryptonEngine.EngineSettings.VirtualResWidth, KryptonEngine.EngineSettings.VirtualResHeight, false, SurfaceFormat.Color, DepthFormat.None);
        }

        #endregion

        #region Methods
        public void LoadContent()
        {

        }

        public void Render()
        {
            KryptonEngine.EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(mHatchingTarget);
            KryptonEngine.EngineSettings.Graphics.GraphicsDevice.Clear(Color.Transparent);

            HatchingShader.Parameters["repeat"].SetValue(mResolution);

            KryptonEngine.EngineSettings.Graphics.GraphicsDevice.Textures[1] = mLightMap;

            HatchingShader.CurrentTechnique.Passes[0].Apply();
            QuadRenderer.Render(KryptonEngine.EngineSettings.Graphics.GraphicsDevice);

        }
        #endregion

    }
}
