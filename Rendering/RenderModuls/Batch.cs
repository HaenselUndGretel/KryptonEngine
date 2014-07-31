using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KryptonEngine.Rendering.Components;

namespace KryptonEngine.Rendering
{
    class Batch
    {

        #region Properties

        public int TextCount;
        
        private GraphicsDevice mGraphicsDevice;

        private int mCurrentTextureID;

        public List<Texture2D> mDiffuseTextureBuffer;
        public List<Texture2D> mNormalTextureBuffer;
        public List<Texture2D> mAoTextureBuffer;
        public List<Texture2D> mDepthTextureBuffer;

        public List<MeshData> mBatchItems;
        public Queue<MeshData> mFreeItems;


        public VertexPositionTexture[] mVertexBuffer = {};
        public int[] mIndexBuffer = {};

        #endregion

        #region Constructor

        public Batch(GraphicsDevice pGraphicsDevice)
        {
            this.mGraphicsDevice = pGraphicsDevice;

           

            this.mBatchItems = new List<MeshData>();
            this.mFreeItems = new Queue<MeshData>();

            this.mDiffuseTextureBuffer = new List<Texture2D>();
            this.mNormalTextureBuffer = new List<Texture2D>();
            this.mAoTextureBuffer = new List<Texture2D>();
            this.mDepthTextureBuffer = new List<Texture2D>();
        }

        #endregion

        #region Render Methods

        public void Render()
        {
            Texture2D testTexture = null;
            if (mBatchItems.Count == 0)
                return;

            int batchCount = this.mBatchItems.Count;
            int vertexCount = 0;
            int triangleCount = 0;

            for (int i = 0; i < batchCount; i++ )
            {
                MeshData item = mBatchItems[i];
                vertexCount += item.vertexCount;
                triangleCount += item.triangleCount;
            }
            EnsureCapaicty(vertexCount, triangleCount);

            vertexCount = 0;
            triangleCount = 0;

            for (int i = 0; i < batchCount; i++ )
            {
                MeshData item = mBatchItems[i];
                int itemVertexCount = item.vertexCount;
                int currentTextureId = 0;

                if(!ReferenceEquals(mDiffuseTextureBuffer[item.TextureID], testTexture) || vertexCount + itemVertexCount > int.MaxValue)
                {
                    this.Flush(vertexCount, triangleCount);
                    vertexCount = 0;
                    triangleCount = 0;
                    testTexture = mDiffuseTextureBuffer[item.TextureID];

                    mGraphicsDevice.Textures[1] = mDiffuseTextureBuffer[item.TextureID];
                    mGraphicsDevice.Textures[2] = mNormalTextureBuffer[item.TextureID];
                    mGraphicsDevice.Textures[3] = mAoTextureBuffer[item.TextureID];
                    mGraphicsDevice.Textures[4] = mDepthTextureBuffer[item.TextureID];
                }

                int[] itemTriangles = item.triangles;
                int itemTriangleCount = item.triangleCount;

                for(int ii = 0, t = triangleCount; ii< itemTriangleCount; ii++, t++)
                {
                    mIndexBuffer[t] = itemTriangles[ii] + vertexCount;
                }
                triangleCount += itemTriangleCount;

                Array.Copy(item.vertices, 0, mVertexBuffer,vertexCount ,itemVertexCount);
                vertexCount += itemVertexCount;

                mFreeItems.Enqueue(item);
            }
            Flush(vertexCount, triangleCount);
            mBatchItems.Clear();
            this.clearTextures();

            //    while (batchCount > 0)
            //    {
            //        short startIndex = 0;
            //        short currentIndex = 0;
            //        int offset = 0;
                    

            //        int batchesToProcess = batchCount;


            //        for (int i = 0; i < batchesToProcess; i++)
            //        {
            //            SpriteData item = mBatchItems[i];

                       

            //            if (!ReferenceEquals(mDiffuseTextureBuffer[item.TextureID], testTexture))
            //            {
            //                if (i > offset)
            //                {
            //                    this.Flush(currentTextureId, offset, i - offset);
            //                }
            //                offset = i;
            //                currentTextureId = item.TextureID;
            //                currentIndex = 0;
            //                testTexture = mDiffuseTextureBuffer[item.TextureID];
            //            }

            //            this.mVertexBuffer[currentIndex++] = item.vertexTL;
            //            this.mVertexBuffer[currentIndex++] = item.vertexTR;
            //            this.mVertexBuffer[currentIndex++] = item.vertexBL;
            //            this.mVertexBuffer[currentIndex++] = item.vertexBR;

            //            this.mFreeItems.Enqueue(item);
            //        }

            //        Flush(currentTextureId, offset, batchesToProcess - offset);
            //        batchCount -= batchesToProcess;
            //    }

            //this.TextCount = mDiffuseTextureBuffer.Count;

            //mBatchItems.Clear();
            //this.clearTextures();
        }

