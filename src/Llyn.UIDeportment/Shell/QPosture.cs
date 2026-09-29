using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Llyn.UIDeportment.Capsule;

namespace Llyn.UIDeportment;

public sealed class QPosture : IDisposable
{
    private readonly LCapsule _qPostureCapsule = new();

    private readonly Func<string> _qPostureRoot;

    private readonly object _qPostureGate = new();

    private string? _qPostureFolder;

    private LCapsuleContent _qPostureContent = new();

    private CancellationTokenSource? _qPosturePending;

    public event Action? QPostureCleared;

    public event Action? QPostureLinkedChanged;

    internal QPosture(Func<string> root)
    {
        ArgumentNullException.ThrowIfNull(root);

        _qPostureRoot = root;
    }

    public LCapsuleContent QPostureRead()
    {
        lock (_qPostureGate)
        {
            return QPostureCurrentRead();
        }
    }

    public LCapsuleColumn? QPostureColumnRead(string tab)
    {
        foreach (LCapsuleColumn column in QPostureRead().LCapsuleContentColumn ?? [])
        {
            if (string.Equals(column.LCapsuleColumnTab, tab, StringComparison.Ordinal))
            {
                return column;
            }
        }

        return null;
    }

    public void QPostureWindowDefer(LCapsuleWindow window, bool minimized, int delay)
    {
        ArgumentNullException.ThrowIfNull(window);

        lock (_qPostureGate)
        {
            QPostureCurrentRead();
            QPostureWindowCancel();
            if (minimized)
            {
                return;
            }

            if (delay > 0)
            {
                CancellationTokenSource pending = new();
                _qPosturePending = pending;
                _ = QPostureWindowRun(pending, window, delay);
                return;
            }

            QPostureSave(_qPostureContent with { LCapsuleContentWindow = window });
        }
    }

    public bool QPostureLinkedSave(bool linked)
    {
        lock (_qPostureGate)
        {
            LCapsuleContent content = QPostureCurrentRead();
            if (content.LCapsuleContentLinked == linked)
            {
                return false;
            }

            QPostureSave(content with { LCapsuleContentLinked = linked });
        }

        QPostureLinkedChanged?.Invoke();
        return true;
    }

    public void QPostureLayoutSave(IEnumerable<LCapsuleColumn> layout)
    {
        ArgumentNullException.ThrowIfNull(layout);

        lock (_qPostureGate)
        {
            LCapsuleContent content = QPostureCurrentRead();
            List<LCapsuleColumn> given = [.. layout];
            List<LCapsuleColumn> next = [];
            bool moved = false;
            foreach (LCapsuleColumn held in content.LCapsuleContentColumn ?? [])
            {
                LCapsuleColumn? change = given.Find(column => column.LCapsuleColumnTab == held.LCapsuleColumnTab);
                LCapsuleColumn kept = change is null
                    ? held
                    : held with
                    {
                        LCapsuleColumnLeft = change.LCapsuleColumnLeft ?? held.LCapsuleColumnLeft,
                        LCapsuleColumnMiddle = change.LCapsuleColumnMiddle ?? held.LCapsuleColumnMiddle,
                    };
                moved |= kept != held;
                next.Add(kept);
            }

            foreach (LCapsuleColumn column in given)
            {
                if (!next.Exists(kept => kept.LCapsuleColumnTab == column.LCapsuleColumnTab))
                {
                    moved = true;
                    next.Add(column);
                }
            }

            if (moved)
            {
                QPostureSave(content with { LCapsuleContentColumn = next });
            }
        }
    }

    public void QPostureLayoutReset()
    {
        lock (_qPostureGate)
        {
            LCapsuleContent content = QPostureCurrentRead();
            List<LCapsuleColumn> list = [];
            foreach (LCapsuleColumn column in content.LCapsuleContentColumn ?? [])
            {
                list.Add(column with { LCapsuleColumnLeft = null, LCapsuleColumnMiddle = null });
            }

            QPostureSave(content with { LCapsuleContentColumn = list });
        }
        QPostureCleared?.Invoke();
    }

    public void Dispose()
    {
        lock (_qPostureGate)
        {
            QPostureWindowCancel();
        }
    }

    private LCapsuleContent QPostureCurrentRead()
    {
        string root = _qPostureRoot();
        if (string.Equals(root, _qPostureFolder, StringComparison.Ordinal))
        {
            return _qPostureContent;
        }

        QPostureWindowCancel();
        _qPostureFolder = root;
        _qPostureContent = string.IsNullOrWhiteSpace(root) ? new() : _qPostureCapsule.LCapsuleRead(root);
        return _qPostureContent;
    }

    private async Task QPostureWindowRun(CancellationTokenSource pending, LCapsuleWindow window, int delay)
    {
        try
        {
            await Task.Delay(delay, pending.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        lock (_qPostureGate)
        {
            if (!ReferenceEquals(_qPosturePending, pending))
            {
                return;
            }

            QPostureWindowCancel();
            QPostureSave(_qPostureContent with { LCapsuleContentWindow = window });
        }
    }

    private void QPostureWindowCancel()
    {
        CancellationTokenSource? pending = _qPosturePending;
        _qPosturePending = null;

        if (pending is null)
        {
            return;
        }

        pending.Cancel();
        pending.Dispose();
    }

    private void QPostureSave(LCapsuleContent content)
    {
        if (content == _qPostureContent)
        {
            return;
        }

        _qPostureContent = content;
        if (string.IsNullOrWhiteSpace(_qPostureFolder))
        {
            return;
        }

        try
        {
            _qPostureCapsule.LCapsuleSave(_qPostureFolder, content);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
