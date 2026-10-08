using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDisplaySound
{
    private static readonly CFont _cDisplayBare = new(null, null, CFontSlant.CFontSlantTheme);

    private static readonly CLecternAnchor _cDisplayUnanchored = new(false, new Dictionary<long, string>());

    private readonly LDisplaySound _cDisplayVoice;

    private readonly CLedgerNoticed _cDisplayNoticed;

    private readonly CDisplay _cDisplayHeader;

    private readonly LDraftPort _cDisplayDraft;

    private readonly LEntryPort _cDisplayEntryPort;

    private readonly LGlyphPort _cDisplayGlyphPort;

    private readonly LFanqiePort _cDisplayFanqie;

    private readonly LDiweiPort _cDisplayDiwei;

    private readonly LScriptPort _cDisplayScript;

    private readonly LParadigmPort _cDisplayParadigm;

    private readonly LReflexPort _cDisplayReflex;

    private readonly LSettingsPort _cDisplaySettings;

    private readonly CEnvoy _cDisplayEnvoy;

    internal CDisplaySound(
        LDisplay display,
        CDisplay header,
        LDraftPort drafts,
        LEntryPort entries,
        LGlyphPort glyphs,
        LFanqiePort fanqies,
        LDiweiPort diweis,
        LScriptPort scripts,
        LParadigmPort paradigms,
        LReflexPort reflexes,
        LSettingsPort settings,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(header);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(glyphs);
        ArgumentNullException.ThrowIfNull(fanqies);
        ArgumentNullException.ThrowIfNull(diweis);
        ArgumentNullException.ThrowIfNull(scripts);
        ArgumentNullException.ThrowIfNull(paradigms);
        ArgumentNullException.ThrowIfNull(reflexes);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cDisplayVoice = display.LDisplaySound;
        _cDisplayNoticed = display.LDisplayNoticed;
        _cDisplayHeader = header;
        _cDisplayDraft = drafts;
        _cDisplayEntryPort = entries;
        _cDisplayGlyphPort = glyphs;
        _cDisplayFanqie = fanqies;
        _cDisplayDiwei = diweis;
        _cDisplayScript = scripts;
        _cDisplayParadigm = paradigms;
        _cDisplayReflex = reflexes;
        _cDisplaySettings = settings;
        _cDisplayEnvoy = envoy;
    }

    public event Action? CDisplayFoldChanged;

    internal event Action<string, long>? CDisplayRowChosen;

    internal event Action<string, string, string>? CDisplayDiweiChosen;

    internal event Action<string, string?>? CDisplayStemChosen;

    public bool CDisplayFoldOpened => _cDisplayVoice.LDisplayFoldOpened;

    private LEntryDraft? LDisplayShown => _cDisplayVoice.LDisplayShown;

    private long? LDisplayEntry => _cDisplayVoice.LDisplayEntry;

    public CLecternGlyph CDisplayGlyphRead()
    {
        CLecternGlyph blank = new(false, string.Empty, string.Empty, [], _cDisplayBare);
        if (LDisplayShown is not LEntryDraft shown)
        {
            return blank;
        }

        LGlyph? glyph;
        IReadOnlyList<LGlyphCell> cells;
        try
        {
            glyph = _cDisplayGlyphPort.LEngineGlyphRead(shown);
            cells = glyph is null ? [] : _cDisplayGlyphPort.LEngineGlyphDivide(shown);
        }
        catch (Exception exception)
        {
            _cDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Sound.LoadFailed", exception);
            return blank;
        }

        if (glyph is null)
        {
            return blank;
        }

        return new CLecternGlyph(
            cells.Count > 0,
            CScheme.CSchemeKeyRead(glyph.LGlyphName),
            glyph.LGlyphName,
            cells.Select(static cell => new CGlyphCell(
                cell.LGlyphCellText, cell.LGlyphCellLanguage, cell.LGlyphCellLinked)).ToList(),
            CDisplayFontRead(CFontRole.CFontRoleGlyph));
    }

    public bool CDisplayGlyphOpen(string character, string language)
    {
        if (CCatalog.LCatalogGlyphOpen(
                _cDisplayEnvoy, _cDisplaySettings, () => _cDisplayEntryPort.LEngineGlyphResolve(character, language))
            is not long entry)
        {
            return false;
        }

        CDisplayRowChosen?.Invoke("Library", entry);
        return true;
    }

    public IReadOnlyList<CTranscriptionDraft> CDisplayTranscriptionRead()
    {
        if (LDisplayShown is not LEntryDraft shown)
        {
            return [];
        }

        try
        {
            return CTranscriptionDraft.CTranscriptionDraftRead(_cDisplayGlyphPort.LEngineTranscriptionRead(shown));
        }
        catch (Exception exception)
        {
            _cDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Sound.LoadFailed", exception);
            return [];
        }
    }

    public CLecternReflex CDisplayReflexRead()
    {
        if (LDisplayShown is null)
        {
            return new CLecternReflex([], _cDisplayUnanchored, false);
        }

        IReadOnlyList<CReflex> rows = LDisplayReflexScan();
        return new CLecternReflex(
            rows, LDisplayAnchorRead(rows), _cDisplayVoice.LDisplayReflexCheck(LDisplayEntry));
    }

    public CLecternReflex CDisplayReflexResonate()
    {
        if (LDisplayShown is not null)
        {
            _cDisplayVoice.LDisplayReflexLoad();
        }

        return CDisplayReflexRead();
    }

    public void CDisplayReflexToggle(bool opened)
    {
        _cDisplayVoice.LDisplayFoldSet(opened);
        CDisplayFoldChanged?.Invoke();
    }

    public CLecternFanqie CDisplayFanqieRead()
    {
        if (LDisplayShown is null || LDisplayEntry is not long id)
        {
            return new CLecternFanqie([], false, string.Empty, _cDisplayUnanchored, _cDisplayBare);
        }

        string headword = _cDisplayHeader.CDisplayShown.CLecternHeadword;
        return new CLecternFanqie(
            CSounding.CSoundingFanqieRead(_cDisplayVoice.LDisplayListRead(
                _cDisplayFanqie.LEngineFanqieDivide, "Display.FanqieReadFailed")),
            _cDisplayVoice.LDisplayFanqieCheck(id),
            _cDisplayNoticed.LLedgerRepaintRead(_cDisplayEnvoy, _cDisplaySettings,
                () => _cDisplayFanqie.LEngineReadingRead(id, headword), string.Empty, "Display.ReadingFailed"),
            LDisplayAnchorRead(LDisplayReflexScan()),
            CDisplayFontRead(CFontRole.CFontRoleGlyph));
    }

    public void CDisplayFanqieSet(long fanqieId, int rank, bool raise)
    {
        if (LDisplayEntry is not long id)
        {
            return;
        }

        try
        {
            _cDisplayFanqie.LEngineFanqieSet(id, fanqieId, rank, raise);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(
                _cDisplayEnvoy, _cDisplaySettings, "Display.FanqieRepresentativeFailed", exception);
        }
    }

    public bool CDisplayDiweiOpen(bool initial, string key)
    {
        if (LDisplayShown is null || _cDisplayDiwei.LEngineDiweiRead(initial, key) is not string kind)
        {
            return false;
        }

        CDisplayDiweiChosen?.Invoke(_cDisplayHeader.CDisplayShown.CLecternLanguage, kind, key);
        return true;
    }

    public bool CDisplayStemOpen(string? key)
    {
        if (LDisplayShown is null)
        {
            return false;
        }

        CDisplayStemChosen?.Invoke(_cDisplayHeader.CDisplayShown.CLecternLanguage, key);
        return true;
    }

    public CLecternScript CDisplayScriptRead()
    {
        if (LDisplayShown is null || LDisplayEntry is not long id)
        {
            return new CLecternScript([], false, _cDisplayBare);
        }

        return new CLecternScript(
            CSounding.CSoundingScriptRead(
                _cDisplayVoice.LDisplayListRead(_cDisplayScript.LEngineScriptDivide, "Display.ScriptReadFailed"),
                _cDisplaySettings),
            _cDisplayVoice.LDisplayScriptCheck(id),
            CDisplayFontRead(CFontRole.CFontRoleGlyph));
    }

    public CLecternParadigm CDisplayParadigmRead()
    {
        if (LDisplayShown is null || LDisplayEntry is not long id)
        {
            return new CLecternParadigm([], _cDisplayBare);
        }

        string language = _cDisplayNoticed.LLedgerRepaintRead(_cDisplayEnvoy, _cDisplaySettings,
            () => _cDisplayParadigm.LEngineLanguageResolve(id), string.Empty, "Display.LanguageFailed");
        IReadOnlyList<LParadigmRow> rows = _cDisplayVoice.LDisplayListRead(
            _cDisplayParadigm.LEngineParadigmScan, "Display.ParadigmReadFailed");
        bool pending = _cDisplayVoice.LDisplayParadigmCheck(id);
        return new CLecternParadigm(
            CSounding.CSoundingParadigmRead(
                _cDisplayParadigm, rows, pending, _cDisplayVoice.LDisplayMorphologyRead(), false),
            CFont.CFontRead(_cDisplaySettings, language, CFontRole.CFontRoleHeadword));
    }

    private IReadOnlyList<CReflex> LDisplayReflexScan()
    {
        try
        {
            return CRespelling.LRespellingReflexScan(
                _cDisplayReflex,
                _cDisplayHeader.CDisplayShown.CLecternLanguage,
                CReflexDraft.CReflexDraftRead(_cDisplayVoice.LDisplayReflexRead()));
        }
        catch (Exception exception)
        {
            _cDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Display.ReflexFailed", exception);
            return [];
        }
    }

    private CLecternAnchor LDisplayAnchorRead(IReadOnlyList<CReflex> rows)
    {
        return CReflex.LReflexAnchorRead(
            _cDisplayEnvoy,
            _cDisplaySettings,
            _cDisplayNoticed,
            _cDisplayDraft,
            LDisplayEntry,
            _cDisplayHeader.CDisplayShown.CLecternHeadword,
            rows);
    }

    public CFont CDisplayFontRead(CFontRole role)
    {
        return CFont.CFontRead(_cDisplaySettings, _cDisplayHeader.CDisplayShown.CLecternLanguage, role);
    }
}
