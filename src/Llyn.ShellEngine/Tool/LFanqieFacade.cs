using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LFanqieFacade
{
    private readonly LEngine _lFanqieFacadeEngine;
    private readonly object _lFanqieFacadeGate;

    public LFanqieFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lFanqieFacadeEngine = engine;
        _lFanqieFacadeGate = engine.LEngineGate;
    }

    public IReadOnlyList<LFanqieBook> LEngineBookRead(string language)
    {
        lock (_lFanqieFacadeGate)
        {
            return LFanqieFacadeStaff.LEngineStaffFanqie.LFanqieBookRead(language);
        }
    }

    public bool LEngineBookCheck(string language)
    {
        return LEngineBookRead(language).Count > 0;
    }

    public string? LEngineBookFind()
    {
        foreach (string language in _lFanqieFacadeEngine.LEngineLanguage.LEngineLanguageRead())
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
        return LFanqieFacadeStaff.LEngineStaffFanqie.LFanqieClerkDivide(entryId);
    }

    public IReadOnlyList<LFanqieGroup> LEngineFanqieRead(long entryId)
    {
        LEngineFanqieStart(entryId);
        return LEngineFanqieDivide(entryId);
    }

    public string LEngineReadingRead(long entryId, string headword)
    {
        LEngineFanqieStart(entryId);
        return LFanqieFacadeStaff.LEngineStaffFanqie.LFanqieClerkFormat(entryId, headword);
    }

    public void LEngineFanqieStart(long entryId)
    {
        LFanqieFacadeStaff.LEngineStaffFanqie.LFanqieClerkStart(entryId);
        LFanqieFacadeStaff.LEngineStaffShengfu.LShengfuClerkStart(entryId);
    }

    public void LEngineFanqieRebuild(long entryId)
    {
        LFanqieFacadeStaff.LEngineStaffFanqie.LFanqieClerkRebuild(entryId);
        LFanqieFacadeStaff.LEngineStaffShengfu.LShengfuClerkRebuild(entryId);
    }

    public void LEngineFanqieSet(long entryId, long fanqieId, int rank, bool raise)
    {
        LFanqieFacadeStaff.LEngineStaffFanqie.LFanqieClerkSet(entryId, fanqieId, rank, raise);
    }

    public bool LEngineFanqieCheck(long entryId)
    {
        return LFanqieFacadeStaff.LEngineStaffFanqie.LFanqieClerkCheck(entryId)
            || LFanqieFacadeStaff.LEngineStaffShengfu.LShengfuClerkCheck(entryId);
    }

    public LDiwei? LEngineDiweiRead(long? id)
    {
        lock (_lFanqieFacadeGate)
        {
            return LFanqieFacadeStaff.LEngineStaffDiwei.LDiweiClerkRead(id);
        }
    }

    public LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize)
    {
        ArgumentNullException.ThrowIfNull(localize);

        lock (_lFanqieFacadeGate)
        {
            LDiwei? diwei = LFanqieFacadeStaff.LEngineStaffDiwei.LDiweiClerkRead(id);
            if (diwei is null)
            {
                return LDiweiPage.LDiweiPageBlank;
            }

            return LFanqieFacadeStaff.LEngineStaffDiwei.LDiweiPageRead(
                diwei,
                _lFanqieFacadeEngine.LEngineSettings.LEngineRespellingCheck(diwei.LDiweiLanguage),
                _lFanqieFacadeEngine.LEngineSettingsHeld.LSettingsTally,
                localize);
        }
    }

    public long LEngineDiweiResolve(long? id, string character)
    {
        LDiwei? diwei;
        lock (_lFanqieFacadeGate)
        {
            diwei = LFanqieFacadeStaff.LEngineStaffDiwei.LDiweiClerkRead(id);
        }

        return _lFanqieFacadeEngine.LEngineEntry.LEngineGlyphResolve(
            character, diwei?.LDiweiLanguage ?? LDiweiPage.LDiweiPageBlank.LDiweiPageLanguage).LEntryId;
    }

    public (long, bool)? LEngineDiweiFind(string language, string kind, string key)
    {
        lock (_lFanqieFacadeGate)
        {
            return LFanqieFacadeStaff.LEngineStaffDiwei.LDiweiClerkFind(language, kind, key) is LDiwei found
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
            diwei = LFanqieFacadeStaff.LEngineStaffDiwei;
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
                LFanqieFacadeStaff.LEngineStaffDiwei.LDiweiEntryScan(language, diweiIds, query);
            return entries.Count == 0
                ? []
                : _lFanqieFacadeEngine.LEngineVista.LEngineVistaBuild(entries, vista?.LVistaChosen);
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

    private LEngineStaff LFanqieFacadeStaff => _lFanqieFacadeEngine.LEngineStaffHeld;
}
