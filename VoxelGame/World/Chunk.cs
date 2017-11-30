using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxelGame.Graphics;

namespace VoxelGame.World
{
    class Chunk
    {
        private GraphicsDevice Device;
        private Effect ShaderEffect;
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

        private List<VertexPositionNormalColor> vertices;
        private List<int> indices;

        private Vector3 GridPosition;
        private Vector3 WorldPosition;

        private float Scale;
        private int Dim;
        private static long Id = -1;
        public bool Selected { get; set; }

        private Block[,,] Blocks;

        private Color color;
        private static float max = 0;

        public BoundingBox aabb { set; get; }
        public Vector3 Min { get; }
        public Vector3 Max { get; }

        private enum Direction
        {
            NORTH,
            SOUTH,
            EAST,
            WEST,
            TOP,
            BOTTOM
        }

        public Chunk(GraphicsDevice device, Effect shaderEffect, Vector3 position, float scale)
        {
            Device = device;
            ShaderEffect = shaderEffect;
            GridPosition = position;
            Dim = 64;

            Scale = scale;

            WorldPosition = new Vector3(GridPosition.X * Scale * (float)Dim,
                                        GridPosition.Y * Scale * (float)Dim,
                                        GridPosition.Z * Scale * (float)Dim);

            float halfScale = Scale / 2f;
            Min = new Vector3(WorldPosition.X - halfScale, WorldPosition.Y - halfScale, WorldPosition.Z - halfScale);
            Max = new Vector3(WorldPosition.X + Dim * Scale - halfScale,
                              WorldPosition.Y + Dim * Scale - halfScale,
                              WorldPosition.Z + Dim * Scale - halfScale);

            aabb = new BoundingBox(Min, Max);

            Blocks = new Block[Dim, Dim, Dim];
            for (int y = 0; y < Dim; y++)
                for (int x = 0; x < Dim; x++)
                    for (int z = 0; z < Dim; z++)
                    {
                        Blocks[x, y, z] = new Block();
                        Blocks[x, y, z].IsActive = false;
                        Blocks[x, y, z].BlockType = BlockType.STONE;

                    }

            vertices = new List<VertexPositionNormalColor>();
            indices = new List<int>();

            Id++;

            Selected = false;
        }

        public void Render(Camera camera)
        {
            if (vertexBuffer != null && indexBuffer != null &&
                vertexBuffer.VertexCount > 0 && indexBuffer.IndexCount > 0 &&
                vertices.Count != 0 && indices.Count != 0)
            {
                ShaderEffect.Parameters["World"].SetValue(Matrix.Identity);
                ShaderEffect.Parameters["View"].SetValue(camera.View);
                ShaderEffect.Parameters["Projection"].SetValue(camera.Projection);
                ShaderEffect.Parameters["EyePosition"].SetValue(camera.Position);

                Device.SetVertexBuffer(vertexBuffer);
                Device.Indices = indexBuffer;
                foreach (var pass in ShaderEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Count / 2);
                }
            }
        }

        public void Update()
        {
            if (vertexBuffer != null && !vertexBuffer.IsDisposed)
            {
                vertices.Clear();
                vertexBuffer.Dispose();
                vertexBuffer = null;
            }
            if (indexBuffer != null && !indexBuffer.IsDisposed)
            {
                indices.Clear();
                indexBuffer.Dispose();
                indexBuffer = null;
            }
            Build();
        }

        public void TurnOffBlock(int x, int y, int z)
        {
            Blocks[x, y, z].IsActive = false; //!Blocks[x, y, z].IsActive;
            Blocks[x, y, z].BlockType = BlockType.NONE;
        }

        public void TurnOnBlock(int x, int y, int z)
        {
            Blocks[x, y, z].IsActive = true; //!Blocks[x, y, z].IsActive;
            Blocks[x, y, z].BlockType = BlockType.STONE;
        }

        public Block GetBlockAt(Vector3 point)
        {
            Vector3 h = WorldToChunkXYZ(point.X, point.Y, point.Z);
            if ((h.X >= Dim || h.X < 0) || (h.Y >= Dim || h.Y < 0) || (h.Z >= Dim || h.Z < 0))
                return null;
            return Blocks[(int)h.X, (int)h.Y, (int)h.Z];
        }

        public long GetId()
        {
            return Id;
        } 
         
