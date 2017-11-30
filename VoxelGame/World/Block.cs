
namespace VoxelGame.World
{

    enum BlockType : byte
    {
        NONE,
        DIRT,
        GRASS,
        STONE,
        WATER,
        COUNT
    };

    class Block
    {
        public bool IsActive { get; set; }
        public BlockType BlockType { get; set; }
    }
}
