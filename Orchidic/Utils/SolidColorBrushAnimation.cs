using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Orchidic.Utils;

public class SolidColorBrushAnimation : AnimationTimeline
{
    public override Type TargetPropertyType => typeof(SolidColorBrush);

    public override object GetCurrentValue(
        object defaultOriginValue,
        object defaultDestinationValue,
        AnimationClock animationClock)
    {
        return GetCurrentValue((defaultOriginValue as SolidColorBrush)!,
            (defaultDestinationValue as SolidColorBrush)!,
            animationClock);
    }

    private object GetCurrentValue(SolidColorBrush defaultOriginValue,
        SolidColorBrush defaultDestinationValue,
        AnimationClock animationClock)
    {
        if (!animationClock.CurrentProgress.HasValue)
            return Brushes.Transparent;

        var originValue = From ?? defaultOriginValue;
        var destinationValue = To ?? defaultDestinationValue;

        if (animationClock.CurrentProgress.Value == 0)
            return originValue;
        if (Math.Abs(animationClock.CurrentProgress.Value - 1) < 0.001d)
            return destinationValue;

        var originColor = originValue.Color;
        var destinationColor = destinationValue.Color;
        var progress = (float)animationClock.CurrentProgress.Value;

        var finalColor = Interpolation(originColor, destinationColor, progress);
        return new SolidColorBrush
        {
            Color = finalColor
        };
    }

    private Color Interpolation(Color color0, Color color1, float progress)
    {
        var scA = color0.ScA * (1 - progress) + color1.ScA * progress;
        var scR = color0.ScR * (1 - progress) + color1.ScR * progress;
        var scG = color0.ScG * (1 - progress) + color1.ScG * progress;
        var scB = color0.ScB * (1 - progress) + color1.ScB * progress;
        return Color.FromScRgb(scA, scR, scG, scB);
    }

    protected override Freezable CreateInstanceCore()
    {
        return new SolidColorBrushAnimation();
    }

    public SolidColorBrush? From
    {
        get => (SolidColorBrush)GetValue(FromProperty);
        set => SetValue(FromProperty, value);
    }

    public SolidColorBrush? To
    {
        get => (SolidColorBrush)GetValue(ToProperty);
        set => SetValue(ToProperty, value);
    }

    public static readonly DependencyProperty FromProperty =
        DependencyProperty.Register(nameof(From), typeof(SolidColorBrush), typeof(SolidColorBrushAnimation));

    public static readonly DependencyProperty ToProperty =
        DependencyProperty.Register(nameof(To), typeof(SolidColorBrush), typeof(SolidColorBrushAnimation));
}