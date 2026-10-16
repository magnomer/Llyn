# Rack

This folder holds the records and pure rules of flashcard decks: the deck, its folder, grain, bearings and schedule.
A deck is studied on a phone flashcard app, and Llyn writes the deck file that app reads.
Every record is positional and immutable, so a change replaces the record whole.
Every rule here reads only its arguments.
Times arrive as stored strings, so nothing here reads the clock.

## `LGrain`

The kind of unit a deck's cards stand for.
A free card has no source, and every other grain names the one row its face reads.
A pair reads a meaning or a collocation, plus the target entry of its translation.
The numbers are stored in `cue.grain`, so a member never changes its number.

## `LBearingLead`

Which side the phone shows first, side one, side two or each in turn.
The numbers are stored in `side_first`, so a member never changes its number.

## `LAlmanacOrder`

The phone's card order of a deck.
The numbers are stored in `study_order`, so a member never changes its number.

## `LDeckFolder`

One folder of the deck list.
A deck in no folder sits at the top level.

## `LDeckSchedule`

The phone's card-order options for one deck.
Every option is deck-specific in the phone, so the deck carries them.
The defaults are the phone's own.
`LDeckScheduleGoal` is the round size in Goal order.
`LDeckScheduleSpaced` is the round size in spaced order.
`LDeckScheduleTarget` counts hours.
`LDeckScheduleQuorum` is the phone's Min Cards in Set for Goal order.
A null quorum means the scheduler's default.

## `LDeck`

One flashcard deck.
`LDeckCode` is the export file name without its extension.
It is set at create, and a rename keeps it.
So a renamed deck still writes the file the phone downloaded.
The deck owns its template, named by `LDeckStencilId`.
Editing one deck's sides therefore never changes another deck.
`LDeckSyncedUtc` is null until the first export.

## `LBearing`

One study direction of a deck.
`LBearingFront` and `LBearingBack` are side bitmasks from 1 to 31.
`LBearingStatistics` is the phone's statistics set, from 1 to 4.

## `LDeckCode`

The rule that keeps deck codes apart.
Two decks must never write one file name.
It sits in Core because it is a pure rule over realm values.
Both members compare with `StringComparer.OrdinalIgnoreCase`, which folds non-ASCII case too.
`taken` holds the codes of the other decks only.
So a save that keeps a deck's own code passes.

## `LDeckCodeResolve`

Returns the normalized name, or that name with ` 2`, ` 3` and so on appended until no code matches.

## `LDeckCodeCheck`

Answers false when a code in `taken` matches, ignoring case.

## `LDeckCensus`

The counts the statistics tab shows for one deck or for all decks.
It is built from stored cards, and nothing of it is stored.
`LDeckCensusBuild` counts the cards, the latest sync and the cards added in each recent week.
Each count reads one property of the card, so a card falls in every count whose property it has.
An active card is one that is not resting, and a flagged card is one with any flag.
The latest sync is the latest stamp that parses, and a deck never synced gives none.
`LDeckCensusSpan` holds the twelve weeks the tab charts.

## `LDeckWeek`

One Monday-to-Sunday week of the caller's time zone and the count of cards added in it.
`LDeckWeekDivide` answers the given number of weeks, oldest first, ending with the week that holds now.
Each stamp is read as UTC and placed by its local date.
So a card added late on a UTC Sunday may count on a local Monday.
A stamp outside the span, or one that does not parse, counts nowhere.
Now and the zone are arguments, so the rule never reads the clock or the machine zone.

## Stamp

Commit: `121691f60459`
- `LAlmanacOrder.cs` `e6f4236b098a1575`
- `LBearing.cs` `dfb5d059b3fca107`
- `LBearingLead.cs` `7441fe1ee544c02b`
- `LDeck.cs` `0b3c3deccdd6f2f6`
- `LDeckCensus.cs` `4c72e47c201bf417`
- `LDeckCode.cs` `a91aa39796229f3d`
- `LDeckFolder.cs` `82bda161d83ab78e`
- `LDeckSchedule.cs` `dab03b25a0d8186f`
- `LDeckWeek.cs` `9c0b1725e2ad2df9`
- `LGrain.cs` `7cb27e3e6371db24`
