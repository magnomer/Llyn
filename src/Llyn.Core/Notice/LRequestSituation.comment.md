# LRequestSituation.cs

The situation requests.
The four structural records name the card and the chip, and the three field records name only the situation.
A situation is one row however many cards link it, so a field edit reaches every card holding that id.
The situation panel sends one body record with the id of the draft's own situation.

## `public sealed record LRequestSituationAddition(`

Adds a new situation to the card with the typed title at `LRequestPosition`.
The engine mints its id, and commit creates the row.

## `public sealed record LRequestSituationPick(`

Links an existing situation to the card at `LRequestPosition`.
`Pick` reads as a noun here, the item the user picked, since it is not a registered verb.
The engine copies the stored row into the card under its positive id.
A card already holding that id is left as it is.

## `public sealed record LRequestSituationRemoval(long LRequestDraftId, long LRequestCardId, long LRequestSituationId)`

Unlinks one situation from the card.
The row itself stays in the store.

## `public sealed record LRequestSituationShift(`

Moves one situation chip to `LRequestPosition` inside its card.

## `public sealed record LRequestSituationTitle(long LRequestDraftId, long LRequestSituationId, LStateValue LRequestValue)`

Replaces the title of the situation, wherever the draft holds it.
A title left empty drops the chip from every card at the next normalize.

## `public sealed record LRequestSituationDescription(`

Replaces the description of the situation, wherever the draft holds it.

## `public sealed record LRequestSituationKind(long LRequestDraftId, long LRequestSituationId, LStateValue LRequestValue)`

Replaces the kind of the situation, wherever the draft holds it.

## `public sealed record LRequestSituationBody`

Carries the title, description and kind of one situation at once, as the form currently holds them.
Each travels as written, with its mark, and the engine resolves the state.
The engine keeps the held id, wherever the draft holds that situation.
A form sends this instead of comparing its controls against the draft field by field.
So the decision of what changed lives in the engine, and an unchanged body saves and announces nothing.
