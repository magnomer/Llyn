using System;
using System.Collections.Generic;

namespace Llyn.Core;

public static class LPortraitSentence
{
    public static LPortraitSection LPortraitSentenceCreate(
        LSentenceDraft sentence,
        LSentenceOrder order,
        string language,
        LPortraitLabel label,
        IReadOnlyDictionary<long, LPortraitLink> targets,
        IReadOnlyDictionary<long, string> sources)
    {
        ArgumentNullException.ThrowIfNull(sentence);
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(targets);
        ArgumentNullException.ThrowIfNull(sources);

        string mark = label.LPortraitLabelUnknown;
        LExampleDraft? example = sentence.LSentenceDraftExample;

        List<LPortraitLine> lines =
        [
            new LPortraitLine(
                LPortraitFrame.LPortraitFrameRead(sentence, order, mark),
                LPortraitText.LPortraitTextRead(example?.LExampleDraftText, mark)),
        ];

        if (example is null)
        {
            return new LPortraitSection(
                label.LPortraitLabelExample,
                lines,
                string.Empty,
                [],
                [],
                LPortraitSectionRole: LPortraitRole.LPortraitRoleQuote);
        }

        List<string> chips = [];
        if (example.LExampleDraftLanguage.Length > 0
            && !string.Equals(example.LExampleDraftLanguage, language, StringComparison.Ordinal))
        {
            chips.Add(example.LExampleDraftLanguage);
        }

        foreach (LGlossDraft gloss in example.LExampleDraftGloss)
        {
            string text = LPortraitText.LPortraitTextRead(gloss.LGlossDraftText, mark);
            if (text.Length > 0)
            {
                string tag = gloss.LGlossDraftLanguage.Length > 0
                    ? gloss.LGlossDraftLanguage
                    : label.LPortraitLabelGloss;
                lines.Add(new LPortraitLine(tag, text));
            }
        }

        if (example.LExampleDraftReference.LStateAnchorState == LState.LStateSpecified
            && sources.TryGetValue(example.LExampleDraftReference.LStateAnchorShow(), out string? source))
        {
            lines.Add(new LPortraitLine(label.LPortraitLabelSource, source));
        }
        else if (example.LExampleDraftReference.LStateAnchorState == LState.LStateUnknown)
        {
            lines.Add(new LPortraitLine(label.LPortraitLabelSource, mark));
        }

        List<LPortraitLink> mentions = [];
        foreach (LMentionDraft mention in example.LExampleDraftMention)
        {
            if (targets.TryGetValue(mention.LMentionDraftEntry, out LPortraitLink? link)
                && !mentions.Contains(link))
            {
                mentions.Add(link);
            }
        }

        List<LPortraitSection> children = [];
        LPortraitCard.LPortraitCardAdd(
            children, label.LPortraitLabelMention, LPortraitRole.LPortraitRoleBridge, [], mentions);

        return new LPortraitSection(
            label.LPortraitLabelExample,
            lines,
            string.Empty,
            [],
            [],
            LPortraitSectionChip: chips,
            LPortraitSectionChild: children,
            LPortraitSectionRole: LPortraitRole.LPortraitRoleQuote);
    }
}
