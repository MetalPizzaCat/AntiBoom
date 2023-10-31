using Avalonia.Controls;

namespace AntiBoom;

public partial class MainWindow : Window
{
    private bool _scaleOption100Selected = false;
    private bool _scaleOption150Selected = true;
    private bool _scaleOption200Selected = false;

    public bool ScaleOption100Selected
    {
        get => _scaleOption100Selected;
        set { _scaleOption100Selected = value; SetFieldCellSize(24); }
    }
    public bool ScaleOption150Selected
    {
        get => _scaleOption150Selected;
        set { _scaleOption150Selected = value; SetFieldCellSize(32); }
    }
    public bool ScaleOption200Selected
    {
        get => _scaleOption200Selected;
        set { _scaleOption200Selected = value; SetFieldCellSize(48); }
    }

    private void SetFieldCellSize(int size)
    {
        MinefieldCanvas.CellSize = size;
        // Width = MinefieldCanvas.CellSize * MinefieldCanvas.Field.Width;
        // Height = MinefieldCanvas.CellSize * MinefieldCanvas.Field.Height;
    }
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        SetFieldCellSize(24);
    }
}