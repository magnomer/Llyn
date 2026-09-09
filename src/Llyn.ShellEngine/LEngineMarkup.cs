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
            List<LEntryDraft> resolved = new List<LEntryDraft>();

            using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

            IReadOnlyDictionary<string, string> rows =
                LEngineCatalogSave(document.LMarkupDocumentCatalog, entries);

            for (int place = 0; place < entries.Count; place++)
            {
                try
                {
                    LEntryDraft draft = LEngineMarkupResolve(entries[place].LMarkupEntryDraft, rows);
                    resolved.Add(draft);
                    saved.Add(LEngineEntrySave(draft));
                }
                catch (Exception exception)
                {
                    throw new FormatException($"Entry {place + 1} could not be imported.", exception);
                }
            }

            LEngineMarkupAttach(entries, resolved, saved);

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

    private static LEntryDraft LEngineMarkupResolve(
        LEntryDraft draft, IReadOnlyDictionary<string, string> rows)
    {
        return draft with
        {
            LEntryDraftMeanings = LEngineCardResolve(draft.LEntryDraftMeanings, rows),
            LEntryDraftCollocations = LEngineCardResolve(draft.LEntryDraftCollocations, rows),
        };
    }

    private void LEngineMarkupAttach(
        IReadOnlyList<LMarkup.LMarkupEntry> entries,
        IReadOnlyList<LEntryDraft> resolved,
        IReadOnlyList<LEntry> saved)
    {
        Dictionary<string, string> named = new Dictionary<string, string>(StringComparer.Ordinal);
        for (int place = 0; place < entries.Count; place++)
        {
            string key = entries[place].LMarkupEntryKey;
            if (key.Length > 0)
            {
                named[key] = saved[place].LEntryId;
            }
        }

        List<LEntryDraft?> loaded = new List<LEntryDraft?>();
        for (int place = 0; place < saved.Count; place++)
        {
            loaded.Add(new LEntryLoader(_lEngineDatabase).LEntryLoad(saved[place].LEntryId));
        }

        for (int place = 0; place < loaded.Count; place++)
        {
            if (loaded[place] is not LEntryDraft stored)
            {
                continue;
            }

            LEngineKeyRead(resolved[place].LEntryDraftMeanings, stored.LEntryDraftMeanings, named);
            LEngineKeyRead(
                resolved[place].LEntryDraftCollocations, stored.LEntryDraftCollocations, named);
        }

        if (named.Count == 0)
        {
            return;
        }

        for (int place = 0; place < loaded.Count; place++)
        {
            if (loaded[place] is not LEntryDraft stored)
            {
                continue;
            }

            LEngineMarkupAttach(
                resolved[place].LEntryDraftMeanings, stored.LEntryDraftMeanings, named, false);
            LEngineMarkupAttach(
                resolved[place].LEntryDraftCollocations, stored.LEntryDraftCollocations, named, true);
        }
    }

    private static void LEngineKeyRead(
        IReadOnlyList<LCardDraft> written,
        IReadOnlyList<LCardDraft> stored,
        Dictionary<string, string> named)
    {
        List<LCardDraft> kept = new List<LCardDraft>();
        foreach (LCardDraft card in LEngineCardRead(written))
        {
            kept.Add(card);
        }

        for (int place = 0; place < kept.Count && place < stored.Count; place++)
        {
            if (kept[place].LCardDraftKey.Length > 0)
            {
                named[kept[place].LCardDraftKey] = stored[place].LCardDraftId;
            }

            LEngineKeyRead(kept[place].LCardDraftChild, stored[place].LCardDraftChild, named);
        }
    }

    private void LEngineMarkupAttach(
        IReadOnlyList<LCardDraft> written,
        IReadOnlyList<LCardDraft> stored,
        IReadOnlyDictionary<string, string> named,
        bool collocation)
    {
        List<LCardDraft> kept = new List<LCardDraft>();
        foreach (LCardDraft card in LEngineCardRead(written))
        {
            kept.Add(card);
        }

        for (int place = 0; place < kept.Count && place < stored.Count; place++)
        {
            List<string> ids = new List<string>();
            foreach (string key in kept[place].LCardDraftTranslation)
            {
                if (named.TryGetValue(key, out string? entryId) && !ids.Contains(entryId))
                {
                    ids.Add(entryId);
                }
            }

            if (ids.Count > 0)
            {
                LEngineTranslationSave(stored[place].LCardDraftId, ids, collocation);
            }

            if (collocation)
            {
                LEngineSynonymSave(stored[place].LCardDraftId, kept[place].LCardDraftInterlink, named);
            }
            else
            {
                LEngineRelationSave(stored[place].LCardDraftId, kept[place].LCardDraftRelation, named);
            }

            LEngineMarkupAttach(
                kept[place].LCardDraftChild, stored[place].LCardDraftChild, named, collocation);
        }
    }

    private void LEngineRelationSave(
        string meaningId,
        IReadOnlyList<LRelationDraft> drafts,
        IReadOnlyDictionary<string, string> named)
    {
        LRelationArchive relations = new(_lEngineDatabase);
        foreach (LRelationDraft draft in drafts)
        {
            if (!LEngineTargetRead(draft.LRelationDraftEntry, draft.LRelationDraftMeaning, named,
                    out string entryId, out string targetId))
            {
                continue;
            }

            relations.LRelationCreate(new LRelation(
                string.Empty,
                meaningId,
                0,
                draft.LRelationDraftType,
                draft.LRelationDraftLabel,
                draft.LRelationDraftLabels,
                entryId.Length > 0 ? entryId : null,
                targetId.Length > 0 ? targetId : null));
        }
    }

    private void LEngineSynonymSave(
        string collocationId,
        IReadOnlyList<LSynonymDraft> drafts,
        IReadOnlyDictionary<string, string> named)
    {
        LSynonymArchive synonyms = new(_lEngineDatabase);
        foreach (LSynonymDraft draft in drafts)
        {
            if (!LEngineTargetRead(draft.LSynonymDraftEntry, draft.LSynonymDraftMeaning, named,
                    out string entryId, out string targetId))
            {
                continue;
            }

            synonyms.LSynonymCreate(new LSynonym(
                string.Empty,
                collocationId,
                0,
                entryId.Length > 0 ? entryId : null,
                targetId.Length > 0 ? targetId : null));
        }
    }

    private static bool LEngineTargetRead(
        string entry,
        string meaning,
        IReadOnlyDictionary<string, string> named,
        out string entryId,
        out string meaningId)
    {
        entryId = string.Empty;
        meaningId = string.Empty;

        string key = entry.Length > 0 ? entry : meaning;
        if (key.Length == 0 || !named.TryGetValue(key, out string? row))
        {
            return false;
        }

        if (entry.Length > 0)
        {
            entryId = row;
            return true;
        }

        meaningId = row;
        return true;
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
                            LExampleDraftId = LEngineKeyResolve(example.LExampleDraftId, rows),
                            LExampleDraftReference =
                                LEngineReferenceResolve(example.LExampleDraftReference, rows),
                        },
                    });
            }

            List<LSituationDraft> situations = new List<LSituationDraft>();
            foreach (LSituationDraft situation in card.LCardDraftSituation)
            {
                situations.Add(situation with
                {
                    LSituationDraftId = LEngineKeyResolve(situation.LSituationDraftId, rows),
                });
            }

            List<LRegisterDraft> registers = new List<LRegisterDraft>();
            foreach (LRegisterDraft register in card.LCardDraftRegister)
            {
                registers.Add(register with
                {
                    LRegisterDraftId = LEngineKeyResolve(register.LRegisterDraftId, rows),
                });
            }

            List<LImageDraft> images = new List<LImageDraft>();
            foreach (LImageDraft image in card.LCardDraftImage)
            {
                images.Add(image with
                {
                    LImageDraftId = LEngineKeyResolve(image.LImageDraftId, rows),
                });
            }

            List<LVideoDraft> videos = new List<LVideoDraft>();
            foreach (LVideoDraft video in card.LCardDraftVideo)
            {
                videos.Add(video with
                {
                    LVideoDraftId = LEngineKeyResolve(video.LVideoDraftId, rows),
                });
            }

            resolved.Add(card with
            {
                LCardDraftSentence = sentences,
                LCardDraftSituation = situations,
                LCardDraftRegister = registers,
                LCardDraftImage = images,
                LCardDraftVideo = videos,
                LCardDraftChild = LEngineCardResolve(card.LCardDraftChild, rows),
            });
        }

        return resolved;
    }

    private static string LEngineKeyResolve(string key, IReadOnlyDictionary<string, string> rows)
    {
        if (key.Length == 0)
        {
            return key;
        }

        if (!rows.TryGetValue(key, out string? row))
        {
            throw new FormatException($"The citation '{key}' names no row the document declares.");
        }

        return row;
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
