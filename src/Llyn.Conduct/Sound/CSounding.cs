using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CSounding
{
    private readonly CDesk _cSoundingDesk;

    private readonly LPhonologyPort _cSoundingPhonologyPort;

    private readonly CEnvoy _cSoundingEnvoy;

    private readonly LSettingsPort _cSoundingSettingsPort;

    private readonly LDisplaySound _cSoundingVoice;

    private readonly CLedgerNoticed _cSoundingNoticed;

    internal CSounding(
        CDesk desk,
        LPhonologyPort phonology,
        LSettingsPort settings,
        LDisplay display,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(display);
        ArgumentNullException.ThrowIfNull(envoy);

        _cSoundingDesk = desk;
        _cSoundingPhonologyPort = phonology;
        _cSoundingEnvoy = envoy;
        _cSoundingSettingsPort = settings;
        _cSoundingVoice = display.LDisplaySound;
        _cSoundingNoticed = display.LDisplayNoticed;
    }

    public event Action? CSoundingChanged;

    internal event Action<string, string, string>? LSoundingDiweiChosen;

    internal void LSoundingObserverAttach(Action<Action> marshal)
    {
        _cSoundingDesk.CDeskVigil.LVigilObserverAttach(
            CSubject.CSubjectFanqie, _ => marshal(() => CSoundingChanged?.Invoke()));
    }

    private long? LSoundingEntry => _cSoundingDesk.CDeskStoredRead();

    private string LSoundingLanguage => _cSoundingDesk.CDeskTenure?.LTenureLanguageRead() ?? string.Empty;

    public CSoundingFanqie CSoundingFanqieRead()
    {
        long? entry = LSoundingEntry;
        return new CSoundingFanqie(
            CSoundingFanqieRead(LSoundingListRead(
                _cSoundingPhonologyPort.LEngineFanqieRead, "Display.FanqieReadFailed")),
            _cSoundingVoice.LDisplayFanqieCheck(entry),
            entry is not null
            && _cSoundingNoticed.LLedgerRepaintRead(_cSoundingEnvoy, _cSoundingSettingsPort,
                () => _cSoundingPhonologyPort.LEngineBookCheck(LSoundingLanguage), false, "Display.BookFailed"),
            CCatalog.LCatalogFontRead(_cSoundingSettingsPort, LSoundingLanguage, CFontRole.CFontRoleGlyph));
    }

    public string CSoundingReadingRead(string headword)
    {
        return LSoundingEntry is long id
            ? _cSoundingNoticed.LLedgerRepaintRead(_cSoundingEnvoy, _cSoundingSettingsPort,
                () => _cSoundingPhonologyPort.LEngineReadingRead(id, headword), string.Empty, "Display.ReadingFailed")
            : string.Empty;
    }

    public void CSoundingFanqieResolve()
    {
        LSoundingMarkSend(_cSoundingPhonologyPort.LEngineFanqieRebuild, "Display.FanqieRebuildFailed");
    }

    public void CSoundingFanqieSet(long fanqieId, int rank, bool raise)
    {
        LSoundingMarkSend(
            id => _cSoundingPhonologyPort.LEngineFanqieSet(id, fanqieId, rank, raise),
            "Display.FanqieRepresentativeFailed");
    }

    public void CSoundingDiweiOpen(bool initial, string key)
    {
        if (LPhonologyPort.LEngineDiweiRead(initial, key) is string kind)
        {
            LSoundingDiweiChosen?.Invoke(LSoundingLanguage, kind, key);
        }
    }

    public CSoundingScript CSoundingScriptRead()
    {
        long? entry = LSoundingEntry;
        return new CSoundingScript(
            CSoundingScriptRead(LSoundingListRead(
                _cSoundingPhonologyPort.LEngineScriptRead, "Display.ScriptReadFailed")),
            _cSoundingVoice.LDisplayScriptCheck(entry),
            entry is not null
            && _cSoundingNoticed.LLedgerRepaintRead(_cSoundingEnvoy, _cSoundingSettingsPort,
                () => _cSoundingPhonologyPort.LEngineStyleCheck(LSoundingLanguage), false, "Display.StyleFailed"),
            CCatalog.LCatalogFontRead(_cSoundingSettingsPort, LSoundingLanguage, CFontRole.CFontRoleGlyph));
    }

    public void CSoundingScriptResolve()
    {
        LSoundingMarkSend(_cSoundingPhonologyPort.LEngineScriptRebuild, "Display.ScriptRebuildFailed");
    }

    public CLecternParadigm CSoundingParadigmRead()
    {
        string language = LSoundingEntry is long id
            ? _cSoundingNoticed.LLedgerRepaintRead(_cSoundingEnvoy, _cSoundingSettingsPort,
                () => _cSoundingPhonologyPort.LEngineLanguageResolve(id), string.Empty, "Display.LanguageFailed")
            : string.Empty;
        IReadOnlyList<LParadigmRow> rows =
            LSoundingListRead(_cSoundingPhonologyPort.LEngineParadigmScan, "Display.ParadigmReadFailed");
        bool pending = _cSoundingVoice.LDisplayParadigmCheck(LSoundingEntry);
        return new CLecternParadigm(
            CSoundingParadigmRead(rows, pending, _cSoundingVoice.LDisplayMorphologyRead(), true),
            CCatalog.LCatalogFontRead(_cSoundingSettingsPort, language, CFontRole.CFontRoleHeadword));
    }

    private IReadOnlyList<LSoundingItem> LSoundingListRead<LSoundingItem>(
        Func<long, IReadOnlyList<LSoundingItem>> read, string key)
    {
        return LSoundingEntry is long id
            ? _cSoundingNoticed.LLedgerRepaintRead(_cSoundingEnvoy, _cSoundingSettingsPort, () => read(id), [], key)
            : [];
    }

    private void LSoundingMarkSend(Action<long> mark, string key)
    {
        if (LSoundingEntry is not long id)
        {
            return;
        }

        try
        {
            mark(id);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cSoundingEnvoy, _cSoundingSettingsPort, key, exception);
            return;
        }

        CSoundingChanged?.Invoke();
    }

    internal static IReadOnlyList<CFanqieGroup> CSoundingFanqieRead(IReadOnlyList<LFanqieGroup> groups)
    {
        ArgumentNullException.ThrowIfNull(groups);

        return groups
            .Select(static group => new CFanqieGroup(
                group.LFanqieGroupHeading,
                group.LFanqieGroupLabel,
                group.LFanqieGroupSource,
                group.LFanqieGroupStems,
                group.LFanqieGroupRows.Select(LSoundingRowRead).ToList()))
            .ToList();
    }

    private static CFanqieRow LSoundingRowRead(LFanqieRow row)
    {
        return new CFanqieRow(
            row.LFanqieRowId,
            row.LFanqieRowRepresentative,
            row.LFanqieRowMarked,
            row.LFanqieRowPrimary,
            row.LFanqieRowOrder,
            row.LFanqieRowClosed,
            row.LFanqieRowSlashed,
            row.LFanqieRowLabel,
            row.LFanqieRowInitial,
            row.LFanqieRowCell,
            row.LFanqieRowBracketed,
            row.LFanqieRowKnotted,
            row.LFanqieRowMedial,
            row.LFanqieRowGraded,
            row.LFanqieRowTone,
            row.LFanqieRowSpelling,
            row.LFanqieRowRemainder);
    }

    internal static IReadOnlyList<CScriptGroup> CSoundingScriptRead(IReadOnlyList<LScriptGroup> groups)
    {
        ArgumentNullException.ThrowIfNull(groups);

        return groups
            .Select(static group => new CScriptGroup(
                group.LScriptGroupHeading,
                group.LScriptGroupStyle,
                group.LScriptGroupGloss,
                group.LScriptGroupImages.Select(LSoundingImageRead).ToList()))
            .ToList();
    }

    private static CScriptImage LSoundingImageRead(LScriptImage image)
    {
        return new CScriptImage(image.LScriptImageData, image.LScriptImageCaption, image.LScriptImageEpoch);
    }

    internal static IReadOnlyList<CParadigmSlot> CSoundingParadigmRead(
        IReadOnlyList<LParadigmRow> rows, bool pending, bool enabled, bool held)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return rows
            .Select(row => LSoundingSlotRead(row, LPhonologyPort.LEngineParadigmCheck(row, pending, enabled), held))
            .ToList();
    }

    private static CParadigmSlot LSoundingSlotRead(LParadigmRow row, LParadigmStatus status, bool held)
    {
        string text = row.LParadigmRowFirst.LParadigmSlotInflection?.LInflectionText ?? string.Empty;
        return status switch
        {
            LParadigmStatus.LParadigmStatusText => new CParadigmSlot(
                row.LParadigmRowPart, row.LParadigmRowName, text, null),
            LParadigmStatus.LParadigmStatusUnknown => new CParadigmSlot(
                row.LParadigmRowPart, row.LParadigmRowName, "—", "Paradigm.Unknown"),
            LParadigmStatus.LParadigmStatusPending => new CParadigmSlot(
                row.LParadigmRowPart, row.LParadigmRowName, "…", "Paradigm.Pending"),
            LParadigmStatus.LParadigmStatusLost => new CParadigmSlot(
                row.LParadigmRowPart, row.LParadigmRowName, "…", held ? "Paradigm.Held" : "Paradigm.Lost"),
            LParadigmStatus.LParadigmStatusAbsent => new CParadigmSlot(
                row.LParadigmRowPart, row.LParadigmRowName, "…", "Paradigm.Absent"),
            _ => throw new ArgumentOutOfRangeException(nameof(status)),
        };
    }

    public static CVariety CSoundingVarietyRead(string language, string variety)
    {
        ArgumentNullException.ThrowIfNull(variety);

        return new CVariety(
            variety, string.Concat("Variety.", variety), LSettingsPort.LEngineEnsignFormat(language, variety));
    }

    internal static CAccent LSoundingAccentRead(string language, LAccentRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        return new CAccent(
            row.LAccentRowId,
            CSoundingVarietyRead(language, row.LAccentRowVariety),
            row.LAccentRowText,
            row.LAccentRowAudio);
    }

    internal static IReadOnlyList<CContour> LSoundingContourRead(IReadOnlyList<LContour> syllables)
    {
        ArgumentNullException.ThrowIfNull(syllables);

        return syllables
            .Select(static syllable => new CContour(
                syllable.LContourText, syllable.LContourLevels, syllable.LContourToned))
            .ToList();
    }

    internal static IReadOnlyList<CTranscriptionDraft> CSoundingTranscriptionRead(
        IReadOnlyList<LTranscriptionDraft> transcriptions)
    {
        ArgumentNullException.ThrowIfNull(transcriptions);

        return transcriptions
            .Select(static row => new CTranscriptionDraft(
                row.LTranscriptionDraftId, row.LTranscriptionDraftScheme, row.LTranscriptionDraftText))
            .ToList();
    }

    internal static IReadOnlyList<CReflexDraft> CSoundingReflexRead(IReadOnlyList<LReflexDraft> reflexes)
    {
        ArgumentNullException.ThrowIfNull(reflexes);

        return reflexes.Select(CSoundingReflexRead).ToList();
    }

    private static CReflexDraft CSoundingReflexRead(LReflexDraft reflex)
    {
        return new CReflexDraft(
            reflex.LReflexDraftId,
            reflex.LReflexDraftLanguage,
            reflex.LReflexDraftKind,
            reflex.LReflexDraftText,
            reflex.LReflexDraftRespelling,
            reflex.LReflexDraftRomanization,
            reflex.LReflexDraftMeaning,
            reflex.LReflexDraftNote,
            reflex.LReflexDraftMain,
            reflex.LReflexDraftRegion,
            reflex.LReflexDraftAnchors);
    }
}
