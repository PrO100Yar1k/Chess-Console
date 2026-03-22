public struct Move
{
    public Vector2 StartPoint { get; private set; }
    public Vector2 FinalPoint { get; private set; }

    public Move(Vector2 from, Vector2 to)
    {
        StartPoint = from;
        FinalPoint = to;
    }
}