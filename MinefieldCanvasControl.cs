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

public class GamePreference
{
    public GamePreference(int mineCount, int width, int height)
    {
        MineCount = mineCount;
        Width = width;
        Height = height;
    }

    public int MineCount { get; }
    public int Width { get; }
    public int Height { get; }
}
public class MinefieldCanvasControl : Control
{
    private Minefield _field;
    public Minefield Field => _field;
    private readonly Bitmap? _blocksImage;

    public static readonly GamePreference EasyGamemode = new GamePreference(10, 9, 9);
    public static readonly GamePreference MediumGamemode = new GamePreference(40, 16, 16);
    public static readonly GamePreference HardGamemode = new GamePreference(99, 16, 30);

    private double _cellSize = 16;

    private int _visitedCellsCount = 0;

    public bool Cheating { get; set; } = false;
    private bool _gameOver = false;

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

        StartGame();

        PointerPressed += FieldClicked;

    }


    /// <summary>
    /// Stars a new game using data from preferences
    /// </summary>
    public void StartGame()
    {
#if !DEBUG
        Random rand = new Random((int)DateTime.Now.Ticks);
#else
        Random rand = new Random();
#endif
        _visitedCellsCount = 0;
        _gameOver = false;
        _field = new Minefield(EasyGamemode.Width, EasyGamemode.Height);
        int bombCount = EasyGamemode.MineCount;
        int x = 0;
        int y = 0;
        do
        {
            do
            {
                x = rand.Next(_field.Width);
                y = rand.Next(_field.Height);
            } while (_field[x, y].IsBomb);
            _field[x, y].IsBomb = true;
        } while ((--bombCount) > 0);
    }


    /// <summary>
    /// Moves the the bomb into a new location<para/>
    /// Intended to be used to prevent loss on first click if player accidentally clicks a bomb on their first click
    /// </summary>
    /// <param name="x">x coord of the bomb</param>
    /// <param name="y">y coord of the bom</param>
    /// <returns>True if managed to move the bomb, false if not. To allow for first click loss if player messed with settings</returns>
    private bool FixFirstClickBomb(int x, int y)
    {
        // move bomb away from player to prevent first click loss
        for (int newX = 0; newX < _field.Width; newX++)
        {
            for (int newY = 0; newY < _field.Width; newY++)
            {
                if (!_field[newX, newY].IsBomb)
                {
                    _field[newX, newY].IsBomb = true;
                    _field[x, y].IsBomb = false;
                    return true;
                }
            }
        }
        return false;
    }

    private void LooseGame()
    {
        _gameOver = true;
        _field.RevealAllBombs();
    }

    private void StepBlock(int startX, int startY)
    {
        FieldCell? cell = _field[startX, startY];
        if (cell == null || cell.IsBomb || cell.State == CellState.Revealed)
        {
            return;
        }
        RevealCell(startX, startY);
        if (cell.BombCount == 0)
        {
            StepBlock(startX - 1, startY);
            StepBlock(startX + 1, startY);
            StepBlock(startX, startY - 1);
            StepBlock(startX, startY + 1);
        }
    }

    private void RevealCell(int x, int y)
    {
        FieldCell? cell = _field[x, y];
        if (cell == null)
        {
            return;
        }
        cell.State = CellState.Revealed;
        // no point in counting for bombs
        if (cell.IsBomb)
        {
            return;
        }
        cell.BombCount = 0;
        for (int cX = x - 1; cX <= x + 1; cX++)
        {
            for (int cY = y - 1; cY <= y + 1; cY++)
            {
                FieldCell? temp = _field[cX, cY];
                if (temp != null && temp.IsBomb)
                {
                    cell.BombCount++;
                }
            }
        }
    }

    private void FieldClicked(object? sender, PointerPressedEventArgs e)
    {
        if (_gameOver)
        {
            return;
        }
        Point clickPoint = e.GetPosition(this);
        int x = (int)clickPoint.X / (int)CellSize;
        int y = (int)clickPoint.Y / (int)CellSize;

        if (_field[x, y].IsBomb)
        {
            if (_visitedCellsCount == 0)
            {
                if (!FixFirstClickBomb(x, y))
                {
                    LooseGame();
                }
            }
            else
            {
                LooseGame();
            }
        }
        // placeholder
        StepBlock(x, y);
        _visitedCellsCount++;
        Console.WriteLine($"CLICK at ({clickPoint.X}, {clickPoint.Y})");
    }

    public override void Render(DrawingContext context)
    {
        context.Custom(new FieldDrawingOperation(
            new Rect(0, 0, Bounds.Width, Bounds.Height),
            _blocksImage,
            _field,
            _cellSize,
#if DEBUG
            Cheating
#else
            false
#endif
            ));
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }
}