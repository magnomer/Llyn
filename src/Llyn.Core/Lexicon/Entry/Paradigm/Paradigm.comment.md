# Paradigm

This folder holds the per-entry paradigm answer and the ready-to-show inflection box built from it.

## `LParadigmSlot`

One form an entry is expected to have, paired with the inflection stored for it.
A paradigm names forms by pack code, and a slot is that form resolved to the workspace's rows.
The part of speech is the one the entry carries, never the parent the paradigm was declared on.
The state is unspecified when no inflection is stored, and specified when one is.
Unknown marks a persisted lacuna, separating an unanswered slot from a reached source that supplied no form.
The paradigm rides along, so a reader can judge whether the stored form is regular.
A slot of a multi-value cell carries every value, and its first value stands as the morphology.
An omitted cell list becomes the single morphology, which keeps one-value slots working.
`LParadigmSlotCodes` sorts the codes, so a cell is the same set whatever order the pack listed it in.
`LParadigmSlotKey` joins the codes with `+`, and a one-value key is the bare code.
The same key marks a missed multi-value cell in the lacuna store.
`LParadigmSlotName` keeps pack order, which reads as a phrase such as subjunctive imperfect first singular.
`LParadigmSlotCheck` lets a written form win, and an empty one counts as absent.
An unknown slot follows, then a pending fetch.
A form still missing is lost when the morphology setting is on and absent when it is off.
The paradigm list and the inflection view both ask this one rule, so their cells never disagree.
`LParadigmSlotShow` feeds that check and the stored text to `LParadigmShown.LParadigmShownResolve`.
`LParadigmSlotMatch` is true when two slots share a part of speech and both carry inflections of the same text.
A slot with no stored inflection matches nothing, so it never merges with a neighbour.
`LParadigmSlotUncertain` is the only state that suppresses a pending or lost verdict when no inflection answers.

## `LParadigmStatus`

What stands in a paradigm row for its inflected form.
`LParadigmSlot.LParadigmSlotCheck` answers it, so no shell decides it again.
Text means a written form, and Unknown means marked as having no known form.
Pending means no form yet while inflections are still being fetched.
Lost means no form, nothing fetching, and the morphology setting on.
Absent means no form with the setting off or the written form empty.

## `LParadigmShown`

What one paradigm cell shows for its status, with no user present.
It lives in Core so the app box, the slot list and the Joplin note read one answer.
`LParadigmShownResolve` is the one owner of the status wording.
A text status keeps its text and has no tip.
An unknown status shows a dash, and a pending, lost or absent status shows an ellipsis.
Only a lost status reads `held`, choosing the held key over the lost key.
The editor passes `held` on, since its draft holds the missing forms.
An unrecognized status throws rather than showing a placeholder.

## `LParadigmRow`

One nonempty row of the paradigm table.
Callers must supply at least one slot because labels and status use the first slot.
It gathers the slots that share a part of speech and an inflection text.
The engine groups the slots, so the shell only draws the rows it is handed.
The lead flag is true when the row opens a new part of speech.
The table must also have more than one part.
`LParadigmRowName` gives a multi-value slot its whole phrase, and a one-value slot its morphology name.
`LParadigmRowScan` cuts wherever the part of speech or the inflection text changes.
A slot with no stored inflection never matches its neighbour, so it stands in a row of its own.

## `LParadigmForm`

One cell of an inflection view, ready to show.
A cell without a written form takes its text and tip from `LParadigmShown.LParadigmShownResolve`, so no reader applies the status rule.
The marks come from the analysis stored with the form, widened to whole parts when divided.
Only the split asks the rule book again, for the predicted root.
A cell the entry has no slot for is empty text with no marks.
A divided form always has letters on both sides of its split.
The pieces stay empty from Core's view build, and only Application fills them from `LParadigmFormScan`.
Conduct maps them as data and never calls Core.
`LParadigmFormScan` is the only copy of the piece walk.
A new piece starts wherever the marking changes and at the split.
Marks are clamped to the text, so a stale range never throws.
Only the piece ending at a split inside the text carries the cut, and empty text answers no piece.
The app box and the sheet writer `LSheetInflection` both paint these pieces, so their cells agree.

## `LParadigmLine`

One row of an inflection view, with its cells in column order.
The group and the label are localization keys from the pack's layout, which Core passes through and never translates.
The group key sits on the first line of its group only, so later lines carry an empty group.

## `LParadigmPiece`

One run of a paradigm cell's text, cut by `LParadigmFormScan`.
Every letter of a piece shares one marking.
The text is never empty.

## `LParadigmTable`

One view of an entry's inflection box, ready to show.
The headers are localization keys for the columns, may be empty, and are never translated by Core.

## `LParadigmView`

The inflection box of one entry, in the collapsed view it opens with and the full expanded table.
It is the whole answer below Conduct, so Conduct only maps it.
Toggling between the views is visual state and asks nothing new.
`LParadigmViewScan` builds both views from the layout and the entry's slots.
Only slots declared on the layout's part take part.
It answers null when no slot belongs to that part, so the box falls back to the list.
It reads the stored marks, and asks the book only for the root of each custom cell.
A null book leaves the stored marks as they are.
`held` picks the held tip for a lost cell, true only for the editor.
`custom` picks the layout's custom pair of sheets, or the default pair when it is off.
A layout without a custom pair uses its default sheets under either setting.

## `LParadigmViewResolve`

Builds one table from one sheet of the layout.
Each cell uses the first slot whose code sequence exactly matches the sheet's cell.
A cell with no slot has empty text, no tip and no marks.
Only text-status cells with a stored inflection expose its text and stored marks.
With `custom` off, those cells show their text without marks and without a split.
With `custom` on, a cell whose prediction has a root is divided into root and ending.
The marks then colour the root or the ending as a whole, through `LParadigmViewDivide`.
A divided cell carries its split only when both sides hold letters.
A cell without a book, a root, or an ending start past 0 keeps its stored marks.
The stored marks stay in the database either way.
Every other matched cell takes its text and tip from `LParadigmShown.LParadigmShownResolve`, without marks.
That is the one place the view applies the status rule, so every reader gets a ready cell.

## `LParadigmViewDivide`

Widens the letter marks to whole parts, so an irregular part reads as one unit.
The root part is marked when any mark overlaps it, and so is the ending part.
An empty ending is never marked, and the answer is empty when no mark overlaps either part.

## Stamp

Commit: `c2f335c331fe`
- `LParadigmForm.cs` `b30f13bfad4d5f4d`
- `LParadigmLine.cs` `71d11de1b157d318`
- `LParadigmPiece.cs` `b70a4ef83c8e8359`
- `LParadigmRow.cs` `4339b60486471504`
- `LParadigmShown.cs` `5a765c283fe4bad6`
- `LParadigmSlot.cs` `3190e0afb2c60883`
- `LParadigmStatus.cs` `23daad39e1468331`
- `LParadigmTable.cs` `f331155e3d0ce659`
- `LParadigmView.cs` `5d1e0fada22b91d9`
