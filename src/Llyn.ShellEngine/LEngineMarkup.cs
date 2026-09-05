using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LEntry> LEngineMarkupImport(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        IReadOnlyList<LMarkup.LMarkupEntry> entries = LMarkup.LMarkupEntryRead(text);
        List<LEntry> saved = new List<LEntry>();
        Dictionary<string, string> writers = new Dictionary<string, string>(StringComparer.Ordinal);

        using LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart();

        for (int place = 0; place < entries.Count; place++)
        {
            try
            {
                saved.Add(LEngineMarkupSave(entries[place], writers));
            }
            catch (Exception exception)
            {
                throw new FormatException($"Entry {place + 1} could not be imported.", exception);
            }
        }

        session.LDatabaseSessionCommit();
        return saved;
    }

    private LEntry LEngineMarkupSave(
        LMarkup.LMarkupEntry entry, IDictionary<string, string> writers)
    {
        Dictionary<string, string> rows = new Dictionary<string, string>(StringComparer.Ordinal);
        List<string> order = new List<string>();

        foreach (LMarkup.LMarkupReference source in entry.LMarkupEntrySource)
        {
            string row = LEngineReferenceSave(source, writers);
            rows[source.LMarkupReferenceId] = row;
            order.Add(row);
        }

        LEntryDraft draft = entry.LMarkupEntryDraft;
        LEntry stored = LEngineEntrySave(draft with
        {
            LEntryDraftMeanings = LEngineCardResolve(draft.LEntryDraftMeanings, rows),
            LEntryDraftCollocations = LEngineCardResolve(draft.LEntryDraftCollocations, rows),
        });

        for (int position = 0; position < order.Count; position++)
        {
            LEngineReferenceAttach(stored.LEntryId, order[position], position, LOwner.LOwnerEntry);
        }

        return stored;
    }

    private string LEngineReferenceSave(
        LMarkup.LMarkupReference source, IDictionary<string, string> writers)
    {
        LReference stored = LEngineReferenceCreate(source.LMarkupReferenceValue);

        int position = 0;
        foreach (string name in source.LMarkupReferenceAuthor)
        {
            if (!writers.TryGetValue(name, out string? writer))
            {
                writer = LEngineAuthorCreate(new LAuthor(string.Empty, name)).LAuthorId;
                writers[name] = writer;
            }

            LEngineAuthorAttach(stored.LReferenceId, writer, position);
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
            List<LExampleDraft> examples = new List<LExampleDraft>();
            foreach (LExampleDraft example in card.LCardDraftExample)
            {
                examples.Add(example with
                {
                    LExampleDraftReference =
                        LEngineReferenceResolve(example.LExampleDraftReference, rows),
                });
            }

            resolved.Add(card with
            {
                LCardDraftExample = examples,
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
            throw new FormatException($"Citation names no source declared in its entry: '{key}'.");
        }

        return LStateValue.LStateValueCreate(row);
    }
}
