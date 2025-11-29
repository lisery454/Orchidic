namespace Orchidic.Utils;

public static class DoubleClickBehavior
    {
        // 双击命令
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(DoubleClickBehavior),
                new PropertyMetadata(null, OnCommandChanged));

        public static void SetCommand(UIElement element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(UIElement element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }

        // 命令参数
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "CommandParameter",
                typeof(object),
                typeof(DoubleClickBehavior),
                new PropertyMetadata(null));

        public static void SetCommandParameter(UIElement element, object value)
        {
            element.SetValue(CommandParameterProperty, value);
        }

        public static object GetCommandParameter(UIElement element)
        {
            return element.GetValue(CommandParameterProperty);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.PreviewMouseLeftButtonDown -= ElementOnPreviewMouseLeftButtonDown;
                if (e.NewValue != null)
                {
                    element.PreviewMouseLeftButtonDown += ElementOnPreviewMouseLeftButtonDown;
                }
            }
        }

        private static void ElementOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && sender is UIElement element)
            {
                var command = GetCommand(element);
                var parameter = GetCommandParameter(element);

                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }
        }
    }