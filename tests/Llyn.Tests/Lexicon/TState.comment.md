# TState.cs

## `public sealed class TState`

Holds the difference between a field nothing was written in and a field holding something that cannot be read back.
The two look alike on a form and must never be stored, loaded, or dropped alike.

## `EntrySave_UnreadableAndEmptyFields_StoresEachState`

Every card field that can stand empty is written with its state.
An unreadable title, sentence, wording or tag is stored as unreadable and keeps no text.
An empty one is stored as nothing recorded.
Both come back that way.

## `EntrySave_FieldNeverWritten_StoresNoRow`

A row nothing was written in refers to nothing and is not stored.
A row holding an unreadable sentence still refers to its Example and is kept.

## `EntrySave_UnreadableCitation_DiffersFromNoSource`

A citation that cannot be read back is stored as unreadable, not as citing nothing.
The two read back apart.

## `EntryUpdate_ClearedUnreadableField_RecordsNothing`

Writing over an unreadable field states it.
Clearing one says nothing is written there.
A Situation rewritten this way keeps the id it was stored under.
