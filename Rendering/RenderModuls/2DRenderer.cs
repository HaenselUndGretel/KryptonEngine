using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KryptonEngine.Rendering.Components;
using KryptonEngine.Rendering.Core;
using KryptonEngine.HG_Data;
using HanselAndGretel.Data;
using System.IO;

namespace KryptonEngine.Rendering
{
    
    public class TwoDRenderer : BaseRenderer 
    {
        #region Properties

        private const int TL = 0;
        private const int TR = 1;
        private const int BL = 2;
        private const int BR = 3;

        float[] vertices = new float[8];
        int[] quadIndecies = { 0, 1, 2, 1, 3, 2 };

        private Batch mBatch;

        private GraphicsDevice mGraphicsDevice;

        private RasterizerState mRasterizerState;
        private BlendState mLightMapBlendState;
        private BlendState mAlphaBlend;
        private DepthStencilState mDepthStencilState;
        private DepthStencilState mNoDepthStencilState;
		private SamplerState mSamplerState;

        private Effect mDraw,mMRTDraw,mSingelDraw;
        private Effect mLightShader;
        private Effect mCombineShader;

        private RenderTarget2D mLightTarget;
        private RenderTarget2D mFinalTarget;

        Texture2D mTextureFinalTarget;

        private GBuffer mGBuffer;

        private Matrix mWorld;
        private Matrix mView;
        private Matrix mTranslatetViewMatrix;
        private Matrix mProjection;

        private bool isBegin = false;

        private Texture2D[] mTextureArray;

        private int mPlaneHeight;

        private float[] mSkeletonVertecies = new float[8];

        private FPSCounter mFPSCounter;


        private AmbientLight mAmbientLight;


        private FogPostShader mFogPost;

        #endregion

        #region Getter & Setter
        public int maxHeight { set { this.mPlaneHeight = value; } }
        public AmbientLight AmbientLight { get { return this.mAmbientLight; } set { mAmbientLight = value; } }
        #endregion

        #region Constructor

        public TwoDRenderer(GraphicsDevice pGraphicsDevice)
        {
            this.mGraphicsDevice = pGraphicsDevice;
            Initialize(this.mGraphicsDevice.Viewport.Width, this.mGraphicsDevice.Viewport.Height);
        }

        public TwoDRenderer(GraphicsDevice pGraphicsDevice, int pWidth, int pHeight)
        {
            this.mGraphicsDevice = pGraphicsDevice;
            Initialize(pWidth, pHeight);
        }
        
        #endregion

        #region Class Methods

        public void Initialize(int pWidth, int pHeight)
        {
            this.mFPSCounter = new FPSCounter();

            this.mRasterizerState = new RasterizerState();
            this.mRasterizerState.CullMode = CullMode.None;

            this.mLightMapBlendState = new BlendState();
            this.mLightMapBlendState.ColorSourceBlend = Blend.One;
            this.mLightMapBlendState.ColorDestinationBlend = Blend.One;
            this.mLightMapBlendState.ColorBlendFunction = BlendFunction.Add;
            this.mLightMapBlendState.AlphaSourceBlend = Blend.One;
            this.mLightMapBlendState.AlphaDestinationBlend = Blend.One;
            this.mLightMapBlendState.AlphaBlendFunction = BlendFunction.Add;

			this.mSamplerState = SamplerState.LinearClamp;

            this.mAlphaBlend = BlendState.AlphaBlend;

            this.mDepthStencilState = new DepthStencilState();
            this.mDepthStencilState.DepthBufferWriteEnable = true;
            this.mDepthStencilState.DepthBufferEnable = true;
            this.mDepthStencilState.DepthBufferFunction = CompareFunction.GreaterEqual;

            this.mNoDepthStencilState = new DepthStencilState();
            this.mNoDepthStencilState.DepthBufferWriteEnable = false;
            this.mNoDepthStencilState.DepthBufferEnable = false;
            this.mNoDepthStencilState.DepthBufferFunction = CompareFunction.GreaterEqual;

           // this.mAlphaBlend = new BlendState();

            //this.mAlphaBlend.ColorDestinationBlend = Blend.InverseSourceAlpha;
            //this.mAlphaBlend.ColorSourceBlend = Blend.SourceAlpha;

            //this.mAlphaBlend.AlphaSourceBlend = Blend.Zero;
            //this.mAlphaBlend.AlphaDestinationBlend = Blend.Zero;

            //this.mAlphaBlend.ColorBlendFunction = BlendFunction.Add;
            //this.mAlphaBlend.AlphaBlendFunction = BlendFunction.Add;

            this.mLightTarget = new RenderTarget2D(this.mGraphicsDevice, pWidth, pHeight, false, SurfaceFormat.Color, DepthFormat.None);
            this.mFinalTarget = new RenderTarget2D(this.mGraphicsDevice, pWidth, pHeight, false, SurfaceFormat.Color, DepthFormat.None);
           
            this.mView  = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
            this.mTranslatetViewMatrix = this.mView;

            this.mWorld = Matrix.Identity;

            this.mGBuffer = new GBuffer(this.mGraphicsDevice);

            this.mTextureArray = new Texture2D[4];
            this.mBatch = new Batch(this.mGraphicsDevice);

            mTextureFinalTarget = (Texture2D)mFinalTarget;

            this.mFogPost = new FogPostShader( ref mTextureFinalTarget, ref mGBuffer.RenderTargets[3]);
        }

