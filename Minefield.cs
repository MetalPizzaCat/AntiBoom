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

    public FieldCell this[int x, int y]
    {
        get => _cells[x + y * Width];
        set => _cells[x + y * Width] = value;
    }

    public void RevealAllBombs()
    {
        foreach (FieldCell cell in _cells.Where(p => p.IsBomb))
        {
            cell.State = CellState.Revealed;
        }
    }

    public int Width { get; }
    public int Height { get; }
}
