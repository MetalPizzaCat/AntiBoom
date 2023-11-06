using System;
using System.Linq;

namespace AntiBoom;

public enum CellState
{
    Hidden,
    Revealed,
    Flagged,
    Question
}

public class FieldCell
{
    public CellState State { get; set; }
    public bool IsBomb { get; set; }

    public int? BombCount { get; set; }

    public FieldCell(bool isBomb = false)
    {
        IsBomb = isBomb;
        State = CellState.Hidden;
        BombCount = null;
    }

}
/// <summary>
/// Stores information about minefield and provides a bit of a better way to access it 
/// </summary>
public class Minefield
{

    private readonly FieldCell[] _cells;

    public Minefield(int width, int height)
    {
        Width = width;
        Height = height;

        _cells = Enumerable.Range(0, width * height).Select(i => new FieldCell()).ToArray();
    }

    /// <summary>
    /// Returns cell at specified coordinates or null if coordinates are out of bounds
    /// </summary>
    public FieldCell? this[int x, int y]
    {
        get
        {
            int id = x + y * Width;
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return null;
            }
            return _cells[id];
        }
    }

    /// <summary>
    /// Sets all bombs to be revealed
    /// </summary>
    public void RevealAllBombs()
    {
        foreach (FieldCell cell in _cells.Where(p => p.IsBomb))
        {
            cell.State = CellState.Revealed;
        }
    }

    /// <summary>
    /// Current width of the playing field
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Current height of the playing field
    /// </summary>
    public int Height { get; }
}
