using Microsoft.Xna.Framework;

namespace VoxelGame.World
{
    static class CubeData
    {
        public static Vector3[] Vertices = {
                new Vector3(1, 1, 1),       // 0
                new Vector3(1, -1, 1),      // 1
                new Vector3(-1, -1, 1),     // 2
                new Vector3(-1, 1, 1),      // 3
                new Vector3(1, 1, -1),      // 4
                new Vector3(-1, 1, -1),     // 5
                new Vector3(-1, -1, -1),    // 6
                new Vector3(1, -1, -1)      // 7
        };

        public static Vector3[] Normals = {
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1),
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, -1, 0)
        };

        public static Vector2[,] TextureCoords = {
            {
            new Vector2(0, 0),  // 0
            new Vector2(0, 54.875f/894f),  // 1 
            new Vector2(54.875f/894f, 0),  // 2
            new Vector2(54.875f/894f, 54.875f/894f)  // 3
            },

            {
            new Vector2(112.875f/894f, 0),  // 0
            new Vector2(112.875f/894f, 54.875f/894f),  // 1 
            new Vector2(112.875f/894f + 54f/894f, 0),  // 2
            new Vector2(112.875f/894f + 54f/894f, 54.875f/894f)  // 3
            },

            {
            new Vector2(224.875f/894f, 0),  // 0
            new Vector2(224.875f/894f, 54.875f/894f),  // 1 
            new Vector2(224.875f/894f + 54f/894f, 0),  // 2
            new Vector2(224.875f/894f + 54f/894f, 54.875f/894f)  // 3
            },
            {
            new Vector2(168.875f/894f, 0),  // 0
            new Vector2(168.875f/894f, 54.875f/894f),  // 1 
            new Vector2(168.875f/894f + 54f/894f, 0),  // 2
            new Vector2(168.875f/894f + 54f/894f, 54.875f/894f)  // 3
            }
    };

        public static Vector2[] StoneCoords = {
            new Vector2(0, 0),  // 0
            new Vector2(0, 54.875f/894f),  // 1 
            new Vector2(54.875f/894f, 0),  // 2
            new Vector2(54.875f/894f, 54.875f/894f)  // 3
        };

        public static Vector2[] DirtCoords = {
            new Vector2(112.875f/894f, 0),  // 0
            new Vector2(112.875f/894f, 54.875f/894f),  // 1 
            new Vector2(112.875f/894f + 54f/894f, 0),  // 2
            new Vector2(112.875f/894f + 54f/894f, 54.875f/894f)  // 3
        };

        public static Vector2[] GrassTopCoords = {
            new Vector2(226.875f/894f, 0),  // 0
            new Vector2(226.875f/894f, 53.875f/894f),  // 1 
            new Vector2(226.875f/894f + 53f/894f, 0),  // 2
            new Vector2(226.875f/894f + 53f/894f, 53.875f/894f)  // 3
        };

        public static Vector2[] GrassSideCoords = {
            new Vector2(168.875f/894f, 0),  // 0
            new Vector2(168.875f/894f, 54.875f/894f),  // 1 
            new Vector2(168.875f/894f + 54f/894f, 0),  // 2
            new Vector2(168.875f/894f + 54f/894f, 54.875f/894f)  // 3
        };

        public static Vector3 north = new Vector3(0, 0, 1);
        public static Vector3 south = new Vector3(0, 0, -1);
        public static Vector3 east = new Vector3(1, 0, 0);
        public static Vector3 west = new Vector3(-1, 0, 0);
        public static Vector3 up = new Vector3(0, 1, 0);
        public static Vector3 down = new Vector3(0, -1, 0);

        public static Vector3[] DirectionOffsets = {
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1),
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, -1, 0),
        };
    }
}
