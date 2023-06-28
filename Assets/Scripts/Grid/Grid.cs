public class Grid<TGridObject>
{
    private int _width;
    private int _height;
    private readonly TGridObject[,] _gridArray;

    public TGridObject[,] GridArray => _gridArray;

    public Grid(int width, int height)
    {
        _width = width;
        _height = height;

        _gridArray = new TGridObject[width, height];
    }
}