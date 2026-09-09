using System;
using System.Windows;
using Llyn.Core;
using Llyn.Media;
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
            PIndicator.PIndicatorApply(Resources);
            PCaret.PCaretHook();
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

        engine.LEnginePressApply(new LPressBrowser());

        LBootstrapRescueShow(engine.LEngineRescueRead());

        base.OnStartup(e);

        new PWindow(engine).Show();
    }

    private void LBootstrapRescueShow(LDoctorRescue rescue)
    {
        if (!rescue.LDoctorRescueDone)
        {
            return;
        }

        MessageBox.Show(
            $"{LBootstrapTextRead("Workspace.DatabaseReset")}\n\n{rescue.LDoctorRescueBackup}\n\n{rescue.LDoctorRescueReason}",
            LBootstrapTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    private string LBootstrapTextRead(string key)
    {
        return TryFindResource(key) as string ?? key;
    }
}
