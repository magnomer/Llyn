# Audio

This folder holds the rows of the recording menu, as markup alone.
The Deportment class of the same name loads this markup and forwards the two clicks to the editor.
The editor fills every named part, since the rows carry no binding.

## `PClipTemplate.xaml`

### `Theme.Clip.Preview`

It is the play button of one recording, and it also reports where its preview stands.
Plain, it is the quiet icon button the popup uses elsewhere, lighting softly under the pointer.
While the recording is fetched it fills orange, and while it sounds it fills blue, the glyph turning white.
A refused recording fills in the warning colour.
The editor gives the button a cue for its state, and the look sheet paints the surface for each cue.
The tagged fills win over the hover, so a filled button stays filled under the pointer.

### `Theme.Clip.Reading`

It shows one recording a source returned: a flag or name, a play button, and a taking button.
A tagged recording carries its variety in front of the buttons.
The variety is a flag with the name as tooltip when the language pack shows varieties as flags.
Otherwise it is the localized name.
An untagged recording carries the two buttons alone.
The column joins a shared size group by the recording's place, so the menus line up.
The play glyph is drawn at 24.
The icon fills less than half its frame, so a smaller size is hard to see and hit.
The taking button carries this recording's own download state, so a later arrival cannot overwrite it.

### `Theme.Clip.Row`

It shows one source, its name set small in the fixed column the pronunciation menu gives it.
Its recordings stand in a row beside the name, wrapping when they run out of width.
A source that offered nothing shows one italic line in place of them.
Hiding them rather than disabling them keeps a row that cannot act from looking merely busy.

## Stamp

Commit: `c2f335c331fe`
- `PClipTemplate.xaml` `f3bde425b7427902`
