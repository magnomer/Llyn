using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LReflexFacade
{
    private readonly LEngine _lReflexFacadeEngine;
    private readonly object _lReflexFacadeGate;

    public LReflexFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lReflexFacadeEngine = engine;
        _lReflexFacadeGate = engine.LEngineGate;
    }

    public IReadOnlyList<long> LEngineAnchorToggle(IReadOnlyList<long> anchors, long fanqieId, bool anchored)
    {
        return LReflexClerk.LReflexAnchorToggle(anchors, fanqieId, anchored);
    }

    public bool LEngineAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)
    {
        return LReflexClerk.LReflexAnchorMatch(one, other);
    }

    public IReadOnlyList<LAnchorRow> LEngineAnchorScan(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string language, string reflex, string tone)
    {
        return LReflexClerk.LReflexAnchorScan(rows, anchors, LEngineToneRead(language), reflex, tone);
    }

    public bool LEngineAnchorCheck(IReadOnlyList<LFanqieRow> rows, string headword)
    {
        return LReflexClerk.LReflexAnchorCheck(rows, headword);
    }

    public string LEngineAnchorFormat(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string headword, string separator)
    {
        return LReflexClerk.LReflexAnchorFormat(rows, anchors, headword, separator);
    }

    public IReadOnlyList<LReflexRule> LEngineReflexRead(string language)
    {
        lock (_lReflexFacadeGate)
        {
            return LReflexFacadeStaff.LEngineStaffReflex.LReflexRuleRead(language);
        }
    }

    public IReadOnlyList<LReflex> LEngineReflexRead(long entryId)
    {
        lock (_lReflexFacadeGate)
        {
            return LReflexFacadeStaff.LEngineStaffReflex.LReflexClerkRead(entryId);
        }
    }

    internal IReadOnlyList<LReflex> LEngineReflexSet(long entryId, IReadOnlyList<LReflex> reflexes)
    {
        lock (_lReflexFacadeGate)
        {
            return LReflexFacadeStaff.LEngineStaffReflex.LReflexClerkSet(entryId, reflexes);
        }
    }

    public void LEngineReflexStart(long entryId)
    {
        LReflexFacadeStaff.LEngineStaffReflex.LReflexClerkStart(entryId);
    }

    public void LEngineReflexRebuild(long entryId)
    {
        LReflexFacadeStaff.LEngineStaffReflex.LReflexClerkRebuild(entryId);
    }

    public bool LEngineReflexCheck(long entryId)
    {
        return LReflexFacadeStaff.LEngineStaffReflex.LReflexClerkCheck(entryId);
    }

    internal Task<IReadOnlyList<LReflexDraft>> LEngineReflexFind(
        string headword, string language, CancellationToken cancellation)
    {
        return LReflexFacadeStaff.LEngineStaffReflex.LReflexClerkFind(headword, language, cancellation);
    }

    public IReadOnlyList<LAnatomyTone> LEngineToneRead(string language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? []
            : _lReflexFacadeEngine.LEngineLanguage.LEngineLanguageLoad(language).LLanguageAnatomyTones;
    }

    private LEngineStaff LReflexFacadeStaff => _lReflexFacadeEngine.LEngineStaffHeld;
}
