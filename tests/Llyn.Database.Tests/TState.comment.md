# TState.cs

## `public sealed class TState`

Holds the difference between a field nothing was written in and a field holding something that cannot be read back. The two look alike on a form and must never be stored, loaded, or dropped alike.

## `AnUnreadableFieldIsStoredAsUnreadableWhileAnEmptyOneIsStoredAsNothingRecorded`

Every card field that can stand empty is written with its state: an unreadable title, sentence, wording or tag is stored as unreadable and keeps no text, an empty one as nothing recorded, and both come back that way.

## `ARowWhoseFieldWasNeverWrittenIsNotStoredWhileAnUnreadableOneIs`

A row nothing was written in refers to nothing and is not stored; a row holding an unreadable sentence still refers to its Example and is kept.

## `AnUnreadableCitationIsNotTheSameAsCitingNoSourceAtAll`

A citation that cannot be read back is stored as unreadable, not as citing nothing, and the two read back apart.

## `RewritingAnUnreadableFieldReplacesItAndClearingItSaysNothingWasWritten`

Writing over an unreadable field states it, and clearing one says nothing is written there — and a Situation rewritten this way keeps the id it was stored under.
