using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Orchidic.Controls;

public partial class SideMenu : UserControl
{
    public SideMenu()
    {
        InitializeComponent();
    }

    #region AttachedProperty

    public static readonly AttachedProperty<bool> IsItemSelectedProperty =
        AvaloniaProperty.RegisterAttached<SideMenu, Control, bool>(
            "IsItemSelected",
            defaultValue: false);

    public static void SetIsItemSelected(AvaloniaObject element, bool value) =>
        element.SetValue(IsItemSelectedProperty, value);

    public static bool GetIsItemSelected(AvaloniaObject element) =>
        element.GetValue(IsItemSelectedProperty);

    #endregion

    #region Converters

    public static readonly IValueConverter IndexToOffsetConverterIns = new IndexToOffsetConverter();

    private class IndexToOffsetConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int index)
                return index * 60.0; // 每项高度 60
            return 0.0;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => BindingOperations.DoNothing;
    }
    
    public static readonly IMultiValueConverter IsSelectedConverterIns = new IsSelectedConverter();
    
    private class IsSelectedConverter : IMultiValueConverter
    {
        public object Convert(IList<object?> values, Type targetType, object? parameter,
            CultureInfo culture)
        {
            var item = values[0];
            var selected = values[1];
            return Equals(item, selected);
        }
    }

    #endregion
}