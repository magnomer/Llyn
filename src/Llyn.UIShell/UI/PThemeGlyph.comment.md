# PThemeGlyph.xaml

The look of the glyph row in the editor and of the character chips in the reading view.
The transcription styles are merged, so the editor row reads as a transcription row without its dropdown.
The reading view draws no text row but one chip per character, each a link into that character's entry.

## `<BooleanToVisibilityConverter x:Key="Theme.Glyph.Sourced" />`

Turns the list's tag, which says whether the pack declares lookup sources, into the lookup button's visibility.

## `<Style x:Key="Theme.Glyph.Phonetician" TargetType="Button">`

The lookup button of the glyph row, raising the glyph notation command so the menu opens in the glyph scheme.
It hides when the section declares no sources, as a Japanese pack does, since a search would find nothing.

## `<Style x:Key="Theme.Glyph.Field" TargetType="TextBox">`

The row's field, drawn as a pronunciation field but in the serif face the editor puts on the list.
The phonetic face is meant for IPA and Latin schemes, and a Han character reads wrong in it.

## `<Style x:Key="Theme.Glyph.Measure" TargetType="TextBlock">`

The unseen twin of the row's field, sized by its text or by the glyph placeholder when blank.
It takes the same serif face as the field, so the two measure alike.

## `<Style x:Key="Theme.Glyph.Chip" TargetType="Button">`

One character of the reading view, drawn in the accent as a link and underlined on hover.
It sets no font family or size, so the glyph typography the display puts on the list reaches it.
Its parameter is the character item, and the display resolves the entry when the command fires.

## `<DataTemplate x:Key="Theme.Glyph.Row">`

The editor row: the scheme label in the shared label column, the field with its twin, and the lookup button.
No plus or minus, because a language has one glyph row at most.

## `<DataTemplate x:Key="Theme.Glyph.Display">`

One chip of the reading view.
