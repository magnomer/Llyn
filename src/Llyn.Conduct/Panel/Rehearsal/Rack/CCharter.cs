using System;
using System.Collections.Generic;
using System.Globalization;
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

    public CCharterState? CCharterRead()
    {
        if (_cCharterDraft is not { } draft)
        {
            return null;
        }

        LDeck deck = draft.LDeckStored;
        LDeckSchedule schedule = deck.LDeckSchedule;
        return new CCharterState(
            (int)schedule.LDeckScheduleOrder - 1,
            schedule.LDeckScheduleGoal,
            schedule.LDeckScheduleSpaced,
            schedule.LDeckSchedulePool,
            schedule.LDeckScheduleStreak,
            schedule.LDeckScheduleTarget,
            schedule.LDeckScheduleQuorum,
            (int)deck.LDeckLead - 1,
            deck.LDeckBasic,
            deck.LDeckCode,
            draft.LDeckBearings
                .Select(static bearing => new CCharterBearing(
                    bearing.LBearingDirection,
                    bearing.LBearingFront,
                    bearing.LBearingBack,
                    bearing.LBearingStatistics,
                    LCharterCaptionRead(bearing.LBearingDirection)))
                .ToList());
    }

    public void CCharterFrontToggle(int direction, int side)
    {
        LCharterBearingSet(direction, bearing => bearing with
        {
            LBearingFront = bearing.LBearingFront ^ (1 << side),
        });
    }

    public void CCharterBackToggle(int direction, int side)
    {
        LCharterBearingSet(direction, bearing => bearing with
        {
            LBearingBack = bearing.LBearingBack ^ (1 << side),
        });
    }

    public void CCharterStatisticsSet(int direction, string statistics)
    {
        ArgumentNullException.ThrowIfNull(statistics);

        LCharterBearingSet(direction, bearing => bearing with
        {
            LBearingStatistics = LCharterNumberRead(statistics, bearing.LBearingStatistics),
        });
    }

    public void CCharterOrderSet(int index)
    {
        if (index >= 0)
        {
            LCharterScheduleSet(schedule => schedule with { LDeckScheduleOrder = (LAlmanacOrder)(index + 1) });
        }
    }

    public void CCharterGoalSet(string goal)
    {
        ArgumentNullException.ThrowIfNull(goal);

        LCharterScheduleSet(schedule => schedule with
        {
            LDeckScheduleGoal = LCharterNumberRead(goal, schedule.LDeckScheduleGoal),
        });
    }

    public void CCharterSpacedSet(string spaced)
    {
        ArgumentNullException.ThrowIfNull(spaced);

        LCharterScheduleSet(schedule => schedule with
        {
            LDeckScheduleSpaced = LCharterNumberRead(spaced, schedule.LDeckScheduleSpaced),
        });
    }

    public void CCharterPoolSet(string pool)
    {
        ArgumentNullException.ThrowIfNull(pool);

        LCharterScheduleSet(schedule => schedule with
        {
            LDeckSchedulePool = LCharterNumberRead(pool, schedule.LDeckSchedulePool),
        });
    }

    public void CCharterStreakSet(string streak)
    {
        ArgumentNullException.ThrowIfNull(streak);

        LCharterScheduleSet(schedule => schedule with
        {
            LDeckScheduleStreak = LCharterNumberRead(streak, schedule.LDeckScheduleStreak),
        });
    }

    public void CCharterTargetSet(string target)
    {
        ArgumentNullException.ThrowIfNull(target);

        LCharterScheduleSet(schedule => schedule with
        {
            LDeckScheduleTarget = LCharterNumberRead(target, schedule.LDeckScheduleTarget),
        });
    }

    public void CCharterQuorumSet(string quorum)
    {
        ArgumentNullException.ThrowIfNull(quorum);

        LCharterScheduleSet(schedule => schedule with
        {
            LDeckScheduleQuorum = string.IsNullOrWhiteSpace(quorum)
                ? null
                : LCharterNumberRead(quorum, schedule.LDeckScheduleQuorum ?? 0),
        });
    }

    public void CCharterLeadSet(int index)
    {
        if (index >= 0)
        {
            LCharterDraftSet(deck => deck with { LDeckLead = (LBearingLead)(index + 1) });
        }
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

    private void LCharterScheduleSet(Func<LDeckSchedule, LDeckSchedule> change)
    {
        LCharterDraftSet(deck => deck with { LDeckSchedule = change(deck.LDeckSchedule) });
    }

    private void LCharterBearingSet(int direction, Func<LBearing, LBearing> change)
    {
        if (_cCharterDraft is { } draft)
        {
            _cCharterDraft = (
                draft.LDeckStored,
                draft.LDeckBearings
                    .Select(held => held.LBearingDirection == direction ? change(held) : held)
                    .ToList());
        }
    }

    private static int LCharterNumberRead(string text, int held)
    {
        return int.TryParse(text, NumberStyles.None, CultureInfo.CurrentCulture, out int number) ? number : held;
    }

    private static string LCharterCaptionRead(int direction)
    {
        return direction switch
        {
            1 => "Bearing.Forward",
            2 => "Bearing.Backward",
            _ => throw new InvalidOperationException($"Unknown bearing direction {direction}."),
        };
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
