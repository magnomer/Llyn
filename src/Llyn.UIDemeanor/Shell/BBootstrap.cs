using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.UIDemeanor.Capsule;

namespace Llyn.UIDemeanor;

public sealed class BBootstrap
{
    internal const string BBootstrapNoticeHead = "llyn: ";

    private readonly BConsole _bBootstrapConsole;
    private readonly BBlueprint _bBootstrapBlueprint;
    private readonly BLoop _bBootstrapLoop;
    private readonly BLocalizationCatalog _bBootstrapCatalog = new();
    private Func<Exception, string?> _bBootstrapAudit = _ => null;
    private BInvocation? _bBootstrapInvocation;
    private BEnvoy? _bBootstrapEnvoy;
    private (BTheme BBootstrapThemeInitial, Func<string, Func<string, string>> BBootstrapThemeSeam,
        string? BBootstrapThemeReason)? _bBootstrapTheme;
    private (string BBootstrapRescueKey, string? BBootstrapRescueBackup, string? BBootstrapRescueReason)?
        _bBootstrapRescue;

    public BBootstrap(BConsole console, BBlueprint blueprint)
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(blueprint);

        _bBootstrapConsole = console;
        _bBootstrapBlueprint = blueprint;
        _bBootstrapLoop = new BLoop(BBootstrapFaultObserve);
    }

    public int BBootstrapIntroduce<BBootstrapEngine>(
        string[] args,
        Func<string, Func<string, string>> themeSeam,
        Func<IReadOnlyDictionary<string, string>> catalogSeam,
        Func<string> workspaceSeam,
        Func<string, bool, BBootstrapEngine> buildSeam,
        Func<Exception, bool> busySeam,
        Func<BBootstrapEngine, int> runSeam)
        where BBootstrapEngine : class
    {
        ArgumentNullException.ThrowIfNull(args);
        ArgumentNullException.ThrowIfNull(themeSeam);
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
                _bBootstrapEnvoy = new BEnvoy(_bBootstrapConsole, _bBootstrapCatalog, _bBootstrapBlueprint);
                try
                {
                    _bBootstrapTheme = (new BTheme(themeSeam(BTheme.BThemeDefault)), themeSeam, null);
                }
                catch (Exception exception)
                {
                    _bBootstrapTheme = (new BTheme(BTheme.BThemeSpareRead), themeSeam, exception.Message);
                }

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
            string key = CAtelier.CAtelierRefusalRead(busySeam(exception));
            if (_bBootstrapEnvoy is BEnvoy envoy && _bBootstrapTheme is { } theme)
            {
                BBootstrapWindowAttach(envoy, theme, null, null, null, null);
                envoy.BEnvoyRefusalShow(key, workspace, exception.Message);
            }

            string refusal = _bBootstrapCatalog.BLocalizationTextRead(key);
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

        if (_bBootstrapEnvoy is not null)
        {
            _bBootstrapRescue = (key, backup, reason);
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

    public int BBootstrapAtelierRun(
        Func<bool, CAtelier> atelierSeam,
        Func<string, bool> presenceSeam,
        Func<string, string[]> folderSeam,
        Func<string, string[]> fileSeam,
        Func<string[]> driveSeam)
    {
        ArgumentNullException.ThrowIfNull(atelierSeam);
        ArgumentNullException.ThrowIfNull(presenceSeam);
        ArgumentNullException.ThrowIfNull(folderSeam);
        ArgumentNullException.ThrowIfNull(fileSeam);
        ArgumentNullException.ThrowIfNull(driveSeam);

        if (_bBootstrapEnvoy is BEnvoy envoy && _bBootstrapTheme is { } theme)
        {
            CAtelier shown = atelierSeam(false);
            BWindow window = BBootstrapWindowAttach(
                envoy, theme, shown, presenceSeam, folder => (folderSeam(folder), fileSeam(folder)), driveSeam);
            _bBootstrapLoop.BLoopDefer(() => BBootstrapNoticeShow(envoy));
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

    private BWindow BBootstrapWindowAttach(
        BEnvoy envoy,
        (BTheme BBootstrapThemeInitial, Func<string, Func<string, string>> BBootstrapThemeSeam,
            string? BBootstrapThemeReason) theme,
        CAtelier? atelier,
        Func<string, bool>? presence,
        Func<string, (IReadOnlyList<string>, IReadOnlyList<string>)>? folder,
        Func<string[]>? drives)
    {
        Func<string, BTheme?> themes = name =>
        {
            try
            {
                return new BTheme(theme.BBootstrapThemeSeam(name));
            }
            catch (Exception exception)
            {
                envoy.BEnvoyThemeShow(exception.Message);
                return null;
            }
        };
        LCapsuleConsole capsule = new();
        BWindow window = new(
            _bBootstrapConsole,
            _bBootstrapLoop,
            _bBootstrapCatalog,
            atelier,
            envoy,
            _bBootstrapBlueprint,
            (theme.BBootstrapThemeReason is null ? BTheme.BThemeDefault : null, theme.BBootstrapThemeInitial),
            themes,
            capsule);
        envoy.BEnvoyWindowAttach(window, atelier, presence, folder, drives, capsule);
        _bBootstrapLoop.BLoopDefer(window.BWindowOpen);
        _bBootstrapLoop.BLoopDefer(window.BWindowAtelierOpen);
        _bBootstrapLoop.BLoopBatchAttach(window.BWindowBatchObserve);
        _bBootstrapLoop.BLoopKeyAttach(_bBootstrapConsole, window.BWindowKeyObserve);
        _bBootstrapLoop.BLoopSizeAttach(_bBootstrapConsole, window.BWindowSizeObserve);
        return window;
    }

    private void BBootstrapNoticeShow(BEnvoy envoy)
    {
        if (_bBootstrapRescue is var (key, backup, reason))
        {
            envoy.BEnvoyRescueShow(key, backup, reason);
        }

        if (_bBootstrapTheme?.BBootstrapThemeReason is string theme)
        {
            envoy.BEnvoyThemeShow(theme);
        }
    }

    private void BBootstrapFaultObserve(Exception exception)
    {
        if (_bBootstrapEnvoy is not BEnvoy envoy)
        {
            BBootstrapFaultPrint(exception);
            return;
        }

        envoy.BEnvoyFaultShow(_bBootstrapAudit(exception), exception.Message, BBootstrapFaultPrint);
    }

    private void BBootstrapFaultPrint(Exception exception)
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
