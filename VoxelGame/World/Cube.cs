using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxelGame.Graphics;

namespace VoxelGame.World
{
    class Cube
    {
        private GraphicsDevice Device;
        private Effect ShaderEffect;
        private VertexBuffer buffer;
        private IndexBuffer indexBuffer;
        private VertexPositionNormalColor[] vertices;
        private Color color;
        private int xSize, ySize, zSize;

        int vertexNum;

        public Cube(GraphicsDevice device, Effect shaderEffect, Color col, int size)
        {
            Device = device;
            ShaderEffect = shaderEffect;
            color = col;
            xSize = ySize = zSize = size;

            BuildCube();
            CreateTriangles();
        }

        public void Render(Camera camera)
        {
            ShaderEffect.Parameters["World"].SetValue(Matrix.Identity);
            ShaderEffect.Parameters["View"].SetValue(camera.View);
            ShaderEffect.Parameters["Projection"].SetValue(camera.Projection);
            ShaderEffect.Parameters["EyePosition"].SetValue(camera.Position);

            Device.SetVertexBuffer(buffer);
            Device.Indices = indexBuffer;
            foreach (var pass in ShaderEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 5000);
            }
        }

        private void BuildCube()
        {
            int cornerVertices = 8;
            int edgeVertices = 4 * (xSize + ySize + zSize - 3);
            int faceVertices = 2 * (
                    (xSize - 1) * (ySize - 1) +
                    (xSize - 1) * (zSize - 1) +
                    (ySize - 1) * (zSize - 1));

            vertexNum = cornerVertices + edgeVertices + faceVertices;
            vertices = new VertexPositionNormalColor[vertexNum];

            int v = 0;
            for (int y = 0; y <= ySize; y++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    vertices[v].Color = color;
                    vertices[v].Normal = new Vector3(0.0f, 0.0f, -1.0f);
                    vertices[v++].Position = new Vector3(x, y, 0);
                }
                for (int z = 1; z <= zSize; z++)
                {
                    vertices[v].Color = color;
                    vertices[v].Normal = new Vector3(1.0f, 0.0f, 0.0f);
                    vertices[v++].Position = new Vector3(xSize, y, z);
                }
                for (int x = xSize - 1; x >= 0; x--)
                {
                    vertices[v].Color = color;
                    vertices[v].Normal = new Vector3(0.0f, 0.0f, 1.0f);
                    vertices[v++].Position = new Vector3(x, y, zSize);
                }
                for (int z = zSize - 1; z > 0; z--)
                {
                    vertices[v].Color = color;
                    vertices[v].Normal = new Vector3(-1.0f, 0.0f, 0.0f);
                    vertices[v++].Position = new Vector3(0, y, z);
                }
            }

            for (int z = 1; z < zSize; z++)
            {
                for (int x = 1; x < xSize; x++)
                {
                    vertices[v].Color = color;
                    vertices[v].Normal = new Vector3(0.0f, 1.0f, 0.0f);
                    vertices[v++].Position = new Vector3(x, ySize, z);
                }
            }
            for (int z = 1; z < zSize; z++)
            {
                for (int x = 1; x < xSize; x++)
                {
                    vertices[v].Color = color;
                    vertices[v].Normal = new Vector3(0.0f, -1.0f, 0.0f);
                    vertices[v++].Position = new Vector3(x, 0, z);
                }
            }

