# Sound

This folder holds the styles and templates for the sound parts of an entry and its sound panels.
The parts are pronunciation, accent, transcription, pitch contour, rime books, placement, scripts, parts of speech and series.
The reading row sheets are shared by the editor and the reading view.
Both modes must place every reading at one x and one height, so a mode switch moves nothing.
Commands, icons, collapse rules and hover effects are `QLook` rows in `QLookSound.cs`, never markup.
Templates are filled by named `Refine` methods, so the markup holds no member path or order string.

## `PThemePronunciation.xaml`

This sheet is the base of every reading row.
The accent, transcription and reflex sheets merge it so their rows sit in one stack.

### `Theme.Pronunciation.Height`

This is the one height of every reading row and of the volume tray beside it.
It is written once so the editor and the reading view cannot keep separate figures.

### `Theme.Pronunciation.Surface`

The row has no border and no fill, so the brackets stand alone against the page.
Its height is fixed to the playback group, so both sit on one line.
The reading view wears the same height, so a reading stands where it stands in the editor.

### `Theme.Pronunciation.Lead`

The lead is the flag and name, or the scheme, that a row starts with.
Every row puts it in a column of the shared size group `PReadingLabel`.
The editor and the reading view each open one such scope over their pronunciation and transcription rows.
So every reading starts at one x, whatever the row leads with.
The panel fills the row height instead of centering itself.
Each piece inside then centers against the row, never against its tallest sibling.
A row with a button and a row without one put their text on the same pixel.

### `Theme.Pronunciation.Body` and `Theme.Pronunciation.Cell`

The body fills the row height for the reason the lead does.
The cell holds a field and its unseen twin, and the field centers its text inside it.
So the mode switch moves neither the field nor the read text by a pixel.

### `Theme.Pronunciation.Flag`

The flag is the size the lookup menu draws it at, so a taken reading keeps its flag size.
Its fourteen pixels center on a half pixel, which rounds a pixel low.
One pixel under it makes the slot even, so the flag middle meets the bracket middle.

### `Theme.Pronunciation.Label`

The label stands in where the pack draws no flags or none resolves.
Its right margin of 24 reserves the slot an edit row scheme chevron fills.
That is the 8-pixel gap, a 4-pixel lead and the 12-pixel chevron.
So every label and reading stands in the same place in view and edit mode.

### `Theme.Pronunciation.Text`

The reading takes exactly the width its text needs.
It is set in the phonetic face, as the field and the measure are.
So the IPA stays whole whatever face the headword language pack chose.
A ceiling keeps a long reading from pushing the row wide.

### `Theme.Pronunciation.Measure`

The measure is an unseen twin of the field, carrying the same text in the same face.
The field is sized by this twin, so the box grows and shrinks with what is typed.
When nothing is typed the twin carries the placeholder, so the empty field still reads it.

### `Theme.Pronunciation.Field`

The field holds no floor width and fills the width its twin measures.
It is as tall as its own line and is centered against the row, its text pinned to its top.
So it takes the one centering step the read text block takes, with the same layout rounding.
Filling the row and centering inside instead let the text box place its line unrounded.
That sat a pixel off the read text at larger sizes.
It overhangs the twin by two pixels each side, since a WPF text box keeps that room for the caret.

## `PThemeAccent.xaml`

This sheet is the look of a further pronunciation row, shared by the editor and the reading view.
It merges the pronunciation styles, so a row reads as the primary pronunciation does.

### Buttons

`Theme.Accent.Tool` is the small icon button every row wears, and it matches the play button beside it.
The playback, phonetician and downloader styles build on it.
The icon is drawn by a template, since one shared element could stand in only one row at a time.
The plus and minus follow the same rule for the same reason.
The row fill hides the play button while the row has no audio.

### `Theme.Accent.Blank`

This is the room the editor lookup, download and plus-minus buttons take after a row play button.
The reading view fills it with the same buttons drawn hidden.
So the volume tray after the primary row stands at one x in both views.
Buttons are used rather than a width, because a width is a figure that drifts.

### `Theme.Accent.Control` and `Theme.Accent.Slot`

The plus and minus pair is unseen and untouchable by default.
`QLook` rows on the row surface show the part `PAccentShelf` on hover or focus.
The pair is built into the slot on first hover or focus from the `Slot` template.
The row fill names the template in the slot tag, so the markup holds no order string.
It is drawn at full opacity, since the slot itself is what the hover reveals.
The pair reads as the example rows pair does, so adding and dropping look the same everywhere.

### `Theme.Accent.Row` and `Theme.Accent.Display`

The row is the editor form and the display is the reading form with nothing to type into.
`QAccentItem.QAccentItemRefine` fills the named parts of both.
The chip sits in the shared lead column, so the bracket starts where every other reading starts.
The row fill tags the reading cell with the field style that grows over it while edited.
`Theme.Accent.Prompt` turns a blank reading into the muted placeholder.

## `PThemeTranscription.xaml`

A transcription is a spelling rather than a sound, so it wears no brackets and has no audio.
The sheet merges the pronunciation and accent styles.

