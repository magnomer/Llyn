# CCardField.cs
Hash: `c74b174ca719dd22`

## `public sealed class CCardField`

The typed fields of the held draft's cards: title, expression and definition.
It is split from `CCard` by role, and its members keep the `CCard` base.
It keeps no state, so the editor builds it fresh over its desk.

## `private LQuillCard? CCardFieldQuill`

The card edits over the held draft, or none while the desk is filling or holds no draft.
So a field echoed during a fill writes nothing back.

## `public void CCardTitleSet(long cardId, string text)`

The user typed a card's title.
`CCardExpressionSet` and `CCardMeaningSet` do the same for the expression and the definition.

## `public void CCardExpressionSet(long cardId, string text)`

The user typed a card's expression.

## `public void CCardMeaningSet(long cardId, string text)`

The user typed a card's definition.