        public void UpdateFPSCounter()
        {
            this.mFPSCounter.Update();
        }

        public void UpdateFPSDrawCounter()
        {
            this.mFPSCounter.UpdateDrawCounter();
        }

        public void LoadContent()
        {
            this.mLightShader = KryptonEngine.Manager.ShaderManager.Instance.GetElementByString("Light");
            this.mCombineShader = KryptonEngine.Manager.ShaderManager.Instance.GetElementByString("CombineShader");
            this.mSingelDraw = KryptonEngine.Manager.ShaderManager.Instance.GetElementByString("Singel");
            this.mMRTDraw = KryptonEngine.Manager.ShaderManager.Instance.GetElementByString("MRT");
       
            this.mGBuffer.LoadContent();
            this.mFogPost.LoadContent();
        }
        #endregion

        #region Render Methods

        #region Begin
        public void Begin()
        {
            this.Begin(Matrix.Identity);
        }

        public void Begin( Matrix pTranslation)
        {
            if (this.isBegin) throw new InvalidOperationException("End must called before Beginn");

            // Setzt States für die Grafikkarte
            //  Rasterizer Statet Culled die Vertecies
            // Blendstate ist für Alphatesting
            // DepthStenciel für draw reihen folge

            this.mGraphicsDevice.RasterizerState = this.mRasterizerState;
            this.mGraphicsDevice.BlendState = this.mAlphaBlend;
            this.mGraphicsDevice.DepthStencilState = this.mDepthStencilState;
			this.mGraphicsDevice.SamplerStates[0] = this.mSamplerState;

            this.mTranslatetViewMatrix = Matrix.Multiply(mView, pTranslation);
            this.mProjection = Matrix.CreateOrthographicOffCenter(0, this.mGraphicsDevice.Viewport.Width, this.mGraphicsDevice.Viewport.Height, 0, 1f, 0f);

            this.isBegin = true;
        }

        #endregion

        #region Draw

        #region Draw Skelleton

        public void Draw(Skeleton pSkeleton,Texture2D[] pMapArray,float pDepth = 1.0f,float pScale = 1.0f)
        {
            this.mTextureArray[0] = pMapArray[0];
            this.mTextureArray[1] = pMapArray[1];
            this.mTextureArray[2] = pMapArray[2];
            this.mTextureArray[3] = pMapArray[3];

            this.InternalDraw(pSkeleton, this.mTextureArray, pDepth,pScale, Color.White);
        }

