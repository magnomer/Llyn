# LSubject.cs

## `public enum LSubject`

Which kind of stored record a bulletin announces a change to.
A subscriber decides what to re-read from the kind alone.
It never has to know which panel or which call made the change.

The workspace value is the whole store changing at once.
That is the folder itself moving, so every kind a subscriber holds is stale.

## `LSubjectEntry,`

An Entry was stored, changed, or deleted.

## `LSubjectExample,`

An Example was stored, changed, deleted, attached, or detached.

## `LSubjectSituation,`

A Situation was stored, changed, deleted, attached, or detached.

## `LSubjectReference,`

A Source was stored, changed, deleted, attached, or detached.

## `LSubjectAuthor,`

An Author was stored, changed, deleted, attached, or detached.

## `LSubjectTag,`

A Tag was written onto a card, renamed, or deleted.

## `LSubjectFavorite,`

An Entry was marked or unmarked as a favorite.

## `LSubjectWorkspace,`

The workspace folder changed, so every stored record a subscriber holds is stale.

## `LSubjectDraft,`

A held Draft changed because a Request was applied to it.
The id is the draft's, and the surface holding that draft re-reads it and renders what differs.
An id of zero means a held draft was cancelled or deleted, so the set of held drafts shrank.
No surface holds draft zero, so only a surface counting the whole set acts on it.

## `LSubjectFrequency,`

An Entry's stored Frequency was fetched or cleared.
The id is the entry's, and the surface showing that entry re-reads its frequency alone.

## `LSubjectGrasp,`

An Entry's stored Grasp was set or cleared by the user.
The id is the entry's, and the surface showing that entry re-reads its grasp alone.

## `LSubjectInflection,`

An Entry's inflected form was stored by a morphology fetch.
The id is the entry's, and the surface showing that entry re-reads its forms alone.

## `LSubjectScript,`

The glyph pictures of one character were stored by a script fetch.
The id is the entry's whose display asked for the character.
A surface showing any entry of that language re-reads its script box, since another entry may share the character.

## `LSubjectFanqie,`

The rime-book rows of one character were stored by a fanqie fetch.
The id is the entry's whose display asked for the character.
A surface showing any entry of that language re-reads its fanqie box, since another entry may share the character.

## `LSubjectReflex,`

The reflex rows of one entry were filled by a web fetch.
The id is the entry's, and a surface showing it re-reads its reflex lines alone.

## `LSubjectSettings,`

A setting that shapes how stored records are shown was flipped, such as the respelling switch.
The id is zero, because no record changed and every surface showing a reading re-reads it.

## `LSubjectTenure,`

The state of one held draft moved: it changed, can undo or redo, or halted.
The id is the draft the tenure holds, so the one panel holding it settles its buttons.

## `LSubjectVista,`

The view state of one catalog tab moved: its order, filter, query or chosen row.
The id is the vista's own, so the one panel holding it re-lists or re-marks its rows.
