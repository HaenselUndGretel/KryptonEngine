using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KryptonEngine.Rendering.Components
{
    class MeshData
    {
        public int TextureID;
        public int vertexCount, triangleCount;
        public VertexPositionTexture[] vertices = { };
        public int[] triangles = { };
    }
}
