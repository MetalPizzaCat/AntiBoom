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

class FieldDrawingOperation : ICustomDrawOperation
{

    private readonly GlyphRun _testMessageGlyphs;
    private Minefield _field;
    public Point? CurrentMousePos { get; private set; }
    private double _cellSize;
    public static readonly string TestMessage = "Everything is trying to work";
    private Rect _fieldRect;
    public FieldDrawingOperation(Rect bounds, Bitmap blocksImage, Minefield field, double cellSize)
    {
        ushort[]? glyphs = TestMessage.Select(ch => Typeface.Default.GlyphTypeface.GetGlyph(ch)).ToArray();
        _testMessageGlyphs = new GlyphRun(Typeface.Default.GlyphTypeface, 12, TestMessage.AsMemory(), glyphs);

        Bounds = bounds;
        BlocksImage = blocksImage;
        _field = field;
        _cellSize = cellSize;
        _fieldRect = new Rect(0, 0, cellSize * field.Width, cellSize * field.Height);
    }

    public Rect Bounds { get; }
    public Bitmap BlocksImage { get; }
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

    public void Render(ImmediateDrawingContext context)
    {
        if (_testMessageGlyphs.TryCreateImmutableGlyphRunReference() is IImmutableGlyphRunReference gl)
        {
            context.DrawGlyphRun(Brushes.Black, gl);
        }
        for (int x = 0; x < _field.Width; x++)
        {
            for (int y = 0; y < _field.Height; y++)
            {
                double spriteOffset = _field[x, y] == Minefield.State.Hidden ? 0 : BlocksImage.PixelSize.Width * 2;
                if (CurrentMousePos != null && x == CurrentMousePos.Value.X && y == CurrentMousePos.Value.Y)
                {
                    spriteOffset += BlocksImage.PixelSize.Width;
                }
                context.DrawBitmap
                (
                    BlocksImage,
                    new Rect(0, spriteOffset, BlocksImage.PixelSize.Width, BlocksImage.PixelSize.Width),
                    new Rect(x * _cellSize, y * _cellSize, _cellSize, _cellSize)
                );
            }
        }
    }
}
