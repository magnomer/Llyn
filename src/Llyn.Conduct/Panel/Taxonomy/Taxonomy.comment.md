# Taxonomy

This folder holds the taxonomy panel's session and the entry list of the chosen Tag.

## `CMembership`

The taxonomy's entry list, holding the entries that carry the chosen Tag and the one the reader stands on.
It is shaped like `CQuotation`, and the taxonomy owns it as `CCorpus` owns its quotation.
Its panel reports a failed load as `Tag.LoadFailed`.
Its empty list reads `Tag.Unmatched` under a search and `Tag.Vacant` without one.
The panel asks the editor's desk before it leaves an entry, and it finishes through the editor.
An entry notice goes to the panel, whose chosen row it may name.
A vista notice raises the entry rows alone, since only the entry search moved.
A notice on the chosen entry rereads its draft.
The rows are the entries under the chosen Tag, or every entry while none is chosen.
Print and export do nothing while none is chosen or the panel is editing.

## `CTaxonomy`

The taxonomy panel owns the Tag list, the entry editor and the entry list it hands to `CMembership`.
The tag vista and the membership vista hold the panel's state, and the drivers only follow.
Every gate holds only the interaction and reaches the engine through a vista, the panel or the editor's desk.
It restores both vistas itself, so no driver holds a port or a vista.
The aperture over the tag vista is built first, under `Tag.LoadFailed`.
The entry list's panel asks the editor's desk before it leaves an entry, and it finishes through the editor.
A cleared panel cancels the editor's draft, and an edited row opens the editor on it.
It registers its close with the workspace beside its draft finish and its vista restore.
It restores its vistas last, so a built area already stands on started vistas.
`CTenor` is the same session for Registers, with `Register` keys and `Coinage.Register`.

## `CTaxonomyAperture`

The list slot of the Tag list, which holds the tag vista and raises the Tag rows.
Its chosen Tag is the station the voyage records.
The entry rows follow a Tag rows read, so the Tag list is always read first.

## `CTaxonomyTagOpened`

Raised when an arrival or a coinage opens a Tag, so the driver shows both searches empty.
The aperture's rows notice follows it, so the lists are read after the fields are emptied.
`CTaxonomyWorkspaceChanged` is raised last when the workspace moved.
That is after the entry is closed, the Tag let go and the rows raised.

## `CTaxonomyTagToggle`

A click on a row records the station it leaves, then toggles the Tag.
It raises the rows, since choosing a Tag announces nothing in the engine.

## `LTaxonomyTagOpen`

The navigation's arrival chooses the Tag and empties both searches, then raises `CTaxonomyTagOpened`.
Emptying the searches is the arrival's own, so no driver has to echo it back through the query gates.
It raises the rows last, since the Tag may be new to the list.

## `LTaxonomyCoinageAllowed`

Whether New should name a new Tag rather than start an entry.
That holds while no Tag is chosen and no entry is shown.
Only the fresh gate reads it, after the leave question settled the shown entry.

## `LTaxonomyVistaRestore`

Starts the tag vista and the membership vista for this tab.
The tag vista is the entry list's roll, and the membership vista goes to the entry list and the editor.
The observers are attached to the fresh vistas last.
The constructor runs it last, and a workspace change runs it again through `CWorkspace`.

## `LTaxonomyObserverAttach`

A vista, reflex, settings or entry notice on the tag vista raises the aperture's rows.
A reflex fill or a flipped setting rewrites the epithet beside a headword.
A workspace notice and a Tag notice each have their own answer.
The entry list attaches its own observers in between, in the order the driver once used.
A draft edit or a fetched frequency, paradigm, script or fanqie row changes no listed row, and is not attached.

## `LTaxonomyWorkspaceResonate`

Puts the panel back on the workspace open now.
It closes the entry, lets go of the Tag, raises the rows, then raises `CTaxonomyWorkspaceChanged`.
A different workspace has its own Tags, so the Tag this panel stood on may not exist there.

## `LTaxonomyTagResonate`

A Tag notice carries a Tag id, not an entry id, so it never selects a row.
It raises the rows, then rereads the shown entry, whose chips may carry the renamed Tag.

## `CTaxonomyRowsRead`

The Tag rows the engine lists, chosen mark included, mapped through the card's one Tag map.
It answers nothing before the vistas are restored.
The engine drops a chosen Tag the rows no longer hold, so the entries fall back to every entry.
A failed read shows `Tag.LoadFailed` and answers no rows.
A read that succeeds raises the entry rows, since the entries hang on the chosen Tag.
A failed read raises nothing more, so the user sees one notice.

## `LTaxonomyTagCreate`

Makes the Tag from the raw wording and opens it as an arrival does.
The clerk below refuses a blank wording.
The panel is cleared first, so an unsaved fresh entry the leave check already settled does not linger.
A refused Tag is shown as `Tag.CreateFailed`, and nothing opens.

## `CTaxonomyEntryCreate`

The New button has one gate for a fresh Tag or a fresh entry.
It asks the leave question first, and a user who stays gets nothing new.
While a Tag may be coined, it asks the wording under `Coinage.Tag` and makes the Tag.
A retreat from that question makes nothing.
Otherwise it opens a fresh entry in the editor, in edit mode.
The chosen Tag goes down with the start, so the engine puts it on the first card in one call.
With no Tag chosen the fresh entry starts blank.

## `LTaxonomyClose`

The taxonomy's part of the window's exit gate `CAtelier.CAtelierClose`.
It closes the entry editor and cancels its display's playback.

## Stamp

Commit: `c2f335c331fe`
- `CMembership.cs` `97db9e3d9eec0c23`
- `CTaxonomy.cs` `25df419e063a091b`
