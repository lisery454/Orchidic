namespace Orchidic.Views.Components;

public partial class SideMenu
{
    public SideMenu()
    {
        InitializeComponent();
    }

    public static readonly IValueConverter IndexToOffsetConverterIns = new IndexToOffsetConverter();
}

public static class SmoothCanvasBehavior
{
    public static readonly DependencyProperty TopProperty =
        DependencyProperty.RegisterAttached(
            "Top", typeof(double), typeof(SmoothCanvasBehavior),
            new PropertyMetadata(0.0, OnTopChanged));

    public static void SetTop(DependencyObject element, double value)
    {
        element.SetValue(TopProperty, value);
    }

    public static double GetTop(DependencyObject element)
    {
        return (double)element.GetValue(TopProperty);
    }

    private static void OnTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element)
        {
            var from = Canvas.GetTop(element);
            var to = (double)e.NewValue;

            // 防止初次加载动画
            if (double.IsNaN(from)) from = to;

            var anim = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromSeconds(0.15),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            // 直接对 Canvas.Top 进行动画
            element.BeginAnimation(Canvas.TopProperty, anim);
        }
    }
}

public class IndexToOffsetConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int index)
            return index * 60.0; // 每项高度 60
        return 0.0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}