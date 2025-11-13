namespace Orchidic.Views.Components;

public partial class SelfSlider
{
    public static readonly DependencyProperty ProgressProperty =
        DependencyProperty.Register(
            nameof(Progress),
            typeof(float),
            typeof(SelfSlider),
            new FrameworkPropertyMetadata(
                0f,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
            )
        );

    public float Progress
    {
        get => (float)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    private double ProgressBarWidth { get; set; }

    public static readonly DependencyProperty CancelDragRequestedProperty =
        DependencyProperty.Register(
            nameof(CancelDragRequested),
            typeof(bool),
            typeof(SelfSlider),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnCancelDragRequestedChanged
            )
        );

    public bool CancelDragRequested
    {
        get => (bool)GetValue(CancelDragRequestedProperty);
        set => SetValue(CancelDragRequestedProperty, value);
    }


    public SelfSlider()
    {
        InitializeComponent();

        ProgressBarBg.SizeChanged += (_, _) => { ProgressBarWidth = ProgressBarBg.ActualWidth; };
    }

    private static void OnCancelDragRequestedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SelfSlider slider && e.NewValue is true)
        {
            slider.CancelDrag();
            slider.CancelDragRequested = false; // 避免循环触发
        }
    }

    private bool _isDragging;

    private void ProgressBar_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            _isDragging = true;

            if (sender is Border border)
            {
                Progress = (float)Math.Clamp(e.GetPosition(border).X / ProgressBarWidth, 0, 1);
                Mouse.Capture(border); // 捕获鼠标
            }
        }
    }

    private void ProgressBar_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;

            if (sender is Border border)
            {
                // Progress = Math.Clamp(e.GetPosition(border).X / ProgressBarWidth, 0, 1);
                Mouse.Capture(null); // 释放捕获
            }
        }
    }

    private void ProgressBar_OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_isDragging && sender is Border border)
        {
            Progress = (float)Math.Clamp(e.GetPosition(border).X / ProgressBarWidth, 0, 1);
        }
    }

    public void CancelDrag()
    {
        if (_isDragging)
        {
            _isDragging = false;
            // 解除鼠标捕获，避免残留状态
            Mouse.Capture(null);
        }
    }
}

public class ProgressToWidthConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2)
            return 0d;

        if (values[0] is not double totalWidth || double.IsNaN(totalWidth))
            return 0d;

        if (values[1] is not float progress)
            return 0d;

        // 进度一般在 [0,1]
        progress = Math.Clamp(progress, 0, 1);
        return totalWidth * progress;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return [];
    }
}