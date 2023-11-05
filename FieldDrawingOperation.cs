/// <summary>
/// This file handles all of the logic related to the minefield canvas drawing and clicking
/// As of now it is similar to grafix code from xp version of minesweeper
/// </summary>
using System;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Media.Imaging;

namespace AntiBoom;

public sealed class FieldDrawingOperation : ICustomDrawOperation
{

    /// <summary>
    /// Current location of the mouse relative to the whole canvas
    /// </summary>
    public Point? CurrentMousePos { get; private set; }
    public Rect Bounds { get; }
    /// <summary>
    /// Bitmap images used for displaying blocks
    /// </summary>
    public Bitmap? BlocksImage { get; }

    private Minefield _field;
    private double _cellSize;
    private Rect _fieldRect;
    private bool _cheating;

    public FieldDrawingOperation(Rect bounds, Bitmap? blocksImage, Minefield field, double cellSize, bool cheating)
    {
        Bounds = bounds;
        BlocksImage = blocksImage;
        _field = field;
        _cellSize = cellSize;
        _fieldRect = new Rect(0, 0, cellSize * field.Width, cellSize * field.Height);
        _cheating = cheating;
    }

    public void Dispose()
    {
        //throw new System.NotImplementedException();
        // nothing really to dispose of 
    }

    public bool Equals(ICustomDrawOperation? other) => false;

    public bool HitTest(Point p)
    {
        if (!Bounds.Contains(p) || !_fieldRect.Contains(p))
        {
            CurrentMousePos = null;
            return false;
        }
        CurrentMousePos = new Point(
            (int)(p.X) / _cellSize,
            (int)(p.Y) / _cellSize);
        return true;
    }

    private void DrawBorder(ImmediateDrawingContext context, Point start, Point end, double width, Brush pen)
    {
        throw new NotImplementedException("No border yet");
    }


    private void DrawBlock(ImmediateDrawingContext context, int x, int y)
    {
        double offset = 0;
        // fallback operation in case no suitable assets are present 
        if (BlocksImage == null)
        {
            offset = _cellSize * 0.1;
            // base
            context.DrawRectangle(Brushes.Aqua, null, new Rect(x * _cellSize, y * _cellSize, _cellSize, _cellSize));
            // ~~cringe~~ inner detail
            context.DrawRectangle(Brushes.AliceBlue, null, new Rect(
                x * _cellSize + offset,
                y * _cellSize + offset,
                _cellSize - offset * 2,
                _cellSize - offset * 2));
            return;
        }

        switch (_cheating ?  CellState.Revealed : _field[x, y].State)
        {
            case CellState.Hidden:
                // do nothing, hidden tile is at id 0
                break;
            case CellState.Revealed:
                offset = (_field[x, y].IsBomb ? 3 : 15) * BlocksImage.PixelSize.Width;
                break;
            case CellState.Flagged:
                offset = BlocksImage.PixelSize.Width;
                break;
            case CellState.Question:
                offset = BlocksImage.PixelSize.Width * 2;
                break;
        }

        context.DrawBitmap
        (
            BlocksImage,
            new Rect(0, offset, BlocksImage.PixelSize.Width, BlocksImage.PixelSize.Width),
            new Rect(x * _cellSize, y * _cellSize, _cellSize, _cellSize)
        );
    }

    public void Render(ImmediateDrawingContext context)
    {
        for (int x = 0; x < _field.Width; x++)
        {
            for (int y = 0; y < _field.Height; y++)
            {
                DrawBlock(context, x, y);
            }
        }
    }
}