        public void Draw(Skeleton pSkeleton, Texture2D[] pMapArray, Color pColor, float pDepth = 1.0f, float pScale = 1.0f)
        {
            this.mTextureArray[0] = pMapArray[0];
            this.mTextureArray[1] = pMapArray[1];
            this.mTextureArray[2] = pMapArray[2];
            this.mTextureArray[3] = pMapArray[3];

            this.InternalDraw(pSkeleton, this.mTextureArray, pDepth, pScale, pColor);
        }

        #endregion

        #region Draw Sprite


        public void Draw(Texture2D[] pMapArray, Vector2 pPosition,float pScale = 1.0f)
        {
            Vector4 vector = default(Vector4);
            vector.X = pPosition.X;
            vector.Y = pPosition.Y;
            vector.Z = pPosition.Y / this.mPlaneHeight;
            vector.W = pScale;

			this.mTextureArray[0] = pMapArray[0];
			this.mTextureArray[1] = pMapArray[1];
			this.mTextureArray[2] = pMapArray[2];
			this.mTextureArray[3] = pMapArray[3];

            Rectangle sourceRectangel = Rectangle.Empty;

            this.InternalDraw(this.mTextureArray, vector, ref sourceRectangel,Color.White);
        }


        public void Draw(Texture2D[] pMapArray, Vector3 pPosition,float pScale = 1.0f)
        {
            Vector4 vector = default(Vector4);
            vector.X = pPosition.X;
            vector.Y = pPosition.Y;
            vector.Z = pPosition.Z;
            vector.W = pScale;

			this.mTextureArray[0] = pMapArray[0];
			this.mTextureArray[1] = pMapArray[1];
			this.mTextureArray[2] = pMapArray[2];
			this.mTextureArray[3] = pMapArray[3];

            Rectangle sourceRectangel = new Rectangle(0, 0, pMapArray[0].Width, pMapArray[0].Height);

            this.InternalDraw(this.mTextureArray, vector,ref sourceRectangel,Color.White);
        }

                public void Draw(Texture2D[] pMapArray, Vector3 pPosition, Color pColor,float pScale = 1.0f)
        {
            Vector4 vector = default(Vector4);
            vector.X = pPosition.X;
            vector.Y = pPosition.Y;
            vector.Z = pPosition.Z;
            vector.W = pScale;

			this.mTextureArray[0] = pMapArray[0];
			this.mTextureArray[1] = pMapArray[1];
			this.mTextureArray[2] = pMapArray[2];
			this.mTextureArray[3] = pMapArray[3];

            Rectangle sourceRectangel = new Rectangle(0, 0, pMapArray[0].Width, pMapArray[0].Height);

            this.InternalDraw(this.mTextureArray, vector,ref sourceRectangel,pColor);
        }

        public void Draw(Texture2D[] pMapArray, Vector3 pPosition,Rectangle sourceRectangel, float pScale = 1.0f)
        {
            Vector4 vector = default(Vector4);
            vector.X = pPosition.X;
            vector.Y = pPosition.Y;
            vector.Z = pPosition.Z;
            vector.W = pScale;

            

            this.mTextureArray[0] = pMapArray[0];
            this.mTextureArray[1] = pMapArray[1];
            this.mTextureArray[2] = pMapArray[2];
            this.mTextureArray[3] = pMapArray[3];

            this.InternalDraw(this.mTextureArray, vector, ref sourceRectangel,Color.White);
        }

                public void Draw(Texture2D[] pMapArray, Vector3 pPosition,Rectangle sourceRectangel, Color pColor,float pScale = 1.0f)
        {
            Vector4 vector = default(Vector4);
            vector.X = pPosition.X;
            vector.Y = pPosition.Y;
            vector.Z = pPosition.Z;
            vector.W = pScale;

            

            this.mTextureArray[0] = pMapArray[0];
            this.mTextureArray[1] = pMapArray[1];
            this.mTextureArray[2] = pMapArray[2];
            this.mTextureArray[3] = pMapArray[3];

            this.InternalDraw(this.mTextureArray, vector, ref sourceRectangel,pColor);
        }


