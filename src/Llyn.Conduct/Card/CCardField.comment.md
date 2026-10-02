# CCardField.cs
Hash: `ae678fc141c8c56c`

## `public sealed class CCardField`

The typed fields of the held draft's cards: title, expression and definition.
It is split from `CCard` by role, and its members keep the `CCard` base.
It keeps no state, so the editor builds it fresh over its desk.

## `public void CCardTitleSet(long cardId, string text)`

The user typed a card's title.
`CCardExpressionSet` and `CCardMeaningSet` do the same for the expression and the definition.
