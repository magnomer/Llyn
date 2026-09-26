using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Llyn.UIDeportment;

public sealed class QBootstrap
{
    private Func<Exception, string?> _qBootstrapAudit = _ => null;

    public QBootstrap()
    {
        PLook.PLookStateAttach();
    }

    public bool QBootstrapThemeApply(
        Func<Func<string, string>> colorSeam, Func<IReadOnlyDictionary<string, string>> catalogSeam)
    {
        try
        {
            ResourceDictionary resources = System.Windows.Application.Current.Resources;
            Func<string, string> colorRead = colorSeam();
            PLocalizationCatalog.PLocalizationCatalogApply(resources, catalogSeam());
            PThemeLoader.PThemeLoaderApply(colorRead, resources);
            PField.PFieldApply(resources);
            PIndicator.PIndicatorApply(resources);
            PCaret.PCaretHook();
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

    public QBootstrapEngine? QBootstrapBuild<QBootstrapEngine>(
        Func<string> workspaceSeam, Func<string, QBootstrapEngine> buildSeam, Func<Exception, bool> busySeam)
        where QBootstrapEngine : class
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
                $"{PLocalizationCatalog.PLocalizationTextRead(key)}\n\n{workspace}\n\n{exception.Message}",
                PLocalizationCatalog.PLocalizationTextRead("Terms.Product"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return null;
        }
    }

    public void QBootstrapCatalogApply(
        Func<bool> defaultSeam, Func<IReadOnlyDictionary<string, string>> loadSeam)
    {
        if (defaultSeam())
        {
            return;
        }

        try
        {
            PLocalizationCatalog.PLocalizationCatalogApply(System.Windows.Application.Current.Resources, loadSeam());
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"The application language could not be loaded.\n\n{exception.Message}",
                PLocalizationCatalog.PLocalizationTextRead("Terms.Product"),
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    public void QBootstrapRescueShow(bool done, string? backup, string? reason)
    {
        if (!done)
        {
            return;
        }

        MessageBox.Show(
            $"{PLocalizationCatalog.PLocalizationTextRead("Workspace.DatabaseReset")}\n\n{backup}\n\n{reason}",
            PLocalizationCatalog.PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    public void QBootstrapFaultAttach(Func<Exception, string?> auditSeam)
    {
        ArgumentNullException.ThrowIfNull(auditSeam);

        _qBootstrapAudit = auditSeam;
        System.Windows.Application.Current.DispatcherUnhandledException += QBootstrapFaultHandle;
        TaskScheduler.UnobservedTaskException += QBootstrapStrayHandle;
    }

    public void QBootstrapWindowShow(PWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        window.PWindowSurface.Show();
    }

    private void QBootstrapFaultHandle(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        string? log = _qBootstrapAudit(e.Exception);
        e.Handled = true;

        MessageBox.Show(
            $"{PLocalizationCatalog.PLocalizationTextRead("Workspace.Fault")}\n\n{log}\n\n{e.Exception.Message}",
            PLocalizationCatalog.PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Error);

        if (!QBootstrapWindowCheck())
        {
            System.Windows.Application.Current.Shutdown(1);
        }
    }

    private static bool QBootstrapWindowCheck()
    {
        foreach (Window window in System.Windows.Application.Current.Windows)
        {
            if (window.IsVisible)
            {
                return true;
            }
        }

        return false;
    }

    private void QBootstrapStrayHandle(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        _qBootstrapAudit(e.Exception);
        e.SetObserved();
    }
}
