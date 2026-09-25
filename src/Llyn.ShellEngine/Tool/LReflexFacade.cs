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

    public IReadOnlyList<LAnatomyTone> LEngineToneRead(string language)
    {
        return string.IsNullOrWhiteSpace(language)
            ? []
            : _lReflexFacadeEngine.LEngineLanguage.LEngineLanguageLoad(language).LLanguageAnatomyTones;
    }

    private LEngineStaff LReflexFacadeStaff => _lReflexFacadeEngine.LEngineStaffHeld;
}