### `Theme.Transcription.Phonetician`

The lookup button stands where the play button would.
So the row field ends where a pronunciation field ends.

### `Theme.Transcription.Option`

`QTranscriptionChoice.QTranscriptionChoiceRefine` disables a scheme another row already holds.
So the engine is never asked twice for one scheme.

### `Theme.Transcription.Scheme`

The dropdown stands where the reading view draws the scheme chip.
Closed, it reads as the chip does, with a small chevron and the accent on hover.
The chevron keeps a 4-pixel gap after the name and claims its full width.
With the dropdown 8-pixel margin that fills the 24-pixel slot `Theme.Pronunciation.Label` reserves.
So the name and the field stand where the reading view draws them.
The dropdown shows at once with no fade, so the pick never looks slow to answer.
`QTranscription.QTranscriptionRefine` fills items and value and writes a pick back to the row.
It also sets the display and value paths and the shown name, so the style holds no member path.

### `Theme.Transcription.Addition`

The plus adds a row in the next free scheme.
It is disabled when no scheme is left.

### Rows

`Theme.Transcription.Row` is filled by `QTranscription.QTranscriptionRefine`, which writes typing back.
`Theme.Transcription.Display` is filled by `QTranscriptionItem.QTranscriptionItemRefine`.

## `PThemeRebuild.xaml`

This is the one regenerate button of every sound part.
It carries the rebuild mark alone and names itself by tooltip, since the parts label their own heads.
The mark turns while the button carries the `Pending` cue, so a running fetch shows where it was asked.
`Theme.Sound.Rebuild.Spin` is one turn a second for as long as the rebuild runs.

## `PThemeContour.xaml`

The contour brushes colour a pitch contour box under the pronunciation chip.

### `Theme.Contour.Top`

The five level brushes run from Chao level five down to level one.
They are distinct hues rather than shades, stepping from warm at the top to cool at the bottom.
So a contour reads by colour as well as by shape.
A fall and a rise differ at a glance.

### `Theme.Contour.Box`

The box sits under the pronunciation row in both modes.
It stretches over its slot, the column of the delimited pronunciation, and draws its plot centred on it.
Its drawn width comes from its syllables and may hang past the slot on both sides.

## `PThemeDiwei.xaml`

This sheet draws the placement of a character by division and place.
Sections hold tally lines and a plate of rows.

### Switch

The switch is a pill at the right end of a section heading, holding the IPA and respelling halves.
The section fill shows it only while respelling is on for the language.
Each half has a rest style and a chosen style in the accent ink and tint.
`QDiweiItem` pulls the chosen styles by contract ID.
The IPA half gives `false` and the respelling half gives `true` as the command parameter.

### Tally

The tally lines sit between the heading and the plate.
A `QLook` row collapses them while the section has none.
The language and kind columns share their widths across the section.
A mark is a part text with a raised count and no box.
A click lists the characters, so `QLook` rows underline on hover and tint the part while checked.
`QTallyMark.QTallyMarkRefine` fills the toggle, opens the popup and lists the characters.
`QTally.QTallyItemRefine` fills the tally line parts.

### `Theme.Diwei.Plate`

The plate wears the line, corners and surface of the fanqie box.
The rows of one section share column widths within the section.

### `Theme.Diwei.Rounded`

The bare 合 mark of a 合口 row has no ground, so it never reads as a chip.
The line fill collapses it on an 開口 row.
The 合 word is red semibold in the warning ink, the same mark the fanqie block draws.

### `Theme.Diwei.Chip`

The chip is a character as a link back to its entry, in accent ink.
The word takes the button ink itself, since the default text style would paint it black.
`Theme.Diwei.CharacterChip` is shared by the plate rows and the mark popups.

### Templates

`QDiweiLine.QDiweiLineRefine` fills the line parts.
`QDiweiItem.QDiweiItemRefine` fills the section parts and attaches the tally and line fills.

## `PThemeFanqie.xaml`

This sheet draws the rime book blocks under the script box and above the first meaning.
The box takes the same plain surface and thin line plate as the script box.

### `Theme.Fanqie.Chip`

The row fill hides the book chip, not collapses it, on a block with no book.
So the column keeps its width.
`Theme.Fanqie.Origin` is lighter than the book chip, since it only names the source site.

### `Theme.Fanqie.Character`

The glyph font comes from `QFontRefine`.
A `QLook` row collapses it when empty, so a one-character headword leaves no column.

### `Theme.Fanqie.Link`

The initial and rime columns are chip buttons, each opening the rime table on its category.
Each reads as the key that category is stored under.
The word takes the button ink itself, since the default text style would paint it black.
The border is always drawn and only coloured under the mouse, so hovering moves nothing beside it.
It fades while pressed and collapses when the part is empty.

### `Theme.Fanqie.Knot`

The 重紐 letter is read out of the link so the chip can carry the rime key alone.
A `QLook` row collapses it on a rime with no letter, which is most of them.

### Columns

