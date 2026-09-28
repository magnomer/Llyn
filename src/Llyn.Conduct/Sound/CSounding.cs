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

    private readonly LDraftPort _cSoundingDraftPort;

    private readonly CEnvoy _cSoundingEnvoy;

    internal CSounding(CDesk desk, LPhonologyPort phonology, LDraftPort drafts, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(envoy);

        _cSoundingDesk = desk;
        _cSoundingPhonologyPort = phonology;
        _cSoundingDraftPort = drafts;
        _cSoundingEnvoy = envoy;
    }

    public event Action? CSoundingChanged;

    private long? LSoundingEntry => _cSoundingDesk.CDeskStoredRead();

    private string LSoundingLanguage => _cSoundingDesk.CDeskTenure?.LTenureLanguageRead() ?? string.Empty;

    public IReadOnlyList<CFanqieGroup> CSoundingFanqieRead()
    {
        return CSoundingFanqieRead(LSoundingListRead(_cSoundingPhonologyPort.LEngineFanqieRead));
    }

    public string CSoundingReadingRead(string headword)
    {
        return LSoundingEntry is long id
            ? LSoundingAnswerRead(() => _cSoundingPhonologyPort.LEngineReadingRead(id, headword), string.Empty)
            : string.Empty;
    }

    public bool CSoundingAnchorCheck(string headword)
    {
        return LSoundingEntry is long id
            && LSoundingAnswerRead(() => _cSoundingDraftPort.LEngineAnchorCheck(id, headword), false);
    }

    public string CSoundingAnchorFormat(IReadOnlyList<long> anchors, string headword)
    {
        return LSoundingEntry is long id
            ? LSoundingAnswerRead(
                () => _cSoundingDraftPort.LEngineAnchorFormat(id, anchors, headword, CReflex.LReflexSeparator),
                string.Empty)
            : string.Empty;
    }

    public IReadOnlyList<CScheme> CSoundingSchemeRead(long transcription)
    {
        return (_cSoundingDesk.CDeskTenure?.LTenureSchemeRead(transcription) ?? [])
            .Select(static row => new CScheme(row.LSchemeRowName, row.LSchemeRowTaken))
            .ToList();
    }

    public IReadOnlyList<CAnchorRow> CSoundingAnchorScan(IReadOnlyList<long> anchors, string reflex, string tone)
    {
        string language = LSoundingLanguage;
        return LSoundingListRead(id => _cSoundingDraftPort.LEngineAnchorScan(id, anchors, language, reflex, tone))
            .Select(static row => new CAnchorRow(
                row.LAnchorRowFanqie.LFanqieRowId,
                row.LAnchorRowFanqie.LFanqieRowSummary,
                row.LAnchorRowHeld,
                row.LAnchorRowEstimated))
            .ToList();
    }

    public void CSoundingFanqieResolve()
    {
        LSoundingMarkSend(_cSoundingPhonologyPort.LEngineFanqieRebuild, "Display.FanqieRebuildFailed");
    }

    public void CSoundingFanqieSet(long fanqieId, int rank)
    {
        LSoundingMarkSend(
            id => _cSoundingPhonologyPort.LEngineFanqieSet(id, fanqieId, rank), "Display.FanqieRepresentativeFailed");
    }

    public IReadOnlyList<CScriptGroup> CSoundingScriptRead()
    {
        return CSoundingScriptRead(LSoundingListRead(_cSoundingPhonologyPort.LEngineScriptRead));
    }

    public void CSoundingScriptResolve()
    {
        LSoundingMarkSend(_cSoundingPhonologyPort.LEngineScriptRebuild, "Display.ScriptRebuildFailed");
    }

    public IReadOnlyList<CParadigmSlot> CSoundingParadigmRead()
    {
        return CSoundingParadigmRead(LSoundingListRead(_cSoundingPhonologyPort.LEngineParadigmScan));
    }

    public string CSoundingLanguageRead()
    {
        return LSoundingEntry is long id
            ? LSoundingAnswerRead(() => _cSoundingPhonologyPort.LEngineLanguageResolve(id), string.Empty)
            : string.Empty;
    }

    private IReadOnlyList<LSoundingItem> LSoundingListRead<LSoundingItem>(
        Func<long, IReadOnlyList<LSoundingItem>> read)
    {
        return LSoundingEntry is long id ? LSoundingAnswerRead(() => read(id), []) : [];
    }

    private static LSoundingAnswer LSoundingAnswerRead<LSoundingAnswer>(
        Func<LSoundingAnswer> read, LSoundingAnswer fallback)
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
            _cSoundingEnvoy.CEnvoyFailureShow(key, exception);
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

    internal static IReadOnlyList<CParadigmSlot> CSoundingParadigmRead(IReadOnlyList<LParadigmRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        return rows.Select(LSoundingSlotRead).ToList();
    }

    private static CParadigmSlot LSoundingSlotRead(LParadigmRow row)
    {
        return new CParadigmSlot(
            row.LParadigmRowPart,
            row.LParadigmRowName,
            row.LParadigmRowFirst.LParadigmSlotInflection?.LInflectionText,
            row.LParadigmRowFirst.LParadigmSlotUncertain);
    }

    internal static IReadOnlyList<CPronunciationDraft> CSoundingPronunciationRead(
        IReadOnlyList<LPronunciationDraft> spoken)
    {
        ArgumentNullException.ThrowIfNull(spoken);

        return spoken.Select(CSoundingPronunciationRead).ToList();
    }

    internal static CPronunciationDraft CSoundingPronunciationRead(LPronunciationDraft spoken)
    {
        ArgumentNullException.ThrowIfNull(spoken);

        return new CPronunciationDraft(
            spoken.LPronunciationDraftId,
            spoken.LPronunciationDraftIpa,
            spoken.LPronunciationDraftRespelling,
            spoken.LPronunciationDraftVariety,
            spoken.LPronunciationDraftAudio);
    }

    public static CVariety CSoundingVarietyRead(string language, string variety)
    {
        ArgumentNullException.ThrowIfNull(variety);

        return new CVariety(
            variety, string.Concat("Variety.", variety), LSettingsPort.LEngineEnsignFormat(language, variety));
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
            reflex.LReflexDraftAnchors,
            reflex.LReflexDraftAnatomy.LAnatomyToneIpa);
    }
}
