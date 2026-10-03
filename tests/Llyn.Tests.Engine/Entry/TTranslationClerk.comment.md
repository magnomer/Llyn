# TTranslationClerk.cs
Hash: `a03bf991367dc4f1`

## `public sealed class TTranslationClerk`

Covers the translation clerk's search and resolve over the entry fake, with no engine and no SQLite.
The fake answers every entry to any query, so the ordering and the exclusions are the clerk's own.

## `public void TranslationClerkFind_ExactHeadword_ListsItBeforePartialMatches()`

The entry whose whole headword reads as the query leads, and the containing one follows.

## `public void TranslationClerkFind_PaddedQuery_RanksAsTheTrimmedQuery()`

The clerk ranks a padded query as its trimmed text, so padding never changes the order.

## `public void TranslationClerkFind_OwnEntry_IsLeftOut()`

A card may not translate its own entry, so the search drops the id it is given.

## `public void TranslationWordRead_PaddedOrBlankText_AnswersTheTrimmedWordOrNothing()`

The word read answers the typed text trimmed, and answers null when only blanks were typed.
A driver asks it once, so no card sends a padded or empty word to the search.

## `public void TranslationClerkResolve_TwoEntriesShareHeadword_AnswersNothing()`

Two entries reading the same word, case folded, are a question, so the resolve answers null.

## `public void TranslationClerkResolve_OneExactHeadword_AnswersIt()`

One exact headword resolves, padding trimmed, and a prefix of it resolves to nothing.
