using System.Windows.Controls;
using System.Windows.Media;

namespace Orchidic.Utils;

public class SuperellipseBorder : Decorator
{
    #region Dependency Properties

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(double),
            typeof(SuperellipseBorder),
            new FrameworkPropertyMetadata(16.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty PowerProperty =
        DependencyProperty.Register(
            nameof(Power),
            typeof(double),
            typeof(SuperellipseBorder),
            new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty FillProperty =
        DependencyProperty.Register(
            nameof(Fill),
            typeof(Brush),
            typeof(SuperellipseBorder),
            new FrameworkPropertyMetadata(Brushes.LightGray, FrameworkPropertyMetadataOptions.AffectsRender));

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public double Power
    {
        get => (double)GetValue(PowerProperty);
        set => SetValue(PowerProperty, value);
    }

    public Brush Fill
    {
        get => (Brush)GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    #endregion

    private Geometry? _clipGeometry;

    protected override Size ArrangeOverride(Size finalSize)
    {
        // 在布局时计算 clip 几何体
        _clipGeometry = CreateSuperellipseGeometry(new Rect(finalSize), CornerRadius, Power);
        if (Child != null)
            Child.Clip = _clipGeometry;

        return base.ArrangeOverride(finalSize);
    }

    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);
        _clipGeometry ??= CreateSuperellipseGeometry(new Rect(RenderSize), CornerRadius, Power);

        dc.DrawGeometry(Fill, null, _clipGeometry);
    }

    private static StreamGeometry CreateSuperellipseGeometry(Rect rect, double radius, double power)
    {
        const double halfPi = Math.PI / 2;
        var geo = new StreamGeometry();
        using (var ctx = geo.Open())
        {
            var w = rect.Width;
            var h = rect.Height;
            const int steps = 30;
            Point pt;

            pt = new Point(radius, 0);
            ctx.BeginFigure(pt, true, true);

            // top edge
            pt = new Point(w - radius, 0);
            ctx.LineTo(pt, true, false);

            // top-right arc
            for (int i = 0; i < steps; i++)
            {
                var t = halfPi * i / steps;
                var dx = radius * Math.Pow(Math.Abs(Math.Sin(t)), 2 / power);
                var dy = radius * Math.Pow(Math.Abs(Math.Cos(t)), 2 / power);
                var x = w - radius + dx;
                var y = radius - dy;
                ctx.LineTo(new Point(x, y), true, false);
            }

            // right edge
            ctx.LineTo(new Point(w, h - radius), true, false);

            // bottom-right arc
            for (int i = 0; i < steps; i++)
            {
                var t = halfPi * i / steps;
                var dx = radius * Math.Pow(Math.Abs(Math.Cos(t)), 2 / power);
                var dy = radius * Math.Pow(Math.Abs(Math.Sin(t)), 2 / power);
                var x = w - radius + dx;
                var y = h - radius + dy;
                ctx.LineTo(new Point(x, y), true, false);
            }

            // bottom edge
            ctx.LineTo(new Point(radius, h), true, false);

            // bottom-left arc
            for (int i = 0; i < steps; i++)
            {
                var t = halfPi * i / steps;
                var dx = radius * Math.Pow(Math.Abs(Math.Sin(t)), 2 / power);
                var dy = radius * Math.Pow(Math.Abs(Math.Cos(t)), 2 / power);
                var x = radius - dx;
                var y = h - radius + dy;
                ctx.LineTo(new Point(x, y), true, false);
            }

            // left edge
            ctx.LineTo(new Point(0, radius), true, false);

            // top-left arc
            for (int i = 0; i < steps; i++)
            {
                var t = halfPi * i / steps;
                var dx = radius * Math.Pow(Math.Abs(Math.Cos(t)), 2 / power);
                var dy = radius * Math.Pow(Math.Abs(Math.Sin(t)), 2 / power);
                var x = radius - dx;
                var y = radius - dy;
                ctx.LineTo(new Point(x, y), true, false);
            }

            ctx.Close();
        }

        geo.Freeze();
        return geo;
    }
}