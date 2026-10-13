# Shelf

This folder holds the sources panel with its session, its source rows and the entries citing the chosen Source.
It also holds the source editor with its byline popup.

## `CByline`

The popup of Authors a typed credit may already name, owned by the source editor.
It keeps the typed word, how many rows it offers, and which offered row is lit.
The rows are read from the engine on every notice and never kept.
It takes the atelier's repaint memory, since its rows are read on every repaint.

## `CBylineIndex`

The lit row's place among the offered rows, or `-1` while none is lit.
A new word and a close both put it back to `-1`.

## `CBylineWordSet`

Typing in a credit field searches again from no lit row.
A field the driver wrote itself has no keyboard, and it opens nothing.

## `CBylineRowsRead`

The Authors the word may name and the draft does not credit yet.
Each name arrives split around the word, so the driver only paints it.

## `CBylineMove`

Down or Up in a credit field lights the next or former row through `CLanternMove`.
It takes the key only while rows are offered.

## `CBylineSelect`

A press on an offered row credits its Author in the focused field's place.
A press with no row or no focused field only closes.

## `CBylineClose`

Closes the popup and forgets the word and the lit row.
`LBylineReset` forgets them without announcing it.
`LBylineCommit` closes the popup, then hands the picked Author to the editor.

## `LBylineFind`

A failed search offers nothing, so the popup stays shut rather than failing the field.
The failure is still shown, under `Source.FindFailed`, through the imprint desk's `LDeskRepaintShow`.
The rows are read on every repaint, so a lasting failure shows once until the user acts.

## `CFootnote`

The sources panel's entry list, holding the entries that cite the chosen Source.
While no Source is chosen, it holds every entry.
The source vista is its parent, and its own vista sits in the panel's aperture.
The engine narrows the rows by the parent's choice, so the list decides nothing about matching.
Its panel has no delete scope, because an entry is never deleted from this list.
The panel runs under the `List.LoadFailed` key over the desk of the shelf's entry editor.
The empty list reads `Source.Vacant` when nothing cites the Source, and `Source.Unmatched` under a search.
The shelf owns the editor, so it wires the panel's clear and edit to it and attaches the display.
An Entry notice selects a stored entry, refills the entry rows, and hands the Source rows to refill.

## `LFootnoteEntryCreate`

Opens a fresh entry without asking, for the shelf that already asked for the whole tab.
The fresh entry starts already citing the chosen Source, in one ShellEngine call.
So the first draft shown carries the citation, as the corpus and repertoire twins do.
A fresh entry is started by this gate alone, so no blank draft paints before the cited one.

## `CImprint`

The source editor owns the desk over one Source draft, its credit rows and the byline under them.
The stated fields go down raw, and the draft notice writes back what the engine holds.
The credit rows are the engine's, and the only credit state kept here is where the one blank row sits.
Every request is built by the desk's quill, so the editor names no request.
`CShelf` builds it over the atelier's ports.
The desk is built under the `Source` scope, and the byline over the same draft port.
It takes the atelier's repaint memory only to hand it to the byline.
The desk's prepared draft is announced as the reference notice.
While the desk fills its fields from a notice, no quill is offered and nothing is sent.
The held flag guards credit edits and byline searches, so they do nothing without a draft.
`LImprintOpen` starts the desk over the Source.
A null id starts a fresh one, with the blank row and byline closed.
`LImprintCancel` drops the draft, announces the rows, then closes the byline.

## `CImprintKindRead`

The kind menu the driver builds once, each option's tag and key decided by the engine.
Every tag appears once, since the read drops a repeated tag and keeps its first kind.
So a driver can pair each option to its tag without meeting two radios for one kind.
`LImprintKindRead` holds that rule and takes rows as a parameter.
A test can feed rows the static source never gives.

## `CImprintCreditRead`

The credit rows of the held draft, with the one blank row already placed among them.
The blank row has id `0`, an empty name, its own place and no move.
The blank row alone carries `CAuthorRowBlank`, and every engine row carries it false.
The driver tests that mark only to paint the row.
The gates never take that mark back from the driver.
Credit edits recognize the blank by id `0`, and adding there also checks its kept place.
Id `0` is the engine's word for no Author, so no stored credit carries it.

## `CImprintAuthorAdd`

The add button of a credit row opens a blank row beneath it and asks for the caret.
The blank row's own add button only asks for the caret.
An id `0` anywhere but the kept place names no row, so nothing happens.

## `CImprintAuthorRemove`

