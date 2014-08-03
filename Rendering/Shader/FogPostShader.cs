using KryptonEngine.Manager;
using KryptonEngine.Rendering.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KryptonEngine.Rendering
{
    class FogPostShader
    {

        #region Properties

        private Effect mFogShader;
        private Effect mSimpleDrawShader;

        private float mTimer;
        private float mSpeed;
        private float mFogFactorMax;
        private float mFogFactorMin;

        public RenderTarget2D mFogTarget;

        private Texture2D mSceneDepthMap;
        private Texture2D mSceneDiffuseMap;
        private Texture2D mFogTexture;

        private GraphicsDevice mGrapicsDevice;
        #endregion

        #region Getter & Setter
        public float mMaxFogHeight { set { this.mFogFactorMax = value > 1.0 ? value / 720 : value; } }
        public float mMinFogHeight { set { this.mFogFactorMin = value > 1.0 ? value / 720 : value; } }
       
        #endregion


        #region Constructor
        public FogPostShader(ref Texture2D pSceneDiffuse, ref Texture2D pSceneDepth)
        {
            mGrapicsDevice = KryptonEngine.EngineSettings.Graphics.GraphicsDevice;

            this.mSceneDepthMap = pSceneDepth;
            this.mSceneDiffuseMap = pSceneDiffuse;

            mFogTarget = new RenderTarget2D(mGrapicsDevice, KryptonEngine.EngineSettings.VirtualResWidth, KryptonEngine.EngineSettings.VirtualResHeight, false, SurfaceFormat.Color, DepthFormat.None);
        }
        #endregion

        #region Methods
        public void LoadContent()
        {
            mFogShader = ShaderManager.Instance.GetElementByString("Fog");
            mSimpleDrawShader = ShaderManager.Instance.GetElementByString("SimpleDraw");

            
            mSpeed = 0.5f;
            mFogTexture = TextureManager.Instance.GetElementByString("clouds");
        }

        public void Render()
        {
            mTimer = (float)(KryptonEngine.EngineSettings.Time.TotalGameTime.TotalMilliseconds / 10000);

            mGrapicsDevice.SetRenderTarget(mFogTarget);
            mGrapicsDevice.Clear(Color.Transparent);

            mFogShader.Parameters["Timer"].SetValue(mTimer);
            mFogShader.Parameters["Speed"].SetValue(mSpeed);
            mFogShader.Parameters["FogFactorMin"].SetValue(mFogFactorMin);
            mFogShader.Parameters["FogFactorMax"].SetValue(mFogFactorMax);
            mFogShader.Parameters["FogStrength"].SetValue(0.3f);


            mGrapicsDevice.Textures[1] = mSceneDepthMap;
            mGrapicsDevice.Textures[2] = mFogTexture;
            mGrapicsDevice.Textures[3] = mSceneDiffuseMap;

            mFogShader.CurrentTechnique.Passes[0].Apply();
            QuadRenderer.Render(mGrapicsDevice);



            mGrapicsDevice.SetRenderTarget(null);
            mGrapicsDevice.Clear(Color.Black);


            mGrapicsDevice.Textures[1] = mFogTarget;
            mSimpleDrawShader.CurrentTechnique.Passes[0].Apply();
            QuadRenderer.Render(mGrapicsDevice);

        }
        #endregion

    }
}
