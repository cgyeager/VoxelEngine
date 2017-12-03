using System;

namespace VoxelGame.Graphics
{
    public struct Point3 : IEquatable<Point3>
    {

        private static readonly Point3 zeroPoint3 = new Point3();

        public int X;
        public int Y;
        public int Z;

        public static Point3 Zero
        {
            get { return zeroPoint3; }
        }

        internal string DebugDisplayString
        {
            get
            {
                return string.Concat(
                    this.X.ToString(), "  ",
                    this.Y.ToString()
                );
            }
        }

        public Point3(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Point3(int value)
        {
            this.X = value;
            this.Y = value;
            this.Z = value;
        }

        public static Point3 operator +(Point3 value1, Point3 value2)
        {
            return new Point3(value1.X + value2.X, value1.Y + value2.Y, value1.Z + value2.Z);
        }

        public static Point3 operator -(Point3 value1, Point3 value2)
        {
            return new Point3(value1.X - value2.X, value1.Y - value2.Y, value1.Z - value2.Z);
        }

        public static Point3 operator *(Point3 value1, Point3 value2)
        {
            return new Point3(value1.X * value2.X, value1.Y * value2.Y, value1.Z * value2.Z);
        }

        public static Point3 operator /(Point3 source, Point3 divisor)
        {
            return new Point3(source.X / divisor.X, source.Y / divisor.Y, source.Z / divisor.Z);
        }

        public static bool operator ==(Point3 a, Point3 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Point3 a, Point3 b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            return (obj is Point3) && Equals((Point3)obj);
        }

        public bool Equals(Point3 other)
        {
            return ((X == other.X) && (Y == other.Y));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }

        }
        public override string ToString()
        {
            return "{X:" + X + " Y:" + Y + " Z:" + Z + "}";
        }

    }
}