        #endregion


        #region InternalDraw
        private void InternalDraw(Texture2D[] pTextureArray, Vector4 pDestination, ref Rectangle sourceRectangel, Color pColor)
        {
            if (!isBegin) throw new Exception("Beginn muss vor Draw aufgerufen werden!");
            MeshData item = this.mBatch.NextItem(4,6);

            float scale = pDestination.W;
            int rectWidth = (int)(sourceRectangel.Width );
            int rectHeight = (int)(sourceRectangel.Height );

            int texWidth = (int)(pTextureArray[0].Width * scale);
            int texHeigt = (int)(pTextureArray[0].Height * scale);

            float texelWidth = 1f / texWidth;
            float texelHeight = 1f / texHeigt;


            item.TextureID = this.mBatch.AddTextures(pTextureArray);
            item.triangles = quadIndecies;


            item.vertices[TL].Position.X = pDestination.X;
            item.vertices[TL].Position.Y = pDestination.Y;
            item.vertices[TL].Position.Z = pDestination.Z;
            item.vertices[TR].Position.X = pDestination.X + rectWidth * scale;
            item.vertices[TR].Position.Y = pDestination.Y;
            item.vertices[TR].Position.Z = pDestination.Z;
            item.vertices[BL].Position.X = pDestination.X;
            item.vertices[BL].Position.Y = pDestination.Y + rectHeight * scale;
            item.vertices[BL].Position.Z = pDestination.Z;
            item.vertices[BR].Position.X = pDestination.X + rectWidth * scale;
            item.vertices[BR].Position.Y = pDestination.Y + rectHeight * scale;
            item.vertices[BR].Position.Z = pDestination.Z;


            item.vertices[TL].TextureCoordinate.X = sourceRectangel.Location.X * texelWidth;
            item.vertices[TL].TextureCoordinate.Y = sourceRectangel.Location.Y * texelHeight;
            item.vertices[TR].TextureCoordinate.X = (sourceRectangel.Location.X * texelWidth) + (rectWidth * texelWidth);
            item.vertices[TR].TextureCoordinate.Y = sourceRectangel.Location.Y * texelHeight;
            item.vertices[BL].TextureCoordinate.X = sourceRectangel.Location.X * texelWidth;
            item.vertices[BL].TextureCoordinate.Y = (sourceRectangel.Location.Y * texelHeight) + (rectHeight * texelHeight);
            item.vertices[BR].TextureCoordinate.X = (sourceRectangel.Location.X * texelWidth) + (rectWidth * texelWidth);
            item.vertices[BR].TextureCoordinate.Y = (sourceRectangel.Location.Y * texelHeight) + (rectHeight * texelHeight);

            item.vertices[TL].Color = pColor;
            item.vertices[BL].Color = pColor;
            item.vertices[BR].Color = pColor;
            item.vertices[TR].Color = pColor;
            //if ( textId == -1) 
            //{
            //   textId = this.mBatch.AddTextures(pTextureArray);
            //}

            //item.TextureID = textId;

        }

