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

    public override bool Equals(object obj)
    {
        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"PosX: {X}; PosY: {Y}";
    }


    public static Vector2 operator -(Vector2 a)
    {
        return new Vector2(-a.X, -a.Y);
    }
}

