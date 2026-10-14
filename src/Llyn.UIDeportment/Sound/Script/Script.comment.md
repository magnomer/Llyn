# Script

The glyph row of the input panel and the script box of glyph pictures.
The glyph row is the traditional form of a Han-script headword.
The script box shows pictures of a character's styles and is shared by the editor and the reading view.

## `QGlyph`

The editor's driver for the glyph row.
The form is stored as a transcription row in the pack's glyph scheme, so the row is a transcription item.
It renders apart from the transcription rows, beneath them, with no scheme dropdown and no plus or minus.
Typed text goes to the one transcription text gate, `CTranscription.CTranscriptionSet`.
The row shows only while the language declares a glyph section.
The list's tag says whether the section declares sources, so the template can hide the lookup button.
The lookup menu is the editor's one `QNotation`, opened under the glyph row.
The menu opens before the search observe, so the old search closes first.
The start request carries only the row id, and Conduct reads the row's scheme from the held draft.
The search runs in the glyph scheme, and the engine finds its sources in the pack's glyph section.
The block is keyed by the transcription id and repaints on every draft change.
The field is filled while its typing is not heard, so a repaint sends nothing.
Rows are built without the transcription change handler, since the field hears its own typing.
Rows take the draft's text before the full refill.
A refill from the previous draft's text would blank a typed field and move its caret to the start.
The row keeps the draft's text until the draft returns with the typed one.
The measure twin takes the text, or the glyph placeholder while the field is blank.
`QEditorFont` writes the row's typography.

## `QGlyphCommand`

The routed commands the glyph row and its character chips raise.
They carry the row or the character as parameter, because the templates have no code of their own.
Notation opens the lookup menu, searching in the glyph scheme.
Entry opens the entry a reading-view chip stands for, making it first when none exists.

## `QScript`

The presentation control for glyph pictures, shared by the editor and the reading view.
Its layout fold flag is separate from the entry's stored opening.
The caller owns persistence and paints the opening through `QScriptFoldRefine`.
Ready rows decide whether it has content.
A pending fetch or renewability keeps it visible without rows, and it starts collapsed otherwise.
Only folded layouts show the heading and its controls.
Changing the fold resets body visibility before the rest repaints.
A folded layout waits for an explicit opening paint rather than flashing its body first.
That callback does not change the switch's checked state.
An unfolded layout keeps the body visible whatever the opening.
The switch is exposed so each view's driver owns clicks and refused-click recovery.
Programmatic checked-state paints do not request persistence.
The panel holds no Conduct dependency.
Pending and renewable are supplied verdicts and not work this control starts.
A rebuild click raises only a renewal notice, and fetching stays outside the control.
Ordinary state changes do not overwrite the stored-opening paint.
A shared-size scope keeps row columns aligned within the box.
Any catalog notification refreshes the picture labels without filtering the property name.
No engine read or fetch is needed, because only presentation wording changes.
The class provides no localization unsubscription.
Picture decode failures leave through a notice, so failure presentation stays in the owning view.
Failures are delivered through the dispatcher and not raised inside a scan or layout pass.

## `QScriptImage`

One glyph picture as the box draws it, with its bitmap once seen, its caption and its drawn width.
The stored picture is the large original, and the row draws it at a fixed small height.
The bytes are held undecoded until the drawing element reports itself in view.
Only the size is read at once, from the picture's header.
The decoder is asked not to cache and to defer.
So the row has its width before it has its pixels.
The later decode goes straight to the drawn height, so the large original is never held as pixels.
`QScriptImageDecode` is the pixels decoded per drawn pixel of height, so a scaled display still reads crisp.
The bitmap is decoded once and frozen for any thread.
The picture serves as an opacity mask.
So the glyph takes the theme's ink and reads on a dark ground too.
The source is null until seen, and the mask then draws nothing.
Bytes the framework cannot read give no picture, reported once to the scan's failure handler.
The deferred decode may run long after the scan returned, so the failure handler is kept.
A failed decode leaves the picture blank and reports.
Only the WPF decoder knows the bytes do not decode, so the failure is reported here.
The age line is a wording key read on every request and not kept.
So it follows a language change with no fetch and no rebuilt row.
After a language change the box raises each picture, and only that one line redraws.
A caption or an age that is empty collapses its line.
The fill runs again when the picture decodes, since the row raises its source then.

## `QScriptItem`

One row of the box: one character in one style, with every picture of it.
It is built once from the engine's pictures and never changes.
The character is empty when the row is not the first of its character or the headword is one character.
A one-character headword already stands above the box.
The style is the pack's name for the row's chip.
The gloss under the pictures collapses when empty.
The scan makes one row per engine group and skips a group whose pictures all fail to decode.
Only the first decode failure of a scan reaches the handler, and later ones are dropped.
So one scan shows one notice, however many pictures fail now or when they come into view.
The drop stays here, because another driver would decode with its own library.

## Stamp

Commit: `c6e60141a172`
- `QGlyph.cs` `8b35d9b15de2d16a`
- `QGlyphCommand.cs` `2e10a0ac5a3b2af2`
- `QScript.cs` `82428afc8c003204`
- `QScriptImage.cs` `5f0922efc7df87bf`
- `QScriptItem.cs` `c07b2dae6b40fd7a`
