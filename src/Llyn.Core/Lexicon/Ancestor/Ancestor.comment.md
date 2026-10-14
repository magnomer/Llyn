# Ancestor

Ancestor holds the values of an entry's etymology map.
The map is a tree of boxes read from the headword upward.

## `LAncestor`

One stored box of an entry's map.
Its child is the box it feeds, and null when it feeds the headword.
Its position orders the co-sources that share one child.
A box with a target is linked to an entry, and a box without one is text.
Text, language and gloss never hold null, since the constructor turns null into an empty string.
The relation labels the connector between the box and its child, and null means unspecified.

## `LAncestorDraft`

The editable form of a box, with an id that is negative for a box not yet stored.
It carries no position, because the order of the draft list is the order of the siblings.
`LAncestorDraftCreate` turns stored boxes into drafts with their ids kept.
`LAncestorDraftResolve` gives each box the position of its arrival among the boxes sharing its child.
A box with no child counts under child zero, so the roots are numbered together.

## `LAncestorRelation`

A relation a connector can carry, builtin or added by the user.
The usage counts the boxes that carry it.
The key names the localized text of a builtin relation and is empty for a custom one.
A builtin name the key table does not list also gets an empty key.

## `LAncestorTongue`

A language the user added for text boxes, with the count of boxes that use it.

## `LAncestorLanguage`

One choice of the language dropdown, either an app language or a user tongue.
Only a user tongue has an id, so its absence marks a language of the app.

## `LLineage`

One box of the full tree as the reader shows it.
Its sources are the boxes that feed it, each with sources of its own.
A linked box takes the headword and language of its entry, and a text box takes its own.
The borrowed flag marks a box read from another entry's map, which the reader draws read-only.

## Stamp

Commit: `c6e60141a172`
- `LAncestor.cs` `de82a1e229b5a3d8`
- `LAncestorDraft.cs` `c26790b42d336186`
- `LAncestorLanguage.cs` `0d807f46035f1f69`
- `LAncestorRelation.cs` `dbe6d005cca3e1f4`
- `LAncestorTongue.cs` `9612cf4b3957782b`
- `LLineage.cs` `ae6a32c252d836d4`
