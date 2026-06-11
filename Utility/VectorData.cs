namespace EchoForge.Utility
{
    public struct VectorData
    {
        public float X;
        public float Y;

        public VectorData(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static VectorData Zero => new VectorData(0f, 0f);

        public static VectorData operator +(VectorData a, VectorData b) =>
            new VectorData(a.X + b.X, a.Y + b.Y);

        public static VectorData operator *(VectorData a, float scalar) =>
            new VectorData(a.X * scalar, a.Y * scalar);

        public static bool operator ==(VectorData a, VectorData b) =>
            a.X == b.X && a.Y == b.Y;

        public static bool operator !=(VectorData a, VectorData b) => !(a == b);

        public override bool Equals(object obj) =>
            obj is VectorData v && this == v;

        public override int GetHashCode() => (X, Y).GetHashCode();

        public static VectorData Lerp(VectorData a, VectorData b, float t)
        {
            t = t < 0f ? 0f : (t > 1f ? 1f : t);
            return new VectorData(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
        }

        public override string ToString() => $"({X}, {Y})";
    }
}
