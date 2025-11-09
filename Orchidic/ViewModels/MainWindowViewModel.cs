using Orchidic.Models;
using Orchidic.Utils;
using Orchidic.Utils.ThemeManager;
using Orchidic.ViewModels.Components;

namespace Orchidic.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public SideMenuViewModel SideMenuViewModel { get; } = new();

    private readonly ObservableAsPropertyHelper<ViewModelBase> _currentPageViewModel;
    public ViewModelBase CurrentPageViewModel => _currentPageViewModel.Value;

    private readonly ObservableAsPropertyHelper<BitmapSource?> _cover;
    public BitmapSource? Cover => _cover.Value;

    private static IThemeManager themeManager => App.Current.Services.GetService<IThemeManager>()!;

    public MainWindowViewModel()
    {
        _currentPageViewModel = this.WhenAnyValue(x => x.SideMenuViewModel.PageType)
            .Select<PageType, ViewModelBase>(x =>
            {
                return x switch
                {
                    PageType.Playing => App.Current.Services.GetRequiredService<PlayingPageViewModel>(),
                    PageType.Queue => App.Current.Services.GetRequiredService<QueuePageViewModel>(),
                    PageType.List => App.Current.Services.GetRequiredService<ListPageViewModel>(),
                    PageType.Search => App.Current.Services.GetRequiredService<SearchPageViewModel>(),
                    PageType.Statistics => App.Current.Services.GetRequiredService<StatisticsPageViewModel>(),
                    PageType.Tools => App.Current.Services.GetRequiredService<ToolsPageViewModel>(),
                    PageType.Settings => App.Current.Services.GetRequiredService<SettingsPageViewModel>(),
                    _ => App.Current.Services.GetRequiredService<PlayingPageViewModel>()
                };
            }).ToProperty(this, x => x.CurrentPageViewModel);

        App.Current.Services.GetService<PlayingPageViewModel>().WhenAnyValue(x => x.Cover)
            .Select(original =>
            {
                // 这里设置目标尺寸和模糊半径
                const int targetWidth = 400; // 可根据需要动态调整
                const int targetHeight = 400;
                double blurRadius = themeManager.GetCurrentTheme() == ThemeType.DARK ? 10 : 20;

                // 生成高分辨率平滑模糊图
                return GenerateBlurredCover(original, targetWidth, targetHeight, blurRadius);
            })
            .ToProperty(this, x => x.Cover, out _cover);
    }

    private static BitmapSource GenerateBlurredCover(BitmapSource source, int targetWidth, int targetHeight,
        double blurRadius)
    {
        // 1. 放大倍数，保证模糊平滑
        const double scale = 2.0; // 可调，越大越平滑
        var width = (int)(targetWidth * scale);
        var height = (int)(targetHeight * scale);

        // 2. RenderTargetBitmap 用于绘制模糊后的高分辨率图
        var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);

        // 3. Image 控件承载原图
        var image = new Image
        {
            Source = source,
            Width = width,
            Height = height,
            Stretch = Stretch.UniformToFill,
            // 4. 设置模糊
            Effect = new BlurEffect { Radius = blurRadius }
        };

        // 5. 测量和排列
        image.Measure(new Size(width, height));
        image.Arrange(new Rect(0, 0, width, height));

        // 6. 渲染到 RenderTargetBitmap
        rtb.Render(image);

        // 7. 缩小到目标尺寸，保证放大后模糊平滑
        var finalRtb = new TransformedBitmap(rtb, new ScaleTransform(1 / scale, 1 / scale));

        // 8. 可选：设置 BitmapScalingMode 高质量
        RenderOptions.SetBitmapScalingMode(finalRtb, BitmapScalingMode.Fant);

        return finalRtb;
    }
}