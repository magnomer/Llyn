using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
        IReadOnlyDictionary<long, string> sources,
        IReadOnlyList<LCardDraft> meanings,
        IReadOnlyList<LCardDraft> collocations,
        IReadOnlySet<(LCardKind, long)> folds)
    {
        ArgumentNullException.ThrowIfNull(cards);
        ArgumentNullException.ThrowIfNull(kind);
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(targets);
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(meanings);
        ArgumentNullException.ThrowIfNull(collocations);
        ArgumentNullException.ThrowIfNull(folds);

        string mark = label.LPortraitLabelUnknown;
        List<LPortraitSection> shown = new List<LPortraitSection>();

        foreach (LCardDraft card in cards)
        {
            string title = LPortraitText.LPortraitTextRead(card.LCardDraftTitle, mark);
            string expression = LPortraitText.LPortraitTextRead(card.LCardDraftExpression, mark);

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

            LPortraitBodyAdd(children, card, order, language, label, targets, sources);
            children.AddRange(LPortraitCardCreate(
                card.LCardDraftChild, kind, order, language, label, targets, sources, meanings, collocations, folds));

            IReadOnlyList<LPortraitSection> rows = LPortraitRowsCreate(
                collocations
                    .Where(collocation => LCollocation.LCollocationHomeRead(meanings, collocation, true)
                        == card.LCardDraftId)
                    .ToList(),
                order,
                language,
                label,
                targets,
                sources,
                folds);
            if (rows.Count > 0)
            {
                children.Add(new LPortraitSection(
                    label.LPortraitLabelHeading,
                    [],
                    string.Empty,
                    [],
                    [],
                    LPortraitSectionChild: rows,
                    LPortraitSectionRole: LPortraitRole.LPortraitRoleList));
            }

            LPortraitStubAdd(children, card, meanings, collocations, label);

            bool named = title.Length > 0;
            shown.Add(new LPortraitSection(
                named ? title : kind,
                LPortraitMeaningRead(card, mark),
                string.Empty,
                LPortraitMedia.LPortraitMediaCreate(card.LCardDraftImage, mark),
                LPortraitMedia.LPortraitMediaCreate(card.LCardDraftVideo, mark),
                card.LCardDraftPosition,
                LPortraitSectionChild: children,
                LPortraitSectionRole: named ? LPortraitRole.LPortraitRoleCard : LPortraitRole.LPortraitRoleKind));
        }

        return shown;
    }

    public static IReadOnlyList<LPortraitSection> LPortraitRowsCreate(
        IReadOnlyList<LCardDraft> collocations,
        LSentenceOrder order,
        string language,
        LPortraitLabel label,
        IReadOnlyDictionary<long, LPortraitLink> targets,
        IReadOnlyDictionary<long, string> sources,
        IReadOnlySet<(LCardKind, long)> folds)
    {
        ArgumentNullException.ThrowIfNull(collocations);
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(label);
        ArgumentNullException.ThrowIfNull(targets);
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(folds);

        return collocations
            .Select(collocation => LPortraitRowCreate(
                collocation,
                order,
                language,
                label,
                targets,
                sources,
                folds.Contains((LCardKind.LCardKindCollocation, collocation.LCardDraftId))))
            .ToList();
    }

    private static LPortraitSection LPortraitRowCreate(
        LCardDraft collocation,
        LSentenceOrder order,
        string language,
        LPortraitLabel label,
        IReadOnlyDictionary<long, LPortraitLink> targets,
        IReadOnlyDictionary<long, string> sources,
        bool folded)
    {
        string mark = label.LPortraitLabelUnknown;
        List<LPortraitSection> children = [];
        LPortraitBodyAdd(children, collocation, order, language, label, targets, sources);

        return new LPortraitSection(
            LPortraitText.LPortraitTextRead(LCollocation.LCollocationPhraseRead(collocation), mark),
            LPortraitMeaningRead(collocation, mark),
            string.Empty,
            LPortraitMedia.LPortraitMediaCreate(collocation.LCardDraftImage, mark),
            LPortraitMedia.LPortraitMediaCreate(collocation.LCardDraftVideo, mark),
            LPortraitSectionChild: children,
            LPortraitSectionRole: LPortraitRole.LPortraitRoleRow,
            LPortraitSectionFold: folded);
    }

    private static List<LPortraitLine> LPortraitMeaningRead(LCardDraft card, string mark)
    {
        string meaning = LPortraitText.LPortraitTextRead(card.LCardDraftMeaning, mark);
        return meaning.Length > 0 ? [new LPortraitLine(string.Empty, meaning)] : [];
    }

    private static void LPortraitBodyAdd(
        List<LPortraitSection> children,
        LCardDraft card,
        LSentenceOrder order,
        string language,
        LPortraitLabel label,
        IReadOnlyDictionary<long, LPortraitLink> targets,
        IReadOnlyDictionary<long, string> sources)
    {
        string mark = label.LPortraitLabelUnknown;

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

    private static void LPortraitStubAdd(
        List<LPortraitSection> children,
        LCardDraft card,
        IReadOnlyList<LCardDraft> meanings,
        IReadOnlyList<LCardDraft> collocations,
        LPortraitLabel label)
    {
        foreach (LCardDraft collocation in collocations)
        {
            if (!LCollocation.LCollocationStubRead(meanings, collocation, true).Contains(card.LCardDraftId)
                || LCollocation.LCollocationHomeRead(meanings, collocation, true) is not long home
                || LMeaning.LMeaningPathRead(meanings, home) is not string number)
            {
                continue;
            }

            string shown = LPortraitText.LPortraitTextRead(
                LCollocation.LCollocationPhraseRead(collocation), label.LPortraitLabelUnknown);
            children.Add(new LPortraitSection(
                string.Empty,
                [new LPortraitLine(
                    string.Empty,
                    string.Format(CultureInfo.CurrentCulture, label.LPortraitLabelStub, shown, number))],
                string.Empty,
                [],
                []));
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
