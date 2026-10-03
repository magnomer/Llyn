# TCatalogExample.cs
Hash: `38eaacf1b647d6cb`

## `public sealed class TCatalogExample`

Covers the Example catalog, both what an Example is ordered by and what it answers.
It covers sentence order, language order, and order by the name of the Source cited.
It covers usage order, which counts the places quoting the Example.
The worded count follows a copy that carries another usage.
It covers the query, over the sentence, the translation and the cited name.
The cited name is matched rather than the id, because the id is never shown.
It covers the names read for the whole shelf of Sources, which one catalog fill asks for once.
It covers the lines a shown entry's citations read, child cards included, with a gone Source written as its id.

## `private static LCardDraft TCatalogCardCreate(params LSentenceDraft[] sentences)`

Builds a card that holds only the given sentences.

## `private static LSentenceDraft TCatalogSentenceCreate(long? source)`

Builds a sentence whose Example cites `source`, or nothing when it is null.