            buffer = new VertexBuffer(Device,
                                      typeof(VertexPositionNormalColor),
                                      vertices.Length,
                                      BufferUsage.WriteOnly);
            buffer.SetData(vertices);
        }

        private static int SetQuad(int[] indices, int i, int v00, int v10, int v01, int v11)
        {
            indices[i+5] = v00;
            indices[i+4] = v01;
            indices[i+3] = v10;
            indices[i+2] = v10;
            indices[i+1] = v01;
            indices[i+0] = v11;
            return i + 6;
        }

        private int CreateTopFace(int[] triangles, int t, int ring)
        {
            int v = ring * ySize;
            for (int x = 0; x < xSize - 1; x++, v++)
            {
                t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
            }
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

            int vMin = ring * (ySize + 1) - 1;
            int vMid = vMin + 1;
            int vMax = v + 2;

            for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
            {
                t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
                for (int x = 1; x < xSize - 1; x++, vMid++)
                {
                    t = SetQuad(triangles, t, vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
                }
                t = SetQuad(triangles, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
            }

            int vTop = vMin - 2;
            t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
            for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
            }
            t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

            return t;
        }

        private int CreateBottomFace(int[] triangles, int t, int ring)
        {
            int v = 1;
            int vMid = vertices.Length - (xSize - 1) * (zSize - 1);
            t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
            for (int x = 1; x < xSize - 1; x++, v++, vMid++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
            }
            t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

            int vMin = ring - 2;
            vMid -= xSize - 2;
            int vMax = v + 2;

            for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
            {
                t = SetQuad(triangles, t, vMin, vMid + xSize - 1, vMin + 1, vMid);
                for (int x = 1; x < xSize - 1; x++, vMid++)
                {
                    t = SetQuad(triangles, t, 
                                vMid + xSize - 1, 
                                vMid + xSize, 
                                vMid, 
                                vMid + 1);
                }
                t = SetQuad(triangles, t, vMid + xSize - 1, vMax + 1, vMid, vMax);
            }

            int vTop = vMin - 1;
            t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
            for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
            {
                t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
            }
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

            return t;
        }
        private void CreateTriangles()
        {
            int quads = 2*((xSize * ySize+1) + (xSize * zSize+1) + (ySize * zSize+1));
            int[] triangles = new int[quads * 6];
            int ring = (xSize + zSize) * 2;
            int t = 0, v = 0;

            for (int y = 0; y < ySize; y++, v++)
            {
                for (int q = 0; q < ring - 1; q++, v++)
                {
                    t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
                }
                t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
            }

            t = CreateTopFace(triangles, t, ring);
            t = CreateBottomFace(triangles, t, ring);

            indexBuffer = new IndexBuffer(Device, typeof(int), triangles.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(triangles);
        }
    }
}

    /*



        private void CreateTriangles()
        {
            int quads = ( (xSize * ySize + 1) + (xSize * zSize + 1) + (ySize * zSize + 1)) * 2;
            int[] triangles = new int[quads * 6];
            int ring = (xSize + zSize) * 2;
            int t = 0, v = 0;

            for (int y = 0; y < ySize; y++, v++)
            {
                for (int q = 0; q < ring - 1; q++, v++)
                {
                    t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
                }
                t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
            }

            t = CreateTopFace(triangles, t, ring);
            t = CreateBottomFace(triangles, t, ring);
            indexBuffer = new IndexBuffer(Device, typeof(int), triangles.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(triangles);
        }

        private int CreateTopFace(int[] triangles, int t, int ring)
        {
            int v = ring * ySize;
            for (int x = 0; x < xSize - 1; x++, v++)
            {
                t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
            }
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

            int vMin = ring * (ySize + 1) - 1;
            int vMid = vMin + 1;
            int vMax = v + 2;

            for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
            {
                t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
                for (int x = 1; x < xSize - 1; x++, vMid++)
                {
                    t = SetQuad(
                        triangles, t,
                        vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
                }
                t = SetQuad(triangles, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
            }

            int vTop = vMin - 2;
            t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
            for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
            }
            t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

            return t;
        }

        private int CreateBottomFace(int[] triangles, int t, int ring)
        {
            int v = 1;
            int vMid = vertices.Length - (xSize - 1) * (zSize - 1);
            t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
            for (int x = 1; x < xSize - 1; x++, v++, vMid++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
            }
            t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

            int vMin = ring - 2;
            vMid -= xSize - 2;
            int vMax = v + 2;

            for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
            {
                t = SetQuad(triangles, t, vMin, vMid + xSize - 1, vMin + 1, vMid);
                for (int x = 1; x < xSize - 1; x++, vMid++)
                {
                    t = SetQuad(
                        triangles, t,
                        vMid + xSize - 1, vMid + xSize, vMid, vMid + 1);
                }
                t = SetQuad(triangles, t, vMid + xSize - 1, vMax + 1, vMid, vMax);
            }

            int vTop = vMin - 1;
            t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
            for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
            {
                t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
            }
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

            return t;
        }

        private static int
        SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
        {
            triangles[i + 5] = v00;
            triangles[i + 4] = v01;
            triangles[i + 3] = v10;
            triangles[i + 2] = v10;
            triangles[i + 1] = v01;
            triangles[i + 0] = v11;
            return i + 6;
        }



        }
    } 


*/
