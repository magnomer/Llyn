using System;
using System.Collections.Generic;

namespace Llyn.Core;

public static class LPortraitCard
{
    public static IReadOnlyList<LPortraitSection> LPortraitCardCreate(
        IReadOnlyList<LCardDraft> cards,
        string kind,
        LSentenceOrder order,
        string language,
        LPortraitLabel label,
        IReadOnlyDictionary<long, LPortraitLink> targets,
        IReadOnlyDictionary<long, string> sources)
    {
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(kind);
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(targets);
        ArgumentNullException.ThrowIfNull(sources);

        string mark = label.LPortraitLabelUnknown;
        List<LPortraitSection> shown = new List<LPortraitSection>();

        foreach (LCardDraft card in cards)
        {
            string title = LPortraitText.LPortraitTextRead(card.LCardDraftTitle, mark);
            string expression = LPortraitText.LPortraitTextRead(card.LCardDraftExpression, mark);
            string meaning = LPortraitText.LPortraitTextRead(card.LCardDraftMeaning, mark);

            List<LPortraitLine> lines = [];
            if (meaning.Length > 0)
            {
                lines.Add(new LPortraitLine(string.Empty, meaning));
            }

            List<string> situations = [];
            foreach (LSituationDraft situation in card.LCardDraftSituation)
            {
                LPortraitCardAdd(situations, situation.LSituationDraftTitle, mark);
            }

            List<string> registers = [];
            foreach (LRegisterDraft register in card.LCardDraftRegister)
            {
                LPortraitCardAdd(registers, register.LRegisterDraftName, mark);
            }

            List<LPortraitLink> links = [];
            foreach (long id in card.LCardDraftTranslation)
            {
                if (targets.TryGetValue(id, out LPortraitLink? link))
                {
                    links.Add(link);
                }
            }

            List<string> tags = [];
            foreach (LTagDraft tag in card.LCardDraftTag)
            {
                if (tag.LTagDraftText.Length > 0)
                {
                    tags.Add(tag.LTagDraftText);
                }
            }

            List<LPortraitSection> children = [];
            if (expression.Length > 0)
            {
                children.Add(new LPortraitSection(
                    string.Empty,
                    [new LPortraitLine(string.Empty, expression)],
                    string.Empty,
                    [],
                    [],
                    LPortraitSectionRole: LPortraitRole.LPortraitRolePhrase));
            }

            LPortraitCardAdd(
                children, label.LPortraitLabelSituation, LPortraitRole.LPortraitRoleScene, situations, []);
            LPortraitCardAdd(
                children, label.LPortraitLabelRegister, LPortraitRole.LPortraitRoleTone, registers, []);
            LPortraitCardAdd(
                children, label.LPortraitLabelTranslation, LPortraitRole.LPortraitRoleBridge, [], links);

            foreach (LSentenceDraft sentence in card.LCardDraftSentence)
            {
                if (!sentence.LSentenceDraftEmpty)
                {
                    children.Add(LPortraitSentence.LPortraitSentenceCreate(
                        sentence, order, language, label, targets, sources));
                }
            }

            LPortraitCardAdd(children, label.LPortraitLabelTag, LPortraitRole.LPortraitRoleLabel, tags, []);
            children.AddRange(LPortraitCardCreate(
                card.LCardDraftChild, kind, order, language, label, targets, sources));

            bool named = title.Length > 0;
            shown.Add(new LPortraitSection(
                named ? title : kind,
                lines,
                string.Empty,
                LPortraitMedia.LPortraitMediaCreate(card.LCardDraftImage, mark),
                LPortraitMedia.LPortraitMediaCreate(card.LCardDraftVideo, mark),
                card.LCardDraftPosition,
                LPortraitSectionChild: children,
                LPortraitSectionRole: named ? LPortraitRole.LPortraitRoleCard : LPortraitRole.LPortraitRoleKind));
        }

        return shown;
    }

    public static void LPortraitCardAdd(
        List<LPortraitSection> children,
        string heading,
        LPortraitRole role,
        IReadOnlyList<string> chips,
        IReadOnlyList<LPortraitLink> links)
    {
        ArgumentNullException.ThrowIfNull(children);
        ArgumentNullException.ThrowIfNull(chips);
        ArgumentNullException.ThrowIfNull(links);

        if (chips.Count > 0 || links.Count > 0)
        {
            children.Add(LPortraitSection.LPortraitSectionCreate(heading, role, chips, links));
        }
    }

    private static void LPortraitCardAdd(List<string> chips, LStateValue value, string mark)
    {
        string text = LPortraitText.LPortraitTextRead(value, mark);
        if (text.Length > 0)
        {
            chips.Add(text);
        }
    }
}
