# Editor

This folder holds the entry editor's gates.
They cover its fields, fresh-draft opening, unit and freedom menus, and the esteem marks on its stored entry.

## `TEditor`

Covers the entry editor's gates end to end on a real workspace.
An open holds the entry.
The fresh drafts live in `TEditorFresh`.
Every open announces the shaped content once.
A save reopens what it stored, but a fresh input draft reopens blank.
An unchanged save stores nothing, and a finish without storing lets the tenure go.
An undo drops the typing by reopening the stored entry.
The head field gates live in `TEntry`.
The tenure's variety set tags the draft's primary reading, and a blank one or missing row changes nothing.
A field edit made while the desk fills its controls is dropped.

## `TEditorEntryPrepare`

Stores the English entry water with one card, so an editor has something to open.
`TEditorFresh` and `TEntry` share it.

## `TEditorPrepare`

Builds an editor over the engine and restores the vista of the named tab, ordered by headword.
It answers the editor wrapped in the fixture relay, so the tests reach its facets and gates there.
`TEditorFresh`, `TEntry`, `TFold`, `TCardFold` and `TReflexOpening` share it.

## `TEditorField`

Covers the editor's typed field gates over an input editor on a real workspace, with no delay.
A meaning's title and definition and a collocation's expression land as typed once the deferred requests are flushed.
The expression is typed on a collocation, because a meaning refuses one.
Each card gate takes a flag saying whether the id is a collocation's.
So its id is read in the right list.
A card's links read back in the card's order, with their resolved headwords.
The changed-draft notification carries a card's resolved link.
A card without links reads none.
An empty desk reads nothing.
An image's location, a video's location and span, and a gloss's text land as typed.
A pressed gloss button appends a gloss in the engine's gloss language, after the glosses held.
A removed gloss leaves, and a picked language retags the gloss that stays.
A gloss with no language carries Conduct's hint key, and a named one carries none.
The close gate lets the held draft and every search go.
An editor closed and then reopened on an entry hears a box fold stored by another editor, once.
The editor reads its draft's language, and an opened workspace opens a fresh draft in it.
The observers the editor attaches show the draft through the marshal the driver hands.

## `TEditorFieldPrepare`

Builds an input editor with no delay and the given marshal, and opens a fresh draft in it.
Its overload without a marshal runs each action at once.
It answers the fixture, so a test reads only the facets it needs.

## `TEditorSentenceAdd`

Gives the card one written sentence and answers its id, since a gloss needs the sentence's example.

## `TEditorFresh`

Covers how the entry editor opens a fresh draft, bare or from a linked object, on a real workspace.
It builds each editor through `TEditor.TEditorPrepare` and its stored entry through `TEditor.TEditorEntryPrepare`.
A null open holds a fresh draft, and a missing entry falls back to one.
A fresh draft holds one card of each kind.
A fresh occurrence start opens a draft already linked to its Situation, and a blank one without.
A fresh quotation start replaces the held draft with one already citing its Example.
A fresh footnote start opens a draft already citing its Source in the first sentence.
A fresh membership start replaces the held draft with one already carrying its Tag.
A fresh cohort start replaces the held draft with one already carrying its Register.

## `TEditorUnit`

Covers the unit and freedom menus of the input panel, read with the part-of-speech field.
Each is set through its own editor gate.
Both menus offer the same rows in every language.
A pick marks its row and stores the value, and a second pick clears it.

## `TEsteem`

Covers the marks an editor sets on its stored entry.
A grasp write reads back from the engine and words its label.
The standing step pressed again clears the grasp.
A grasp move past either end stops at the end, and the engine stores that end.
A fresh draft ignores the write, words nothing and still announces a re-read.
A favourite mark and its clearing read back the same way, each announced.
The grasp limit equals the Core grasp step on a draft and a stored editor.
A fresh draft reads no frequency chip.
Any engine grasp limit reads unchanged, except a negative one, which reads zero.
Any stored step the engine answers reads between zero and that limit.
These two hostile cases build the marks on a stored entry over `TDisplayGrasp.TGraspPortCreate`.

## Stamp

Commit: `8c403d1df768`
- `TEditor.cs` `6f943e488d6bccfd`
- `TEditorField.cs` `846031195c853678`
- `TEditorFresh.cs` `0f095b23c43c4cd3`
- `TEditorUnit.cs` `38c4aeb9ce8622e6`
- `TEsteem.cs` `5ddb21513d0a534a`