The remove button closes the byline, then closes the blank row or sends the credit's removal.
Id `0` is closed only while Conduct's marker says one is open, and is never sent to the engine.

## `CImprintAuthorMove`

The earlier or later button sends the credit one place along.
The blank row has no place in the draft, so it never moves.

## `CImprintAuthorFinish`

Enter in a credit field, answering whether it took the key.
With the byline offering a lit row, the lit Author takes the field's place.
Otherwise the typed name is credited, and a blank name reverts the field.
`CImprintAuthorCancel` handles Escape.
An offered byline only closes, and with none the field reverts.

## `LImprintAuthorInsert`

Credits a picked Author in place of the row's former credit.
The blank row closes before the quill sends, since the new credit repaints the rows at once.
When the quill sends nothing, the blank row comes back and the field reverts.
`LImprintCreditCommit` does the same for a typed name in one quill call.

## `CShelf`

The sources panel's session over the Source list, the entries citing the chosen Source and two editors.
The two vistas' chosen rows and editing flags are the panel's mode, and the drivers only follow.
The routing between the two lists lives in `CDiptych`, shared with the other two-list panels.
Every gate holds only the interaction and reaches the engine through a panel, a desk or the editor.
It asks the user and reports failures through the envoy, never through a seam a driver hands up.
It restores both vistas itself and chooses which side prints or exports.
The Source panel asks the source desk before it leaves a Source, and both panels finish through the session.
The entry list and the session take only the entry editor's desk.
So the shelf wires the entry list's clear and edit to the editor and attaches the editor's display to it.
The session runs over the source desk and defers to the entry editor while the entry side is in front.
The diptych's create seam drops the source draft and opens a fresh entry citing the chosen Source.
A cleared Source panel cancels the source draft, and an edited row opens the source editor on it.
A loaded Source draft is shaped into its colophon through `COeuvre`'s map, with no second panel.
The Source panel's row notices reach the entry list, whose rows follow the chosen Source.
The shelf's close joins the workspace's closures and closes the byline first, then the entry editor with its playback.
It restores its vistas last, so a built area already stands on started vistas.
The store verdict follows the desk in front.
Print is allowed for a shown entry or a shown Source outside edit mode.
A Source counts only on the Source side.
Export is allowed only for a shown entry.

## `LShelfVistaRestore`

Starts the Source vista and the entry vista and hands them to the panels and desks.
The entry list follows the Source vista as its parent, and the entry editor stands on the entry vista.
The Source panel's aperture carries the former query across, so the driver never re-sends its text.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.

## `LShelfObserverAttach`

Vista, Author, Reference, Example, Reflex and Settings notices refill the Source rows.
An Author notice also rereads the source draft.
A Workspace notice closes both sides through `LDiptychEntryClose`.

## `CShelfRollRead`

Reads the Sources the vista finds, mapped as the oeuvre maps its own, into one answer.
A shown Source the rows no longer choose is closed, so a stale selection never stays in front.
The tally is read after that close, so it never counts a Source the panel just left.
A failed read shows `Source.LoadFailed` and goes on with no Sources.
The roll is then the same empty roll a vista with no Sources gives.
The load task runs this read after the flag fill, so a throw here would fault the driver.

## `CShelfRoll`

The shelf's rows answer, ready to paint in one go.
The tally comes with the rows, so the view never reads the shelf a second time.
The tally is read after a stale choice closes.

## `CShelfReferenceSelect`

Opens the clicked Source after the leave question, keeping the edit mode of the side in front.
The navigation records the voyage station only once the leave is settled, so a kept leave records nothing.
`LShelfReferenceOpen` is the navigation's arrival, without asking.

## `CShelfEntrySelect`

Opens the clicked entry after the leave question, keeping the edit mode, and drops the source draft.
`LShelfEntryResonate` refreshes the entry draft and falls back to the chosen Source once the entry side has closed.

## `CShelfScribeToggle`

Switches the edit mode of the side in front.
A Source side leaving edit mode drops the source draft.
An entry side that closes falls back to the chosen Source.

## `CShelfPortraitPrint`

Prints the shown entry, or else the shown Source with the Source realm's legend.
It does nothing while neither is shown.
Export takes only the shown entry.

## Stamp

Commit: `c2f335c331fe`
- `CByline.cs` `e0137a6efea61338`
- `CFootnote.cs` `55784fc460d27692`
- `CImprint.cs` `6405ba82930d399e`
- `CShelf.cs` `83077db25d32b075`
- `CShelfRoll.cs` `5b98cc881a816079`
