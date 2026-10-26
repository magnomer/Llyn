# Articulation

The phonology panel: its inventory list, its IPA input aid and its drivers.
It stands apart from the other sound folders because it is a catalog panel in its own right.

## `QPhonology`

Drives the phonology panel and forwards what the user asks of it.
Every branch it once carried lives in Conduct's `CPhonology` and the `CPanel` it holds.
The panel is the veneer's `PPhonology` page, which the window places.
It names no Core type, since its rows arrive as `CCatalogPronunciation`.
The ordering, the filter and the command row each have their own shared driver.
The articulation driver is built over the nested page, as a parent driver builds a nested one.
It gets the fold toggle and the aid's seam, since folding the aid is its own concern.
The rail's print and export buttons sit inside the page, so their commands reach this page's bindings.
The sequence menu hangs under the whole bar and is built once from the orders Conduct offers.

### Introduce and refresh

Only the medium knows its dispatcher, so the marshal for the area's notices is built here.
The window fills the panel when it restores the stored ordering, and every later change arrives as an announcement.
So the panel is current whether or not its tab is in front.
The vista refine answers the workspace opening, after Conduct restored its vista and attached its observers.
It loads the flags before the first rows are built, because a row reads its flag at construction.
The filter menu lists the languages the flag load answers.
Its one request is `CPhonologyRowsLoad`, which runs the flag fill and then answers the rows it paints.
A workspace change redraws the new workspace's flags and then repaints the inventory.
A failed flag load is reported by Conduct, and the rows are still painted without flags.
The inventory refills spliced, so the list keeps its scroll position.
The paint takes the rows the area answered ready, so it asks Conduct nothing.
The store button is enabled only while the editor's desk can store.
The chronicle buttons light only while the editor has a step to walk.
The shown seam answers whether this tab is in front, which only the page knows.
Exit releases the editor's recording player.
The editor and playback stop in Conduct, through the close the panel registered with the workspace.

### Print and export

Print is live exactly when an entry is read in the display, and the deportment answers without reading any control.
The binding can be asked before the deportment is built, and then answers false.
Print and export portray the entry the engine reads, and nothing is read back from the screen.
The gate asks the envoy for the ticket or for the file and format.
The engine writes the document from stored rows.

### Aid fields

The aid is given the search field first.
A reader who opens the charts with nothing focused is looking a pronunciation up.
The editor's field takes over as soon as the reader focuses it.
That field is pulled by contract ID `PPronunciationField` once and not looked up per chosen character.

## `QArticulation`

Drives the IPA input aid, which holds two charts and one action, inserting a character.
The aid is the veneer's `PArticulation` page, nested in the phonology page.
Both charts are laid out by one builder, since only the table and chart differ.
The aid owns its own fold, so the toggle's icon is set here.
Its page styles are registered with `QLook`, so the glyph chip lights on hover and press.
No chart is built until the catalog arrives.
Both charts come from one catalog read.
The vowel chart has tongue height for rows and backness for columns, as the IPA chart arranges them.
A vowel cell holds the unrounded vowel and then the rounded one.
The consonant chart has manner for rows and place for columns.
One extra column and row hold the headers, so a cell is placed one further along in both.
Header text is a resource reference, because the chart is built once and the language may change.
Chart styles live in the page's own resources, which no ancestor of a new cell can see yet.
So they are looked up from the rack inside that markup.

### Typing target

The aid follows the keyboard between registered fields and types into the one last focused.
So it is tied to no field instance and needs no rewiring when the editor reopens.
It stands on the first registered field until the reader focuses another.
The search field is registered first, so a character typed before any focus lands in the search.
A last-focused field may have been hidden since, as when the editor closes.
A hidden field is skipped, and the first field still on screen takes the character.
A selection is replaced, as typing would, and the caret goes after the character.

### Layout

The fold toggle is the only state, so the handler reads it and keeps nothing.
The lane asks for one row of charts and takes a second only when the two do not fit across.
The consonant chart moves, because the vowel chart is the narrower.
The widths come from what each card asked for, not what it was given.
The lane measures its content unbounded, so those widths stay natural in either arrangement.
A chart not yet measured is left alone, so an early size change does not stack the charts.

## `QInventoryItem`

One entry row in the `PInventory` list, built from `CCatalogPronunciation` only.
It carries the pronunciation, the headword shown and the entry id the row loads through.
The id is identity and never displayed.
The pronunciation arrives bracketed and is shown as it is.
Conduct words an entry with no pronunciation as a bracket pair, so the row stays a row.
The language stands at the far end behind its flag.
The flag is resolved once for the language and handed to the row, not read from disk by it.
The name is numbered `(1)`, `(2)` while another row carries the same headword.
The engine numbers it on the row it returns, because a repeat is only visible across rows.
The plain headword stays for everything that is not display.
The epithet is the reading the pack names, small and muted.
It is empty when the setting is off or none is kept.
It leads with an en space, which the markup's format string once added.
The chosen mark is the only value that changes after build.
`QSplice` moves it in place, so the list keeps its scroll position.
Match compares everything but the mark, including pronunciation and flag.
So a changed reading or a late flag refills the list, and otherwise containers survive a refresh.

## Stamp

Commit: `c05429890cec`
- `QArticulation.cs` `3604f1f3de45e235`
- `QInventoryItem.cs` `b544669a3ae33bbd`
- `QPhonology.cs` `0e457ac98f204af3`
