using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LCourierClerk
{
    private readonly LOutpost _lCourierClerkOutpost;
    private readonly LWarrant _lCourierClerkWarrant;
    private readonly LWorkspaceVault _lCourierClerkWorkspaces;
    private readonly LLanguageVault _lCourierClerkLanguages;
    private readonly LEntryQueryClerk _lCourierClerkEntry;
    private readonly object _lCourierClerkGate;
    private readonly Func<LSettings> _lCourierClerkSettings;
    private readonly Action<Exception> _lCourierClerkFault;
    private static int _lCourierClerkBusy;

    public LCourierClerk(
        LRig rig,
        LEntryQueryClerk entry,
        object gate,
        Func<LSettings> settings,
        Action<Exception> fault)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(gate);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(fault);
        _lCourierClerkOutpost = rig.LRigOutpost;
        _lCourierClerkWarrant = rig.LRigWarrant;
        _lCourierClerkWorkspaces = rig.LRigKeeping.LRigKeepingWorkspaces;
        _lCourierClerkLanguages = rig.LRigLanguages;
        _lCourierClerkEntry = entry;
        _lCourierClerkGate = gate;
        _lCourierClerkSettings = settings;
        _lCourierClerkFault = fault;
    }

    public async Task<LReceipt> LCourierClerkSend(
        Func<long, LLiveryPage?> page,
        Func<string, LLiveryLanguage> language,
        Func<string, string> lookup,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(lookup);
        if (Interlocked.CompareExchange(ref _lCourierClerkBusy, 1, 0) != 0)
        {
            throw new LRefusal(LRefusal.LRefusalCourier);
        }

        try
        {
            LSettings settings = _lCourierClerkSettings();
            if (string.IsNullOrWhiteSpace(settings.LSettingsWarrant))
            {
                throw new LRefusal(LRefusal.LRefusalMissing);
            }

            string token = _lCourierClerkWarrant.LWarrantRestore(settings.LSettingsWarrant)
                ?? throw new LRefusal(LRefusal.LRefusalWarrant);

            Guid realm;
            IReadOnlyList<LEntry> entries;
            lock (_lCourierClerkGate)
            {
                realm = _lCourierClerkWorkspaces.LWorkspaceRealmRead();
                entries = _lCourierClerkEntry.LEntryFind(string.Empty);
            }

            LOutpostCargo cargo = new(
                settings.LSettingsOutpost,
                token,
                settings.LSettingsNotebook,
                realm.ToString("N"),
                entries,
                _lCourierClerkLanguages.LLanguageScan(),
                page,
                language,
                lookup,
                _lCourierClerkFault);
            return await _lCourierClerkOutpost.LOutpostSend(cargo, cancellation).ConfigureAwait(false);
        }
        finally
        {
            Volatile.Write(ref _lCourierClerkBusy, 0);
        }
    }

    public async Task<string> LCourierClerkAttach(CancellationToken cancellation)
    {
        if (Interlocked.CompareExchange(ref _lCourierClerkBusy, 1, 0) != 0)
        {
            throw new LRefusal(LRefusal.LRefusalCourier);
        }

        try
        {
            string token = await _lCourierClerkOutpost
                .LOutpostAttach(_lCourierClerkSettings().LSettingsOutpost, _lCourierClerkFault, cancellation)
                .ConfigureAwait(false);
            return _lCourierClerkWarrant.LWarrantHide(token);
        }
        finally
        {
            Volatile.Write(ref _lCourierClerkBusy, 0);
        }
    }

    public bool LCourierClerkCheck()
    {
        return !string.IsNullOrWhiteSpace(_lCourierClerkSettings().LSettingsWarrant);
    }

    public static bool LCourierClerkMatch(Exception exception)
    {
        return exception is LRefusal { LRefusalReason: LRefusal.LRefusalWarrant };
    }
}
