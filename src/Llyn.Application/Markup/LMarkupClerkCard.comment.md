# LMarkupClerkCard.cs
Hash: `b1babad16a7ece73`

## `public sealed class LMarkupClerkCard`

The card side of an export: meaning and collocation cards with their translations.
Examples on a card are translated through the shared example clerk.

## `public LMarkupClerkCard(LRig rig, LMarkupClerkExample example)`

Reads the entry port out of `rig` and keeps the example clerk the sentences are translated through.

## `public IReadOnlyList<LMarkupCard> LMarkupCardCreate(IReadOnlyList<LCardDraft> cards, IReadOnlyList<LCardDraft> meanings)`

Translates meaning or collocation cards, recursing into child senses.
Every leaf draft is copied with its id set to zero.
`meanings` is the entry's meaning tree, whatever list `cards` came from.
Each link of a card is written as the path of its meaning in that tree, in link order.
A link whose meaning has no path there is left out.

## `private IReadOnlyList<LMarkupTranslation> LMarkupTranslationCreate(IReadOnlyList<long> ids)`

Reads the headword and language of every translated entry.
A translation whose entry is gone is skipped.
