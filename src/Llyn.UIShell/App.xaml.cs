using System;
using System.Windows;

namespace Llyn.UIShell;

public partial class LBootstrap : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            PLocalizationLoader.Apply(Resources, PLocalizationLoader.DefaultLanguage);
            PThemeLoader.Apply(Resources);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"The application theme could not be loaded.\n\n{exception.Message}",
                "Llyn",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
            return;
        }

        base.OnStartup(e);
    }
}
