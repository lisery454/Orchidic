namespace Orchidic.Views.Components;

public partial class SvgIcon : UserControl
{
    public SvgIcon()
    {
        InitializeComponent();
    }

    // Geometry 属性
    public static readonly DependencyProperty GeometryProperty =
        DependencyProperty.Register(
            nameof(Geometry),
            typeof(Geometry),
            typeof(SvgIcon),
            new PropertyMetadata(null));

    public Geometry Geometry
    {
        get => (Geometry)GetValue(GeometryProperty);
        set => SetValue(GeometryProperty, value);
    }

    // Fill 属性
    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register(
            nameof(Fill),
            typeof(Brush),
            typeof(SvgIcon),
            new PropertyMetadata(Brushes.Black));

    public Brush Fill
    {
        get => (Brush)GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }
}