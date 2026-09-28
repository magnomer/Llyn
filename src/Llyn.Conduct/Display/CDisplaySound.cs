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
        false,
        string.Empty,
        false,
        CSounding.CSoundingVarietyRead(string.Empty, string.Empty),
        [],
        false);

    private static readonly CFont _cDisplayBare = new(null, null, null);

    private static readonly CLecternAnchor _cDisplayUnanchored = new(false, new Dictionary<long, string>());

    private readonly LDisplaySound _cDisplayVoice;

    private readonly CDisplay _cDisplayHeader;

    private readonly LDraftPort _cDisplayDraft;

    private readonly LEntryPort _cDisplayPort;

    private readonly LPhonologyPort _cDisplayPhonology;

    private readonly LMediaPort _cDisplayMedia;

    private readonly LSettingsPort _cDisplaySettings;

    private readonly CEnvoy _cDisplayEnvoy;

    internal CDisplaySound(
        LDisplaySound voice,
        CDisplay header,
        LDraftPort drafts,
        LEntryPort entries,
        LPhonologyPort phonology,
        LMediaPort media,
        LSettingsPort settings,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(voice);
        ArgumentNullException.ThrowIfNull(header);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(media);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cDisplayVoice = voice;
        _cDisplayHeader = header;
        _cDisplayDraft = drafts;
        _cDisplayPort = entries;
        _cDisplayPhonology = phonology;
        _cDisplayMedia = media;
        _cDisplaySettings = settings;
        _cDisplayEnvoy = envoy;
    }

    public event Action? CDisplayFoldChanged;

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
            _cDisplayEnvoy.CEnvoyFailureShow("Sound.LoadFailed", exception);
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
        catch (Exception)
        {
            return null;
        }

        return ReferenceEquals(shown, LDisplayShown) ? LDisplayAccentRead(sheet) : null;
    }

    private static CLecternAccent LDisplayAccentRead(LAccentSheet sheet)
    {
        string language = sheet.LAccentSheetLanguage;
        return new CLecternAccent(
            new CRespellingMark(sheet.LAccentSheetRespelled, sheet.LAccentSheetOpener, sheet.LAccentSheetCloser),
            sheet.LAccentSheetTonal,
            sheet.LAccentSheetPrimary.LAccentRowText,
            sheet.LAccentSheetSpoken,
            CSounding.CSoundingVarietyRead(language, sheet.LAccentSheetPrimary.LAccentRowVariety),
            sheet.LAccentSheetRows
                .Select(row => new CAccent(
                    row.LAccentRowId,
                    CSounding.CSoundingVarietyRead(language, row.LAccentRowVariety),
                    row.LAccentRowText,
                    row.LAccentRowAudio))
                .ToList(),
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
            _cDisplayEnvoy.CEnvoyFailureShow("Sound.LoadFailed", exception);
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
            _cDisplayEnvoy.CEnvoyFailureShow("Sound.PlayFailed", exception);
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
            _cDisplayEnvoy.CEnvoyFailureShow("Sound.LoadFailed", exception);
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
            LDisplayFontRead(CFontRole.CFontRoleGlyph));
    }

    public bool CDisplayGlyphOpen(string character, string language, Func<long, bool> entrySeam)
    {
        ArgumentNullException.ThrowIfNull(entrySeam);

        return CCatalog.LCatalogGlyphOpen(
                _cDisplayEnvoy, () => _cDisplayPort.LEngineGlyphResolve(character, language)) is long entry
            && entrySeam(entry);
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
            _cDisplayEnvoy.CEnvoyFailureShow("Sound.LoadFailed", exception);
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
            CSounding.CSoundingFanqieRead(_cDisplayVoice.LDisplayListRead(_cDisplayPhonology.LEngineFanqieDivide)),
            _cDisplayVoice.LDisplayFanqieCheck(id),
            LDisplayAnswerRead(() => _cDisplayPhonology.LEngineReadingRead(id, headword), string.Empty),
            LDisplayAnchorRead(LDisplayReflexScan()),
            LDisplayFontRead(CFontRole.CFontRoleGlyph));
    }

    public void CDisplayFanqieSet(long fanqieId, int rank)
    {
        if (LDisplayEntry is not long id)
        {
            return;
        }

        try
        {
            _cDisplayPhonology.LEngineFanqieSet(id, fanqieId, rank);
        }
        catch (Exception exception)
        {
            _cDisplayEnvoy.CEnvoyFailureShow("Display.FanqieRepresentativeFailed", exception);
        }
    }

    public bool CDisplayDiweiOpen(string kind, string key, Action<string, string, string> diweiSeam)
    {
        ArgumentNullException.ThrowIfNull(diweiSeam);

        if (LDisplayShown is null)
        {
            return false;
        }

        diweiSeam(_cDisplayHeader.CDisplayShown.CLecternLanguage, kind, key);
        return true;
    }

    public bool CDisplayStemOpen(string? key, Action<string, string?> stemSeam)
    {
        ArgumentNullException.ThrowIfNull(stemSeam);

        if (LDisplayShown is null)
        {
            return false;
        }

        stemSeam(_cDisplayHeader.CDisplayShown.CLecternLanguage, key);
        return true;
    }

    public CLecternScript CDisplayScriptRead()
    {
        if (LDisplayShown is null || LDisplayEntry is not long id)
        {
            return new CLecternScript([], false, _cDisplayBare);
        }

        return new CLecternScript(
            CSounding.CSoundingScriptRead(_cDisplayVoice.LDisplayListRead(_cDisplayPhonology.LEngineScriptDivide)),
            _cDisplayVoice.LDisplayScriptCheck(id),
            LDisplayFontRead(CFontRole.CFontRoleGlyph));
    }

    public CLecternParadigm CDisplayParadigmRead()
    {
        if (LDisplayShown is null || LDisplayEntry is not long id)
        {
            return new CLecternParadigm([], false, false, _cDisplayBare);
        }

        string language = LDisplayAnswerRead(() => _cDisplayPhonology.LEngineLanguageResolve(id), string.Empty);
        return new CLecternParadigm(
            CSounding.CSoundingParadigmRead(_cDisplayVoice.LDisplayListRead(_cDisplayPhonology.LEngineParadigmScan)),
            _cDisplayVoice.LDisplayParadigmCheck(id),
            _cDisplayVoice.LDisplayMorphologyRead(),
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
        catch (Exception)
        {
            return [];
        }
    }

    private CLecternAnchor LDisplayAnchorRead(IReadOnlyList<CReflex> rows)
    {
        if (LDisplayEntry is not long id)
        {
            return _cDisplayUnanchored;
        }

        string headword = _cDisplayHeader.CDisplayShown.CLecternHeadword;
        try
        {
            Dictionary<long, string> texts = [];
            foreach (CReflex reflex in rows)
            {
                texts[reflex.CReflexId] = _cDisplayDraft.LEngineAnchorFormat(
                    id, reflex.CReflexAnchors, headword, CReflex.LReflexSeparator);
            }

            return new CLecternAnchor(_cDisplayDraft.LEngineAnchorCheck(id, headword), texts);
        }
        catch (Exception)
        {
            return _cDisplayUnanchored;
        }
    }

    private CFont LDisplayFontRead(CFontRole role)
    {
        return CCatalog.LCatalogFontRead(_cDisplaySettings, _cDisplayHeader.CDisplayShown.CLecternLanguage, role);
    }

    private static LDisplayAnswer LDisplayAnswerRead<LDisplayAnswer>(Func<LDisplayAnswer> read, LDisplayAnswer fallback)
    {
        try
        {
            return read();
        }
        catch (Exception)
        {
            return fallback;
        }
    }
}
