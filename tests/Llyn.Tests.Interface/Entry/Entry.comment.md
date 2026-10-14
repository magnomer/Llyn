# Entry

This folder holds the relays for an entry's lexical records.
They cover meanings, sentences, examples, inflections, state values and fanqie rows.
It also holds the rime-table fixture.

## `TInterfaceAncestor.cs`

The etymology half of the test interface.
Tests never construct a production type, so every Ancestor shape crosses here.
Each request relay names a draft and a box, and the engine's request flow applies it.
The requests set a box's text, target entry, language with its tongue, gloss and relation.
They also insert above a box, add beside it, remove it and restore it after a pick.
An id of zero names no box, tongue or relation.
So the same relay inserts at the roots or clears a relation.
The clerk relays read, add and delete the relation and tongue lists.
The engine relays read and add relations and add a tongue through the engine.
The lineage relay reads the tree of a draft's map for an entry.
`TAncestorBoxCreate` builds a stored box at position zero, and `TAncestorMapSave` and `TAncestorMapRead` store and read a map through the rig.

## `TInterfaceExample`

The relays that build the example and sentence records and their drafts.
A record's missing state values default to unspecified, so a test names only what it checks.
Each relay is transparent.
In `TExampleCreate` a present translation becomes the example's single gloss with no language.
An empty or missing translation leaves the example without a gloss.

## `TInterfaceFanqie`

The relays for the reads built on fanqie rows and their rimes.
That is the anchor scans, the diwei rime, rank and section reads, the tally marks, and the reading hypothesis.
Every relay but the one-row section scan is transparent.
The fanqie row builders stay on `TInterface`, beside the full builder in `TInterfaceFact`.
The one-row diwei section scan varies only the respelling switch and the localizer.
The row overload returns every section.
So a test reads both the section order and how one line lists its characters.
The hypothesis builder fills the tables directly, so a test needs no language pack to check the join.

## `TInterfaceInflection`

The relays that build the inflection records of an entry.
That is the forms, the inflections, the paradigms, the features and the morphology values.
It also builds the view tables, lines, forms and marks, and a rule book with its stems and endings.
The paradigm rule resolve and the mark text are relayed here too.
So are the book's prediction and division, and the difference scan and division.
Each relay is transparent.
The form relay calls Core's own piece cut, so it adds no rule either.
Records built here are unstored and unanalysed, leaving production code to assign identity and derive regularity.
Expected marks, mark text, difference ranges and predictions come from the production writer, reader, scanner and pipeline.
So no test-side format, parse or rewrite can drift from production.
The paradigm builder defaults its rules and exceptions to null, since that is the common fixture.
The book builder takes layout and stamp apart, so analysis identity and display shape can be exercised separately.

## `TParadigmFormCreate`

A test passes the ready text and tip a cell shows, as the Core view build answers them.
The pieces are cut here by `LParadigmFormScan`, as the paradigm clerk cuts them before the engine answers.
The split defaults to 0, so a cell built without one stands for an undivided form.
A test of a divided form passes where its ending starts.

## `TParadigmFormScan`

Cuts a form into its pieces and answers each piece as text, marked flag and cut flag.
`TParadigmPieceRow` is a file alias for that tuple, so the signature stays short.
The tuples let a test write the expected pieces as plain literals.

## TInterfaceLexicon

The relays for the lexical records of the core layer.
They build the lexical records and drafts a test hands to the engine.
The video film and span reads, the grasp range check and the grasp star scan are relayed here too.
The application's reflex find over a draft is relayed here too.
The core's meaning path read and collocation home read are relayed here too.
Each relay is transparent.
The reflex draft builder takes optional anchors, so a scan test can read a row's anchors back.
`TEntryDraftCreate` takes a pronunciation, its audio and part-of-speech names, and builds the draft shapes around them.
`TSpeechNameRead` reads those names back off a loaded draft.

Sibling files cover the rest of the records.
`TInterfaceExample` builds the example, gloss and sentence records and drafts.
`TInterfaceMentionSpan`, in the Mention folder, builds the mentions and reads the spans a test divides.
`TInterfaceInflection` builds the form, inflection, paradigm, feature and morphology records.
`TInterfaceState` builds and reads the state values and anchors.

## TInterfaceMeaning

The relays for the stores of the meaning side of an entry.
That is the meanings and collocations themselves, and the registers, situations, tags and translations hung on them.
They open a store and run one store operation.
Each relay is transparent.
The stores of the entry itself are relayed in `TInterfaceDatabase`, in the Vault folder.

## TInterfaceSentence

The relays for the sentence and mention stores.
They open the store and run one operation over a card's sentences or an example's mentions.
The sentence order load and the particle and dependence reads take a language instead.
Each relay is transparent.

## `TInterfaceState`

The relays for the state values and state anchors every lexical record carries.
They create, resolve, show and read the states a test hands in or checks.
Each relay is transparent.
A missing value reads as the empty written state.
A present value keeps its shown text and whether it is unknown.

## `TXiaoyunFixture`

Places characters in a rime table cell for tests in Engine and Conduct.
The place method stores one entry for the character and places it under one onset.
It also anchors the entry's reflex to it.
The anchor is what the cell scan finds, so a placement alone lists nothing.
The reflex is a Korean reading, so a page under a book language tallies it.
The Engine rime table tests and the Conduct yunjing panel tests place their cells through it.

## Stamp

Commit: `c6e60141a172`
- `TInterfaceAncestor.cs` `0ba588ff693a76f4`
- `TInterfaceExample.cs` `f2df04344ad12a53`
- `TInterfaceFanqie.cs` `9835e3d194d52aa7`
- `TInterfaceInflection.cs` `c64a5c850d40f157`
- `TInterfaceLexicon.cs` `6bf8c76bb901db9d`
- `TInterfaceMeaning.cs` `fa7d0c0de042a497`
- `TInterfaceSentence.cs` `f85ee90b93b5c834`
- `TInterfaceState.cs` `23703b8485b4e3c9`
- `TXiaoyunFixture.cs` `e87fb087f6871a3e`