        public void SetAllToStone()
        {
             for (int y = 0; y < Dim; y++)
            {
                for (int x = 0; x < Dim; x++)
                {
                    for (int z = 0; z < Dim; z++)
                    {
                        Blocks[x, y, z].IsActive = true;
                        Blocks[x, y, z].BlockType = BlockType.STONE;
                    }
                }
            } 
            Build();
        }

        public void GenSphere()
        {
            int dim = (int)(Dim);
            for (int y = 0; y < Dim; y++)
            {
                for (int x = 0; x < Dim; x++)
                {
                    for (int z = 0; z < Dim; z++)
                    {
                        if(Math.Sqrt((x - dim / 2) * (x - dim / 2) + (y - dim / 2) * (y - dim / 2) + (z - dim / 2) * (z - dim / 2)) <= dim / 2)
                        {
                            Blocks[x, y, z].IsActive = true;
                        }
                    }
                }
            } 
            Build();
        }

        public void GenHemiSphere()
        {
            int dim = (int)(Dim);
            for (int y = 0; y < Dim; y++)
            {
                for (int x = 0; x < Dim; x++)
                {
                    for (int z = 0; z < Dim; z++)
                    {
                        if(y <= 2*Dim/3 ||  Math.Sqrt((x - dim/2) * (x - dim/2) + (y - dim/2) * (y - dim/2) + (z - dim/2) * (z - dim/2)) <= dim/2)
                        {
                            Blocks[x, y, z].IsActive = true;
                        }
                    }
                }
            } 
            Build();
        }

        public void SetupLandscape()
        {
            Random random = new Random();

            int W = (int)Dim * (int)Scale;
            int H = (int)Dim * (int)Scale;
            float[,] heightmap = new float[W, H];

            // create heightmap from nosie
            for (int x = (int)WorldPosition.X; x < (int)WorldPosition.X + Dim*Scale; x += (int)Scale)
            {
                for (int z = (int)WorldPosition.Z; z < (int)WorldPosition.Z + Dim*Scale; z += (int)Scale)
                {
                    Vector2 voxIndex = WorldToChunkXZ(x, z);
                    int X = (int)voxIndex.X;
                    int Z = (int)voxIndex.Y;

                    float height = ((float)NoiseGenerator.Noise(Math.Abs(x), Math.Abs(z)) + 1f) * 0.5f +
                        ((float)NoiseGenerator2.Noise(Math.Abs(x), Math.Abs(z)) + 1f) * 0.25f;
                    heightmap[X, Z] = height;
                }
            }

            // find max noise value
            foreach (float h in heightmap)
            {
                if (h > max)
                    max = h;
            }
            
            // normalize to 0 - 1
            for (int x = (int)WorldPosition.X; x < (int)WorldPosition.X + Dim*Scale; x += (int)Scale)
            {
                for (int z = (int)WorldPosition.Z; z < (int)WorldPosition.Z + Dim*Scale; z += (int)Scale)
                {
                    Vector2 voxIndex = WorldToChunkXZ(x, z);
                    int X = (int)voxIndex.X;
                    int Z = (int)voxIndex.Y;

                    heightmap[X, Z] /= max;
                }
            }
 
            for (int x = (int)WorldPosition.X; x < (int)WorldPosition.X + Dim*Scale; x += (int)Scale)
            {
                for (int z = (int)WorldPosition.Z; z < (int)WorldPosition.Z + Dim*Scale; z += (int)Scale)
                {
                    /*
                    float height = ((float)NoiseGenerator.Noise(Math.Abs(x), Math.Abs(z)) + 1f)*0.5f + 
                        ((float)NoiseGenerator2.Noise(Math.Abs(x), Math.Abs(z)) + 1f)*0.15f;
                    if (height > 1.0) height /= 2.0f;
                    */
                    Vector2 voxIndex = WorldToChunkXZ(x, z);
                    int X = (int)voxIndex.X;
                    int Z = (int)voxIndex.Y;

                    float height = heightmap[X, Z];
                    height *= (float)Dim;

                    for (int y = 0; y < (int)height; y++)
                    {
                        Blocks[(int)voxIndex.X, y, (int)voxIndex.Y].IsActive = true;

                        if (y > 26 * height / 30)
                            Blocks[X, y, Z].BlockType = BlockType.GRASS;
                        else if (y <= 26 * height / 30 && y > 18*height/30)
                            Blocks[X, y, Z].BlockType = BlockType.DIRT;
                        else
                            Blocks[X, y, Z].BlockType = BlockType.STONE;

                    }
                }
            }
            Build();
        }

