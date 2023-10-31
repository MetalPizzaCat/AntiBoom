/// <summary>
/// This file handles all of the logic related to the minefield canvas drawing and clicking
/// As of now it is similar to grafix code from xp version of minesweeper
/// </summary>
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System.IO;
using System.Collections.Generic;
using Avalonia.Input;

namespace AntiBoom;

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
public class MinefieldCanvasControl : Control
{

    private Minefield _field;
    public Minefield Field => _field;
    private Bitmap _blocksImage;

    public double _cellSize = 16;
    public double CellSize
    {
        get => _cellSize;
        set
        {
            _cellSize = value;
            Width = value * Field.Width;
            Height = value * Field.Height;
        }
    }
    public MinefieldCanvasControl()
    {
        ClipToBounds = true;
        if (File.Exists("./assets/blocks.bmp"))
        {
            _blocksImage = new Bitmap("./assets/blocks.bmp");
        }
        else
        {
            Console.WriteLine("Failed to load game assets");
        }

        _field = new Minefield(10, 10);

        PointerPressed += FieldClicked;
    }

    private void FieldClicked(object? sender, PointerPressedEventArgs e)
    {
        Point clickPoint = e.GetPosition(this);
        Point cell = new Point(
            (int)(clickPoint.X) / CellSize,
            (int)(clickPoint.Y) / CellSize);
        _field[(int)cell.X, (int)cell.Y] = Minefield.State.Revealed;
        Console.WriteLine($"CLICK at ({clickPoint.X}, {clickPoint.Y})");
    }

    public override void Render(DrawingContext context)
    {
        context.Custom(new FieldDrawingOperation(
            new Rect(0, 0, Bounds.Width, Bounds.Height),
            _blocksImage,
            _field,
            _cellSize));
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }
}