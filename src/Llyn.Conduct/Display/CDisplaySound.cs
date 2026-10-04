using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CDisplaySound
{
    private static readonly CLecternAccent _cDisplayMute = new(
        new CRespellingMark(false, string.Empty, string.Empty),
        [],
        string.Empty,
        false,
        CSounding.CSoundingVarietyRead(string.Empty, string.Empty),
        [],
        false);

    private static readonly CFont _cDisplayBare = new(null, null, null);

    private static readonly CLecternAnchor _cDisplayUnanchored = new(false, new Dictionary<long, string>());

    private readonly LDisplaySound _cDisplayVoice;

    private readonly CLedgerNoticed _cDisplayNoticed;

    private readonly CDisplay _cDisplayHeader;

    private readonly LDraftPort _cDisplayDraft;

    private readonly LEntryPort _cDisplayPort;

    private readonly LPhonologyPort _cDisplayPhonology;

    private readonly LMediaPort _cDisplayMedia;

    private readonly LSettingsPort _cDisplaySettings;

    private readonly CEnvoy _cDisplayEnvoy;

    internal CDisplaySound(
        LDisplay display,
        CDisplay header,
        LDraftPort drafts,
        LEntryPort entries,
        LPhonologyPort phonology,
        LMediaPort media,
        LSettingsPort settings,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(header);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(media);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cDisplayVoice = display.LDisplaySound;
        _cDisplayNoticed = display.LDisplayNoticed;
        _cDisplayHeader = header;
        _cDisplayDraft = drafts;
        _cDisplayPort = entries;
        _cDisplayPhonology = phonology;
        _cDisplayMedia = media;
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

    public CLecternAccent CDisplayAccentRead()
    {
        if (LDisplayShown is not LEntryDraft shown)
        {
            return _cDisplayMute;
        }

        try
        {
            return LDisplayAccentRead(_cDisplayPhonology.LEngineAccentRead(shown));
        }
        catch (Exception exception)
        {
            _cDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Sound.LoadFailed", exception);
            return _cDisplayMute;
        }
    }

    public async Task<CLecternAccent?> CDisplayEnsignLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(store);

        if (LDisplayShown is not LEntryDraft shown)
        {
            return null;
        }

        LAccentSheet sheet;
        try
        {
            sheet = await _cDisplayPhonology.LEngineAccentLoad(
                shown, (rows, delete) => store(CCatalog.CCatalogEnsignRead(rows), delete));
        }
        catch (Exception exception)
        {
            _cDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Sound.LoadFailed", exception);
            return null;
        }

        return ReferenceEquals(shown, LDisplayShown) ? LDisplayAccentRead(sheet) : null;
    }

    private static CLecternAccent LDisplayAccentRead(LAccentSheet sheet)
    {
        string language = sheet.LAccentSheetLanguage;
        return new CLecternAccent(
            new CRespellingMark(sheet.LAccentSheetRespelled, sheet.LAccentSheetOpener, sheet.LAccentSheetCloser),
            CSounding.LSoundingContourRead(sheet.LAccentSheetContour),
            sheet.LAccentSheetPrimary.LAccentRowText,
            sheet.LAccentSheetSpoken,
            CSounding.CSoundingVarietyRead(language, sheet.LAccentSheetPrimary.LAccentRowVariety),
            sheet.LAccentSheetRows.Select(row => CSounding.LSoundingAccentRead(language, row)).ToList(),
            sheet.LAccentSheetFlagged);
    }

    public CLecternPlayback CDisplayPlaybackRead()
    {
        if (LDisplayShown is not LEntryDraft shown)
        {
            return new CLecternPlayback(false, false);
        }

        try
        {
            (bool recorded, bool audible) = _cDisplayMedia.LEnginePlaybackRead(shown);
            return new CLecternPlayback(recorded, audible);
        }
        catch (Exception exception)
        {
            _cDisplayNoticed.LLedgerRepaintShow(_cDisplayEnvoy, _cDisplaySettings, "Sound.LoadFailed", exception);
            return new CLecternPlayback(false, false);
        }
    }

    public void CDisplayPlaybackStart(double volume)
    {
        if (LDisplayShown is not LEntryDraft shown)
        {
            return;
        }

        LDisplayPlaybackStart(() => _cDisplayMedia.LEngineRecordingPlay(shown, volume));
    }

    public void CDisplayPlaybackStart(string? audio, double volume)
    {
        LDisplayPlaybackStart(() => _cDisplayMedia.LEngineRecordingPlay(audio, volume));
    }

    private void LDisplayPlaybackStart(Func<int> play)
    {
        try
        {
            _cDisplayVoice.LDisplayTicket = play();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cDisplayEnvoy, _cDisplaySettings, "Sound.PlayFailed", exception);
        }
    }

    public void CDisplayPlaybackCancel()
    {
        _cDisplayVoice.LDisplayPlaybackStop();
    }

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
            glyph = _cDisplayPort.LEngineGlyphRead(shown);
            cells = glyph is null ? [] : _cDisplayPort.LEngineGlyphDivide(shown);
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
                _cDisplayEnvoy, _cDisplaySettings, () => _cDisplayPort.LEngineGlyphResolve(character, language))
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
            return CSounding.CSoundingTranscriptionRead(_cDisplayPort.LEngineTranscriptionRead(shown));
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
                _cDisplayPhonology.LEngineFanqieDivide, "Display.FanqieReadFailed")),
            _cDisplayVoice.LDisplayFanqieCheck(id),
            _cDisplayNoticed.LLedgerRepaintRead(_cDisplayEnvoy, _cDisplaySettings,
                () => _cDisplayPhonology.LEngineReadingRead(id, headword), string.Empty, "Display.ReadingFailed"),
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
            _cDisplayPhonology.LEngineFanqieSet(id, fanqieId, rank, raise);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(
                _cDisplayEnvoy, _cDisplaySettings, "Display.FanqieRepresentativeFailed", exception);
        }
    }

    public bool CDisplayDiweiOpen(bool initial, string key)
    {
        if (LDisplayShown is null || LPhonologyPort.LEngineDiweiRead(initial, key) is not string kind)
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
            CSounding.CSoundingScriptRead(_cDisplayVoice.LDisplayListRead(
                _cDisplayPhonology.LEngineScriptDivide, "Display.ScriptReadFailed")),
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
            () => _cDisplayPhonology.LEngineLanguageResolve(id), string.Empty, "Display.LanguageFailed");
        IReadOnlyList<LParadigmRow> rows = _cDisplayVoice.LDisplayListRead(
            _cDisplayPhonology.LEngineParadigmScan, "Display.ParadigmReadFailed");
        bool pending = _cDisplayVoice.LDisplayParadigmCheck(id);
        return new CLecternParadigm(
            CSounding.CSoundingParadigmRead(rows, pending, _cDisplayVoice.LDisplayMorphologyRead(), false),
            CCatalog.LCatalogFontRead(_cDisplaySettings, language, CFontRole.CFontRoleHeadword));
    }

    private IReadOnlyList<CReflex> LDisplayReflexScan()
    {
        try
        {
            return CRespelling.LRespellingReflexScan(
                _cDisplayPhonology,
                _cDisplayHeader.CDisplayShown.CLecternLanguage,
                CSounding.CSoundingReflexRead(_cDisplayVoice.LDisplayReflexRead()));
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
        return CCatalog.LCatalogFontRead(_cDisplaySettings, _cDisplayHeader.CDisplayShown.CLecternLanguage, role);
    }
}
