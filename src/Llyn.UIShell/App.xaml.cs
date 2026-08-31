using System;
using System.Windows;

namespace Llyn.UIShell;

public partial class LBootstrap : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            PLocalizationLoader.PLocalizationLoaderApply(Resources, PLocalizationLoader.PLocalizationLoaderLanguage);
            PThemeLoader.PThemeLoaderApply(Resources);
            PField.PFieldApply(Resources);
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