        private void InternalDraw(Skeleton pSkeleton,Texture2D[] pTextureArray,float pDepth ,float scale, Color pColor)
        {
            if (!isBegin) throw new Exception("Beginn muss vor Draw aufgerufen werden!");
            
            float[] vertices = this.mSkeletonVertecies;
            List<Slot> drawOrder = pSkeleton.DrawOrder;
            float x = pSkeleton.X, y = pSkeleton.Y;
			int textId =  this.mBatch.AddTextures(pTextureArray);
           
            for (int i = 0, n = drawOrder.Count; i < n; i++)
            {
                Slot slot = drawOrder[i];
                Attachment attachment = slot.Attachment;

                if (attachment is RegionAttachment)
                {

                    RegionAttachment regionAttachment = (RegionAttachment)attachment;

                    MeshData item = this.mBatch.NextItem(4, 6);

                    item.triangles = quadIndecies;

                    AtlasRegion region = (AtlasRegion)regionAttachment.RendererObject;                
                    regionAttachment.ComputeWorldVertices(x, y, slot.Bone, vertices);

                    
 
                    item.vertices[TL].Position.X = vertices[RegionAttachment.X1];
                    item.vertices[TL].Position.Y = vertices[RegionAttachment.Y1];
					item.vertices[TL].Position.Z = pDepth;// -orderDepth * (n - (i + 1));
                    item.vertices[BL].Position.X = vertices[RegionAttachment.X2];
                    item.vertices[BL].Position.Y = vertices[RegionAttachment.Y2];
                    item.vertices[BL].Position.Z = pDepth;// -orderDepth * (n - (i + 1));
                    item.vertices[BR].Position.X = vertices[RegionAttachment.X3];
                    item.vertices[BR].Position.Y = vertices[RegionAttachment.Y3];
                    item.vertices[BR].Position.Z = pDepth;// -orderDepth * (n - (i + 1));
                    item.vertices[TR].Position.X = vertices[RegionAttachment.X4];
                    item.vertices[TR].Position.Y = vertices[RegionAttachment.Y4];
                    item.vertices[TR].Position.Z = pDepth;// -orderDepth * (n - (i + 1));

                    float[] uvs = regionAttachment.UVs;
                    item.vertices[TL].TextureCoordinate.X = uvs[RegionAttachment.X1];
                    item.vertices[TL].TextureCoordinate.Y = uvs[RegionAttachment.Y1];
                    item.vertices[BL].TextureCoordinate.X = uvs[RegionAttachment.X2];
                    item.vertices[BL].TextureCoordinate.Y = uvs[RegionAttachment.Y2];
                    item.vertices[BR].TextureCoordinate.X = uvs[RegionAttachment.X3];
                    item.vertices[BR].TextureCoordinate.Y = uvs[RegionAttachment.Y3];
                    item.vertices[TR].TextureCoordinate.X = uvs[RegionAttachment.X4];
                    item.vertices[TR].TextureCoordinate.Y = uvs[RegionAttachment.Y4];

                    item.vertices[TL].Color = pColor;
                    item.vertices[BL].Color = pColor;
                    item.vertices[BR].Color = pColor;
                    item.vertices[TR].Color = pColor;

                    item.TextureID = textId;
                }
                else if(attachment is MeshAttachment)
                {
                    MeshAttachment mesh = (MeshAttachment)attachment;

                    int vertexCount = mesh.Vertices.Length;
                    if (vertices.Length < vertexCount) vertices = new float[vertexCount];

                    mesh.ComputeWorldVertices(x, y, slot, vertices);

                    int[] triangles = mesh.triangles;
                    MeshData item = mBatch.NextItem(vertexCount, triangles.Length);

                    item.triangles = triangles;
                    item.TextureID = textId;

                    AtlasRegion region = (AtlasRegion)mesh.RendererObject;

                    float[] uvs = mesh.UVs;
                    VertexPositionColorTexture[] itemVertices = item.vertices;

                    for (int ii = 0, v = 0; v < vertexCount; ii++, v += 2)
                    {
                        itemVertices[ii].Position.X = vertices[v];
                        itemVertices[ii].Position.Y = vertices[v + 1];
                        itemVertices[ii].Position.Z = pDepth;
                        itemVertices[ii].TextureCoordinate.X = uvs[v];
                        itemVertices[ii].TextureCoordinate.Y = uvs[v + 1];
                    }

                }
                else if(attachment is SkinnedMeshAttachment)
                {
                    SkinnedMeshAttachment mesh = (SkinnedMeshAttachment)attachment;
                    int vertexCount = mesh.UVs.Length;
                    if (vertices.Length < vertexCount) vertices = new float[vertexCount];
                    mesh.ComputeWorldVertices(x, y, slot, vertices);

                    int[] triangles = mesh.Triangles;
                    MeshData item = mBatch.NextItem(vertexCount, triangles.Length);
                    item.triangles = triangles;
                    item.TextureID = textId;


                    float[] uvs = mesh.UVs;
                    VertexPositionColorTexture[] itemVertices = item.vertices;
                    for (int ii = 0, v = 0; v < vertexCount; ii++, v += 2)
                    {
                        
                        itemVertices[ii].Position.X = vertices[v];
                        itemVertices[ii].Position.Y = vertices[v + 1];
                        itemVertices[ii].Position.Z = 0;
                        itemVertices[ii].TextureCoordinate.X = uvs[v];
                        itemVertices[ii].TextureCoordinate.Y = uvs[v + 1];
                    }

                }

            }
        }
        #endregion

