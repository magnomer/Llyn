using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private LPress? _lEnginePress;

    public void LEnginePressApply(LPress press)
    {
        ArgumentNullException.ThrowIfNull(press);

        _lEnginePress = press;
    }

    public IReadOnlyList<LMarkupLoss> LEngineMarkupExport(
        IReadOnlyList<string> entryIds, string path)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(entryIds);
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            List<LMarkupLoss> lost = [];
            File.WriteAllText(path, LEngineMarkupFormat(entryIds, lost), new UTF8Encoding(false));
            return LEngineLossRead(lost);
        }
    }

    public async Task LEnginePortraitExport(
        string entryId, string path, LPortraitFormat format, LPortraitLabel label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(label);

        if (format == LPortraitFormat.LPortraitFormatMarkup)
        {
            File.WriteAllText(path, LEngineMarkupFormat([entryId], []), new UTF8Encoding(false));
            return;
        }

        LPortrait portrait = LEnginePortraitRead(entryId, label);
        LTheme theme = LTheme.LThemeLoad();

        switch (format)
        {
            case LPortraitFormat.LPortraitFormatHtml:
                File.WriteAllText(
                    path, LSheet.LSheetFormat(portrait, theme), new UTF8Encoding(false));
                return;
            case LPortraitFormat.LPortraitFormatMarkdown:
                File.WriteAllText(
                    path, LOutline.LOutlineFormat(portrait), new UTF8Encoding(false));
                return;
            case LPortraitFormat.LPortraitFormatDocx:
                LFolio.LFolioSave(portrait, theme, path);
                return;
            case LPortraitFormat.LPortraitFormatPdf:
                if (_lEnginePress is not LPress press)
                {
                    throw new InvalidOperationException(
                        "No printing surface stands ready for this workspace.");
                }

                await press.LPressSave(LSheet.LSheetFormat(portrait, theme), path);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(format));
        }
    }

    private string LEngineMarkupFormat(IReadOnlyList<string> entryIds, List<LMarkupLoss> lost)
    {
        List<LMarkup.LMarkupEntry> entries = [];
        Dictionary<string, string> keys = new(StringComparer.Ordinal);
        HashSet<string> taken = new(StringComparer.Ordinal);
        List<LEntryDraft> drafts = [];

        foreach (string entryId in entryIds)
        {
            LEntryDraft draft = new LEntryLoader(_lEngineDatabase).LEntryLoad(entryId)
                ?? throw new InvalidOperationException("The entry no longer stands in the workspace.");

            drafts.Add(draft);
            keys[entryId] = LMarkup.LMarkupKeyCreate(
                draft.LEntryDraftHeadword, LMarkup.LMarkupRowKind.LMarkupRowEntry, taken);
        }

        foreach (LEntryDraft draft in drafts)
        {
            LEngineKeyCreate(draft.LEntryDraftMeanings, keys, taken);
        }

        for (int place = 0; place < drafts.Count; place++)
        {
            entries.Add(new LMarkup.LMarkupEntry(drafts[place], keys[entryIds[place]]));
        }

        LMarkup.LMarkupDocument document =
            new(LEngineCatalogCreate(drafts, keys, taken), entries);

        lost.AddRange(LMarkupLoss.LMarkupLossRead(document, keys));
        return LMarkupDraft.LMarkupDraftFormat(document, keys);
    }

    private static void LEngineKeyCreate(
        IReadOnlyList<LCardDraft> cards, Dictionary<string, string> keys, HashSet<string> taken)
    {
        foreach (LCardDraft card in cards)
        {
            if (card.LCardDraftId.Length > 0)
            {
                keys[card.LCardDraftId] = LMarkup.LMarkupKeyCreate(
                    card.LCardDraftGloss ?? card.LCardDraftTitle.LStateValueShow(),
                    LMarkup.LMarkupRowKind.LMarkupRowSense,
                    taken);
            }

            LEngineKeyCreate(card.LCardDraftChild, keys, taken);
        }
    }

    private IReadOnlyList<LMarkupLoss> LEngineLossRead(IReadOnlyList<LMarkupLoss> lost)
    {
        if (lost.Count == 0)
        {
            return lost;
        }

        LEntryArchive entries = new(_lEngineDatabase);
        List<LMarkupLoss> named = new(lost.Count);
        foreach (LMarkupLoss loss in lost)
        {
            LEntry? target = entries.LEntryRead(loss.LMarkupLossTarget);
            named.Add(target is null
                ? loss
                : loss with { LMarkupLossTarget = target.LEntryHeadword });
        }

        return named;
    }

    private LMarkup.LMarkupCatalog LEngineCatalogCreate(
        IReadOnlyList<LEntryDraft> drafts,
        Dictionary<string, string> keys,
        HashSet<string> taken)
    {
        LEngineCatalog harvest = new();
        foreach (LEntryDraft draft in drafts)
        {
            LEngineCatalogRead(harvest, draft.LEntryDraftMeanings);
            LEngineCatalogRead(harvest, draft.LEntryDraftCollocations);
        }

        Dictionary<string, LAuthor> authors = new(StringComparer.Ordinal);
        foreach (LAuthor author in LEngineSort(LEngineAuthorRead(), row => row.LAuthorName, row => row.LAuthorId))
        {
            keys[author.LAuthorId] = LMarkup.LMarkupKeyCreate(
                author.LAuthorName, LMarkup.LMarkupRowKind.LMarkupRowAuthor, taken);
            authors[keys[author.LAuthorId]] = author;
        }

        Dictionary<string, LMarkup.LMarkupReference> sources = new(StringComparer.Ordinal);
        IReadOnlyList<LReference> held = LEngineSort(
            LEngineReferenceRead(), row => row.LReferenceNameRead(), row => row.LReferenceId);
        foreach (LReference reference in held)
        {
            keys[reference.LReferenceId] = LMarkup.LMarkupKeyCreate(
                reference.LReferenceNameRead(), LMarkup.LMarkupRowKind.LMarkupRowSource, taken);
        }

        foreach (LReference reference in held)
        {
            List<string> credited = [];
            foreach (LAuthor author in LEngineAuthorRead(reference.LReferenceId, LOwner.LOwnerReference))
            {
                if (keys.TryGetValue(author.LAuthorId, out string? named))
                {
                    credited.Add(named);
                }
            }

            sources[keys[reference.LReferenceId]] = new LMarkup.LMarkupReference(
                keys[reference.LReferenceId], reference, credited);
        }

        Dictionary<string, LExample> examples = new(StringComparer.Ordinal);
        foreach (LExample example in LEngineSort(
            harvest.LEngineCatalogExample, row => row.LExampleText.LStateValueShow(), row => row.LExampleId))
        {
            string named = LMarkup.LMarkupKeyCreate(
                example.LExampleText.LStateValueShow(),
                LMarkup.LMarkupRowKind.LMarkupRowExample,
                taken);
            keys[example.LExampleId] = named;
            examples[named] = example with
            {
                LExampleSource = LEngineCitationRead(example.LExampleSource, keys),
            };
        }

        Dictionary<string, LSituation> situations = new(StringComparer.Ordinal);
        foreach (LSituation situation in LEngineSort(
            harvest.LEngineCatalogSituation,
            row => row.LSituationTitle.LStateValueShow(),
            row => row.LSituationId))
        {
            string named = LMarkup.LMarkupKeyCreate(
                situation.LSituationTitle.LStateValueShow(),
                LMarkup.LMarkupRowKind.LMarkupRowSituation,
                taken);
            keys[situation.LSituationId] = named;
            situations[named] = situation;
        }

        Dictionary<string, LRegister> registers = new(StringComparer.Ordinal);
        foreach (LRegister register in LEngineSort(
            harvest.LEngineCatalogRegister,
            row => row.LRegisterName.LStateValueShow(),
            row => row.LRegisterId))
        {
            string named = LMarkup.LMarkupKeyCreate(
                register.LRegisterName.LStateValueShow(),
                LMarkup.LMarkupRowKind.LMarkupRowRegister,
                taken);
            keys[register.LRegisterId] = named;
            registers[named] = register;
        }

        Dictionary<string, LImage> images = new(StringComparer.Ordinal);
        foreach (LImage image in LEngineSort(
            harvest.LEngineCatalogImage,
            row => row.LImageLocation.LStateValueShow(),
            row => row.LImageId))
        {
            string named = LMarkup.LMarkupKeyCreate(
                image.LImageLocation.LStateValueShow(),
                LMarkup.LMarkupRowKind.LMarkupRowImage,
                taken);
            keys[image.LImageId] = named;
            images[named] = image;
        }

        Dictionary<string, LVideo> videos = new(StringComparer.Ordinal);
        foreach (LVideo video in LEngineSort(
            harvest.LEngineCatalogVideo,
            row => row.LVideoLocation.LStateValueShow(),
            row => row.LVideoId))
        {
            string named = LMarkup.LMarkupKeyCreate(
                video.LVideoLocation.LStateValueShow(),
                LMarkup.LMarkupRowKind.LMarkupRowVideo,
                taken);
            keys[video.LVideoId] = named;
            videos[named] = video;
        }

        return new LMarkup.LMarkupCatalog(
            new Dictionary<string, LMarkup.LMarkupRowKind>(StringComparer.Ordinal),
            authors,
            sources,
            examples,
            situations,
            registers,
            images,
            videos);
    }

    private static LStateValue LEngineCitationRead(
        LStateValue citation, IReadOnlyDictionary<string, string> keys)
    {
        if (citation.LStateValueState != LState.LStateSpecified)
        {
            return citation;
        }

        return keys.TryGetValue(citation.LStateValueShow(), out string? named)
            ? LStateValue.LStateValueCreate(named)
            : LStateValue.LStateValueUnknown;
    }

    private static void LEngineCatalogRead(LEngineCatalog harvest, IReadOnlyList<LCardDraft> cards)
    {
        foreach (LCardDraft card in cards)
        {
            foreach (LSentenceDraft sentence in card.LCardDraftSentence)
            {
                if (sentence.LSentenceDraftExample is LExampleDraft quoted
                    && quoted.LExampleDraftId.Length > 0)
                {
                    harvest.LEngineCatalogExample.TryAdd(
                        quoted.LExampleDraftId,
                        new LExample(
                            quoted.LExampleDraftId,
                            quoted.LExampleDraftLanguage,
                            quoted.LExampleDraftText,
                            quoted.LExampleDraftTranslation,
                            quoted.LExampleDraftReference));
                }
            }

            foreach (LSituationDraft situation in card.LCardDraftSituation)
            {
                if (situation.LSituationDraftId.Length > 0)
                {
                    harvest.LEngineCatalogSituation.TryAdd(
                        situation.LSituationDraftId,
                        new LSituation(
                            situation.LSituationDraftId,
                            situation.LSituationDraftTitle,
                            situation.LSituationDraftDescription,
                            situation.LSituationDraftKind));
                }
            }

            foreach (LRegisterDraft register in card.LCardDraftRegister)
            {
                if (register.LRegisterDraftId.Length > 0)
                {
                    harvest.LEngineCatalogRegister.TryAdd(
                        register.LRegisterDraftId,
                        new LRegister(
                            register.LRegisterDraftId,
                            register.LRegisterDraftName,
                            register.LRegisterDraftLanguage,
                            register.LRegisterDraftBuiltin));
                }
            }

            foreach (LImageDraft image in card.LCardDraftImage)
            {
                if (image.LImageDraftId.Length > 0)
                {
                    harvest.LEngineCatalogImage.TryAdd(
                        image.LImageDraftId,
                        new LImage(image.LImageDraftId, image.LImageDraftLocation));
                }
            }

            foreach (LVideoDraft video in card.LCardDraftVideo)
            {
                if (video.LVideoDraftId.Length > 0)
                {
                    harvest.LEngineCatalogVideo.TryAdd(
                        video.LVideoDraftId,
                        new LVideo(
                            video.LVideoDraftId, video.LVideoDraftLocation, video.LVideoDraftSpan));
                }
            }

            LEngineCatalogRead(harvest, card.LCardDraftChild);
        }
    }

    private static IReadOnlyList<TRow> LEngineSort<TRow>(
        IEnumerable<TRow> rows, Func<TRow, string> seed, Func<TRow, string> identify)
    {
        List<TRow> sorted = [];
        foreach (TRow row in rows)
        {
            sorted.Add(row);
        }

        sorted.Sort((one, other) =>
        {
            int order = string.CompareOrdinal(seed(one), seed(other));
            return order != 0 ? order : string.CompareOrdinal(identify(one), identify(other));
        });

        return sorted;
    }

    private static IReadOnlyList<TRow> LEngineSort<TRow>(
        Dictionary<string, TRow> rows, Func<TRow, string> seed, Func<TRow, string> identify)
    {
        return LEngineSort((IEnumerable<TRow>)rows.Values, seed, identify);
    }

    private sealed class LEngineCatalog
    {
        public Dictionary<string, LExample> LEngineCatalogExample { get; } =
            new(StringComparer.Ordinal);

        public Dictionary<string, LSituation> LEngineCatalogSituation { get; } =
            new(StringComparer.Ordinal);

        public Dictionary<string, LRegister> LEngineCatalogRegister { get; } =
            new(StringComparer.Ordinal);

        public Dictionary<string, LImage> LEngineCatalogImage { get; } =
            new(StringComparer.Ordinal);

        public Dictionary<string, LVideo> LEngineCatalogVideo { get; } =
            new(StringComparer.Ordinal);
    }
}
