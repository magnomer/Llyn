using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LFanqieFacade : LFanqiePort, LDiweiPort
{
    private readonly LEngineHearth _lFanqieFacadeHearth;
    private readonly LEntryFacade _lFanqieFacadeEntry;
    private readonly LSettingsFacade _lFanqieFacadeSettings;
    private readonly LVistaRowFacade _lFanqieFacadeRow;
    private readonly object _lFanqieFacadeGate;

    internal LFanqieFacade(LEngineHearth hearth, LEntryFacade entry, LSettingsFacade settings, LVistaRowFacade row)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(row);
        _lFanqieFacadeHearth = hearth;
        _lFanqieFacadeEntry = entry;
        _lFanqieFacadeSettings = settings;
        _lFanqieFacadeRow = row;
        _lFanqieFacadeGate = _lFanqieFacadeHearth.LEngineGate;
    }

    public IReadOnlyList<LFanqieBook> LEngineBookRead(string language)
    {
        lock (_lFanqieFacadeGate)
        {
            return LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffFanqie.LFanqieBookRead(language);
        }
    }

    public bool LEngineBookCheck(string language)
    {
        return LEngineBookRead(language).Count > 0;
    }

    public string? LEngineBookFind()
    {
        IReadOnlyList<string> languages;
        lock (_lFanqieFacadeGate)
        {
            languages = LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage.LLanguageClerkRead();
        }

        foreach (string language in languages)
        {
            if (LEngineBookCheck(language))
            {
                return language;
            }
        }

        return null;
    }

    public bool LEngineBookCheck()
    {
        return LEngineBookFind() is not null;
    }

    public IReadOnlyList<LFanqieGroup> LEngineFanqieDivide(long entryId)
    {
        return LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffFanqie.LFanqieClerkDivide(entryId);
    }

    public IReadOnlyList<LFanqieGroup> LEngineFanqieRead(long entryId)
    {
        LEngineFanqieStart(entryId);
        return LEngineFanqieDivide(entryId);
    }

    public string LEngineReadingRead(long entryId, string headword)
    {
        LEngineFanqieStart(entryId);
        return LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffFanqie.LFanqieClerkFormat(entryId, headword);
    }

    public void LEngineFanqieStart(long entryId)
    {
        LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffFanqie.LFanqieClerkStart(entryId);
        LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffShengfu.LShengfuClerkStart(entryId);
    }

    public void LEngineFanqieRebuild(long entryId)
    {
        LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffFanqie.LFanqieClerkRebuild(entryId);
        LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffShengfu.LShengfuClerkRebuild(entryId);
    }

    public void LEngineFanqieSet(long entryId, long fanqieId, int rank, bool raise)
    {
        LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffFanqie.LFanqieClerkSet(entryId, fanqieId, rank, raise);
    }

    public bool LEngineFanqieCheck(long entryId)
    {
        return LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffFanqie.LFanqieClerkCheck(entryId)
            || LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffShengfu.LShengfuClerkCheck(entryId);
    }

    public string? LEngineDiweiRead(bool initial, string key)
    {
        return LDiweiClerk.LDiweiKindRead(!initial, key);
    }

    public LDiwei? LEngineDiweiRead(long? id)
    {
        lock (_lFanqieFacadeGate)
        {
            return LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffDiwei.LDiweiClerkRead(id);
        }
    }

    public LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize)
    {
        ArgumentNullException.ThrowIfNull(localize);

        lock (_lFanqieFacadeGate)
        {
            LDiwei? diwei = LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffDiwei.LDiweiClerkRead(id);
            if (diwei is null)
            {
                return LDiweiPage.LDiweiPageBlank;
            }

            return LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffDiwei.LDiweiPageRead(
                diwei,
                _lFanqieFacadeSettings.LEngineRespellingCheck(diwei.LDiweiLanguage),
                _lFanqieFacadeHearth.LEngineSettingsHeld.LSettingsTally,
                localize);
        }
    }

    public long LEngineDiweiResolve(long? id, string character)
    {
        LDiwei? diwei;
        lock (_lFanqieFacadeGate)
        {
            diwei = LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffDiwei.LDiweiClerkRead(id);
        }

        return _lFanqieFacadeEntry.LEngineGlyphResolve(
            character, diwei?.LDiweiLanguage ?? LDiweiPage.LDiweiPageBlank.LDiweiPageLanguage).LEntryId;
    }

    public (long, bool)? LEngineDiweiFind(string language, string kind, string key)
    {
        lock (_lFanqieFacadeGate)
        {
            return LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffDiwei
                .LDiweiClerkFind(language, kind, key) is LDiwei found
                ? (found.LDiweiId, found.LDiweiFinal)
                : null;
        }
    }

    public IReadOnlyList<LDiwei> LEngineDiweiFind(LVista vista, long? chosen, bool final)
    {
        ArgumentNullException.ThrowIfNull(vista);

        if (!LEngineBookCheck())
        {
            return [];
        }

        string language = LEngineLanguageRead(chosen);
        string kind = LDiweiClerk.LDiweiKindRead(final);
        LDiweiClerk diwei;
        lock (_lFanqieFacadeGate)
        {
            diwei = LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffDiwei;
        }

        List<LDiwei> rows = [];
        bool kept = false;
        foreach (LDiwei row in diwei.LDiweiClerkFind(language, kind, vista.LVistaQuery, vista.LVistaOrder))
        {
            bool held = vista.LVistaMatch(row.LDiweiId);
            kept |= held;
            rows.Add(row with { LDiweiChosen = held });
        }

        if (!kept)
        {
            vista.LVistaSelect(null);
        }

        return rows;
    }

    public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(
        string language, IReadOnlyList<long> diweiIds, string query, LVista? vista = null)
    {
        lock (_lFanqieFacadeGate)
        {
            IReadOnlyList<LEntry> entries =
                LFanqieFacadeStaff.LEngineStaffLanguage.LLanguageStaffDiwei.LDiweiEntryScan(
                    language, diweiIds, query, vista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword);
            return entries.Count == 0
                ? []
                : _lFanqieFacadeRow.LEngineVistaBuild(entries, vista?.LVistaChosen);
        }
    }

    public IReadOnlyList<LVistaRow> LEngineXiaoyunFind(long? chosen, LVista onset, LVista rime, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(onset);
        ArgumentNullException.ThrowIfNull(rime);
        ArgumentNullException.ThrowIfNull(vista);

        List<long> wanted = [];
        if (onset.LVistaChosen is long initial)
        {
            wanted.Add(initial);
        }

        if (rime.LVistaChosen is long final)
        {
            wanted.Add(final);
        }

        if (wanted.Count == 0)
        {
            return [];
        }

        return LEngineXiaoyunFind(LEngineLanguageRead(chosen), wanted, vista.LVistaQuery.Trim(), vista);
    }

    private string LEngineLanguageRead(long? chosen)
    {
        return LEngineDiweiRead(chosen)?.LDiweiLanguage ?? LEngineBookFind() ?? string.Empty;
    }

    private LEngineStaff LFanqieFacadeStaff => _lFanqieFacadeHearth.LEngineStaffHeld;
}
