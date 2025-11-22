namespace Orchidic.Utils;

public static class AnimatedWidthBehavior
{
    public static readonly DependencyProperty AnimatedWidthProperty =
        DependencyProperty.RegisterAttached(
            "AnimatedWidth",
            typeof(double),
            typeof(AnimatedWidthBehavior),
            new PropertyMetadata(double.NaN, OnAnimatedWidthChanged));

    public static void SetAnimatedWidth(DependencyObject obj, double value)
        => obj.SetValue(AnimatedWidthProperty, value);

    public static double GetAnimatedWidth(DependencyObject obj)
        => (double)obj.GetValue(AnimatedWidthProperty);

    private static void OnAnimatedWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element)
        {
            double newWidth = (double)e.NewValue;

            // 🚀 关键点（防止 NaN 动画起始报错）
            if (double.IsNaN(element.Width))
                element.Width = element.ActualWidth;

            var animation = new DoubleAnimation
            {
                To = newWidth,
                Duration = TimeSpan.FromSeconds(0.15),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            element.BeginAnimation(FrameworkElement.WidthProperty, animation, HandoffBehavior.SnapshotAndReplace);
        }
    }
}
