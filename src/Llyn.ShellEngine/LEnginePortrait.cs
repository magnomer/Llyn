using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LPortrait LEnginePortraitRead(string entryId, LPortraitLabel label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
        ArgumentNullException.ThrowIfNull(label);

        LEntryDraft draft = LEngineEntryLoad(entryId)
            ?? throw new InvalidOperationException("The entry no longer stands in the workspace.");

        string mark = label.LPortraitLabelUnreadable;
        LSentenceOrder order = LEngineFrameRead(draft.LEntryDraftLanguage);

        List<string> ids = new List<string>();
        LPortraitLink.LPortraitLinkRead(draft.LEntryDraftMeanings, ids);
        LPortraitLink.LPortraitLinkRead(draft.LEntryDraftCollocations, ids);

        Dictionary<string, LPortraitLink> targets =
            new Dictionary<string, LPortraitLink>(StringComparer.Ordinal);

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

        IReadOnlyList<LUsage> incoming;
        try
        {
            incoming = LEngineIncomingRead(entryId);
        }
        catch (Exception)
        {
            incoming = [];
        }

        bool favorite;
        try
        {
            favorite = LEngineFavoriteCheck(entryId);
        }
        catch (Exception)
        {
            favorite = false;
        }

        return new LPortrait(
            draft.LEntryDraftHeadword,
            draft.LEntryDraftLanguage,
            draft.LEntryDraftIpa,
            LEngineSpeechShow(draft.LEntryDraftSpeeches),
            LPortraitCard.LPortraitCardCreate(
                draft.LEntryDraftMeanings, label.LPortraitLabelMeaning, order, mark, targets),
            LPortraitCard.LPortraitCardCreate(
                draft.LEntryDraftCollocations, label.LPortraitLabelCollocation, order, mark, targets),
            LPortraitUsage.LPortraitUsageCreate(incoming, label),
            draft.LEntryDraftNote,
            favorite,
            label);
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
