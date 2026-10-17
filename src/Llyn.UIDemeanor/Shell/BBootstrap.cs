using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

public sealed class BBootstrap
{
    internal const string BBootstrapNoticeHead = "llyn: ";

    private readonly BConsole _bBootstrapConsole;
    private readonly BLoop _bBootstrapLoop;
    private readonly BLocalizationCatalog _bBootstrapCatalog = new();
    private Func<Exception, string?> _bBootstrapAudit = _ => null;
    private BInvocation? _bBootstrapInvocation;
    private BEnvoy? _bBootstrapEnvoy;

    public BBootstrap(BConsole console)
    {
        ArgumentNullException.ThrowIfNull(console);

        _bBootstrapConsole = console;
        _bBootstrapLoop = new BLoop(BBootstrapFaultObserve);
    }

    public int BBootstrapIntroduce<BBootstrapEngine>(
        string[] args,
        Func<IReadOnlyDictionary<string, string>> catalogSeam,
        Func<string> workspaceSeam,
        Func<string, bool, BBootstrapEngine> buildSeam,
        Func<Exception, bool> busySeam,
        Func<BBootstrapEngine, int> runSeam)
        where BBootstrapEngine : class
    {
        ArgumentNullException.ThrowIfNull(args);
        ArgumentNullException.ThrowIfNull(catalogSeam);
        ArgumentNullException.ThrowIfNull(workspaceSeam);
        ArgumentNullException.ThrowIfNull(buildSeam);
        ArgumentNullException.ThrowIfNull(busySeam);
        ArgumentNullException.ThrowIfNull(runSeam);

        SynchronizationContext.SetSynchronizationContext(_bBootstrapLoop);
        _bBootstrapConsole.BConsoleModeApply(BBootstrapConsoleRestore);
        try
        {
            try
            {
                _bBootstrapCatalog.BLocalizationCatalogApply(catalogSeam());
            }
            catch (Exception exception)
            {
                _bBootstrapConsole.BConsoleNoticePrint(
                    $"{BBootstrapNoticeHead}The application language could not be loaded. {exception.Message}");
                return 1;
            }

            if (args.Length == 0 && _bBootstrapConsole.BConsoleTerminalCheck())
            {
                _bBootstrapEnvoy = new BEnvoy(_bBootstrapConsole, _bBootstrapCatalog);
                _bBootstrapConsole.BConsoleWindowApply();
            }
            else
            {
                _bBootstrapInvocation = BInvocation.BInvocationParse(args, _bBootstrapCatalog, out string usage);
                if (_bBootstrapInvocation is null)
                {
                    _bBootstrapConsole.BConsoleNoticePrint(BBootstrapNoticeHead + usage);
                    return 2;
                }
            }

            if (BBootstrapBuild(workspaceSeam, buildSeam, busySeam, _bBootstrapInvocation?.BInvocationLeased ?? true)
                is not BBootstrapEngine engine)
            {
                return 3;
            }

            try
            {
                return runSeam(engine);
            }
            catch (Exception exception)
            {
                BBootstrapFaultObserve(exception);
                return 4;
            }
        }
        finally
        {
            BBootstrapConsoleRestore();
        }
    }

    private void BBootstrapConsoleRestore()
    {
        _bBootstrapConsole.BConsoleWindowRestore();
        _bBootstrapEnvoy?.BEnvoyNoticePrint();
        _bBootstrapConsole.BConsoleModeRestore();
    }

    private BBootstrapEngine? BBootstrapBuild<BBootstrapEngine>(
        Func<string> workspaceSeam,
        Func<string, bool, BBootstrapEngine> buildSeam,
        Func<Exception, bool> busySeam,
        bool leased)
        where BBootstrapEngine : class
    {
        string workspace = string.Empty;
        try
        {
            workspace = workspaceSeam();
            return buildSeam(workspace, leased);
        }
        catch (Exception exception)
        {
            string refusal = _bBootstrapCatalog.BLocalizationTextRead(
                CAtelier.CAtelierRefusalRead(busySeam(exception)));
            BBootstrapNoticePrint(BBootstrapNoticeHead + refusal);
            BBootstrapNoticePrint(workspace);
            BBootstrapNoticePrint(exception.Message);
            return null;
        }
    }

