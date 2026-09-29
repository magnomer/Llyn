# CCardField.cs

## `public sealed class CCardField`

The typed fields of the held draft's cards: title, expression, definition and links.
It is split from `CCard` by role, and its members keep the `CCard` base.
It keeps no state, so the editor builds it fresh over its desk.

## `public void CCardTitleSet(long cardId, string text)`

The user typed a card's title.
`CCardExpressionSet` and `CCardMeaningSet` do the same for the expression and the definition.

## `public IReadOnlyList<CTranslationTarget> CCardTranslationRead(long cardId)`

The entries one card links to, ready as its chips show them.
