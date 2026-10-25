# Sentence

Sentence holds the Example rows of a card, with their frame, citation and Gloss lines.
The engine holds the rows, and these classes render them by id and report what the user does.

## `QPassage`

This is one Example row, carrying the id of the row itself.
So a sentence the user rewrites stays the same row.
The id is the engine's, and the row never mints one.
It is fixed at construction, so a later redraw cannot rebind the row.
The row also holds `QPassageLine`, the line key Conduct handed with the draft, and a redraw refreshes it.
The line key matches the row to its chips, because an inserted or dropped row moves later positions.
An empty row may never have been written, or may have been written and be unknown now.
The second is marked rather than shown, and the converter reads the mark off the engine's value.
The row holds that value and nothing typed.
So the row never resolves a state and never holds a second copy of one.
It re-raises every change its frame announces, so a list watching the row repaints it.
The catalogs of Sources, languages, particles and dependences are shared by every row.
Nothing ships a particle or a dependence, so those lists stay empty until a user writes one.

### `QPassageCited`

It is Conduct's verdict, which the list driver reads to keep a cited row's citation box shown while idle.

### `QSentenceFrame`

The frame is built once from the row's first draft.
Later drafts reach it through `QPassageShow`, so it is never replaced.

### `QPassageChip`

The editor paints the chips from the sentence area's ready read after each redraw.
That read is keyed by line key, and the row looks its chips up by `QPassageLine`.
The row never resolves a Mention itself.
Every change goes out through a gate and comes back through the redraw.

### `QPassageShow`

The row id is not taken, since it was fixed at construction.
The line key is taken, since it follows the row's current position.
A field already reading what the engine holds is left alone, so the caret survives its own echo.

### `QPassageCitation`

The field shows its byline through a lookup made when it is drawn, so the row keeps no name.

### `QPassageCitationShow`

It is for when the list of Sources changed underneath the row.

## `QSentenceFrame`

This is the frame a sentence row is read under, a marker and a role.
Which of the two is written first is the language pack's to say and never the frame's.
The frame holds only what it was told and starts in the default order, marker first.

### `QSentenceFrameVisible`

A row that carries a marker or a role always shows the frame, because the reading view draws it.
A row carrying neither shows one only when the card has been asked for it.

### `QSentenceFrameWritten`

The card's switch stands down while the frame stands on what the row holds.
A frame already written cannot be opened or closed by asking.
A frame field says something unless Conduct words it muted.

### `QSentenceFrameGap`

It is one space when both fields are written and nothing otherwise.
So the writing view spaces the frame exactly as the reading view does.

### `QSentenceFrameOrder`

The row painter turns each slot into a grid column, so the frame holds no column of its own.
Until the pack's order arrives it holds the plain order, so the frame states no order itself.

### `QSentenceFrameShow`

A change also announces visibility, the written verdict and the gap, since all three read both fields.

## `QCitation`

This is the citation field of an Example row.
A citation is either a stored Source or nothing, so the field never keeps a line that matches no Source.
One list of Sources serves every row on the form.
The lookup and the commit name the row's card by the placard's collocation flag and id.
The list is refilled when the sentence facet's Sources change and when a workspace opens.
Each event is hooked once, so a refill leaves one handler.

### `QCitationFieldRefine`

Only a field with the keyboard in it reports, since a write from the draft echoes through the same event.

### `QCitationCommitObserve`

The card gate shows its own failure.
The dropdown's key handlers hear each key first, so a key the open dropdown takes never reaches here.

### `QCitationLeaveRefine`

Leaving shuts the dropdown and puts the cited Source's text back.
So a typed line matching no Source does not stay.

## `QExample`

This class fills the sentence row's template and shows its controls on hover.
It calls no Conduct gate, since every gate stays with the drivers whose handlers it hooks.
Each event is hooked once, so a refill leaves one handler.

### `QExampleRefine`

The card's row fill calls it before `QExampleIntroduce` on every fill and refill.

### `QExampleRevealRefine`

An uncited row's citation field dims while the list is neither hovered nor focused.
Whether a row is cited is the ready verdict `QPassageCited`, and the driver adds only its own hover state.

### `QExampleRowRefine`

The frame follows whether it is open or written.
Each frame field takes the column twice its slot in the frame's order.
The gap stands in column 1, so the fields take columns 0 and 2.
The icons are set here, since an icon is drawn by code.

### `QExampleLayoutRefine`

The layout is bound once, where element bindings stood in the markup.
The sentence column and the citation drop to the frame's baseline through the font converter.
A row already bound is left alone, so a refill binds nothing twice.

## `QSentence`

This is the sentence row's driver for adding, dropping, typing, mention commands and the frame.
It walks the editor's two card lists and holds the Conduct facets once introduced.
Each gate names its card by the placard's collocation flag and id.
The engine finds a card by kind and id.
The mention dropdown is handed in because it was built with this driver.

### `QSentenceOrder`

This is the GUI's own copy of the word order the rows show.
A row built between two frame reads takes it, so no card reads the engine for an order.

### `QSentenceCardFind`

A row's data context is the sentence, not the card, so every sentence gate first asks this.
The Gloss, citation and mention drivers ask it too.

### `QSentenceFrameRefine`

The gate owns the language and the empty lists a failed read leaves.

### `QSentenceMentionRefine`

It answers the draft change after the cards, so a new row already stands.
It passes the shown entry on to the sentence area, which mints the keys it looks up.

### `QSentenceTextObserve`

Each field is hooked to its own observer, so no field name decides the gate.
Only a field with the keyboard in it reports, since a draft write echoes through the same event.

### `QSentenceParticleObserve`

The choice hears the text change of its inner box, which is the raw text handed on.

### `QSentenceChipRefine`

A row with no entry is drawn with no chips, so a dropped line never leaves stale chips.

### `QSentenceAddObserve`

It hands the row's index in the card's sentences, the place Conduct defines for `below`.

### `QSentenceMeaningRefine`

It subscribes the ask the menu answers, so it hears only its own pick.

### `QSentenceSilenceObserve`

The silence gate is its own, so no magic id is sent for a mention with no target.

## Stamp

Commit: `8c403d1df768`
- `QCitation.cs` `a6f4014c0b0806dc`
- `QExample.cs` `bf40c0f67bf2e9b8`
- `QPassage.cs` `57505d44cd07c7c6`
- `QSentence.cs` `389de76f9480178d`
- `QSentenceFrame.cs` `9af5d661b1716af6`
