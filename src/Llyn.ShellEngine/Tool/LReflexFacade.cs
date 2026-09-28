using System;
using System.Collections.Generic;
using System.Linq;
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

    public IReadOnlyList<LAnchorRow> LEngineAnchorScan(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string language, string reflex, string tone)
    {
        return LReflexClerk.LReflexAnchorScan(rows, anchors, LEngineToneRead(language), reflex, tone);
    }

    public IReadOnlyList<LAnchorRow> LEngineAnchorScan(
        long entryId, IReadOnlyList<long> anchors, string language, string reflex, string tone)
    {
        return LEngineAnchorScan(LEngineAnchorRead(entryId), anchors, language, reflex, tone);
    }

    public bool LEngineAnchorCheck(long entryId, string headword)
    {
        return LEngineAnchorCheck(LEngineAnchorRead(entryId), headword);
    }

    public string LEngineAnchorFormat(long entryId, IReadOnlyList<long> anchors, string headword, string separator)
    {
        return LEngineAnchorFormat(LEngineAnchorRead(entryId), anchors, headword, separator);
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

    public IReadOnlyList<LReflexGuise> LEngineGuiseRead(string language, IReadOnlyList<string> reflexes)
    {
        ArgumentNullException.ThrowIfNull(reflexes);

        IReadOnlyList<string> folded;
        lock (_lReflexFacadeGate)
        {
            folded = LReflexFacadeStaff.LEngineStaffReflex.LReflexFoldedRead(language);
        }

        return reflexes.Select(reflex => LEngineGuiseBuild(reflex.Trim(), folded)).ToList();
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

    private LReflexGuise LEngineGuiseBuild(string reflex, IReadOnlyList<string> folded)
    {
        return new LReflexGuise(
            _lReflexFacadeEngine.LEngineSettings.LEngineRespellingCheck(reflex),
            _lReflexFacadeEngine.LEngineSettings.LEnginePhonemicCheck(reflex),
            folded.Contains(reflex, StringComparer.Ordinal));
    }

    private IReadOnlyList<LFanqieRow> LEngineAnchorRead(long entryId)
    {
        return [.. _lReflexFacadeEngine.LEngineFanqie.LEngineFanqieRead(entryId)
            .SelectMany(static group => group.LFanqieGroupRows)];
    }

    private LEngineStaff LReflexFacadeStaff => _lReflexFacadeEngine.LEngineStaffHeld;
}
