using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LPortraitPage LEnginePortraitRead(long id, LOwner owner, LPortraitLegend legend)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentNullException.ThrowIfNull(legend);

        lock (_lEngineGate)
        {
            switch (owner)
            {
                case LOwner.LOwnerExample:
                    LExampleArchive examples = new(_lEngineDatabase);
                    LExample example = examples.LExampleRead(id) ?? throw LEnginePageRaise();
                    LReference? cited = example.LExampleSource.LStateAnchorState == LState.LStateSpecified
                        ? new LReferenceArchive(_lEngineDatabase)
                            .LReferenceRead(example.LExampleSource.LStateAnchorShow())
                        : null;
                    return LEnginePageRead(example, cited, examples.LExampleReferenceRead(id), legend);
                case LOwner.LOwnerReference:
                    LReference reference = new LReferenceArchive(_lEngineDatabase).LReferenceRead(id)
                        ?? throw LEnginePageRaise();
                    IReadOnlyList<LAuthor> credits = new LAuthorArchive(_lEngineDatabase).LAuthorReferenceRead(id);
                    int cites = new LReferenceUsage(_lEngineDatabase).LReferenceUsageRead(id).Count;
                    return LEnginePageRead(reference, credits, cites, legend);
                case LOwner.LOwnerSituation:
                    LSituationArchive situations = new(_lEngineDatabase);
                    LSituation situation = situations.LSituationRead(id) ?? throw LEnginePageRaise();
                    return LEnginePageRead(situation, situations.LSituationReferenceRead(id), legend);
                default:
                    throw LEngineOwnerRaise(owner);
            }
        }
    }

    private static InvalidOperationException LEnginePageRaise()
    {
        return new InvalidOperationException("The page no longer stands in the workspace.");
    }

    private static LPortraitPage LEnginePageRead(
        LExample example, LReference? cited, int count, LPortraitLegend legend)
    {
        string mark = legend.LPortraitLegendUnknown;
        List<LPortraitSection> sections = [];

        List<LPortraitLine> glosses = [];
        foreach (LGloss gloss in example.LExampleGloss)
        {
            glosses.Add(new LPortraitLine(
                gloss.LGlossLanguage, LPortraitText.LPortraitTextRead(gloss.LGlossText, mark)));
        }

        if (glosses.Count > 0)
        {
            sections.Add(LPortraitSection.LPortraitSectionCreate(legend.LPortraitLegendTranslation, glosses));
        }

        string source = cited?.LReferenceNameRead() ?? string.Empty;

        if (source.Length > 0)
        {
            sections.Add(LPortraitSection.LPortraitSectionCreate(legend.LPortraitLegendSource, source));
        }

        return new LPortraitPage(
            LEngineTitleRead(example.LExampleText, legend.LPortraitLegendUnwritten, mark),
            example.LExampleLanguage,
            [legend.LPortraitTallyFormat(count)],
            sections);
    }

    private static LPortraitPage LEnginePageRead(
        LReference reference, IReadOnlyList<LAuthor> credits, int count, LPortraitLegend legend)
    {
        string mark = legend.LPortraitLegendUnknown;
        List<string> chips = [];

        if (reference.LReferenceKind != LReferenceKind.LReferenceKindUnspecified)
        {
            chips.Add(legend.LPortraitKindFormat(reference.LReferenceKind));
        }

        chips.Add(legend.LPortraitTallyFormat(count));

        List<LPortraitSection> sections = [];

        if (credits.Count > 0)
        {
            string[] names = new string[credits.Count];
            for (int index = 0; index < credits.Count; index++)
            {
                names[index] = credits[index].LAuthorName;
            }

            sections.Add(LPortraitSection.LPortraitSectionCreate(
                legend.LPortraitLegendAuthor, string.Join(", ", names)));
        }
        else if (reference.LReferenceAuthorState.LStateMarkState == LState.LStateUnknown)
        {
            sections.Add(LPortraitSection.LPortraitSectionCreate(legend.LPortraitLegendAuthor, mark));
        }

        LEngineSectionAdd(sections, legend.LPortraitLegendYear, reference.LReferenceYear, mark);
        LEngineSectionAdd(sections, legend.LPortraitLegendUrl, reference.LReferenceUrl, mark);
        LEngineSectionAdd(sections, legend.LPortraitLegendNote, reference.LReferenceNote, mark);

        return new LPortraitPage(
            LEngineTitleRead(reference.LReferenceTitle, legend.LPortraitLegendUntitled, mark),
            string.Empty,
            chips,
            sections);
    }

    private static LPortraitPage LEnginePageRead(LSituation situation, int count, LPortraitLegend legend)
    {
        string mark = legend.LPortraitLegendUnknown;
        List<string> chips = [];

        string kind = LPortraitText.LPortraitTextRead(situation.LSituationKind, mark);
        if (kind.Length > 0)
        {
            chips.Add(kind);
        }

        chips.Add(legend.LPortraitTallyFormat(count));

        List<LPortraitSection> sections = [];

        string description = LPortraitText.LPortraitTextRead(situation.LSituationDescription, mark);
        if (description.Length > 0)
        {
            sections.Add(new LPortraitSection(legend.LPortraitLegendDescription, [], description, [], []));
        }

        IReadOnlyList<LPortraitMedia> images = LPortraitMedia.LPortraitMediaCreate(situation.LSituationImage, mark);
        IReadOnlyList<LPortraitMedia> videos = LPortraitMedia.LPortraitMediaCreate(situation.LSituationVideo, mark);
        if (images.Count > 0 || videos.Count > 0)
        {
            sections.Add(new LPortraitSection(string.Empty, [], string.Empty, images, videos));
        }

        return new LPortraitPage(
            LEngineTitleRead(situation.LSituationTitle, legend.LPortraitLegendUntitled, mark),
            string.Empty,
            chips,
            sections);
    }

    private static string LEngineTitleRead(LStateValue value, string vacant, string mark)
    {
        string text = LPortraitText.LPortraitTextRead(value, mark);
        return text.Length > 0 ? text : vacant;
    }

    private static void LEngineSectionAdd(
        List<LPortraitSection> sections, string heading, LStateValue value, string mark)
    {
        string text = LPortraitText.LPortraitTextRead(value, mark);
        if (text.Length > 0)
        {
            sections.Add(LPortraitSection.LPortraitSectionCreate(heading, text));
        }
    }
}
