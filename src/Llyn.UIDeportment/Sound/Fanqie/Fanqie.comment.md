# Fanqie

The Yunjing panel and the fanqie box.
The panel browses the workspace as a rime table by onset and rime.
The fanqie box shows a character's rime-book placements and is shared by the editor and the reading view.

## `QYunjing`

The panel's driver.
It is shown only while a loaded language pack carries rime books, since without them there is no table.
Every decision lives in [CYunjing](../../../Llyn.Conduct/Panel/CYunjing.comment.md), and this file writes controls on notice.
It drives the Veneer page `PYunjing`, so the page holds only markup.
The two columns have their own driver `QDiweiIndex`, and the entry list has `QXiaoyun`.
The category page, the reader and the editor are served from this file.
Only the medium knows its dispatcher, so the marshal for the area's notices is built at introduce.
The column and entry drivers are introduced with the session, so they subscribe their own rows.
The print and portrait bindings are added last, so no can-execute query meets a session not yet built.
The mode refine writes reader, editor and page visibility off the session's verdicts.
The shown check tells the session whether the page is on screen, since the session cannot see the window.
The page content is blank while it is hidden.
The vista refine answers the workspace opening once the session restored its vistas and attached its observers.
A workspace change redraws the flags after the area let both columns and the chosen entry go.
Then only the entry list repaints, since its rows alone carry a flag.
So rows built while the load ran gain their flags.
A failed flag load is reported by Conduct, and rows are painted without flags.
Its one request is `CEntryListLoad`, which runs the flag fill and then answers the rows it paints.
The store button is enabled while the editor holds something storable.
The chronicle buttons light only while the editor has a step to walk.
Exit releases the editor's player, and the area's own close stops playback.

## `QDiweiIndex`

The onset and rime columns, each with its search field over it.
Picking a cell opens its category page.
It subscribes what it paints itself, so the owner `QYunjing` only builds and introduces it.
The ordering pickers stay with the owner as shared `QChoiceOrder` parts.
Each column relists on the area's change and on the workspace opening, with the empty text the session names.
Opening a cell a fanqie chip names empties both search fields.
The area has already emptied the query, so the field only shows it.
The field's handler still hears the change, and its gate finds the query already empty.
A click on either column hands the cell's id and final flag to the select gate.

## `QXiaoyun`

The entry list, with the entry search field over it.
It shows the Entries at the chosen cell and the one the reader stands on.
Choosing a row loads it into the reader, or into the editor when that side is open.
The paint takes the rows the area answered ready, so it asks Conduct nothing.
The parameterless form reads them, and a flag-fill refine hands in what its load answered.

## `PXiaoyunItem`

One Entry as a row of the entry list, written with a character at the chosen cell.
It mirrors the tenor panel's row: flag, headword and the name, numbered apart when headwords twin.
The epithet is the reading the pack names, empty when the setting is off or none is kept.
It leads with an en space, which the markup's format string once added.
The chosen mark is the one thing that changes after build.
Match compares all but the mark, flag included, so a late flag refills the list.
Otherwise `QSplice` keeps the held rows and their containers.

## `PYunjingItem`

One category as a row of the onset or rime column: its key and its entry count.
Both columns share the row, since an initial and a rime are the same kind of thing to choose.
The row carries its final flag, so a click can say which kind of category it names.
The chosen mark is the one thing that changes after build.
Both columns share the cell, so both attach this fill.

## `QFanqie`

The presentation control for fanqie blocks, shared by the editor and the reading view.
Its layout fold flag is separate from the entry's stored opening.
The caller owns persistence and paints the opening through `QFanqieFoldRefine`.
Ready blocks decide whether the control has content to show.
A pending fetch keeps it visible without blocks, and so does renewability, which keeps an empty editor box rebuildable.
It starts collapsed until one of those makes it visible.
Pending is the supplied fetch verdict and not work this control starts.
The rebuild verdict is supplied, so no Conduct object enters the panel.
Only folded layouts show the heading and its controls.
Changing the fold resets body visibility before the rest is repainted.
A folded layout waits for an explicit opening paint rather than flashing its body first.
An unfolded layout keeps the body visible whatever the opening.
The fold flag selects headed layout and is not persistent opening state.
That callback does not change the switch's checked state.
The switch is exposed so each view's driver owns clicks and refused-click recovery.
Programmatic checked-state paints do not request persistence.
Ordinary state changes do not overwrite the stored-opening paint.
A rebuild click raises only the renewal notice, and the caller selects the gate.
Fetching stays outside the control.
Category clicks forward the initial verdict and raw key, and only a fanqie line supplies that identity.
Stem text stays raw, so the receiving view interprets it.
Representative clicks carry identity, held rank and Ctrl state, read at execution time.
The control never chooses a replacement rank.
A shared-size scope keeps block columns aligned within the box.
The loading line follows pending state independently of block count.
The automation peer exposes focusable descendants instead of every placement cell.

## `QFanqieCommand`

The routed commands the fanqie line's links raise, bound by the box itself.
Each carries its line or text as its parameter, because the template has no code of its own.
Initial and rime open the rime table on the line's initial or rime.
Stem opens the stem a block's key names and carries the raw key text.
The key heads a block of lines.
Representative moves the line's rank when its star is pressed.
It carries only the line, so the box reads the Ctrl state when it hears it.

## `QFanqieItem`

One block of the box, holding one character's placements in one rime book from one source.
The lines draw as `QFanqieLine` rows in shared columns.
Blocks are built from the groups the engine handed over, with no engine call of their own.
The character heads only the first block of each character when the headword has several, and is empty otherwise.
The book chip is empty when the block above carries the same book from another source.
So it is not repeated.
The stems are the character's phonetic series, and only the character's first block holds them.
The stem line shows only while there are stems, and the book chip keeps its room when blank.

## `QFanqieLine`

One placement as one row of a block, its parts in shared columns.
The columns read as a 音韻地位 in the order reading, tone class, initial, rime, 重紐, 開合, division, tone and 反切.
Columns are copied from a stored row the engine already formatted.
No hypothesis runs here, so drawing an entry derives nothing.
The rank is one for the first representative reading and zero when unmarked.
The primary line is drawn at full strength.
The star carries the `Marked` or `Faded` cue and its content is the order.
The rime link shows the key as the diwei store keys it, without the heading and the 重紐 letter.
It adds the 開合 mark and division, such as `寒W I`.
The link carries the key it opens, so what the chip reads is what the rime table shows.
The heading and the 重紐 bracket stand outside the link.
The 重紐 bracket names the placement and not the rime category.
An initial or rime link with no text folds away.
A rounded medial is 合, drawn in the warning colour and semibold.
The placement text as fetched shows alone when the row carries no parts.

## Stamp

Commit: `c2f335c331fe`
- `PXiaoyunItem.cs` `69be0515189fbe50`
- `PYunjingItem.cs` `088c374bb3ae2233`
- `QDiweiIndex.cs` `97b9151e81a415bd`
- `QFanqie.cs` `afbea8e8cc33b7bf`
- `QFanqieCommand.cs` `998f6295db3e54af`
- `QFanqieItem.cs` `10e8fcc47e24b5d3`
- `QFanqieLine.cs` `24b335be02441cde`
- `QXiaoyun.cs` `b90f355fb2bf9c9e`
- `QYunjing.cs` `0893207efc19de56`
