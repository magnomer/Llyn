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
