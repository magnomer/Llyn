using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Llyn.UIDeportment;

public sealed class LBootstrap
{
    private readonly System.Windows.Application _lBootstrapApplication;

    private readonly Func<string, string> _lBootstrapText;

    private Func<Exception, string?> _lBootstrapAudit = _ => null;

    public LBootstrap(System.Windows.Application application, Func<string, string> textSeam)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(textSeam);

        _lBootstrapApplication = application;
        _lBootstrapText = textSeam;
        PLook.PLookStateAttach();
    }

    public bool LBootstrapThemeApply(Action themeSeam)
    {
        try
        {
            themeSeam();
            return true;
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"The application theme could not be loaded.\n\n{exception.Message}",
                "Llyn",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return false;
        }
    }

    public LBootstrapEngine? LBootstrapBuild<LBootstrapEngine>(
        Func<string> workspaceSeam, Func<string, LBootstrapEngine> buildSeam, Func<Exception, bool> busySeam)
        where LBootstrapEngine : class
    {
        string workspace = string.Empty;
        try
        {
            workspace = workspaceSeam();
            return buildSeam(workspace);
        }
        catch (Exception exception)
        {
            string key = busySeam(exception) ? "Workspace.Busy" : "Workspace.OpenFailed";
            MessageBox.Show(
                $"{_lBootstrapText(key)}\n\n{workspace}\n\n{exception.Message}",
                _lBootstrapText("Terms.Product"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return null;
        }
    }

    public void LBootstrapCatalogApply(
        Func<bool> defaultSeam,
        Func<IReadOnlyDictionary<string, string>> loadSeam,
        Action<IReadOnlyDictionary<string, string>> applySeam)
    {
        if (defaultSeam())
        {
            return;
        }

        try
        {
            applySeam(loadSeam());
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"The application language could not be loaded.\n\n{exception.Message}",
                _lBootstrapText("Terms.Product"),
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    public void LBootstrapRescueShow(bool done, string? backup, string? reason)
    {
        if (!done)
        {
            return;
        }

        MessageBox.Show(
            $"{_lBootstrapText("Workspace.DatabaseReset")}\n\n{backup}\n\n{reason}",
            _lBootstrapText("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    public void LBootstrapFaultAttach(Func<Exception, string?> auditSeam)
    {
        ArgumentNullException.ThrowIfNull(auditSeam);

        _lBootstrapAudit = auditSeam;
        _lBootstrapApplication.DispatcherUnhandledException += LBootstrapFaultHandle;
        TaskScheduler.UnobservedTaskException += LBootstrapStrayHandle;
    }

    private void LBootstrapFaultHandle(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        string? log = _lBootstrapAudit(e.Exception);
        e.Handled = true;

        MessageBox.Show(
            $"{_lBootstrapText("Workspace.Fault")}\n\n{log}\n\n{e.Exception.Message}",
            _lBootstrapText("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        if (!LBootstrapWindowCheck())
        {
            _lBootstrapApplication.Shutdown(1);
        }
    }

    private bool LBootstrapWindowCheck()
    {
        foreach (Window window in _lBootstrapApplication.Windows)
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
        _lBootstrapAudit(e.Exception);
        e.SetObserved();
    }
}