The reading and label columns collapse when the line has none.
The 反切 column is smaller and muted, since it is the source evidence rather than the placement.
`Theme.Fanqie.Text` shows the placement as fetched and collapses on a line that carries parts.

### `Theme.Fanqie.MedialText`

The medial word is muted by default and the line fill turns it red and semibold for 合.
Red is the warning ink, chosen because it is the one colour the link chips never wear.
The medial column has no ground, so it never reads as a link chip.

### `Theme.Fanqie.Representative`

The button marks a reconstruction as representative among its siblings.
It is independent per line, so more than one reconstruction can carry the mark.
The line fill sets the cue to `Marked`, or `Faded` for a marked secondary line.
`QLook` rows show the star and order by that tag, dim it on hover, and carry its command.

### `Theme.Fanqie.Line`

The line columns share their widths across every block of the box, so the parts line up.
The rime heading sits in the column after the rime link.
`QFanqieLine.QFanqieLineRefine` fills its named parts.

### `Theme.Fanqie.Loading`

This is the only content of the box while nothing is stored yet.

### Series

The series line sits above the books and keeps a gap under itself.
The row fill collapses it while there is no series.
The stem chip wears the book chip colours, so series and books read as one family.
Each series key is a pressable chip that opens that series in the xiesheng panel.
A character under two series shows two chips.

### `Theme.Fanqie.Row`

The series chips stand over the reading column, clearing the star column the lines carry.
`QFanqieItem.QFanqieItemRefine` fills the named parts and attaches the line fill.

## `PThemeScript.xaml`

This sheet draws the script pictures above the first meaning.

### `Theme.Script.Box`

The plate is lighter than the paradigm box so the black glyphs stand out.
It stretches across the view, unlike that box, so a long row of pictures wraps.

### `Theme.Script.Style`

The style name uses the interface font so the glyph font does not carry into it.

### `Theme.Script.Shape`

The glyph is a rectangle of the theme ink masked by the picture.
The stored picture is black on a clear ground, so as a mask it takes whatever ink the theme paints.
The picture fill sets its size and mask from the item, keeping the original aspect.

### Captions

The caption sits right under the age when there is one.
So the two read as one caption of two lines.
The age uses the interface font because the box inherits the glyph font, which is for the source words.
A `QLook` row collapses the age when blank, as when no shipped language names the stored code.

### `Theme.Script.Loading`

This is the only content of the box while nothing is stored yet.

### Templates

The picture template keeps the age and caption in a panel of their own.
The gap under the picture then stands whichever line is shown.
`QScriptImage.QScriptImageRefine` fills the picture and hands the lazy loader its row.
The first two row columns share their widths across rows, so the chips line up.
`QScriptItem.QScriptItemRefine` fills the row parts and attaches the picture fill.

## `PThemeSpeech.xaml`

### `Theme.Speech.Chip`

The chip matches the editor marker chip apart from the removal button.
The parts of speech take a row of their own, below the pronunciation.

### `Theme.Unit.Chip`

The view draws the unit as the editor draws it: an outlined chip on the speech row.
The outline, not a fill, marks it as a kind of piece rather than one of its roles.
It copies the editor's dropper border, so View and Edit show the same chip.
`Theme.Unit.Name` is its text, written as a speech name.
`Theme.Freedom.Chip` and `Theme.Freedom.Name` are written as the unit's, right after it.

## `PThemeXiesheng.xaml`

This sheet is the look of the series page of the xiesheng panel.
It merges the palette, input and reflex sheets, so the styles find their brushes.

### `Theme.Stem.Fold`

The member hinge is the entry page fold toggle with its own text keys.
Folded it reads `Xiesheng.ReadingsShow`, and its checked row swaps in `Xiesheng.ReadingsHide`.
The look comes from `Theme.Reflex.Fold`, so only the content, alignment and bottom gap are restated.

### `Theme.Stem.Member`

The reading line wears `Theme.Text.Reading`, the look of the reading under the entry page headword.
The reflex rows reuse `Theme.Reflex.Display` inside the same `QReflexFrame`.
The frame sets the plain text font back, so the glyph font the list carries reaches only the character.
The rows and hinge share a stack no wider than the rows, so the hinge stays left.
The hinge keeps its place folded or opened, because the rows unfold below it.

## Stamp

Commit: `aae5349dbb94`
- `PThemeAccent.xaml` `9c0e30ae29c4e71f`
- `PThemeContour.xaml` `5d453453e0924ed5`
- `PThemeDiwei.xaml` `3bc509521e75d83f`
- `PThemeFanqie.xaml` `9617690fd799a490`
- `PThemePronunciation.xaml` `786bf25f1d301c65`
- `PThemeRebuild.xaml` `0967ab6bc83fe474`
- `PThemeScript.xaml` `0982495d8d4d730c`
- `PThemeSpeech.xaml` `e500ae67d8cda4a1`
- `PThemeTranscription.xaml` `1ad3479f49cd16a8`
- `PThemeXiesheng.xaml` `9871e402a53af889`
