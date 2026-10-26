# Shengfu

The Xiesheng panel, which browses the workspace by the phonetic series its characters belong to.
It is the counterpart of the Yunjing panel's rime table.

## `QXiesheng`

The panel's driver, shown only while a loaded language pack declares a series source.
Every decision lives in `CXiesheng`, and this file writes controls on notice.
The series column, the reader and the editor are served from this file.
The entry list has `QKindred`, and the series page has `QStem`.
Only the medium knows its dispatcher, so the marshal for the area's notices is built at introduce.
The entry list driver is introduced with the session, so it subscribes its own rows.
The rail is introduced last, with the atelier's navigation for its trail and the editor for its chronicle.
The vista refine answers the workspace opening once the session restored its vistas and attached its observers.
A workspace change redraws the flags after the area let the series and the chosen entry go.
Then only the entry list repaints, since its rows carry a flag.
So rows built while the load ran gain their flags.
A failed flag load is reported by Conduct, and rows are painted without flags.
Its one request is `CEntryListLoad`, which runs the flag fill and then answers the rows.
Opening a series a chip names empties the column's query field.
The area has already emptied the query, so the field only shows it, and its gate finds the query empty.
The series page also answers the panel's clearing, while the lectern empties the reader itself.
Print and export are allowed only while an entry is read.
They answer no before the panel is attached, since the commands are bound at construction.
Exit releases the editor's player, and the session's close already let the draft go and stopped playback.

## `QKindred`

The entry list, with the entry search field over it.
It shows the Entries the chosen series reaches and the one the reader stands on.
Choosing a row loads it into the reader, or into the editor when that side is open.
The row gate records the voyage station and opens it.
The paint takes the rows the area answered ready, so it asks Conduct nothing.
The parameterless form reads them, and a flag-fill refine hands in what its load answered.

## `QKindredItem`

One entry of the chosen series: name, epithet, language flag and chosen mark.
The headword and language are held only so a refresh can tell a changed row.
Match compares all but the mark, flag included, so a late flag refills the list.
The epithet leads with an en space, which the markup's format string once added.

## `QGroveItem`

One row of the series column: its key, its entry count and its chosen mark.
Only the chosen mark changes in place, so a refresh never rebuilds a row the user is pointing at.
Match compares the id, key and count.

## `QStem`

The driver of the series page, the counterpart of the Yunjing category page.
It decides nothing, since the engine composes the page and this file writes the controls.
`QXiesheng` builds it over the nested page, as `QPhonology` builds `QArticulation`.
Headword and glyph fonts arrive ready on the page.
Members go to the list as `QStemItem` rows and never as Conduct records.
The blank page leaves the headword empty and shows the empty line.
A character chip hands its character to the panel gate, which asks the shell for its entry.
Every member hinge click on the list is heard once, and it calls `CXieshengFoldToggle`.
A refused toggle flips the hinge back, and a stored one redraws the page through `CXieshengChanged`.

## `QStemCommand`

The command a character of a series page is opened through.
A template raises it, so the page needs no handler per character.

## `QStemItem`

One member of the series page: the character, its reading line and its reflex rows.
It copies the Conduct member, so no Conduct record reaches a control.
Each reflex row is wrapped as the entry page's `QReflexItem`.
No row is hidden one by one, since the series fold collapses the whole block.
The rows are read-only with no anchor or spinner, since the series page wires neither.
The reading line is empty when no reading is marked, and the `QLook` Empty row of `Theme.Text.Reading` collapses it.
The reflex rows show only while the member is opened, and collapse as one block while folded.
The hinge shows the opened state and appears only on a foldable member, as Conduct decides.

## Stamp

Commit: `c05429890cec`
- `QGroveItem.cs` `823d0ceea72ab80c`
- `QKindred.cs` `d2cc76085ab94811`
- `QKindredItem.cs` `88afc38dc2aabcda`
- `QStem.cs` `0a674cdf075c504c`
- `QStemCommand.cs` `d6be3a8a0a6c3372`
- `QStemItem.cs` `6bddfb5ad0f18164`
- `QXiesheng.cs` `b4fefffbd0f25493`
