using System;
using System.Windows;
using Llyn.ShellEngine;

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
            // The catalog is what would have translated this message, so this one stays inline: it is
            // the documented emergency fallback, shown only when the interface text is unavailable.
            MessageBox.Show(
                $"The application theme could not be loaded.\n\n{exception.Message}",
                "Llyn",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
            return;
        }

        // The engine opens the workspace — settings, database file, schema — so it is built here rather
        // than in a field initializer of the window: an unwritable folder, a corrupt database, or a file
        // from a newer build must reach the user as a message naming the folder, not as a crash inside a
        // constructor no one is watching.
        string workspace = string.Empty;
        LEngine engine;
        try
        {
            workspace = LEngine.LEngineWorkspaceResolve();
            engine = new LEngine(workspace);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"{LBootstrapTextRead("Workspace.OpenFailed")}\n\n{workspace}\n\n{exception.Message}",
                LBootstrapTextRead("Terms.Product"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
            return;
        }

        base.OnStartup(e);

        // The window owns the engine from here: it disposes it when it closes.
        new PWindow(engine).Show();
    }

    private string LBootstrapTextRead(string key)
    {
        return TryFindResource(key) as string ?? key;
    }
}
