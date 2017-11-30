using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelGame.Graphics
{
    class LineBox
    {
        private GraphicsDevice Device;
        private Effect ShaderEffect;
        private Line[] lines;
        
        // TODO: uses 4 vertices with indices instead 
        public LineBox(GraphicsDevice device, Effect effect, Vector3 min, Vector3 max, Color color)
        {
            Device = device;
            ShaderEffect = effect;

            lines = new Line[12];

            // vertical lines
            lines[0] = new Line(Device, ShaderEffect, min, new Vector3(min.X, max.Y, min.Z), color);
            lines[1] = new Line(Device, ShaderEffect, new Vector3(min.X, min.Y, max.Z), new Vector3(min.X, max.Y, max.Z), color);
            lines[2] = new Line(Device, ShaderEffect, new Vector3(max.X, min.Y, min.Z), new Vector3(max.X, max.Y, min.Z), color);
            lines[3] = new Line(Device, ShaderEffect, new Vector3(max.X, min.Y, max.Z), new Vector3(max.X, max.Y, max.Z), color);

            // bottom horizontal
            lines[4] = new Line(Device, ShaderEffect, min, new Vector3(min.X, min.Y, max.Z), color);
            lines[5] = new Line(Device, ShaderEffect, new Vector3(min.X, min.Y, max.Z), new Vector3(max.X, min.Y, max.Z), color);
            lines[6] = new Line(Device, ShaderEffect, new Vector3(max.X, min.Y, max.Z), new Vector3(max.X, min.Y, min.Z), color);
            lines[7] = new Line(Device, ShaderEffect, new Vector3(max.X, min.Y, min.Z), min, color);

            // top horizontal
            lines[8] = new Line(Device, ShaderEffect, new Vector3(min.X, max.Y, min.Z), new Vector3(min.X, max.Y, max.Z), color);
            lines[9] = new Line(Device, ShaderEffect, new Vector3(min.X, max.Y, max.Z), new Vector3(max.X, max.Y, max.Z), color);
            lines[10] = new Line(Device, ShaderEffect, new Vector3(max.X, max.Y, max.Z), new Vector3(max.X, max.Y, min.Z), color);
            lines[11] = new Line(Device, ShaderEffect, new Vector3(max.X, max.Y, min.Z), new Vector3(min.X, max.Y, min.Z), color);

        }

        public void Render(Camera camera)
        {
            foreach (var line in lines)
                line.Render(camera);
        }

        public void Dispose()
        {
            foreach (var line in lines)
                line.Dispose();
        }
    }
}