        #endregion

        #region End
        public void End()
        {
            if (!this.isBegin) throw new InvalidOperationException("Beginn must called before End");

            if (this.mGBuffer.IsGBufferActive) this.mDraw = this.mMRTDraw;
            else { this.mDraw = this.mSingelDraw; this.mGraphicsDevice.DepthStencilState = this.mNoDepthStencilState; }

            this.mDraw.Parameters["World"].SetValue(this.mWorld);
            this.mDraw.Parameters["View"].SetValue(this.mTranslatetViewMatrix);
            this.mDraw.Parameters["Projection"].SetValue(this.mProjection);
            this.mDraw.CurrentTechnique.Passes[0].Apply();

            this.mBatch.Render();
            this.isBegin = false;
        }
        #endregion
        
        #endregion

        #region Light Methods
        public void ProcessLight(List<Light> pLightList, Matrix pTranslation)
        {

            EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(mLightTarget);
            EngineSettings.Graphics.GraphicsDevice.Clear(Color.Transparent);

            EngineSettings.Graphics.GraphicsDevice.BlendState = mLightMapBlendState;

            KryptonEngine.EngineSettings.Graphics.GraphicsDevice.Textures[0] = mGBuffer.RenderTargets[1];
            KryptonEngine.EngineSettings.Graphics.GraphicsDevice.Textures[1] = mGBuffer.RenderTargets[3];

            this.mTranslatetViewMatrix = Matrix.Multiply(mView, pTranslation);

            //this.mLightShader.Parameters["World"].SetValue(this.mWorld);
            this.mLightShader.Parameters["View"].SetValue(pTranslation);
            //this.mLightShader.Parameters["Projection"].SetValue(this.mProjection);


            foreach (Light l in pLightList)
            {
                if (!l.IsVisible) continue;
			   Vector4 lightPos = new Vector4(l.Position, l.Depth * 720, 1f);

			   this.mLightShader.Parameters["View"].SetValue(pTranslation);
               this.mLightShader.Parameters["LightIntensity"].SetValue(l.Intensity);
               this.mLightShader.Parameters["LightColor"].SetValue(l.LightColor);
               this.mLightShader.Parameters["LightPosition"].SetValue(lightPos);
               this.mLightShader.Parameters["screen"].SetValue(new Vector2(EngineSettings.VirtualResWidth, EngineSettings.VirtualResHeight));

               if (l.GetType() == typeof(PointLight))
               {
                   PointLight tempPl = (PointLight)l;

                   mLightShader.Parameters["LightRadius"].SetValue(tempPl.Radius);
                   mLightShader.CurrentTechnique.Passes[0].Apply();
               }
                //directional Light!

               QuadRenderer.Render(this.mGraphicsDevice);
            }

            EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(null);
        }
       
        public void ProcessFinalScene()
        {
            EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(mFinalTarget);
            EngineSettings.Graphics.GraphicsDevice.Clear(Color.Transparent);


            EngineSettings.Graphics.GraphicsDevice.Textures[0] = this.mGBuffer.RenderTargets[0];
            EngineSettings.Graphics.GraphicsDevice.Textures[1] = mLightTarget;
            EngineSettings.Graphics.GraphicsDevice.Textures[2] = this.mGBuffer.RenderTargets[2];
            

            this.mCombineShader.Parameters["ambientColor"].SetValue(AmbientLight.LightColor);
            this.mCombineShader.Parameters["ambientIntensity"].SetValue(AmbientLight.Intensity);

            mCombineShader.CurrentTechnique.Passes[0].Apply();

			QuadRenderer.Render(this.mGraphicsDevice);
			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(null);
        }
        #endregion

