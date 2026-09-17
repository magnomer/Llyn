using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LPortraitPage LEnginePortraitRead(long entryId, LPortraitLabel label)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(label);

        LEntryDraft draft = LEngineEntryLoad(entryId)
            ?? throw new InvalidOperationException("The entry no longer stands in the workspace.");

        LSentenceOrder order = LEngineFrameRead(draft.LEntryDraftLanguage);

        List<long> ids = [];
        LPortraitLink.LPortraitLinkRead(draft.LEntryDraftMeanings, ids);
        LPortraitLink.LPortraitLinkRead(draft.LEntryDraftCollocations, ids);

        IReadOnlyDictionary<long, LPortraitLink> targets = LEngineTargetScan(ids);
        IReadOnlyDictionary<long, string> sources = LEngineSourceScan();

        bool favorite;
        try
        {
            favorite = LEngineFavoriteCheck(entryId);
        }
        catch (Exception)
        {
            favorite = false;
        }

        IReadOnlyList<LFanqieRow> fanqie = LEngineFanqieScan(entryId, draft.LEntryDraftLanguage);

        List<LPortraitSection> sections = [];
        LEngineGlyphAdd(sections, draft, label);
        LEngineFrequencyAdd(sections, entryId, label);
        LEngineFormAdd(sections, draft, label);
        LEngineParadigmAdd(sections, entryId, label);
        LEngineFanqieAdd(sections, fanqie, label);
        LEngineBandAdd(
            sections,
            label.LPortraitLabelMeanings,
            LPortraitCard.LPortraitCardCreate(
                draft.LEntryDraftMeanings,
                label.LPortraitLabelMeaning,
                order,
                draft.LEntryDraftLanguage,
                label,
                targets,
                sources));
        LEngineBandAdd(
            sections,
            label.LPortraitLabelCollocations,
            LPortraitCard.LPortraitCardCreate(
                draft.LEntryDraftCollocations,
                label.LPortraitLabelCollocation,
                order,
                draft.LEntryDraftLanguage,
                label,
                targets,
                sources));
        LEngineIncomingAdd(sections, entryId, label);
        LEngineNoteAdd(sections, draft, label);
        LEngineScriptAdd(sections, entryId, draft.LEntryDraftLanguage, label);

        return new LPortraitPage(
            draft.LEntryDraftHeadword,
            draft.LEntryDraftLanguage,
            LEngineSpeechShow(draft.LEntryDraftSpeeches),
            sections,
            favorite,
            [
                .. LPortraitReading.LPortraitReadingCreate(
                    draft.LEntryDraftPronunciations,
                    LEngineRespellingCheck(draft.LEntryDraftLanguage),
                    LEnginePhonemicCheck(draft.LEntryDraftLanguage)),
                .. LPortraitReading.LPortraitReadingCreate(draft.LEntryDraftTranscriptions),
                .. LPortraitReading.LPortraitReadingCreate(
                    draft.LEntryDraftReflexes, LEngineRespellingCheck, LEnginePhonemicCheck, fanqie),
            ]);
    }

    private IReadOnlyDictionary<long, LPortraitLink> LEngineTargetScan(IReadOnlyList<long> ids)
    {
        Dictionary<long, LPortraitLink> targets = [];

        try
        {
            foreach (LTranslationTarget target in LEngineTargetRead(ids))
            {
                targets[target.LTranslationTargetId] = new LPortraitLink(
                    target.LTranslationTargetId,
                    target.LTranslationTargetHeadword,
                    target.LTranslationTargetLanguage);
            }
        }
        catch (Exception)
        {
            targets.Clear();
        }

        return targets;
    }

    private IReadOnlyDictionary<long, string> LEngineSourceScan()
    {
        try
        {
            return LEngineCitationRead();
        }
        catch (Exception)
        {
            return new Dictionary<long, string>();
        }
    }

    private IReadOnlyList<LFanqieRow> LEngineFanqieScan(long entryId, string language)
    {
        try
        {
            return LEngineBookRead(language).Count == 0 ? [] : LEngineFanqieRead(entryId);
        }
        catch (Exception)
        {
            return [];
        }
    }

    private LSentenceOrder LEngineFrameRead(string language)
    {
        try
        {
            return LEngineOrderRead(language);
        }
        catch (Exception)
        {
            return LSentenceOrder.LSentenceOrderDefault;
        }
    }
}