        public Vector2 WorldToChunkXZ(float x, float z)
        {
            float wx = x < 0 ? Math.Abs(WorldPosition.X) : -WorldPosition.X;
            float wz = z < 0 ? Math.Abs(WorldPosition.Z) : -WorldPosition.Z;
            return new Vector2((x + wx)/Scale, (z + wz)/Scale);
        }

        public Vector3 WorldToChunkXYZ(float x, float y, float z)
        {
            float wx = x < 0 ? Math.Abs(WorldPosition.X) : -WorldPosition.X;
            float wy = y < 0 ? Math.Abs(WorldPosition.Y) : -WorldPosition.Y;
            float wz = z < 0 ? Math.Abs(WorldPosition.Z) : -WorldPosition.Z;
            return new Vector3((x + wx)/Scale, (y + wy)/Scale, (z + wz)/Scale);
        }

        public void Dispose()
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
        }

        private bool CheckNeighbor(int x, int y, int z, Direction dir)
        {
            Vector3 offset = CubeData.DirectionOffsets[(int)dir];
            int iX = x + (int)offset.X;
            int iY = y + (int)offset.Y;
            int iZ = z + (int)offset.Z;

            if (iX >= 0 && iX < Blocks.GetLength(0) && 
                iY >= 0 && iY < Blocks.GetLength(1) &&
                iZ >= 0 && iZ < Blocks.GetLength(2))
                return Blocks[iX, iY, iZ].IsActive;
            else
            {
                return false;
            }
        }

        private void Build()
        {
            for (int y = 0; y < Dim; y++)
            {
                for (int x = 0; x < Dim; x++)
                {
                    for (int z = 0; z < Dim; z++)
                    {
                        // check and make quad faces for 6 sides of cube
                        if (Blocks[x, y, z].IsActive)
                            for (int i = 0; i < 6; i++)
                            {
                                // Check Neighbors
                                if (CheckNeighbor(x, y, z, (Direction)i))
                                    continue;
                                else
                                {
                                    if (Blocks[x, y, z].BlockType == BlockType.GRASS)
                                        color = Color.ForestGreen;
                                    else if (Blocks[x, y, z].BlockType == BlockType.DIRT)
                                        color = new Color(100, 85, 50);
                                    else if (Blocks[x, y, z].BlockType == BlockType.STONE) 
                                        color = Color.SlateGray;
                                    else if (Blocks[x, y, z].BlockType == BlockType.NONE) 
                                        color = Color.PapayaWhip;

                                        MakeFace((Direction)i,
                                                 Scale * 0.5f,
                                                 new Vector3((int)x * Scale + GridPosition.X * Scale * (float)Dim,
                                                             (int)y * Scale + GridPosition.Y * Scale * (float)Dim,
                                                             (int)z * Scale + GridPosition.Z * Scale * (float)Dim));

                                }
                            }
                    }
                }
            }
            if (vertices.Count != 0)
            {
                vertexBuffer = new VertexBuffer(Device, typeof(VertexPositionNormalColor), vertices.Count, BufferUsage.WriteOnly);
                vertexBuffer.SetData(vertices.ToArray());

                indexBuffer = new IndexBuffer(Device, typeof(int), indices.Count, BufferUsage.WriteOnly);
                indexBuffer.SetData(indices.ToArray());
            }
        }

        private void MakeFace(Direction dir, float scale, Vector3 pos)
        {
            switch(dir)
            {
                case Direction.NORTH:
                {
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[3]*scale + pos, color, CubeData.Normals[(int)Direction.NORTH]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[0]*scale + pos, color, CubeData.Normals[(int)Direction.NORTH]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[1]*scale + pos, color, CubeData.Normals[(int)Direction.NORTH]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[2]*scale + pos, color, CubeData.Normals[(int)Direction.NORTH]));
                } break;
                case Direction.SOUTH:
                {
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[4]*scale + pos, color, CubeData.Normals[(int)Direction.SOUTH]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[5]*scale + pos, color, CubeData.Normals[(int)Direction.SOUTH]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[6]*scale + pos, color, CubeData.Normals[(int)Direction.SOUTH]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[7]*scale + pos, color, CubeData.Normals[(int)Direction.SOUTH]));
                } break;
                case Direction.EAST:
                {
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[0]*scale + pos, color, CubeData.Normals[(int)Direction.EAST]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[4]*scale + pos, color, CubeData.Normals[(int)Direction.EAST]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[7]*scale + pos, color, CubeData.Normals[(int)Direction.EAST]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[1]*scale + pos, color, CubeData.Normals[(int)Direction.EAST]));
                } break;
                case Direction.WEST:
                {
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[5]*scale + pos, color, CubeData.Normals[(int)Direction.WEST]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[3]*scale + pos, color, CubeData.Normals[(int)Direction.WEST]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[2]*scale + pos, color, CubeData.Normals[(int)Direction.WEST]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[6]*scale + pos, color, CubeData.Normals[(int)Direction.WEST]));
                } break;
                case Direction.TOP:
                {
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[0]*scale + pos, color, CubeData.Normals[(int)Direction.TOP]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[3]*scale + pos, color, CubeData.Normals[(int)Direction.TOP]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[5]*scale + pos, color, CubeData.Normals[(int)Direction.TOP]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[4]*scale + pos, color, CubeData.Normals[(int)Direction.TOP]));
                } break;
                case Direction.BOTTOM:
                {
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[6]*scale + pos, color, CubeData.Normals[(int)Direction.BOTTOM]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[2]*scale + pos, color, CubeData.Normals[(int)Direction.BOTTOM]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[1]*scale + pos, color, CubeData.Normals[(int)Direction.BOTTOM]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[7]*scale + pos, color, CubeData.Normals[(int)Direction.BOTTOM]));
                } break;
            } 
            indices.Add(vertices.Count - 4);
            indices.Add(vertices.Count - 3);
            indices.Add(vertices.Count - 2);
            indices.Add(vertices.Count - 2);
            indices.Add(vertices.Count - 1);
            indices.Add(vertices.Count - 4);
        }
    }
}



