namespace Orchidic.Utils;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

public class SuperellipseBorder : Decorator
{
    #region Fields

    public static readonly StyledProperty<double> CornerRadiusProperty =
        AvaloniaProperty.Register<SuperellipseBorder, double>(nameof(CornerRadius), 16);

    public static readonly StyledProperty<double> PowerProperty =
        AvaloniaProperty.Register<SuperellipseBorder, double>(nameof(CornerRadius), 10);

    public static readonly StyledProperty<IBrush?> FillProperty =
        AvaloniaProperty.Register<SuperellipseBorder, IBrush?>(nameof(Fill));


    public double CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public double Power
    {
        get => GetValue(PowerProperty);
        set => SetValue(PowerProperty, value);
    }

    public IBrush? Fill
    {
        get => GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    #endregion


    private StreamGeometry? _clipGeometry;

    public SuperellipseBorder()
    {
        this.GetObservable(FillProperty).Subscribe(_ => InvalidateVisual());
        this.GetObservable(PowerProperty).Subscribe(_ => InvalidateVisual());
        this.GetObservable(CornerRadiusProperty).Subscribe(_ => InvalidateVisual());
    }


    protected override Size ArrangeOverride(Size finalSize)
    {
        // 布局时准备裁剪路径
        _clipGeometry = CreateSuperellipseGeometry(new Rect(finalSize), CornerRadius, Power);
        if (Child != null)
            Child.Clip = _clipGeometry;
        return base.ArrangeOverride(finalSize);
    }

    public override void Render(DrawingContext context)
    {
        _clipGeometry ??= CreateSuperellipseGeometry(new Rect(Bounds.Size), CornerRadius, Power);

        if (Fill != null)
            context.DrawGeometry(Fill, null, _clipGeometry);
    }

    private static StreamGeometry CreateSuperellipseGeometry(Rect rect, double radius, double power)
    {
        const double halfPi = Math.PI / 2;

        var geo = new StreamGeometry();
        using var ctx = geo.Open();

        var w = rect.Width;
        var h = rect.Height;
        const int steps = 30;

        // x = r ~ W - r; y = 0 直线
        var pt = new Point(radius, 0);
        ctx.BeginFigure(pt, true);
        pt = new Point(w - radius, 0);
        ctx.LineTo(pt);
        // x = W - r ~ W; y = 0 ~ r 圆弧
        for (var i = 0; i < steps; i++)
        {
            var t = halfPi * i / steps; // 四分之一个圆弧
            // dx ^ n + dy ^ n = radius ^ 2
            var dx = radius * Math.Pow(Math.Abs(Math.Sin(t)), 2 / power);
            var dy = radius * Math.Pow(Math.Abs(Math.Cos(t)), 2 / power);
            var x = w - radius + dx;
            var y = radius - dy;
            pt = new Point(x, y);
            ctx.LineTo(pt);
        }

        // x = W; y = r ~ H - r 直线
        pt = new Point(w, h - radius);
        ctx.LineTo(pt);
        // x = W ~ W - r; y = H - r ~ H 圆弧
        for (var i = 0; i < steps; i++)
        {
            var t = halfPi * i / steps; // 四分之一个圆弧
            // dx ^ n + dy ^ n = radius ^ 2
            var dx = radius * Math.Pow(Math.Abs(Math.Cos(t)), 2 / power);
            var dy = radius * Math.Pow(Math.Abs(Math.Sin(t)), 2 / power);
            var x = w - radius + dx;
            var y = h - radius + dy;
            pt = new Point(x, y);
            ctx.LineTo(pt);
        }

        // x = W - r ~ r; y = H 直线
        pt = new Point(radius, h);
        ctx.LineTo(pt);
        // x = r ~ 0; y = H ~ H - r 圆弧
        for (var i = 0; i < steps; i++)
        {
            var t = halfPi * i / steps; // 四分之一个圆弧
            // dx ^ n + dy ^ n = radius ^ 2
            var dx = radius * Math.Pow(Math.Abs(Math.Sin(t)), 2 / power);
            var dy = radius * Math.Pow(Math.Abs(Math.Cos(t)), 2 / power);
            var x = radius - dx;
            var y = h - radius + dy;
            pt = new Point(x, y);
            ctx.LineTo(pt);
        }

        // x = 0; y = H - r ~ r 直线
        pt = new Point(0, radius);
        ctx.LineTo(pt);
        // x = 0 ~ r; y = r ~ 0 圆弧
        for (var i = 0; i < steps; i++)
        {
            var t = halfPi * i / steps; // 四分之一个圆弧
            // dx ^ n + dy ^ n = radius ^ 2
            var dx = radius * Math.Pow(Math.Abs(Math.Cos(t)), 2 / power);
            var dy = radius * Math.Pow(Math.Abs(Math.Sin(t)), 2 / power);
            var x = radius - dx;
            var y = radius - dy;
            pt = new Point(x, y);
            ctx.LineTo(pt);
        }

        ctx.EndFigure(true);
        return geo;
    }
}