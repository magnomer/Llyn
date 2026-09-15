using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Llyn.Core;
using Llyn.Media;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class LBootstrap : System.Windows.Application
{
    private LEngine? _lBootstrapEngine;

    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            PLocalizationLoader.PLocalizationLoaderApply(Resources, PLocalizationLoader.PLocalizationLoaderLanguage);
            PThemeLoader.PThemeLoaderApply(Resources);
            PField.PFieldApply(Resources);
            PIndicator.PIndicatorApply(Resources);
            PCaret.PCaretHook();
            PSwath.PSwathHook();
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
            string key = LEngine.LEngineBusyCheck(exception) ? "Workspace.Busy" : "Workspace.OpenFailed";
            MessageBox.Show(
                $"{LBootstrapTextRead(key)}\n\n{workspace}\n\n{exception.Message}",
                LBootstrapTextRead("Terms.Product"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
            return;
        }

        _lBootstrapEngine = engine;
        DispatcherUnhandledException += LBootstrapFaultHandle;
        TaskScheduler.UnobservedTaskException += LBootstrapStrayHandle;

        engine.LEnginePressApply(new LPressBrowser());

        LBootstrapLocalizationApply(engine);
        LBootstrapRescueShow(engine.LEngineRescueRead());

        base.OnStartup(e);

        new PWindow(engine).Show();
    }

    private void LBootstrapLocalizationApply(LEngine engine)
    {
        string language = PLocalizationLoader.PLocalizationLoaderNormalize(
            engine.LEngineSettingsRead().LSettingsLocalization);
        if (string.Equals(language, PLocalizationLoader.PLocalizationLoaderLanguage, StringComparison.Ordinal))
        {
            return;
        }

        try
        {
            PLocalizationLoader.PLocalizationLoaderApply(Resources, language);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"The application language could not be loaded.\n\n{exception.Message}",
                LBootstrapTextRead("Terms.Product"),
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    private void LBootstrapFaultHandle(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        string? log = _lBootstrapEngine?.LEngineAuditRecord(e.Exception);
        e.Handled = true;

        MessageBox.Show(
            $"{LBootstrapTextRead("Workspace.Fault")}\n\n{log}\n\n{e.Exception.Message}",
            LBootstrapTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    private void LBootstrapStrayHandle(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        _lBootstrapEngine?.LEngineAuditRecord(e.Exception);
        e.SetObserved();
    }

    private void LBootstrapRescueShow(LDoctorRescue rescue)
    {
        if (!rescue.LDoctorRescueDone)
        {
            return;
        }

        MessageBox.Show(
            $"{LBootstrapTextRead("Workspace.DatabaseReset")}\n\n"
                + $"{rescue.LDoctorRescueBackup}\n\n{rescue.LDoctorRescueReason}",
            LBootstrapTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    private string LBootstrapTextRead(string key)
    {
        return TryFindResource(key) as string ?? key;
    }
}
