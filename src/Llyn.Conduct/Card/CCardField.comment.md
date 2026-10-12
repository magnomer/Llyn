# CCardField.cs
Hash: `1c45a2b59a846aaf`

## `public sealed class CCardField`

The fields of the held draft's cards: title, expression, definition and meaning links.
It is split from `CCard` by role, and its members keep the `CCard` base.
It keeps no draft state, only its desk, two ports and the envoy.

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

## `internal CCardField(CDesk desk, LDraftPort drafts, LSettingsPort settings, CEnvoy envoy)`

Only the editor builds it, over its own desk.
The ports and the envoy serve only the sense list read, which shows its failure notice through them.

## `public void CCardSenseAttach(long cardId, long senseId)`

The user linked a collocation card to a meaning of the entry.
A desk filling its controls or holding no draft takes no edit.

## `public void CCardSenseDetach(long cardId, long senseId)`

The user removed one link chip from a collocation card.
A desk filling its controls or holding no draft takes no edit.

## `public IReadOnlyList<CMeaning> CCardSenseRead()`

The meanings of the held entry at any depth, as the link picker lists them.
A desk holding no tenure answers an empty list.
The scan words an unnamed meaning as `Display.Unknown`.
A refused scan shows `Input.LoadFailed` and answers an empty list, so the picker still opens.
