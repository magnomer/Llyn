# Markup

The Markup folder translates an entry to and from markup.
Each clerk takes one part of the work: export, cards, ancestors, examples, draft resolving, links, merge and import.
No clerk opens a database of its own, so a test can hand in in-memory ports.
Names replace ids in a file, and names are resolved back to ids on import.

## `LMarkupClerk`

The clerk holds the cargo read and the export.
The per-entry load lives in `LMarkupClerkEntry`, and the import in `LMarkupClerkIntake`.
It reads the same port instances the engine holds.

### `LMarkupClerkRead`

The file is parsed, and entries in a language the workspace cannot load lose it with an omission.
Twin names are applied so two entries with one headword are told apart in the intake list.
Only entries of one language are twins.

### `LMarkupClerkExport`

The entries are loaded in one session, formatted as one tree and written through the markup port.
An id no entry carries refuses the export.

## `LMarkupClerkAncestor`

The etymology map of an entry, translated to markup boxes and back.
Markup carries no ids, so the boxes name each other by keys that count from one.

### `LMarkupAncestorCreate`

Writes the draft boxes breadth-first from the headword, each key being its place in that order.
A box names its child by the child's key and keeps its position among co-sources.
A linked box is written by its entry's headword and language, with no text.
A linked box whose entry is gone is written as a text box.
A text box is written with its text, and its language is the tongue's name when it holds a tongue.
The relation is written by name, so a custom relation survives a move to another workspace.
A box id met twice is written once, and a box not reachable from the headword is not written.

### `LMarkupAncestorResolve`

Rebuilds draft boxes from markup, one tree level at a time, with negative ids that are the keys negated.
A key that is zero, negative or repeated is omitted and noted.
So is a box whose child chain never reaches the headword, noted after the walk.
A box with a headword is linked to the entry that headword and language name.
A box that has sources of its own is never linked.
A linked box has none above it in its own map.
When a box has sources or its entry is not found, the loss is noted.
The box then keeps the headword as plain text.
A text box language not yet known becomes a tongue through the ancestor clerk.
A relation not yet known is created there too.
A language that matches an app language stays as text and makes no tongue.

## `LMarkupClerkCard`

The card side of an export: meaning and collocation cards with their translations.
Examples on a card are translated through the shared example clerk.

### `LMarkupCardCreate`

Translates meaning or collocation cards, recursing into child senses.
Every leaf draft is copied with its id set to zero.
The meanings are the entry's meaning tree, whatever list the cards came from.
Each link of a card is written as the path of its meaning in that tree, in link order.
A link whose meaning has no path there is left out.

### `LMarkupTranslationCreate`

Reads the headword and language of every translated entry.
A translation whose entry is gone is skipped.

## `LMarkupClerkDraft`

One markup entry turned into the entry draft the entry clerk saves.
Speeches and morphologies are looked up by name in the workspace.

### `LMarkupDraftResolve`

The draft of one entry comes with its held mentions and cited references gathered for the intake to settle.
A speech the workspace does not know stays as a named draft.
An inflection's unknown speech is omitted.
A recording whose location cannot stand loses the recording.
The lexical unit and freedom pass through as the file names them.
The ancestor boxes are resolved through the ancestor clerk, against the entries already prepared.
When a collocation names meaning paths, the meanings are first given temporary negative ids.
Each path is then looked up in the meaning tree, and the collocation links the ids found.
A path that finds no meaning is omitted, and a repeated one links once.

### `LMarkupSpeechResolve`

The speech value named in the language, omitted when not found.
The name is trimmed for the lookup, as the reader keeps text untrimmed and the archive trims only spaces.
The omission shows the name as the file wrote it.

### `LMarkupInflectionResolve`

One inflection with its speech and morphologies resolved by name, unknown morphologies omitted.
Each morphology name is trimmed for the lookup, and an omission shows it as written.

### `LMarkupCardResolve`

The card drafts, their examples, translations and media are resolved through the link, children in turn.
A translation whose target is not found is omitted.

### `LMarkupCardPrepare`

Gives every meaning and its children a fresh negative id, counting down from the given start.
The ids cannot clash with the held mentions and cited references, which count down from minus one.

## `LMarkupClerkEntry`

One entry translated into markup, with its inflections.
Cards go through the card clerk.
The engine's reflex clerk is kept so an export lists reflex rows in the pack's declared order.