    public void BBootstrapCatalogApply(
        Func<bool> defaultSeam, Func<IReadOnlyDictionary<string, string>> loadSeam)
    {
        ArgumentNullException.ThrowIfNull(defaultSeam);
        ArgumentNullException.ThrowIfNull(loadSeam);

        if (defaultSeam())
        {
            return;
        }

        try
        {
            _bBootstrapCatalog.BLocalizationCatalogApply(loadSeam());
        }
        catch (Exception exception)
        {
            BBootstrapNoticePrint(
                $"{BBootstrapNoticeHead}The application language could not be loaded. {exception.Message}");
        }
    }

    public void BBootstrapRescueConsult(bool done, string? backup, string? reason)
    {
        if (CAtelier.CAtelierRescueRead(done) is not string key)
        {
            return;
        }

        BBootstrapNoticePrint(BBootstrapNoticeHead + _bBootstrapCatalog.BLocalizationTextRead(key));
        BBootstrapNoticePrint($"{backup}");
        BBootstrapNoticePrint($"{reason}");
    }

    public void BBootstrapFaultIntroduce(Func<Exception, string?> auditSeam)
    {
        ArgumentNullException.ThrowIfNull(auditSeam);

        _bBootstrapAudit = auditSeam;
        TaskScheduler.UnobservedTaskException += BBootstrapStrayObserve;
    }

    public int BBootstrapAtelierRun(Func<bool, CAtelier> atelierSeam, Func<string, bool> presenceSeam)
    {
        ArgumentNullException.ThrowIfNull(atelierSeam);
        ArgumentNullException.ThrowIfNull(presenceSeam);

        if (_bBootstrapEnvoy is BEnvoy envoy)
        {
            CAtelier shown = atelierSeam(false);
            BWindow window = new(_bBootstrapConsole, _bBootstrapCatalog, shown, envoy);
            _bBootstrapLoop.BLoopDefer(window.BWindowOpen);
            _bBootstrapLoop.BLoopBatchAttach(window.BWindowBatchObserve);
            _bBootstrapLoop.BLoopKeyAttach(_bBootstrapConsole, window.BWindowKeyObserve);
            _bBootstrapLoop.BLoopSizeAttach(_bBootstrapConsole, window.BWindowSizeObserve);
            try
            {
                _bBootstrapLoop.BLoopRun(window.BWindowExit);
            }
            finally
            {
                shown.CAtelierClose();
            }

            return 0;
        }

        if (_bBootstrapInvocation is not BInvocation invocation)
        {
            throw new InvalidOperationException("The atelier runs only after an invocation was parsed.");
        }

        CAtelier atelier = atelierSeam(true);
        TaskCompletionSource<int> run = new();
        _bBootstrapLoop.BLoopDefer(async () =>
        {
            try
            {
                run.SetResult(await invocation.BInvocationRun(
                    atelier, _bBootstrapConsole, _bBootstrapCatalog, presenceSeam));
            }
            catch (Exception exception)
            {
                BBootstrapFaultObserve(exception);
                run.SetResult(4);
            }
        });

        try
        {
            _bBootstrapLoop.BLoopRun(run.Task);
        }
        finally
        {
            atelier.CAtelierClose();
        }

        return run.Task.Result;
    }

    private void BBootstrapFaultObserve(Exception exception)
    {
        string? log = _bBootstrapAudit(exception);
        BBootstrapNoticePrint(BBootstrapNoticeHead + _bBootstrapCatalog.BLocalizationTextRead("Workspace.Fault"));
        BBootstrapNoticePrint($"{log}");
        BBootstrapNoticePrint(exception.Message);
    }

    private void BBootstrapNoticePrint(string text)
    {
        if (_bBootstrapEnvoy is BEnvoy envoy)
        {
            envoy.BEnvoyNoticeAdd(text);
            return;
        }

        _bBootstrapConsole.BConsoleNoticePrint(text);
    }

    private void BBootstrapStrayObserve(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        _bBootstrapAudit(e.Exception);
        e.SetObserved();
    }
}
