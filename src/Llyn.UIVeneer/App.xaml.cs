using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.Media;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

public partial class LBootstrap : System.Windows.Application
{
    private readonly HttpClient _lBootstrapClient = LRigFactory.LRigClientCreate();

    private readonly LUsher _lBootstrapUsher = new LUsherShell(new LUsherFile());

    private LEngine? _lBootstrapEngine;

    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            PLocalizationCatalog.PLocalizationCatalogApply(
                Resources,
                LLocalization.LLocalizationLoad(new LLocalizationLoader(), LLocalization.LLocalizationDefault));
            PThemeLoader.PThemeLoaderApply(LThemeLoader.LThemeLoaderLoad().LThemeColorRead, Resources);
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
            workspace = LWorkspaceRoot.LWorkspaceRootRead();
            engine = new LEngine(LRigFactory.LRigFactoryBuild(workspace, _lBootstrapClient, _lBootstrapUsher));
        }
        catch (Exception exception)
        {
            string key = LDoctor.LDoctorBusyCheck(exception) ? "Workspace.Busy" : "Workspace.OpenFailed";
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

        LWindow window = new(new LPosture(engine), engine, engine, engine, engine, engine, engine);
        new PWindow(window, LBootstrapWorkspaceChange).Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _lBootstrapEngine?.Dispose();
        _lBootstrapClient.Dispose();
        base.OnExit(e);
    }

    private void LBootstrapWorkspaceChange(string path)
    {
        _lBootstrapEngine?.LEngineRigApply(LRigFactory.LRigFactoryBuild(path, _lBootstrapClient, _lBootstrapUsher));
        LWorkspaceRoot.LWorkspaceRootChange(path);
    }

    private void LBootstrapLocalizationApply(LEngine engine)
    {
        if (LLocalization.LLocalizationDefaultCheck(engine.LEngineSettingsRead().LSettingsLocalization))
        {
            return;
        }

        try
        {
            PLocalizationCatalog.PLocalizationCatalogApply(
                Resources,
                engine.LEngineLocalizationLoad(
                    LLocalization.LLocalizationNormalize(engine.LEngineSettingsRead().LSettingsLocalization)));
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

        if (!LBootstrapWindowCheck())
        {
            Shutdown(1);
        }
    }

    private bool LBootstrapWindowCheck()
    {
        foreach (Window window in Windows)
        {
            if (window.IsVisible)
            {
                return true;
            }
        }

        return false;
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

        string reset = LBootstrapTextRead("Workspace.DatabaseReset");
        MessageBox.Show(
            $"{reset}\n\n{rescue.LDoctorRescueBackup}\n\n{rescue.LDoctorRescueReason}",
            LBootstrapTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    private static string LBootstrapTextRead(string key)
    {
        return PLocalizationCatalog.PLocalizationTextRead(key);
    }
}
