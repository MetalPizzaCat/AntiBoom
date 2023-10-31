namespace AntiBoom;

/// <summary>
/// Stores information about minefield and provides a bit of a better way to access it 
/// </summary>
public class Minefield
{
    public enum State
    {
        Hidden,
        Revealed,
        Flagged,
        Question
    }
    private byte[] _bombs;

    public Minefield(int width, int height)
    {
        Width = width;
        Height = height;

        _bombs = new byte[width * height];
    }

    public State this[int x, int y]
    {
        get => (State)_bombs[x + y * Width];
        set => _bombs[x + y * Width] = (byte)value;
    }

    public int Width { get; }
    public int Height { get; }
}
