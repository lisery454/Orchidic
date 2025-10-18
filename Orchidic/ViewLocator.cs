using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Orchidic.Utils;

namespace Orchidic;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        if (data == null) return new TextBlock { Text = "ViewModel is null!" };
        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        // Lisery.ViewModels.AAAViewModel -> Lisery.Views.AAAView
        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }
        
        // Lisery.Views.AAAView -> Lisery.Views.AAA
        var name2 = string.Empty;
        if (name.EndsWith("View"))
        {
            name2 = name![..^4];
            var type2 = Type.GetType(name2);
            if (type2 != null)
            {
                return (Control)Activator.CreateInstance(type2)!;
            }
        }

        return new TextBlock { Text = "Not Found: " + name + " or " + name2 };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}