using System.Windows;
using Orchidic.Utils;

namespace Orchidic;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        Loaded += Window_Loaded;
        WindowCornerRestorer.ApplyRoundCorner(this);
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        WindowAnimRestorer.AddAnimTo(this);
    }
}