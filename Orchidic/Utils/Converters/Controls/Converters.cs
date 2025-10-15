using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Orchidic.Models;

namespace Orchidic.Utils.Converters;

public class IsSelectedConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter,
        CultureInfo culture)
    {
        var item = values[0];
        var selected = values[1];
        return Equals(item, selected);
    }
}

public class PageTitleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is PageType pageType)
        {
            return pageType switch
            {
                PageType.Playing => "正在播放",
                PageType.Queue => "队列",
                PageType.List => "列表",
                PageType.Search => "搜索",
                PageType.Statistics => "统计",
                PageType.Tools => "工具",
                PageType.Settings => "设置",
                _ => "default"
            };
        }

        return BindingOperations.DoNothing;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}