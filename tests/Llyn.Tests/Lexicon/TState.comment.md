# TState.cs

## `public sealed class TState`

Holds the difference between a field nothing was written in and a field holding something that cannot be read back.
The two look alike on a form and must never be stored, loaded, or dropped alike.

## `EntrySave_UnknownAndEmptyFields_StoresEachState`

Every card field that can stand empty is written with its state.
An unknown title, sentence, wording or tag is stored as unknown and keeps no text.
An empty one is stored as nothing recorded.
Both come back that way.

## `EntrySave_FieldNeverWritten_StoresNoRow`

A row nothing was written in refers to nothing and is not stored.
A row holding an unknown sentence still refers to its Example and is kept.

## `EntrySave_UnknownSourceCited_LinksTheSeededRow`

A source that is not known is the seeded Unknown Reference, cited like any other row.
The example table never writes an unknown wording for its source, and a sentence citing nothing stays unspecified.

## `ExampleRead_UnknownSourceWording_ReadsUnreadable`

The unknown wording an older workspace wrote for a source reads as unreadable, since no anchor answers to it.
Migration rewrites the wording before a read like this happens.

## `EntryUpdate_ClearedUnknownField_RecordsNothing`

Writing over an unknown field states it.
Clearing one says nothing is written there.
A Situation rewritten this way keeps the id it was stored under.

## `StateValueResolve_TextAndMark_MapsOneStateEach`

The one rule maps every pair a form can hand it.
A marked field is unknown and keeps no text.
An unmarked field holding nothing, or nothing but whitespace, is unspecified.
Any other text states itself.

## `StateValueResolve_WhitespaceRoundTrip_ReportsNoChange`

A field holding only whitespace is stored, loaded, and mapped again by the same rule.
What comes back maps to what was sent, so the editor holding it is not dirty.
