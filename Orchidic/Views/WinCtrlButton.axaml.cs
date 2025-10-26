using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Orchidic.Views;

public partial class WinCtrlButton : UserControl
{
    public WinCtrlButton()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static readonly StyledProperty<double> SizeProperty =
        AvaloniaProperty.Register<WinCtrlButton, double>(nameof(Size), 24);

    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public static readonly StyledProperty<IBrush?> FillColorProperty =
        AvaloniaProperty.Register<WinCtrlButton, IBrush?>(nameof(FillColor), Brushes.Gray);

    public IBrush? FillColor
    {
        get => GetValue(FillColorProperty);
        set => SetValue(FillColorProperty, value);
    }

    public event EventHandler<RoutedEventArgs>? Click;

    private void InnerButton_Click(object? sender, RoutedEventArgs e)
    {
        Click?.Invoke(this, e);
    }


    // 悬浮颜色
    public static readonly StyledProperty<IBrush?> HoverColorProperty =
        AvaloniaProperty.Register<WinCtrlButton, IBrush?>(nameof(HoverColor), Brushes.LightGray);

    public IBrush? HoverColor
    {
        get => GetValue(HoverColorProperty);
        set => SetValue(HoverColorProperty, value);
    }

    // 按下颜色
    public static readonly StyledProperty<IBrush?> PressedColorProperty =
        AvaloniaProperty.Register<WinCtrlButton, IBrush?>(nameof(PressedColor), Brushes.DarkGray);

    public IBrush? PressedColor
    {
        get => GetValue(PressedColorProperty);
        set => SetValue(PressedColorProperty, value);
    }

    // 缩放比例
    public static readonly StyledProperty<double> PressedScaleProperty =
        AvaloniaProperty.Register<WinCtrlButton, double>(nameof(PressedScale), 0.85);

    public double PressedScale
    {
        get => GetValue(PressedScaleProperty);
        set => SetValue(PressedScaleProperty, value);
    }
}