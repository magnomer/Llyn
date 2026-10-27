# Xiesheng

This folder holds the xiesheng panel's gates and the editor's reflex block.
They cover phonetic series, their fold, the panel's kindred entry list and vista, and the reflex rows.

## `TKindred`

Covers the editor's reflex block read and its lookup start, over a real desk and ports a test may fake.
The block carries every row, the anchor labels, the held entry's stored opening and the fetching line.
The anchor check is asked once for the whole block and the format once per row.
An empty desk, a fresh draft or a refused anchor answers no anchor.
A refused anchor format leaves the rows in place and answers the whole block unanchored.
Opening a stored entry without reflexes starts the lookup once the draft is prepared.
A reflected or fresh draft starts none.
The rows show in the declared order the entry load gives them, here alphabetical since the language has no pack.
The new row copies the pressed row's language and kind, and lands directly below it inside its group.
The test picks the pressed row by its text, so the declared order never changes which row is pressed.
Zero or a row the draft no longer holds appends a blank row last.
A held row goes by its id, and the rows around it stay in their shown order.
Each press flips the held row's main mark.
A typed language is written and the leads answer the rows with the typed language standing in.
Leads follow the shown order, so a run of one language prints its name once.
A desk holding no entry takes nothing and answers no row.
A typed text goes to the respelling when the row's language respells, and to the phonetic text otherwise.
The phonetic text of a respelled row stays as stored.
Kind, romanization, meaning and note are each written, and none answers a lead.
An edit sent while the desk fills its view is refused.
The answer is then the row as the draft holds it, so the driver's cell drops the refused text.

## `TXiesheng`

Covers the xiesheng panel's gates end to end on a real workspace.
The series are fetched from fake pages, so nothing here reaches the web.
`TEntryList` covers the entry list's load, export, scribe mode and close over this panel.
The bundled series pack shows the tab, and with nothing fetched the panel lists nothing under the bare keys.
`TXieshengPack` declares one fanqie source and one series source, each fetched from a fake page.
Opening a fetched series by key lists it chosen with its count, and shows its page and entry.
The page carries the headword and glyph fonts of the series language, and the blank page carries none.
A blank or unknown key opens nothing.
An arrival empties the series search before the driver hears of it.
Choosing the chosen series unchooses it, choosing it again brings it back, and a workspace change clears it.
Choosing a series over an unsaved draft asks the leave question first.
A kept draft leaves the series, the draft and the change event as they were.
A discarded draft closes the editor and raises the change once.
A glyph opens in the library tab only while a series page shows, in the language of that page.
A blank glyph opens nothing, and a character opens the entry the engine resolves for it.
Unmatched queries read the unmatched keys, and a narrowed column unchooses its series.
The column starts ordered by name, and a null order keeps the chosen one.
`TXieshengPrepare` builds the panel over a fake envoy that answers no, or over a given envoy.
`TEntryList` builds its export panel through the given-envoy form.
`TXieshengEngineStart` starts the engine with fake pages for the rime book and the series module of one character.
`TXieshengStemSave` waits for the series fetch to settle.

## `TXieshengFold`

Covers the xiesheng gate that folds one series member, on a real workspace with fake pages.
It reuses the series pack and fetch helpers of `TXiesheng`.
Toggling the shown member opens it on the page and tells the driver to redraw.
The entry page's fold of the same entry stays closed.
An entry page fold raises the plain Fold notice, which the series page does not hear.
A series member toggle raises its own notice, which redraws the page.
With no series page shown the gate answers false and writes no mark.
A refusing port shows `Reflex.SpreadFailed` once, answers false and writes no mark.
A member whose rows are all in unfolded languages still folds on the series page.
A bare member has no reflex row and so no toggle.

## `TXieshengVista`

Covers the xiesheng panel's vista restore, its observers and its order menu on a real workspace.
A Workspace notice unchooses the series, empties the entry list and tells the driver once.
A Fanqie or Settings notice raises the grove and the entry list.
An Entry notice also raises the panel.
A second restore carries the held queries into the fresh vistas.
After it a Fanqie notice raises the grove once, through the marshal.
A create without a restore raises the same notice once too.
The menu offers the three orderings.
The atelier's close of the entry list is covered by `TEntryList`.

## Inline notes

### TKindred.cs: private static CPhonologyBundle TKindredGuisePrepare(bool respelled = false)

Fakes the guise answer so every row is respelled or plain as the test needs.
Each folded guise's shown text follows the guise rule over the row's text and respelling.
So the fake answers what the engine would.

## Stamp

Commit: `9377e657b395`
- `TKindred.cs` `e7328ebf1730fe20`
- `TXiesheng.cs` `52c0635ba1e1ea4c`
- `TXieshengFold.cs` `cc5aec5c9e1f5485`
- `TXieshengVista.cs` `21139593f7a7551b`
