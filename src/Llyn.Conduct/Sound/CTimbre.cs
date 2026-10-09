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

    private readonly LSettingsPort _cTimbreSettingsPort;

    private readonly CEnvoy _cTimbreEnvoy;

    internal CTimbre(CDesk desk, LLanguagePort languages, LSettingsPort settings, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(envoy);

        _cTimbreDesk = desk;
        _cTimbreLanguagePort = languages;
        _cTimbreSettingsPort = settings;
        _cTimbreEnvoy = envoy;
    }

    public event Action? CTimbreParadigmChanged;

    public event Action? CTimbreScriptChanged;

    public bool CTimbreSpoken => !_cTimbreLanguagePort.LEngineSilentCheck(LTimbreLanguage);

    public bool CTimbrePhonemic =>
        _cTimbreSettingsPort.LEngineRespellingCheck(LTimbreLanguage)
        && _cTimbreSettingsPort.LEnginePhonemicCheck(LTimbreLanguage);

    public IReadOnlyList<CContour> CTimbreContourRead(string ipa)
    {
        return CContour.CContourRead(
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
        return _cTimbreDesk.CDeskDraft.CDeskDraftTenure is LTenure held
            && new LQuillPronunciation(held).LQuillAccentRead() is { } sheet
            ? LTimbreAccentRead(sheet)
            : new CTimbreAccent(
                new CRespellingMark(false, string.Empty, string.Empty),
                CVariety.CVarietyRead(string.Empty, string.Empty),
                [],
                false);
    }

    public async Task<CTimbreAccent?> CTimbreFlagRead(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)
    {
        ArgumentNullException.ThrowIfNull(store);

        if (_cTimbreDesk.CDeskDraft.CDeskDraftTenure is not LTenure held)
        {
            return null;
        }

        try
        {
            return await new LQuillPronunciation(held).LQuillAccentLoad(
                        (rows, delete) => store(CCatalog.CCatalogEnsignRead(rows), delete))
                    is LAccentSheet sheet
                && ReferenceEquals(held, _cTimbreDesk.CDeskDraft.CDeskDraftTenure)
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
            CVariety.CVarietyRead(language, sheet.LAccentSheetPrimary.LAccentRowVariety),
            sheet.LAccentSheetRows.Select(row => CAccent.CAccentRead(language, row)).ToList(),
            sheet.LAccentSheetFlagged);
    }

    public CFont CTimbreFontRead(CFontRole role)
    {
        return CFont.CFontRead(_cTimbreSettingsPort, LTimbreLanguage, role);
    }

    public CTimbreGlyph CTimbreGlyphRead()
    {
        return _cTimbreDesk.CDeskDraft.CDeskDraftTenure is LTenure held
            && new LQuillPronunciation(held).LQuillGlyphRead() is LGlyphBlock block
            ? new CTimbreGlyph(
                block.LGlyphBlockShown,
                block.LGlyphBlockSourced,
                CTranscriptionDraft.CTranscriptionDraftRead(block.LGlyphBlockRows))
            : new CTimbreGlyph(false, false, []);
    }

    internal void LTimbreObserverAttach(Action<Action> marshal)
    {
        _cTimbreDesk.CDeskVigil.LVigilEntryAttach(
            CSubject.CSubjectInflection, _ => marshal(() => CTimbreParadigmChanged?.Invoke()));
        _cTimbreDesk.CDeskVigil.LVigilObserverAttach(
            CSubject.CSubjectInflection,
            bulletin =>
            {
                if (bulletin.CBulletinId <= 0)
                {
                    marshal(() => CTimbreParadigmChanged?.Invoke());
                }
            });
        _cTimbreDesk.CDeskVigil.LVigilObserverAttach(
            CSubject.CSubjectScript, _ => marshal(() => CTimbreScriptChanged?.Invoke()));
    }

    private string LTimbreLanguage => _cTimbreDesk.CDeskDraft.CDeskDraftTenure?.LTenureLanguageRead() ?? string.Empty;

    private LTenure? LTimbreTenure =>
        _cTimbreDesk.CDeskDraft.CDeskDraftFilling ? null : _cTimbreDesk.CDeskDraft.CDeskDraftTenure;
}
