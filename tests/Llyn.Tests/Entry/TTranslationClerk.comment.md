# TTranslationClerk.cs

## `public sealed class TTranslationClerk`

Covers the translation clerk's search and resolve over the entry fake, with no engine and no SQLite.
The fake answers every entry to any query, so the ordering and the exclusions are the clerk's own.

## `public void TranslationClerkFind_ExactHeadword_ListsItBeforePartialMatches()`

The entry whose whole headword reads as the query leads, and the containing one follows.

## `public void TranslationClerkFind_OwnEntry_IsLeftOut()`

A card may not translate its own entry, so the search drops the id it is given.

## `public void TranslationClerkResolve_TwoEntriesShareHeadword_AnswersNothing()`

Two entries reading the same word, case folded, are a question, so the resolve answers null.

## `public void TranslationClerkResolve_OneExactHeadword_AnswersIt()`

One exact headword resolves, padding trimmed, and a prefix of it resolves to nothing.
