# Sentence

This folder holds a card's sentence rows as a markup file carries them, with example, mentions and reference by value.

## `LMarkupSentence`

One card's hold on one example.
The frame belongs to the card, and the example travels inside the row, since the file shares nothing by id.
A row may state a frame and no example at all.

## `LMarkupExample`

One example sentence, with its reference carried by value rather than by id.
Its mentions name their targets by natural key.
The reference is null when the example cites none, and equality compares it by value.

## `LMarkupMention`

A word inside an example that stands for an entry.
The target is named by headword and language, and a sense by its one-based position path such as `1.2`.
An empty headword makes a plain span that stands for no entry.

## `LMarkupReference`

A bibliographic source by value and with no id.
Authors are names in order rather than author references.
On import a stored reference with the same normalized title and year wins over these values.
A reference with no title matches a stored one by normalized url instead.

## Stamp

Commit: `c2f335c331fe`
- `LMarkupExample.cs` `37ee565aa8477893`
- `LMarkupMention.cs` `2e7b97e5ba67f17e`
- `LMarkupReference.cs` `2f84e64ee7543778`
- `LMarkupSentence.cs` `1c9a356c34ff560d`
