using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LReflexFacade : LReflexPort
{
    private readonly LEngineHearth _lReflexFacadeHearth;
    private readonly LFanqieFacade _lReflexFacadeFanqie;
    private readonly object _lReflexFacadeGate;

    internal LReflexFacade(LEngineHearth hearth, LFanqieFacade fanqie)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(fanqie);
        _lReflexFacadeHearth = hearth;
        _lReflexFacadeFanqie = fanqie;
        _lReflexFacadeGate = _lReflexFacadeHearth.LEngineGate;
    }

    public IReadOnlyList<LAnchorRow> LEngineAnchorScan(
        IReadOnlyList<LFanqieRow> rows, IReadOnlyList<long> anchors, string language, string reflex, string tone)
    {
        return LReflexClerk.LReflexAnchorScan(rows, anchors, LEngineDescentRead(language), reflex, tone);
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
            return LReflexFacadeStaff.LEngineStaffLanguage.LLanguageStaffReflex.LReflexRuleRead(language);
        }
    }

    public IReadOnlyList<LReflexGuise> LEngineGuiseRead(string language, IReadOnlyList<LReflexDraft> reflexes)
    {
        ArgumentNullException.ThrowIfNull(reflexes);

        lock (_lReflexFacadeGate)
        {
            LLanguageStaff staff = LReflexFacadeStaff.LEngineStaffLanguage;
            return staff.LLanguageStaffLanguage.LLanguageGuiseScan(
                reflexes,
                staff.LLanguageStaffReflex.LReflexFoldedRead(language),
                _lReflexFacadeHearth.LEngineSettingsHeld.LSettingsRespelled);
        }
    }

    public void LEngineReflexStart(long entryId)
    {
        LReflexFacadeStaff.LEngineStaffLanguage.LLanguageStaffReflex.LReflexClerkFetch.LReflexFetchStart(entryId);
    }

    public void LEngineReflexRebuild(long entryId)
    {
        LReflexFacadeStaff.LEngineStaffLanguage.LLanguageStaffReflex.LReflexClerkFetch.LReflexFetchRebuild(entryId);
    }

    public bool LEngineReflexCheck(long entryId)
    {
        return LReflexFacadeStaff.LEngineStaffLanguage.LLanguageStaffReflex
            .LReflexClerkFetch.LReflexFetchCheck(entryId);
    }

    public bool LEngineSpreadCheck(long entryId)
    {
        lock (_lReflexFacadeGate)
        {
            return LReflexFacadeStaff.LEngineStaffEntry.LEntryStaffFold.LFoldClerkLoad(entryId);
        }
    }

    public void LEngineReflexSpread(long entryId, bool opened)
    {
        lock (_lReflexFacadeGate)
        {
            LReflexFacadeStaff.LEngineStaffEntry.LEntryStaffFold.LFoldClerkSpread(entryId, opened);
        }

        _lReflexFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectFold, entryId);
    }

    public bool LEngineBoxCheck(long entryId, LFoldBox box)
    {
        lock (_lReflexFacadeGate)
        {
            return LReflexFacadeStaff.LEngineStaffEntry.LEntryStaffFold.LFoldClerkLoad(entryId, box);
        }
    }

    public void LEngineBoxSpread(long entryId, LFoldBox box, bool opened)
    {
        lock (_lReflexFacadeGate)
        {
            LReflexFacadeStaff.LEngineStaffEntry.LEntryStaffFold.LFoldClerkSpread(entryId, box, opened);
        }

        _lReflexFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectFold, entryId);
    }

    public IReadOnlyList<LDescent> LEngineDescentRead(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return [];
        }

        LLanguageClerk languages;
        lock (_lReflexFacadeGate)
        {
            languages = LReflexFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage;
        }

        return languages.LLanguageClerkLoad(language).LLanguageDescents;
    }

    private IReadOnlyList<LFanqieRow> LEngineAnchorRead(long entryId)
    {
        _lReflexFacadeFanqie.LEngineFanqieStart(entryId);
        return LReflexFacadeStaff.LEngineStaffLanguage.LLanguageStaffFanqie.LFanqieClerkRead(entryId);
    }

    private LEngineStaff LReflexFacadeStaff => _lReflexFacadeHearth.LEngineStaffHeld;
}