### `LMarkupLoad`

Loads the entry draft and translates every id-bearing field into names.
Null means no entry has that id.
Draft ids and positions are dropped, because list order carries the positions.
A collocation's meaning links travel as meaning paths, resolved against the draft's meanings.
Reflex rows are written in the order the pack declares, through `LReflexClerkSort`, never in storage order.
The lexical unit and freedom are carried as the draft holds them.
The ancestor boxes are written through the ancestor clerk.
Grasp, frequency, favorite and timestamps are never read.
Forms pass whole, since they hold no id worth hiding.
A reflex also drops its anatomy and anchors, since the receiving engine derives the one and owns the other.

### `LMarkupInflectionCreate`

Replaces the speech value id and morphology ids with their stored names.
A morphology that no longer exists is skipped rather than written as a number.

## `LMarkupClerkExample`

The example side of an export: example texts, mentions, sense paths and cited references.
The card clerk names entries through it.

### `LMarkupExampleCreate`

Copies the example with its glosses, translated mentions and resolved reference.

### `LMarkupMentionCreate`

Names the mentioned entry by headword and language and its sense by position path.
A mention with no entry, or whose entry is gone, keeps its span and names nothing.

### `LMarkupMeaningResolve`

Walks from the meaning up to the root of the entry's meaning tree.
The path is one-based positions joined by dots, so the first sub-sense of the first meaning is `1.1`.
No sense yields the empty string.

### `LMarkupReferenceCreate`

Reads the cited reference and its credited authors in their stored order.
An unspecified or unknown anchor, or a reference that is gone, yields null.

## `LMarkupClerkIntake`

The import of a parsed markup cargo under the reader's intake choices.
All entries are saved in one session and one revision, held mentions and cited references settled last.
The revision clerk stamps the one revision an import records.
The query clerk finds the stored entries a parsed one may join.

### `LMarkupIntakeCreate`

The intake one declared row stands for, built by the Core record's factory.
The shell reaches that rule through it, so the shell calls no Core member for it.

### `LMarkupTargetFind`

The stored entries each parsed entry may join, in file order, one list per entry.
A stored entry shares the parsed headword and language, as the query clerk's headword find matches them.
Each target counts its cards, so the import window shows what a replacement drops without a second read.
A stored entry is loaded through the entry clerk, so its reflex rows come in the pack's declared order.

### `LMarkupClerkImport`

One intake per entry, each index once, else the import is refused.
A replace first detaches the senses that point into the entry.
A merge or replace with a blank language keeps the stored language.
A replace with a blank headword keeps the stored headword, since the save refuses a blank one.
A merge folds the parsed draft into the stored one.
Every entry is saved through the entry clerk with its pending inflection fetch cancelled.
The frequency fetches start once the session committed.
The entry's omissions, the detach rows included, take the entry's line when they carry none.
The outcome is clamped to the omission ceiling once, so import rows never follow the `...` row.

### `LMarkupLineSet`

Omissions added since a mark that carry no line get the entry's line.

### `LMarkupIntakeValidate`

A target that is not stored, or named twice, refuses the import.
A target held in an open entry draft refuses it as stale.

### `LMarkupIntakePrepare`

The entry id of every index, fresh entries created bare so later entries can link to them.
A fresh entry with a blank headword refuses the import.

### `LMarkupSenseDetach`

Clears the sense of every mention that points at a meaning of the entry, noting each example touched.
Each example is named by its text, since its id is never shown to the user.
The text is cut to forty text elements, so a long sentence keeps the row on one glance.
An example can stand in several entries or none, so a headword would not name it.

### `LMarkupMentionSettle`

The sense of every held mention is resolved now that the entries stand, and a sense not found is omitted.
A held mention absent from the identity map was never stored, so it is skipped.

### `LMarkupReferenceSettle`

The source of every cited example is found or created, and written onto the stored example.
An example absent from the identity map was dropped, as text-less or already present in a merge.
Its reference then stays untouched.

## `LMarkupClerkLink`

The links an import resolves: entries by headword, senses by path, references by title and locations by path.
The draft resolver, the ancestor clerk and the intake lean on it.

### `LMarkupEntryFind`

The stored entries with the headword in the language, both trimmed.

### `LMarkupExampleResolve`

