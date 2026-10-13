using System;
using System.Collections.Generic;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal sealed class CLedgerNoticed
{
    private readonly HashSet<string> _cLedgerNoticed = new(StringComparer.Ordinal);

    internal void LLedgerRepaintShow(CEnvoy envoy, LSettingsPort settings, string key, Exception exception)
    {
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        lock (_cLedgerNoticed)
        {
            if (!_cLedgerNoticed.Add(key))
            {
                return;
            }
        }

        CLedger.LLedgerFailureShow(envoy, settings, key, exception);
    }

    internal LLedgerAnswer LLedgerRepaintRead<LLedgerAnswer>(
        CEnvoy envoy, LSettingsPort settings, Func<LLedgerAnswer> read, LLedgerAnswer fallback, string key)
    {
        ArgumentNullException.ThrowIfNull(read);

        try
        {
            return read();
        }
        catch (Exception exception)
        {
            LLedgerRepaintShow(envoy, settings, key, exception);
            return fallback;
        }
    }

    internal void LLedgerNoticedClear()
    {
        lock (_cLedgerNoticed)
        {
            _cLedgerNoticed.Clear();
        }
    }
}
