# PClipTemplate.xaml

## `<Style x:Key="Theme.Clip.Preview" TargetType="Button" BasedOn="{StaticResource Theme.Popup.IconAction}">`

The play button of one recording, which also reports where its preview stands.
Plain, it is the quiet icon button the popup uses elsewhere, lighting softly under the pointer.
While the recording is fetched it fills orange, and while it sounds it fills blue, the glyph turning white.
The state triggers come after the hover one, so a filled button stays filled under the pointer.
The template paints only the bound background, so the fill is set by the style alone.

## `<DataTemplate x:Key="Theme.Clip.Reading">`

One recording a source returned, as its flag or name, a play button, and a taking button.
A tagged recording carries its variety in front of the buttons.
That is the flag when the language pack shows varieties as flags, with the name as tooltip.
It is the localized name when the pack shows them as text.
An untagged recording carries the two buttons alone.
Its own style hides the name under a flag, since a template trigger cannot test for a present image.
A recording itself has nothing to read, so nothing is written about it, it is played.
The play glyph is drawn at 24, the size the row's own play button uses.
The icon fills less than half its frame, so a smaller size is hard to see and to hit.
The taking button carries this recording's own download state, so a later arrival cannot overwrite it.
Each button lights on its own hover alone, since a row holds several and none is the row's choice.

## `<DataTemplate x:Key="Theme.Clip.Row">`

One source, its name set small in the fixed column the pronunciation menu gives it.
The two menus line up.
Its recordings stand in a row beside the name, wrapping when they run out of width.
A source that offered nothing shows one italic line in place of them.
Hiding them rather than disabling them keeps a row that cannot act from looking like one that is merely busy.
