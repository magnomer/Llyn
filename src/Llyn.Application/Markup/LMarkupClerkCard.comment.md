# LMarkupClerkCard.cs
Hash: `01974b257925a305`

## `public sealed class LMarkupClerkCard`

The card side of an export: meaning and collocation cards with their translations.
Examples on a card are translated through the shared example clerk.

## `public LMarkupClerkCard(LRig rig, LMarkupClerkExample example)`

Reads the entry port out of `rig` and keeps the example clerk the sentences are translated through.

## `public IReadOnlyList<LMarkupCard> LMarkupCardCreate(IReadOnlyList<LCardDraft> cards)`

Translates meaning or collocation cards, recursing into child senses.
Every leaf draft is copied with its id set to zero.

## `private IReadOnlyList<LMarkupTranslation> LMarkupTranslationCreate(IReadOnlyList<long> ids)`

Reads the headword and language of every translated entry.
A translation whose entry is gone is skipped.
