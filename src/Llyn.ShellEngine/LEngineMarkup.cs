using System;
using System.Collections.Generic;
using System.IO;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LEntry> LEngineMarkupImport(string path)
    {
        IReadOnlyList<LEntry> imported;
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("No markup file stands at the chosen path.", path);
            }

            LMarkup.LMarkupDocument document = LMarkup.LMarkupEntryRead(File.ReadAllText(path));
            IReadOnlyList<LMarkup.LMarkupEntry> entries = document.LMarkupDocumentEntry;
            List<LEntry> saved = new List<LEntry>();

            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

            IReadOnlyDictionary<string, string> rows =
                LEngineCatalogSave(document.LMarkupDocumentCatalog, entries);

            for (int place = 0; place < entries.Count; place++)
            {
                try
                {
                    saved.Add(LEngineMarkupSave(entries[place], rows));
                }
                catch (Exception exception)
                {
                    throw new FormatException($"Entry {place + 1} could not be imported.", exception);
                }
            }

            session.LDatabaseSessionCommit();
            imported = saved;
        }

        LEngineBulletinRaise(LSubject.LSubjectEntry, string.Empty);
        return imported;
    }

    private IReadOnlyDictionary<string, string> LEngineCatalogSave(
        LMarkup.LMarkupCatalog catalog, IReadOnlyList<LMarkup.LMarkupEntry> entries)
    {
        Dictionary<string, string> rows = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (KeyValuePair<string, LAuthor> author in catalog.LMarkupCatalogAuthor)
        {
            rows[author.Key] = LEngineAuthorCreate(
                new LAuthor(string.Empty, author.Value.LAuthorName)).LAuthorId;
        }

        foreach (KeyValuePair<string, LMarkup.LMarkupReference> source in catalog.LMarkupCatalogSource)
        {
            rows[source.Key] = LEngineReferenceSave(source.Value, rows);
        }

        LExampleArchive examples = new(_lEngineDatabase);
        foreach (KeyValuePair<string, LExample> example in catalog.LMarkupCatalogExample)
        {
            rows[example.Key] = examples.LExampleCreate(new LExample(
                string.Empty,
                LEngineLanguageRead(example.Key, example.Value, entries),
                example.Value.LExampleText,
                example.Value.LExampleTranslation,
                LEngineReferenceResolve(example.Value.LExampleSource, rows))).LExampleId;
        }

        foreach (KeyValuePair<string, LSituation> situation in catalog.LMarkupCatalogSituation)
        {
            rows[situation.Key] = LEngineSituationCreate(situation.Value with
            {
                LSituationId = string.Empty,
            }).LSituationId;
        }

        LRegisterArchive registers = new(_lEngineDatabase);
        foreach (KeyValuePair<string, LRegister> register in catalog.LMarkupCatalogRegister)
        {
            rows[register.Key] = LEngineRegisterSave(registers, register.Value);
        }

        LImageArchive images = new(_lEngineDatabase);
        foreach (KeyValuePair<string, LImage> image in catalog.LMarkupCatalogImage)
        {
            rows[image.Key] = images.LImageCreate(
                new LImage(string.Empty, image.Value.LImageLocation)).LImageId;
        }

        LVideoArchive videos = new(_lEngineDatabase);
        foreach (KeyValuePair<string, LVideo> video in catalog.LMarkupCatalogVideo)
        {
            rows[video.Key] = videos.LVideoCreate(new LVideo(
                string.Empty,
                video.Value.LVideoLocation,
                video.Value.LVideoSpan)).LVideoId;
        }

        return rows;
    }

    private static string LEngineLanguageRead(
        string key, LExample example, IReadOnlyList<LMarkup.LMarkupEntry> entries)
    {
        if (!string.IsNullOrWhiteSpace(example.LExampleLanguage))
        {
            return example.LExampleLanguage;
        }

        foreach (LMarkup.LMarkupEntry entry in entries)
        {
            if (!string.IsNullOrWhiteSpace(entry.LMarkupEntryDraft.LEntryDraftLanguage))
            {
                return entry.LMarkupEntryDraft.LEntryDraftLanguage;
            }
        }

        throw new FormatException(
            $"The example '{key}' names no language and the document declares none.");
    }

    private string LEngineRegisterSave(LRegisterArchive registers, LRegister written)
    {
        LEngineRegisterCreate(written.LRegisterLanguage);

        if (written.LRegisterBuiltin)
        {
            foreach (LRegister stored in registers.LRegisterRead())
            {
                if (string.Equals(
                        stored.LRegisterName.LStateValueShow().Trim(),
                        written.LRegisterName.LStateValueShow().Trim(),
                        StringComparison.CurrentCultureIgnoreCase)
                    && string.Equals(
                        stored.LRegisterLanguage,
                        written.LRegisterLanguage,
                        StringComparison.Ordinal))
                {
                    return stored.LRegisterId;
                }
            }
        }

        return registers.LRegisterCreate(new LRegister(
            string.Empty,
            written.LRegisterName,
            written.LRegisterLanguage,
            false)).LRegisterId;
    }

    private LEntry LEngineMarkupSave(
        LMarkup.LMarkupEntry entry, IReadOnlyDictionary<string, string> rows)
    {
        LEntryDraft draft = entry.LMarkupEntryDraft;

        return LEngineEntrySave(draft with
        {
            LEntryDraftMeanings = LEngineCardResolve(draft.LEntryDraftMeanings, rows),
            LEntryDraftCollocations = LEngineCardResolve(draft.LEntryDraftCollocations, rows),
        });
    }

    private string LEngineReferenceSave(
        LMarkup.LMarkupReference source, IReadOnlyDictionary<string, string> rows)
    {
        LReference stored = LEngineReferenceCreate(source.LMarkupReferenceValue with
        {
            LReferenceId = string.Empty,
        });

        int position = 0;
        foreach (string key in source.LMarkupReferenceAuthor)
        {
            LEngineAuthorAttach(stored.LReferenceId, rows[key], position);
            position++;
        }

        return stored.LReferenceId;
    }

    private static IReadOnlyList<LCardDraft> LEngineCardResolve(
        IReadOnlyList<LCardDraft> cards, IReadOnlyDictionary<string, string> rows)
    {
        List<LCardDraft> resolved = new List<LCardDraft>();

        foreach (LCardDraft card in cards)
        {
            List<LSentenceDraft> sentences = new List<LSentenceDraft>();
            foreach (LSentenceDraft sentence in card.LCardDraftSentence)
            {
                sentences.Add(sentence.LSentenceDraftExample is not LExampleDraft example
                    ? sentence
                    : sentence with
                    {
                        LSentenceDraftExample = example with
                        {
                            LExampleDraftReference =
                                LEngineReferenceResolve(example.LExampleDraftReference, rows),
                        },
                    });
            }

            resolved.Add(card with
            {
                LCardDraftSentence = sentences,
            });
        }

        return resolved;
    }

    private static LStateValue LEngineReferenceResolve(
        LStateValue citation, IReadOnlyDictionary<string, string> rows)
    {
        if (citation.LStateValueState != LState.LStateSpecified)
        {
            return citation;
        }

        string key = citation.LStateValueText ?? string.Empty;
        if (!rows.TryGetValue(key, out string? row))
        {
            throw new FormatException($"The citation '{key}' names no row the document declares.");
        }

        return LStateValue.LStateValueCreate(row);
    }
}
