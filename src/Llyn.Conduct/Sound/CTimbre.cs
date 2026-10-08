using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CTimbre
{
    private readonly CDesk _cTimbreDesk;

    private readonly LLanguagePort _cTimbreLanguagePort;

    private readonly LReflexPort _cTimbreReflexPort;

    private readonly LDisplay _cTimbreDisplay;

    private readonly LDraftPort _cTimbreDraftPort;

    private readonly LSettingsPort _cTimbreSettingsPort;

    private readonly CEnvoy _cTimbreEnvoy;

    internal CTimbre(
        CDesk desk,
        LLanguagePort languages,
        LReflexPort reflexes,
        LDisplay display,
        LDraftPort drafts,
        LSettingsPort settings,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(reflexes);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cTimbreDesk = desk;
        _cTimbreLanguagePort = languages;
        _cTimbreReflexPort = reflexes;
        _cTimbreDisplay = display;
        _cTimbreDraftPort = drafts;
        _cTimbreSettingsPort = settings;
        _cTimbreEnvoy = envoy;
        desk.CDeskDraftPrepared += LTimbreReflexStart;
    }

    public event Action? CTimbreParadigmChanged;

    public event Action? CTimbreScriptChanged;

    public event Action? CTimbreReflexChanged;

    public bool CTimbreSpoken => !_cTimbreLanguagePort.LEngineSilentCheck(LTimbreLanguage);

    public bool CTimbrePhonemic =>
        _cTimbreSettingsPort.LEngineRespellingCheck(LTimbreLanguage)
        && _cTimbreSettingsPort.LEnginePhonemicCheck(LTimbreLanguage);

    public bool CTimbreReflexPending => _cTimbreDisplay.LDisplaySound.LDisplayReflexCheck(LTimbreEntry);

    public IReadOnlyList<CContour> CTimbreContourRead(string ipa)
    {
        return CSounding.LSoundingContourRead(
            _cTimbreLanguagePort.LEngineContourRead(LTimbreLanguage, ipa), _cTimbreLanguagePort.LEngineContourScale);
    }

    public CAccentTyped CTimbreAccentSet(long accent, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (LTimbreTenure is not LTenure held)
        {
            return new CAccentTyped(LTimbreAccentFind(accent));
        }

        new LQuillPronunciation(held).LQuillAccentSet(accent, text);
        return new CAccentTyped(text);
    }

    private string LTimbreAccentFind(long accent)
    {
        return CTimbreAccentRead().CTimbreAccentRows.FirstOrDefault(row => row.CAccentId == accent) is CAccent row
            ? row.CAccentText
            : string.Empty;
    }

    public void CTimbrePronunciationAdd(long? accent)
    {
        if (LTimbreTenure is LTenure held)
        {
            new LQuillPronunciation(held).LQuillPronunciationAdd(accent ?? 0);
        }
    }

    public void CTimbrePronunciationRemove(long? accent)
    {
        if (accent is long held && LTimbreTenure is LTenure tenure)
        {
            new LQuillPronunciation(tenure).LQuillPronunciationRemove(held);
        }
    }

    public CTimbreAccent CTimbreAccentRead()
    {
        return _cTimbreDesk.CDeskTenure is LTenure held
            && new LQuillPronunciation(held).LQuillAccentRead() is { } sheet
            ? LTimbreAccentRead(sheet)
            : new CTimbreAccent(
                new CRespellingMark(false, string.Empty, string.Empty),
                CSounding.CSoundingVarietyRead(string.Empty, string.Empty),
                [],
                false);
    }

    public async Task<CTimbreAccent?> CTimbreFlagRead(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(store);

        if (_cTimbreDesk.CDeskTenure is not LTenure held)
        {
            return null;
        }

        try
        {
            return await new LQuillPronunciation(held).LQuillAccentLoad(
                        (rows, delete) => store(CCatalog.CCatalogEnsignRead(rows), delete))
                    is LAccentSheet sheet
                && ReferenceEquals(held, _cTimbreDesk.CDeskTenure)
                ? LTimbreAccentRead(sheet)
                : null;
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cTimbreEnvoy, _cTimbreSettingsPort, "Sound.LoadFailed", exception);
            return null;
        }
    }

    private static CTimbreAccent LTimbreAccentRead(LAccentSheet sheet)
    {
        string language = sheet.LAccentSheetLanguage;
        return new CTimbreAccent(
            new CRespellingMark(sheet.LAccentSheetRespelled, sheet.LAccentSheetOpener, sheet.LAccentSheetCloser),
            CSounding.CSoundingVarietyRead(language, sheet.LAccentSheetPrimary.LAccentRowVariety),
            sheet.LAccentSheetRows.Select(row => CSounding.LSoundingAccentRead(language, row)).ToList(),
            sheet.LAccentSheetFlagged);
    }

    public CFont CTimbreFontRead(CFontRole role)
    {
        return CCatalog.LCatalogFontRead(_cTimbreSettingsPort, LTimbreLanguage, role);
    }

    public CTimbreGlyph CTimbreGlyphRead()
    {
        return _cTimbreDesk.CDeskTenure is LTenure held
            && new LQuillPronunciation(held).LQuillGlyphRead() is LGlyphBlock block
            ? new CTimbreGlyph(
                block.LGlyphBlockShown,
                block.LGlyphBlockSourced,
                CSounding.CSoundingTranscriptionRead(block.LGlyphBlockRows))
            : new CTimbreGlyph(false, false, []);
    }

    public CTimbreReflex CTimbreReflexRead()
    {
        bool opened = _cTimbreDisplay.LDisplaySound.LDisplayFoldOpened;
        if (_cTimbreDesk.CDeskTenure is not LTenure held || held.LTenureRead() is not { } draft)
        {
            return new CTimbreReflex(
                false, [], new CLecternAnchor(false, new Dictionary<long, string>()), opened, false);
        }

        LEntryDraft content = draft.LDraftContent;
        IReadOnlyList<CReflex> rows = CRespelling.LRespellingReflexScan(
            _cTimbreReflexPort,
            content.LEntryDraftLanguage,
            CSounding.CSoundingReflexRead(content.LEntryDraftReflexes));
        return new CTimbreReflex(
            new LQuillReflex(held, _cTimbreReflexPort).LQuillReflexCheck(),
            rows,
            CReflex.LReflexAnchorRead(
                _cTimbreEnvoy,
                _cTimbreSettingsPort,
                _cTimbreDisplay.LDisplayNoticed,
                _cTimbreDraftPort,
                LTimbreEntry,
                content.LEntryDraftHeadword,
                rows),
            opened,
            CTimbreReflexPending);
    }

    private void LTimbreReflexStart(LDraft _)
    {
        if (_cTimbreDesk.CDeskTenure is LTenure held)
        {
            new LQuillReflex(held, _cTimbreReflexPort).LQuillReflexStart();
        }
    }

    public void CTimbreReflexRebuild()
    {
        _cTimbreDisplay.LDisplaySound.LDisplayReflexRebuild(LTimbreEntry);
    }

    public void CTimbreReflexAdd(long? reflex)
    {
        LTimbreQuill?.LQuillReflexAdd(reflex ?? 0);
    }

    public void CTimbreReflexRemove(long reflex)
    {
        LTimbreQuill?.LQuillReflexRemove(reflex);
    }

    public void CTimbreReflexToggle(long reflex)
    {
        LTimbreQuill?.LQuillReflexToggle(reflex);
    }

    public CReflexTyped CTimbreReflexSet(long reflex, CReflexField field, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (LTimbreQuill is not LQuillReflex quill)
        {
            return new CReflexTyped(field, LTimbreReflexFind(reflex, field), []);
        }

        switch (field)
        {
            case CReflexField.CReflexFieldLanguage:
                return new CReflexTyped(field, text, LTimbreLeadRead(quill.LQuillLanguageSet(reflex, text)));
            case CReflexField.CReflexFieldKind:
                quill.LQuillKindSet(reflex, text);
                break;
            case CReflexField.CReflexFieldText:
                quill.LQuillTextSet(reflex, text);
                break;
            case CReflexField.CReflexFieldRomanization:
                quill.LQuillRomanizationSet(reflex, text);
                break;
            case CReflexField.CReflexFieldMeaning:
                quill.LQuillMeaningSet(reflex, text);
                break;
            case CReflexField.CReflexFieldNote:
                quill.LQuillNoteSet(reflex, text);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(field), field, null);
        }

        return new CReflexTyped(field, text, []);
    }

    private string LTimbreReflexFind(long reflex, CReflexField field)
    {
        return CTimbreReflexRead().CTimbreReflexRows.FirstOrDefault(row => row.CReflexId == reflex) is CReflex row
            ? row.LReflexFieldRead(field)
            : string.Empty;
    }

    private static IReadOnlyList<CReflexHead> LTimbreLeadRead(IReadOnlyList<LReflexDraft> typed)
    {
        IReadOnlyList<CReflexDraft> rows = CSounding.CSoundingReflexRead(typed);
        IReadOnlyList<bool> leads =
            CReflex.LReflexLeadRead(rows.Select(static row => row.CReflexDraftLanguage).ToList());
        return rows.Select((row, index) => new CReflexHead(row.CReflexDraftId, leads[index])).ToList();
    }

    internal void LTimbreObserverAttach(Action<Action> marshal)
    {
        _cTimbreDesk.CDeskVigil.LVigilEntryAttach(
            CSubject.CSubjectInflection, _ => marshal(() => CTimbreParadigmChanged?.Invoke()));
        _cTimbreDesk.CDeskVigil.LVigilEntryAttach(
            CSubject.CSubjectReflex, _ => marshal(() => CTimbreReflexChanged?.Invoke()));
        _cTimbreDesk.CDeskVigil.LVigilObserverAttach(
            CSubject.CSubjectScript, _ => marshal(() => CTimbreScriptChanged?.Invoke()));
    }

    private string LTimbreLanguage => _cTimbreDesk.CDeskTenure?.LTenureLanguageRead() ?? string.Empty;

    private long? LTimbreEntry => _cTimbreDesk.CDeskStoredRead();

    private LTenure? LTimbreTenure => _cTimbreDesk.CDeskFilling ? null : _cTimbreDesk.CDeskTenure;

    internal LQuillReflex? LTimbreQuill =>
        LTimbreTenure is LTenure held ? new LQuillReflex(held, _cTimbreReflexPort) : null;
}
