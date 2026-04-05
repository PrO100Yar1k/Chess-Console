public struct Vector2
{
    public int Y { get; set; }
    public int X { get; set; }

    public Vector2(int initialPosX, int initialPosY)
    {
        X = initialPosX;
        Y = initialPosY;
    }

    public static Vector2 operator + (Vector2 a, Vector2 b)
    {
        return new Vector2(a.X + b.X, a.Y + b.Y);
    }

    public static Vector2 operator - (Vector2 a, Vector2 b)
    {
        return new Vector2(a.X - b.X, a.Y - b.Y);
    }

    public static Vector2 operator - (Vector2 a) // inverse
    {
        return new Vector2(-a.X, -a.Y);
    }

    public static Vector2 operator * (Vector2 a, Vector2 b)
    {
        return new Vector2(a.X * b.X, a.Y * b.Y);
    }

    public static Vector2 operator / (Vector2 a, Vector2 b)
    {
        return new Vector2(a.X / b.X, a.Y / b.Y);
    }

    public static bool operator == (Vector2 a, Vector2 b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator != (Vector2 a, Vector2 b)
    {
        return !(a.X == b.X && a.Y == b.Y);
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector2 other && Equals(other);
    }

    public bool Equals(Vector2 other)
    {
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override string ToString()
    {
        return $"Pos X: {X}; Pos Y: {Y}";
    }
}

