using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxelGame.Graphics;

namespace VoxelGame.World
{
    class Grid
    {
        private GraphicsDevice Device;
        private Effect ShaderEffect;
        private VertexBuffer buffer;
        private IndexBuffer indexBuffer;
        private VertexPositionNormalColor[] vertices;
        private int[] indices;

        public float CellSize;
        public Vector3 GridOffset;
        int GridSize;
        Random random;        

        public Grid(GraphicsDevice device, Effect shaderEffect)
        {
            Device = device;
            ShaderEffect = shaderEffect;
            CellSize = 10.0f;
            GridSize = 1000;
            GridOffset = new Vector3(0.0f, 0.0f, 0.0f);
            random = new Random();

            BuildGrid();
        }

        public void Render(Camera camera)
        {
            ShaderEffect.Parameters["World"].SetValue(Matrix.Identity);
            ShaderEffect.Parameters["View"].SetValue(camera.View);
            ShaderEffect.Parameters["Projection"].SetValue(camera.Projection);
            ShaderEffect.Parameters["EyePosition"].SetValue(camera.Position);

            Device.SetVertexBuffer(buffer);
            Device.Indices = indexBuffer;
            foreach(var pass in ShaderEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, GridSize*GridSize*2);
            }
        }

        private void BuildGrid()
        { 
            /*
            vertices = new VertexPositionNormalColor[(GridSize+1)*(GridSize+1)];
            indices = new int[GridSize*GridSize*6];

            float vertexOffset = CellSize * 0.5f;

            Color[] colors = { Color.Red, Color.Green, Color.Blue, Color.Black};

            int index = 0;
            for (int z = 0; z <= GridSize; z++)
            {
                for (int x = 0; x <= GridSize; x++)
                {
                    Vector3 cellOffset = new Vector3(x*CellSize, 0.0f, z*CellSize);
                    float y = ((float)NoiseGenerator.Noise(x, z) + 1f)*0.5f;
                    y *= 60;
                    vertices[index].Position = new Vector3(x*CellSize - vertexOffset, y, z*CellSize - vertexOffset);
                    Vector3 normal = new Vector3(y, 1.0f + y, y); 
                    normal.Normalize();
                    vertices[index].Normal = normal;

                    if (y < 25)
                        vertices[index].Color = new Color(80, 50, 30);
                    else if (y >= 25 && y < 38)
                        vertices[index].Color = new Color(120, 90, 75);
                    else
                        vertices[index].Color = new Color(188, 210, 255);//Color.White;

                    index += 1;
                }
            }

            index = 0;
            int iIndex = 0;
            for (int z = 0; z < GridSize; z++)
            {
                for (int x = 0; x < GridSize; x++)
                {
                    indices[iIndex + 0] = index;  
                    indices[iIndex + 1] = index + 1;  
                    indices[iIndex + 2] = index + GridSize + 1;  
                    indices[iIndex + 3] = index + GridSize + 1;  
                    indices[iIndex + 4] = index + 1;  
                    indices[iIndex + 5] = index + GridSize + 2;
                    iIndex += 6;
                    index += 1;
                }
                index += 1;
            }

            buffer = new VertexBuffer(Device, typeof(VertexPositionNormalColor), (GridSize+1)*(GridSize+1), BufferUsage.WriteOnly);
            buffer.SetData(vertices);

            indexBuffer = new IndexBuffer(Device, typeof(int), GridSize*GridSize*6, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
            */
        }
    }
}
