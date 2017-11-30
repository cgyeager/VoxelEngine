using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VoxelGame.Graphics
{
    public class Camera
    {
        public float Pitch, Yaw;
        public Vector3 Position;
        public Matrix View = Matrix.Identity;
        public Matrix Projection = Matrix.Identity;

        public Camera(GraphicsDevice device)
        {
            Pitch = 0;
            Yaw = 0;
            Position = Vector3.Zero;

            float aspectRatio = device.Viewport.AspectRatio;
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70), aspectRatio, 0.1f, 5000.0f);
        }

        // Returns a unit vector pointing in the direction that we're looking.
        public Vector3 GetLookVector()
        {
            Matrix rotation = Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);
            return Vector3.Transform(Vector3.Forward, rotation);
        }

        public Vector3 GetRightVector()
        {
            Matrix rotation = Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);
            return Vector3.Transform(Vector3.Right, rotation);
        }

        public void Update()
        {
            Vector3 target = Position + GetLookVector();
            this.View = Matrix.CreateLookAt(Position, target, Vector3.Up);
        }
    }
}
