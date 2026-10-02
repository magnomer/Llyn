# CMentionResult.cs
Hash: `b6fa25e7cd5757cf`

## `public sealed record CMentionResult(`

What a click on a text finds at the clicked place.

**Parameters**

- `CMentionResultOffset`: where the found word starts in the text.
- `CMentionResultStored`: the stored Mention under the click, null when none is there.
- `CMentionResultEntry`: the entries whose headword matches the clicked word.

## `public bool CMentionResultSingle`

Whether exactly one entry matches, so the click opens it at once.

## `public bool CMentionResultMany`

Whether several entries match, so the click offers a menu.

## `public long CMentionResultFirst`

The one matching entry, zero unless exactly one matches.