An example draft from the markup, its mentions checked and their targets resolved.
A mention out of range, overlapping another, or whose headword finds no entry is omitted.
A mention with a sense is held for settling once the entries stand, its draft id a negative slot.
A referenced example is cited under its own negative draft id, its source left unspecified.
The source is found or created only once the example is stored, so a dropped example leaves no reference.
Both holds draw from one counter, so their slots never collide in the shared identity map.

### `LMarkupEntryResolve`

The entry the import is creating under that name, else the single stored match, else zero.

### `LMarkupSenseResolve`

The meaning a dotted position path names under the entry, or zero.

### `LMarkupLocationResolve`

Whether a media location can stand.
An unresolvable location is omitted and refused.
A missing file is omitted but kept.

### `LMarkupReferenceResolve`

The stored reference with the same title and year, or with no title the same address.
Else a fresh one is created with its authors.
The intake calls it only for an example it stored, so no reference is created without a citing example.

### `LMarkupMentionCheck`

Whether the mention lies inside the text and beside every mention already kept.

## `LMarkupClerkUnion`

The merge mode of an import.
The parsed draft is folded into the stored one without repeating a row.
Rows are matched on normalized text, so a retyped row with the same words is one row.

### `LMarkupUnionRead`

The loaded draft comes back with every parsed list appended and the cards re-placed.
The stored lexical unit and freedom each stand, and the file's is taken only when the entry has none.
The stored ancestor boxes stand whole, and the file's are taken only when the entry has none.
Transcriptions are matched on their scheme, because the save allows one row per scheme.
The stored row of a scheme wins, since a merge never removes.
A pronunciation without ipa is matched on its audio location, so a re-merged recording is one row.
With neither ipa nor audio there is nothing to match, so such a row is always appended.
A parsed collocation's links are first rewritten to the loaded meanings that absorbed their meanings.
So a link follows its meaning when the meaning merged into a stored one.

### `LMarkupRowAppend`

Parsed rows whose key is new, or empty, are appended after the loaded ones.
Keys are compared in the folded form of `LCatalogTextNormalize`.
The overload with a fold lets the caller choose the compare.
A transcription scheme is case sensitive in the save, so its key is only trimmed.

### `LMarkupStateFormat`

The match key of a stated value is its folded text with whether the value is unknown.
Registers, situation titles and media locations all key on it.
Two unknown values are one row, and never match a row named by text.
A value with no text and no unknown mark keeps an empty key, so it is never matched.

### `LMarkupNoteAppend`

A parsed note the stored note does not already hold is appended as a new paragraph.
The parsed note is appended as written, since the save normalizes it.

### `LMarkupNoteMatch`

Whether the parsed note's blocks appear in a row among the stored note's blocks.
Only whole blocks count, so a sentence inside a longer paragraph is no match.

### `LMarkupNoteParse`

The blocks of a note are normalized as the save does, then split at blank lines.
Normalizing both sides means trailing spaces and line ends never make a note look new.

### `LMarkupCardAppend`

A parsed card that matches a loaded one by title, expression and meaning merges into it, else it is appended.
A merged parsed card with an id is recorded in the absorbed map against the loaded id it joined.
The card overload appends every list of the card, children merged in turn.
Meaning links are appended as ids, so a link already held is not repeated.

### `LMarkupCardFind`

The index of the card it matches, or minus one.
A card with no text never matches.
`LMarkupCardFormat` gives the match key of a card, empty when it has no text.

### `LMarkupCardPlace`

The cards are renumbered from one, children in turn.

## Stamp

Commit: `c6e60141a172`
- `LMarkupClerk.cs` `ee9d329322a31da0`
- `LMarkupClerkAncestor.cs` `b8814c456369959a`
- `LMarkupClerkCard.cs` `b1babad16a7ece73`
- `LMarkupClerkDraft.cs` `d98929a0b3e1f2b9`
- `LMarkupClerkEntry.cs` `3a306752127d0bb2`
- `LMarkupClerkExample.cs` `2d5f8f0e1a7e4106`
- `LMarkupClerkIntake.cs` `a7b87b94b3d4cf6d`
- `LMarkupClerkLink.cs` `4abee0ef45d9bcf4`
- `LMarkupClerkUnion.cs` `7b58bd0ced5f8755`