/*
        private void MakeFace(Direction dir, float scale, Vector3 pos)
        {
            switch(dir)
            {
                case Direction.NORTH:
                {
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[3]*scale + pos, color, CubeData.Normals[(int)Direction.NORTH]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[0]*scale + pos, color, CubeData.Normals[(int)Direction.NORTH]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[1]*scale + pos, color, CubeData.Normals[(int)Direction.NORTH]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[2]*scale + pos, color, CubeData.Normals[(int)Direction.NORTH]));
                } break;
                case Direction.SOUTH:
                {
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[4]*scale + pos, color, CubeData.Normals[(int)Direction.SOUTH]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[5]*scale + pos, color, CubeData.Normals[(int)Direction.SOUTH]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[6]*scale + pos, color, CubeData.Normals[(int)Direction.SOUTH]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[7]*scale + pos, color, CubeData.Normals[(int)Direction.SOUTH]));
                } break;
                case Direction.EAST:
                {
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[0]*scale + pos, color, CubeData.Normals[(int)Direction.EAST]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[4]*scale + pos, color, CubeData.Normals[(int)Direction.EAST]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[7]*scale + pos, color, CubeData.Normals[(int)Direction.EAST]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[1]*scale + pos, color, CubeData.Normals[(int)Direction.EAST]));
                } break;
                case Direction.WEST:
                {
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[5]*scale + pos, color, CubeData.Normals[(int)Direction.WEST]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[3]*scale + pos, color, CubeData.Normals[(int)Direction.WEST]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[2]*scale + pos, color, CubeData.Normals[(int)Direction.WEST]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[6]*scale + pos, color, CubeData.Normals[(int)Direction.WEST]));
                } break;
                case Direction.TOP:
                {
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[0]*scale + pos, color, CubeData.Normals[(int)Direction.TOP]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[3]*scale + pos, color, CubeData.Normals[(int)Direction.TOP]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[5]*scale + pos, color, CubeData.Normals[(int)Direction.TOP]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[4]*scale + pos, color, CubeData.Normals[(int)Direction.TOP]));
                } break;
                case Direction.BOTTOM:
                {
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[6]*scale + pos, color, CubeData.Normals[(int)Direction.BOTTOM]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[2]*scale + pos, color, CubeData.Normals[(int)Direction.BOTTOM]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[1]*scale + pos, color, CubeData.Normals[(int)Direction.BOTTOM]));
                    vertices.Add(new VertexPositionNormalColor(CubeData.Vertices[7]*scale + pos, color, CubeData.Normals[(int)Direction.BOTTOM]));
                } break;
            } 
            indices.Add(vertices.Count - 4);
            indices.Add(vertices.Count - 3);
            indices.Add(vertices.Count - 2);
            indices.Add(vertices.Count - 2);
            indices.Add(vertices.Count - 1);
            indices.Add(vertices.Count - 4);
        }
*/
