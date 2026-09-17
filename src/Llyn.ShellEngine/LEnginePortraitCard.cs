using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private static void LEngineBandAdd(
        List<LPortraitSection> sections, string heading, IReadOnlyList<LPortraitSection> cards)
    {
        if (cards.Count > 0)
        {
            sections.Add(LPortraitSection.LPortraitSectionCreate(heading, cards));
        }
    }

    private void LEngineIncomingAdd(List<LPortraitSection> sections, long entryId, LPortraitLabel label)
    {
        IReadOnlyList<LUsage> incoming;
        try
        {
            incoming = LEngineIncomingRead(entryId);
        }
        catch (Exception)
        {
            incoming = [];
        }

        if (incoming.Count == 0)
        {
            return;
        }

        List<LPortraitSection> rows = [];
        foreach (LUsage usage in incoming)
        {
            string owner = usage.LUsageOwner == LOwner.LOwnerCollocation
                ? label.LPortraitLabelCollocation
                : label.LPortraitLabelMeaning;
            string title = LPortraitText.LPortraitTextRead(usage.LUsageTitle, label.LPortraitLabelUnknown);
            rows.Add(new LPortraitSection(
                string.Empty,
                [new LPortraitLine(owner, title)],
                string.Empty,
                [],
                [],
                LPortraitSectionLink:
                    [new LPortraitLink(usage.LUsageEntry, usage.LUsageHeadword, usage.LUsageLanguage)],
                LPortraitSectionRole: LPortraitRole.LPortraitRoleUsage));
        }

        sections.Add(LPortraitSection.LPortraitSectionCreate(label.LPortraitLabelIncoming, rows));
    }

    private static void LEngineNoteAdd(List<LPortraitSection> sections, LEntryDraft draft, LPortraitLabel label)
    {
        if (draft.LEntryDraftNote.Length > 0)
        {
            sections.Add(new LPortraitSection(label.LPortraitLabelNote, [], draft.LEntryDraftNote, [], []));
        }
    }
}
