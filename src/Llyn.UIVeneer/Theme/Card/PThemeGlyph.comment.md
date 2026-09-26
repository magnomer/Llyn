# PThemeGlyph.xaml

The look of the glyph row in the editor and of the character chips in the reading view.
The transcription styles are merged, so the editor row reads as a transcription row without its dropdown.
The reading view draws the same row with one chip per character in the field's place.
Each Han chip is a link into its entry.
Both panels read the glyph face and size from `Theme.Glyph.Family` and `Theme.Glyph.Size`.
The display and the editor put those keys on their lists.

## `<FontFamily x:Key="Theme.Glyph.Family">Segoe UI, Arial</FontFamily>`

The glyph face when the pack declares none, the same face `Theme.Phonetic` gives the other reading rows.
It is spelled out rather than aliased, because a `StaticResource` entry in a deferred dictionary broke the styles after it.
The list's own resources override it for a pack that names a face.

## `<sys:Double x:Key="Theme.Glyph.Size">19</sys:Double>`

The glyph size when the pack declares none, the size of the other reading rows.

## `<Style x:Key="Theme.Glyph.Phonetician" TargetType="Button">`

The lookup button of the glyph row, raising the glyph notation command so the menu opens in the glyph scheme.
It hides when the section declares no sources, as a Japanese pack does, since a search would find nothing.
The command is a `PLook` row, and the editor sets the visibility from the list tag.

## `<Style x:Key="Theme.Glyph.Field" TargetType="TextBox">`

The row's field, drawn as a pronunciation field but in the glyph face and size read from the resources.
The phonetic face is meant for IPA and Latin schemes, and a Han character reads wrong in it.

## `<Style x:Key="Theme.Glyph.Measure" TargetType="TextBlock">`

The unseen twin of the row's field, sized by its text or by the glyph placeholder when blank.
The editor fills its text from the row.
It takes the same face and size as the field, so the two measure alike.

## `<Style x:Key="Theme.Glyph.Chip" TargetType="Button">`

One character of the reading view, drawn in ink like the field's text and turned accent and underlined on hover.
It reads the glyph face and size from the resources and carries the field's medium weight.
So a chip and the field's character render alike.
No margin between chips, because the field runs its characters together and the row must match it.
Its parameter is the character item, and the display resolves the entry when the command fires.
A chip whose item carries no language is inert: no hand cursor, no tooltip, no hit test.
The command and the hover are `PLook` rows, and the display fills the character and the parameter.

## `<Style x:Key="Theme.Glyph.Line" TargetType="ItemsControl">`

The chip run that stands in the field's place inside the shared cell.
Its vertical margin is the field's, so the chips sit on the field's line.

## `<DataTemplate x:Key="Theme.Glyph.Row">`

The editor row: the scheme label in the shared label column, the field with its twin, and the lookup button.
No plus or minus, because a language has one glyph row at most.
`PEditor.PGlyphApply` fills the named parts, the scheme label included, and writes the typed text back to the row.

## `<DataTemplate x:Key="Theme.Glyph.Display">`

One chip of the reading view.
The display fills the named chip through `PLookItemAttach`.
