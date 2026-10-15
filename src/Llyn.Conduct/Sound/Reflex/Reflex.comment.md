# Reflex

This folder holds the editor's reflex block, its rows, and their typed cells and leads.
It also holds the anchor menus that tie a reflex row to rime-book rows.

## `CKindred`

The reflex block of the entry an editor holds, split from `CTimbre` by concern.
The reflex rows have their own ports, quill and lookup.
The held draft supplies its rows, while fold state is read separately from storage.

`CKindredRead` shows every row, the blank ones included, each resolved by the shared reflex scan.
The anchor labels are read against the draft's headword through the shared anchor map.
A refused anchor read shows `Display.AnchorFailed` once until the user acts.
The opened state is the one the engine stores for the held entry.
An empty desk answers no rows, no anchor and no fetching line.
A refused scan is not caught here.

`LKindredStart` starts the reflex lookup whenever the desk prepares a draft, before the editor's bulletin repaints.
So the first paint of a stored entry without reflexes already shows the fetching line.
A failure is not caught there, so it reaches the desk's draft load and shows its `LoadFailed` notice.

`CKindredAdd` maps null to the engine's id 0, so the driver never sends a magic id.
Null means the header was pressed, so the new row lands after all rows.
`CKindredToggle` sends nothing for a row the draft no longer holds.

`CKindredSpread` is the gate for the editor's "More readings" hinge.
It makes one engine call, which stores `opened` for the held entry.
The engine then raises the fold bulletin, so the editor and a reading view on that entry repaint.
True means the port returned without throwing, not that a stored row changed.
A fresh draft or an empty desk answers false and writes nothing.
A refusal shows `Reflex.SpreadFailed` every time and answers false, so the driver puts the hinge back.

`CKindredSet` answers the row as it now reads, which the driver keeps in place of its own copy.
It finds the row in the held draft before the write and copies it through `CReflex.CReflexTypedApply`.
A taken edit answers the typed text, since the deferred draft has not caught up yet.
A desk that fills its view takes no edit, and answers the row as the draft holds it.
A row the held draft lacks answers no row.
A typed language answers every row's lead, with the typed language standing in for the row's stored one.
Other cells answer no lead, and neither does an edit not taken.
Only the edit in hand is overlaid, so another row's edit still deferred reads as stored.
The lead compares the raw typed language, as the scan compares the stored one.

`LKindredFind` scans the rows alone, so a keystroke reads no anchor.
`LKindredTenure` is null while the desk fills, so a row the render writes raises no request.
`LKindredQuill` is internal so `CSoundingAnchor` sends its anchor edit through the same quill.

## `CReflex`

One reflex row, ready to show in the editor or the reading view.
Conduct resolved its text and its mark, so no driver asks the pack about the row.
`CReflexText` is the form the mark picks, the respelling or the phonetic text.
`CReflexRegion` is null when the draft names none, so a driver binds it as the tooltip as it stands.
`CReflexLead` marks a row that opens a run of one language, so it alone prints the language.

`CReflexHiddenCheck` is true for a folded row while the fold is closed.
`CReflexTypedApply` copies the row with one cell changed, and the lead and other fields stay.

`LReflexFoldCheck` is the one owner of whether the "More readings" toggle shows.
The reading view and the editor ask it over their own rows.

`LReflexLeadRead` marks each row that opens a run, so the language prints once per run.
`CKindred.CKindredSet` asks it again while a language is typed, since a row leads by position.

`LReflexAnchorRead` answers whether the headword offers anchoring and the anchor text of each row.
The engine's entry-based anchor rule answers both, and placements are joined by `LReflexSeparator`.
The reading view and the editor's reflex block both read their anchors here, so the map has one owner.
No stored entry or a refusal answers no anchor.
A refusal also shows `Display.AnchorFailed` through `envoy`, once through `noticed`, since both callers read on a repaint.

`LReflexKeyRead` gives `Reflex.` plus the name.
A blank name keys nothing a catalog holds, so a driver prints it as it stands.
A typed copy keys its language or kind by the same rule, so a typed label matches a stored one.

## `CReflexField`

The cell of a reflex row a user typed into.
The driver maps the cell it heard to one member, and `CKindred.CKindredSet` picks the engine member that writes it.

## `CReflexHead`, `CReflexTyped`

`CReflexHead` is one row's lead, answered while a language is typed.
`CReflexTyped` is the answer to one typed cell, and a driver writes its row from it alone.
Its row is null for a row the held draft lacks.
Its heads are empty unless a language was typed.

## `CSoundingAnchor`

The anchor menu of the editor's reflex rows, one row at a time.
It is split from `CSounding` by role, since its menu holds a selection and the `CSounding` reads hold none.
The chosen row is the session's selection, so it lives here and no driver holds it.
It borrows the reflex block's quill rather than building a reflex port.
`CSoundingAnchorCreate` takes only the kindred facet and checks it for null.

`CSoundingAnchorOpen` makes the row the selection and answers the readings it may anchor to.
The rows stay empty while the desk fills its view or holds nothing stored.
A refused scan shows `Input.FindFailed` once until the user acts, through `CKindred.LKindredAnchorRead`, and the rows stay empty.
The menu then shows its notice.
`CSoundingAnchorSet` does nothing while the menu is closed or the desk fills.
The row repaints from the draft change, so the gate answers nothing.

## `CAnchor`, `CAnchorRow`

The anchor menu of one reflex row and the fanqie readings it offers, in the engine's order.
The empty flag shows the menu's notice in place of rows.
A row may be only estimated by the engine.

## `CTimbreReflex`

The editor's reflex block, ready to paint.
The block shows when the pack declares a rule or the draft holds a row.

## Stamp

Commit: `2f9a06b8eb95`
- `CAnchor.cs` `35297b8cb4f7d0b6`
- `CAnchorRow.cs` `285a2303c7372f60`
- `CKindred.cs` `4fa083c4e3854388`
- `CReflex.cs` `96b4b23bb1dc93dd`
- `CReflexField.cs` `c82285178f656b0b`
- `CReflexHead.cs` `c080539fc6f40a2c`
- `CReflexTyped.cs` `5ebe59ff82f5b3d2`
- `CSoundingAnchor.cs` `d4e710aec3622a7d`
- `CTimbreReflex.cs` `822e57a2173be39b`