        #region Function Methods

        public void SetGBuffer()
        {
            this.mGBuffer.SetGBuffer();
        }

        public void DisposeGBuffer()
        {
            this.mGBuffer.DisposeGBuffer();
            KryptonEngine.EngineSettings.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.None;
        }

        public void ClearGBuffer()
        {
            this.mGBuffer.Clear();
        }

		public Texture2D GetRenderTargetTexture(int index)
		{
			return mGBuffer.RenderTargets[index];
		}

        public void SaveRenderTargetToTexture(int index)
        {
            FileStream file = new FileStream("target"+1,FileMode.CreateNew);
            this.mGBuffer.RenderTargets[index].SaveAsPng(file, this.mGBuffer.RenderTargets[index].Width, this.mGBuffer.RenderTargets[index].Height);

        }
        #endregion

        #region PostEffects

        public void ApplyFog(Matrix pTranslation)
        {
            this.mTranslatetViewMatrix = Matrix.Multiply(mView, pTranslation);
            this.mFogPost.Render(pTranslation);
        }

        public void SetMaxFogHeight(float pMaxHeight)
        {
            this.mFogPost.mMaxFogHeight = pMaxHeight;
        }

        public void SetMinFogHeight(float pMinHeight)
        {
            this.mFogPost.mMinFogHeight = pMinHeight;
        }

        #endregion

        #region Debug
        public void DrawDebugRendertargets(SpriteBatch batch)
        {
            int width = mGraphicsDevice.Viewport.Width;
            int height = mGraphicsDevice.Viewport.Height;
            int smallWidth  = width / 4;
            int smallHeigth = height / 4;

            batch.Begin(SpriteSortMode.Texture, BlendState.Opaque, SamplerState.PointClamp,DepthStencilState.None,RasterizerState.CullNone);
            batch.Draw(this.mGBuffer.RenderTargets[0], new Rectangle(smallWidth * 0, height - smallHeigth, smallWidth, smallHeigth), Color.White);
            batch.Draw(this.mGBuffer.RenderTargets[1], new Rectangle(smallWidth * 1, height - smallHeigth, smallWidth, smallHeigth), Color.White);
            batch.Draw(this.mGBuffer.RenderTargets[2], new Rectangle(smallWidth * 2, height - smallHeigth, smallWidth, smallHeigth), Color.White);
            batch.Draw(this.mGBuffer.RenderTargets[3], new Rectangle(smallWidth * 3, height - smallHeigth, smallWidth, smallHeigth), Color.White);

            batch.DrawString(KryptonEngine.Manager.FontManager.Instance.GetElementByString("font"),this.mFPSCounter.FPS.ToString() + " FPS", Vector2.Zero, Color.White);
            batch.DrawString(KryptonEngine.Manager.FontManager.Instance.GetElementByString("font"), this.mBatch.mDiffuseTextureBuffer.Count.ToString() + " Textures", new Vector2(0,20), Color.White);
            batch.DrawString(KryptonEngine.Manager.FontManager.Instance.GetElementByString("font"), this.mBatch.mFreeItems.Count.ToString() + " Free Items", new Vector2(0, 40), Color.White);
            
            batch.End();
        }
        
        public void DrawRenderTargetOnScreen(SpriteBatch batch,int index)
        {
            batch.Begin();
            batch.Draw(this.mGBuffer.RenderTargets[index], Vector2.Zero, Color.White);
            batch.End();
        }

        public void DrawLightTargettOnScreen(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(mLightTarget, Vector2.Zero, Color.White);
            batch.End();
        }


        public void DrawFinalTargettOnScreen(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(mFinalTarget, Vector2.Zero, Color.White);
            batch.End();
        }

        #endregion
    }
}
