using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QBootstrap
{
    private Func<Exception, string?> _qBootstrapAudit = _ => null;

    public QBootstrap()
    {
        QLook.QLookStateAttach();
    }

    public void QBootstrapIntroduce<QBootstrapEngine>(
        Func<Func<string, string>> colorSeam,
        Func<IReadOnlyDictionary<string, string>> catalogSeam,
        Func<string> workspaceSeam,
        Func<string, QBootstrapEngine> buildSeam,
        Func<Exception, bool> busySeam,
        Action<QBootstrapEngine> runSeam)
        where QBootstrapEngine : class
    {
        ArgumentNullException.ThrowIfNull(runSeam);

        if (!QBootstrapThemeApply(colorSeam, catalogSeam))
        {
            return;
        }

        if (QBootstrapBuild(workspaceSeam, buildSeam, busySeam) is QBootstrapEngine engine)
        {
            runSeam(engine);
        }
    }

    private static bool QBootstrapThemeApply(
        Func<Func<string, string>> colorSeam, Func<IReadOnlyDictionary<string, string>> catalogSeam)
    {
        try
        {
            ResourceDictionary resources = System.Windows.Application.Current.Resources;
            Func<string, string> colorRead = colorSeam();
            QLocalizationCatalog.QLocalizationCatalogApply(resources, catalogSeam());
            QTheme.QThemeApply(colorRead, resources);
            QField.QFieldApply(resources);
            QIndicator.QIndicatorApply(resources);
            QCaret.QCaretHook();
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

    private static QBootstrapEngine? QBootstrapBuild<QBootstrapEngine>(
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
            QBootstrapBuildConsult(CAtelier.CAtelierRefusalRead(busySeam(exception)), workspace, exception);
            return null;
        }
    }

    private static void QBootstrapBuildConsult(string key, string workspace, Exception exception)
    {
        MessageBox.Show(
            $"{QLocalizationCatalog.QLocalizationTextRead(key)}\n\n{workspace}\n\n{exception.Message}",
            QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Error);
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
            QLocalizationCatalog.QLocalizationCatalogApply(System.Windows.Application.Current.Resources, loadSeam());
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"The application language could not be loaded.\n\n{exception.Message}",
                QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    public void QBootstrapRescueConsult(bool done, string? backup, string? reason)
    {
        if (CAtelier.CAtelierRescueRead(done) is not string key)
        {
            return;
        }

        MessageBox.Show(
            $"{QLocalizationCatalog.QLocalizationTextRead(key)}\n\n{backup}\n\n{reason}",
            QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    public void QBootstrapFaultIntroduce(Func<Exception, string?> auditSeam)
    {
        ArgumentNullException.ThrowIfNull(auditSeam);

        _qBootstrapAudit = auditSeam;
        System.Windows.Application.Current.DispatcherUnhandledException += QBootstrapFaultObserve;
        TaskScheduler.UnobservedTaskException += QBootstrapStrayObserve;
    }

    public void QBootstrapWindowShow(QWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        window.QWindowSurface.Show();
    }

    private void QBootstrapFaultObserve(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        string? log = _qBootstrapAudit(e.Exception);
        e.Handled = true;
        QBootstrapFaultConsult(log, e.Exception);
        QBootstrapWindowRefine();
    }

    private static void QBootstrapFaultConsult(string? log, Exception exception)
    {
        MessageBox.Show(
            $"{QLocalizationCatalog.QLocalizationTextRead("Workspace.Fault")}\n\n{log}\n\n{exception.Message}",
            QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    private static void QBootstrapWindowRefine()
    {
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

    private void QBootstrapStrayObserve(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        _qBootstrapAudit(e.Exception);
        e.SetObserved();
    }
}
