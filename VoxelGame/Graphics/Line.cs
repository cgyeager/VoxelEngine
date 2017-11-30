using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelGame.Graphics
{
    class Line
    {
        private VertexBuffer vertexBuffer;
        private GraphicsDevice Device;
        private Effect ShaderEffect;
        private BasicEffect basicEffect;
        private VertexPositionNormalColor[] vertices;
        private Vector3 startPoint = new Vector3(0, 0, 0);
        private Vector3 endPoint = new Vector3(0, 0, -50);

        //VertexPositionColor[] vertices = new[] { new VertexPositionColor(startPoint, Color.Red), new VertexPositionColor(endPoint, Color.Red) };

        public Line(GraphicsDevice device, Effect effect, Vector3 start, Vector3 end, Color color)
        {
            Device = device;
            ShaderEffect = effect;
            vertexBuffer = new VertexBuffer(Device, typeof(VertexPositionNormalColor), 2, BufferUsage.WriteOnly);

            VertexPositionNormalColor o = new VertexPositionNormalColor();
            o.Position = start;
            o.Normal = Vector3.Zero;
            o.Color = color;

            VertexPositionNormalColor d = new VertexPositionNormalColor();
            d.Position = end;
            d.Normal = Vector3.Zero;
            d.Color = color;

            vertices = new VertexPositionNormalColor[] { o, d };

            vertexBuffer.SetData(vertices);
        }


        public void Update(Vector3 start, Vector3 end, Color color)
        {
            vertexBuffer.Dispose();

            VertexPositionNormalColor o = new VertexPositionNormalColor();
            o.Position = start;
            o.Normal = Vector3.Zero;
            o.Color = color;

            VertexPositionNormalColor d = new VertexPositionNormalColor();
            d.Position = end;
            d.Normal = Vector3.Zero;
            d.Color = color;

            vertices = new VertexPositionNormalColor[] { o, d };

            vertexBuffer.SetData(vertices);

        }

        public void Render(Camera camera)
        {

            ShaderEffect.Parameters["World"].SetValue(Matrix.Identity);
            ShaderEffect.Parameters["View"].SetValue(camera.View);
            ShaderEffect.Parameters["Projection"].SetValue(camera.Projection);
            ShaderEffect.Parameters["EyePosition"].SetValue(camera.Position);

            Device.SetVertexBuffer(vertexBuffer);
            foreach(var pass in ShaderEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Device.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
            }
        }
        public void Dispose()
        {
            vertexBuffer.Dispose();
        }
    }
}
