using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CFold
{
    private readonly Func<long?> _cFoldEntry;

    private readonly LReflexPort _cFoldReflexPort;

    private readonly LSettingsPort _cFoldSettingsPort;

    private readonly CEnvoy _cFoldEnvoy;

    private readonly CLedgerNoticed _cFoldNoticed;

    internal CFold(
        Func<long?> entry, LReflexPort reflexes, LSettingsPort settings, CEnvoy envoy, CLedgerNoticed noticed)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(reflexes);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(noticed);

        _cFoldEntry = entry;
        _cFoldReflexPort = reflexes;
        _cFoldSettingsPort = settings;
        _cFoldEnvoy = envoy;
        _cFoldNoticed = noticed;
    }

    public bool CFoldFanqieOpened => LFoldCheck(
        _cFoldReflexPort, _cFoldNoticed, _cFoldEnvoy, _cFoldSettingsPort, LFoldEntry, LFoldBox.LFoldBoxFanqie);

    public bool CFoldScriptOpened => LFoldCheck(
        _cFoldReflexPort, _cFoldNoticed, _cFoldEnvoy, _cFoldSettingsPort, LFoldEntry, LFoldBox.LFoldBoxScript);

    public bool CFoldFanqieSpread(bool opened)
    {
        if (LFoldEntry is not long id)
        {
            return false;
        }

        try
        {
            _cFoldReflexPort.LEngineBoxSpread(id, LFoldBox.LFoldBoxFanqie, opened);
            return true;
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cFoldEnvoy, _cFoldSettingsPort, "Box.SpreadFailed", exception);
            return false;
        }
    }

    public bool CFoldScriptSpread(bool opened)
    {
        if (LFoldEntry is not long id)
        {
            return false;
        }

        try
        {
            _cFoldReflexPort.LEngineBoxSpread(id, LFoldBox.LFoldBoxScript, opened);
            return true;
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cFoldEnvoy, _cFoldSettingsPort, "Box.SpreadFailed", exception);
            return false;
        }
    }

    private static bool LFoldCheck(
        LReflexPort reflexes, CLedgerNoticed noticed, CEnvoy envoy, LSettingsPort settings, long? entry, LFoldBox box)
    {
        if (entry is not long id)
        {
            return false;
        }

        try
        {
            return reflexes.LEngineBoxCheck(id, box);
        }
        catch (Exception exception)
        {
            noticed.LLedgerRepaintShow(envoy, settings, "Box.SpreadReadFailed", exception);
            return false;
        }
    }

    private long? LFoldEntry => _cFoldEntry();
}
