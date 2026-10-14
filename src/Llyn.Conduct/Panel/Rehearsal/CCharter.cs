using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCharter
{
    private readonly LDeckPort _cCharterPort;

    private readonly CEnvoy _cCharterEnvoy;

    private readonly LSettingsPort _cCharterSettings;

    private (LDeck LDeckStored, IReadOnlyList<LBearing> LDeckBearings)? _cCharterStored;

    private (LDeck LDeckStored, IReadOnlyList<LBearing> LDeckBearings)? _cCharterDraft;

    internal CCharter(CAtelier atelier, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);

        _cCharterPort = atelier.CAtelierEntryBundle.CEntryBundleDeck;
        _cCharterEnvoy = envoy;
        _cCharterSettings = atelier.CAtelierSettingsPort;
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(LCharterChangeCheck, LCharterFinish);
    }

    public void CCharterBearingSet(int direction, int front, int back, int statistics)
    {
        if (_cCharterDraft is not { } draft)
        {
            return;
        }

        LBearing bearing = new(direction, front, back, statistics);
        List<LBearing> bearings = draft.LDeckBearings
            .Select(held => held.LBearingDirection == direction ? bearing : held)
            .ToList();
        if (!draft.LDeckBearings.Any(held => held.LBearingDirection == direction))
        {
            bearings.Add(bearing);
        }

        _cCharterDraft = (draft.LDeckStored, bearings);
    }

    public void CCharterScheduleSet(int order, int goal, int spaced, int pool, int streak, int target, int? quorum)
    {
        LDeckSchedule schedule = new((LAlmanacOrder)order, goal, spaced, pool, streak, target, quorum);
        LCharterDraftSet(deck => deck with { LDeckSchedule = schedule });
    }

    public void CCharterLeadSet(int lead)
    {
        LCharterDraftSet(deck => deck with { LDeckLead = (LBearingLead)lead });
    }

    public void CCharterBasicSet(bool basic)
    {
        LCharterDraftSet(deck => deck with { LDeckBasic = basic });
    }

    public void CCharterCodeSet(string code)
    {
        ArgumentNullException.ThrowIfNull(code);

        LCharterDraftSet(deck => deck with { LDeckCode = code });
    }

    public bool CCharterSave()
    {
        if (_cCharterDraft is not { } draft)
        {
            return true;
        }

        try
        {
            _cCharterPort.LEngineDeckUpdate(draft.LDeckStored, draft.LDeckBearings);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCharterEnvoy, _cCharterSettings, "Deck.SaveFailed", exception);
            return false;
        }

        _cCharterStored = draft;
        return true;
    }

    private void LCharterDraftSet(Func<LDeck, LDeck> change)
    {
        if (_cCharterDraft is { } draft)
        {
            _cCharterDraft = (change(draft.LDeckStored), draft.LDeckBearings);
        }
    }

    internal void LCharterOpen(long deck)
    {
        LCharterClear();
        try
        {
            _cCharterStored = _cCharterPort.LEngineDeckRead(deck);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCharterEnvoy, _cCharterSettings, "Deck.LoadFailed", exception);
            return;
        }

        _cCharterDraft = _cCharterStored;
    }

    internal void LCharterClear()
    {
        _cCharterStored = null;
        _cCharterDraft = null;
    }

    internal bool LCharterLeaveConfirm()
    {
        if (!LCharterChangeCheck())
        {
            return true;
        }

        return _cCharterEnvoy.CEnvoyLeaveConfirm() is bool store && LCharterFinish(store);
    }

    private bool LCharterChangeCheck()
    {
        if (_cCharterDraft is not { } draft || _cCharterStored is not { } stored)
        {
            return false;
        }

        return draft.LDeckStored != stored.LDeckStored
            || !draft.LDeckBearings.SequenceEqual(stored.LDeckBearings);
    }

    private bool LCharterFinish(bool store)
    {
        if (store && LCharterChangeCheck())
        {
            return CCharterSave();
        }

        _cCharterDraft = _cCharterStored;
        return true;
    }
}
