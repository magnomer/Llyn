using System;
using System.Diagnostics;
using Llyn.Core;

namespace Llyn.Core.Windows;

public sealed class LUsherShell : LUsher
{
    private readonly LUsher _lUsherInner;

    public LUsherShell(LUsher inner)
    {
        ArgumentNullException.ThrowIfNull(inner);

        _lUsherInner = inner;
    }

    public bool LUsherPathExist(string? path)
    {
        return _lUsherInner.LUsherPathExist(path);
    }

    public void LUsherPathDelete(string path)
    {
        _lUsherInner.LUsherPathDelete(path);
    }

    public bool LUsherLockCheck(Exception exception)
    {
        return _lUsherInner.LUsherLockCheck(exception);
    }

    public void LUsherOpen(string target)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(target);

        Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
    }
}
