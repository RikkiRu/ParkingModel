public enum Layer
{
    None = 0,
    Ground,
    Polygon,
    Place,
    NodeBase,
    Road,
    NodeLine,
}

public class SortingOrder
{
    public static int Is(Layer type)
    {
        return (int)type;
    }
}