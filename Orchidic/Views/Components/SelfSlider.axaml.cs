using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;

namespace Orchidic.Views.Components;

public partial class SelfSlider : UserControl
{
    public static readonly StyledProperty<double> ProgressProperty =
        AvaloniaProperty.Register<SelfSlider, double>(nameof(Progress), defaultBindingMode: BindingMode.TwoWay,
            defaultValue: 0.0);

    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    private double ProgressBarWidth { get; set; }

    public static readonly StyledProperty<bool> CancelDragRequestedProperty =
        AvaloniaProperty.Register<SelfSlider, bool>(nameof(CancelDragRequested),
            defaultBindingMode: BindingMode.TwoWay);

    public bool CancelDragRequested
    {
        get => GetValue(CancelDragRequestedProperty);
        set => SetValue(CancelDragRequestedProperty, value);
    }


    public SelfSlider()
    {
        InitializeComponent();

        ProgressBarBg.GetObservable(BoundsProperty)
            .Subscribe(bounds => { ProgressBarWidth = bounds.Width; });

        this.GetObservable(CancelDragRequestedProperty).Subscribe(value =>
        {
            if (value)
            {
                CancelDrag();
                // 复位，避免循环触发
                CancelDragRequested = false;
            }
        });
    }

    private bool _isDragging;
    private IPointer? _pointer;

    private void ProgressBar_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isDragging = true;

            Progress = Math.Clamp(e.GetPosition(sender as Border).X / ProgressBarWidth, 0, 1);
            _pointer = e.Pointer;
            e.Pointer.Capture((IInputElement)sender!); // 捕获鼠标
        }
    }

    private void ProgressBar_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            Progress = Math.Clamp(e.GetPosition(sender as Border).X / ProgressBarWidth, 0, 1);
            _pointer = null;
            e.Pointer.Capture(null); // 释放捕获
        }
    }

    private void ProgressBar_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging)
        {
            _pointer = e.Pointer;
            Progress = Math.Clamp(e.GetPosition(sender as Border).X / ProgressBarWidth, 0, 1);
        }
    }

    public void CancelDrag()
    {
        if (_isDragging)
        {
            _isDragging = false;
            // 解除鼠标捕获，避免残留状态
            _pointer?.Capture(null);
            _pointer = null;
        }
    }
}

public class ProgressToWidthConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return 0d;

        if (values[0] is not double totalWidth || double.IsNaN(totalWidth))
            return 0d;

        if (values[1] is not double progress)
            return 0d;

        // 进度一般在 [0,1]
        progress = Math.Clamp(progress, 0, 1);
        return totalWidth * progress;
    }
}