        public void Flush(int vertexCount, int triangleCount)
        {
          // int vertexCount = count*4;
          // int indexCount  = count*2;

          // //int vertexOffset = offset * 4;
          //// int indexOffset  = offset * 6;

           
          // mGraphicsDevice.Textures[1] = mDiffuseTextureBuffer[TextureID];
          // mGraphicsDevice.Textures[2] = mNormalTextureBuffer[TextureID];
          // mGraphicsDevice.Textures[3] = mAoTextureBuffer[TextureID];
          // mGraphicsDevice.Textures[4] = mDepthTextureBuffer[TextureID];

          // this.mGraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
          //                                                this.mVertexBuffer, 0, vertexCount,
          //                                                this.mIndexBuffer, 0, indexCount,
          //                                                VertexPositionTexture.VertexDeclaration);

            if (vertexCount == 0) return;
            this.mGraphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList,
                mVertexBuffer, 0, vertexCount,
                mIndexBuffer, 0, triangleCount / 3,
                VertexPositionTexture.VertexDeclaration);

        }
        #endregion

        #region Methods

        //public SpriteData createBatchItem()
        //{
        //    SpriteData item;

        //    if (mFreeItems.Count > 0)
        //        item = mFreeItems.Dequeue();
        //    else
        //        item = new SpriteData();
        //    this.mBatchItems.Add(item);
        //    return item;

        //}

        public MeshData NextItem(int pVertexCount, int pIndexCount)
        {
            MeshData item = mFreeItems.Count > 0 ? mFreeItems.Dequeue() : new MeshData();
            if (item.vertices.Length < pVertexCount) item.vertices = new VertexPositionTexture[pVertexCount];
            if (item.triangles.Length < pIndexCount) item.triangles = new int[pIndexCount];

            item.vertexCount = pVertexCount;
            item.triangleCount = pIndexCount;
            mBatchItems.Add(item);
            return item;
        }



        public int getTextureIndex(Texture2D pTexture)
        {
            return mDiffuseTextureBuffer.IndexOf(pTexture);
        }

        public int AddTextures(Texture2D[] pTextureArray)
        {
            this.mDiffuseTextureBuffer.Add(pTextureArray[0]);
            this.mNormalTextureBuffer.Add(pTextureArray[1]);
            this.mAoTextureBuffer.Add(pTextureArray[2]);
            this.mDepthTextureBuffer.Add(pTextureArray[3]);

            return this.mDiffuseTextureBuffer.Count - 1;
        }

        public void clearTextures()
        {
            this.mDiffuseTextureBuffer.Clear();
            this.mNormalTextureBuffer.Clear();
            this.mAoTextureBuffer.Clear();
            this.mDepthTextureBuffer.Clear();
        }

        private void EnsureCapaicty(int pVertexCount, int pIndexCount)
        {
            if (mVertexBuffer.Length < pVertexCount) mVertexBuffer = new VertexPositionTexture[pVertexCount];
            if (mIndexBuffer.Length < pIndexCount) mIndexBuffer = new int[pIndexCount];
        }

        //private void EnsureIndexArraySize(int itemAmount)
        //{

        //    int[] newIndex = new int[6 * (itemAmount)];
        //    int start = 0;

        //    //if (mIndexBuffer != null)
        //    //{
        //    //    mIndexBuffer.CopyTo(newIndex, 0);
        //    //    start = mIndexBuffer.Length / 6;
        //    //}

        //    for (int i = start; i < itemAmount; i++)
        //    {
        //        // Triangle 1
        //        newIndex[i * 6 + 0] = (short)(i * 4);
        //        newIndex[i * 6 + 1] = (short)(i * 4 + 1);
        //        newIndex[i * 6 + 2] = (short)(i * 4 + 2);

        //        // Triangle 2
        //        newIndex[i * 6 + 3] = (short)(i * 4 + 1);
        //        newIndex[i * 6 + 4] = (short)(i * 4 + 3);
        //        newIndex[i * 6 + 5] = (short)(i * 4 + 2);
        //    }

        //    mIndexBuffer = newIndex;
        //    mVertexBuffer = new VertexPositionTexture[itemAmount*4];
        //}

        #endregion

    }
}
