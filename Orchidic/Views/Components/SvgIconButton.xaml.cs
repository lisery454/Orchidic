namespace Orchidic.Views.Components;

public partial class SvgIconButton
{
    public SvgIconButton()
    {
        InitializeComponent();
    }

    // ▶ 图标 Geometry
    public Geometry IconData
    {
        get => (Geometry)GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }
    public static readonly DependencyProperty IconDataProperty =
        DependencyProperty.Register(nameof(IconData), typeof(Geometry), typeof(SvgIconButton));

    // ▶ 默认状态颜色
    public Brush IconBrush
    {
        get => (Brush)GetValue(IconBrushProperty);
        set => SetValue(IconBrushProperty, value);
    }
    public static readonly DependencyProperty IconBrushProperty =
        DependencyProperty.Register(nameof(IconBrush), typeof(Brush), typeof(SvgIconButton));

    // ▶ 悬浮颜色
    public Brush HoverBrush
    {
        get => (Brush)GetValue(HoverBrushProperty);
        set => SetValue(HoverBrushProperty, value);
    }
    public static readonly DependencyProperty HoverBrushProperty =
        DependencyProperty.Register(nameof(HoverBrush), typeof(Brush), typeof(SvgIconButton));

    // ▶ 按下颜色
    public Brush PressedBrush
    {
        get => (Brush)GetValue(PressedBrushProperty);
        set => SetValue(PressedBrushProperty, value);
    }
    public static readonly DependencyProperty PressedBrushProperty =
        DependencyProperty.Register(nameof(PressedBrush), typeof(Brush), typeof(SvgIconButton));


    // ▶ Command
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(SvgIconButton));

    // ▶ CommandParameter
    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(SvgIconButton));
}