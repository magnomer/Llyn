# TExemplar.cs

## `internal static class TExemplar`

The one fully populated entry draft the export coverage tests share.
Every typed field carries its own distinct string, so a field an export drops names itself in the failure.
The strings are plain, so no escaping can hide a miss.
It sits in the interface folder because it builds production records directly.

## `internal static LEntryDraft TExemplarCreate(LEngine engine)`

Saves the translation target, the mention target, the cited reference and its author first.
Then builds the draft that points at them by id.
Speeches and inflections take their ids from the English pack, since markup names them and import resolves them there.
A third speech is typed by hand, so a custom speech travels too.
The mention names the sense of its target, so the sense path is written and resolved.
No string is a prefix of another, so a contains check on one cannot pass on another.
The noun plural is irregular on purpose, so the paradigm section shows it.
Regular stays false, since the engine owns it.

## `internal static IReadOnlyList<long> TExemplarSave(LEngine engine, LEntryDraft draft)`

Saves the draft and returns its id first, then every entry it links to.
A markup export of the whole list resolves every name on arrival.

## `internal static readonly string[] TExemplarHidden`

The typed strings the reading view never prints, so the portrait coverage test does not expect them.
The respellings stand behind the English switch, which is off, so the IPA prints in their place.
Audio, source and the syllable cut sit behind a reading, which prints alone.
A situation's description and kind belong to its own page, and an image is a picture, not text.

## `internal static string TExemplarShapeRead(LEntryDraft draft)`

The draft as indented JSON with every id zeroed, so two workspaces' drafts compare as one shape.
Ids, link ids, sense ids, pack value ids and the id lists are zeroed in place, their counts kept.
The pronunciation with no syllables keeps its empty list, so a syllable dropped on import still shows.
Derived properties are left out the way the draft archive leaves them out.
So a derived id never enters the shape.

## `private static void TExemplarNormalize(JsonTypeInfo info)`

Drops every property without a setter from the contract, the same trim the draft archive applies.

## `internal static IReadOnlyList<string> TExemplarTextRead(LEntryDraft draft)`

Every string the draft carries, read from the draft itself and never from a second list.
Ids, numbers and unset values are not strings and are not listed.
