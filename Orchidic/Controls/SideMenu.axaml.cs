using Avalonia;
using Avalonia.Controls;

namespace Orchidic.Controls;

public partial class SideMenu : UserControl
{
    public SideMenu()
    {
        InitializeComponent();
    }

    public static readonly AttachedProperty<bool> IsItemSelectedProperty =
        AvaloniaProperty.RegisterAttached<SideMenu, Control, bool>(
            "IsItemSelected",
            defaultValue: false);

    // ✅ 标准的 getter / setter
    public static void SetIsItemSelected(AvaloniaObject element, bool value) =>
        element.SetValue(IsItemSelectedProperty, value);

    public static bool GetIsItemSelected(AvaloniaObject element) =>
        element.GetValue(IsItemSelectedProperty);
}