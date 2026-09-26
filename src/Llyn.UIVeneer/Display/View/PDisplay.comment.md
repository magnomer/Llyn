# PDisplay.xaml

## `<UserControl.Resources>`

The read-only card shapes are merged in from [PDisplayCard.xaml](../Template/PDisplayCard.comment.md).
A Meaning card and a Collocation card are one card (LCardDraft), so the two templates share their tail.
They are kept apart from this file.
The card is its own shape, not part of the page around it.
Every member the loaded draft carries is drawn.
A field the card left empty collapses rather than leaving a blank line.
The incoming rows and the stamps take their shapes from [PDisplayUsage.xaml](../Template/PDisplayUsage.comment.md).
The contents column takes its row from [PDisplayCompass.xaml](../Compass/PDisplayCompass.comment.md).
That dictionary is merged from code, because the row answers a click.
The root and the header resize the contents through the compass, so neither names a handler here.
No element here names a handler, a command or a binding.
The Deportment class of the same name loads this markup and wires each named part.

## `<StackPanel x:Name="PDisplayIncomingSection" Margin="0,28,0,0">`

What other entries point at this one.
A card lists its own links, so the entry it points at would otherwise say nothing about them.
Each row opens the entry that carries it, because that is where such a link is edited.
The heading says "Links here".
It names the direction without claiming that the whole source entry is a translation.
Each source is a standalone link card rather than a choice row nested inside a form card.
An icon establishes direction, the source headword leads, and the referring Meaning or Collocation is a compact badge.
The side's title and source language remain secondary context.
When no entry links here, the whole section collapses.
An empty relationship does not occupy the page.

## `<StackPanel Grid.Column="2" Style="{StaticResource Theme.Favorite.Row}">`

The heart, the stars and their words, in the row the editor draws them in.
The row, the heart, the stars and the words each take their shape from the theme.
So the two views cannot place them apart.

## `<ToggleButton x:Name="PDisplayFavorite" ...>`

The heart stands beside the headword, so the mark is made where the entry is read.
Every entry-browsing tab carries it, because the mark belongs to the entry and not to one panel.
A filled heart says the entry is marked.
The heart icon is named `PFavoriteMark`, so the toggle's `PLook` rows fill it hollow or filled.

## `<local:PGrasp x:Name="PDisplayGrasp" ...>`

The star row follows the heart, where the entry is read.
It shows how well the user says they know the word, and a click sets it there.
It draws the stored step alone, never a value the store has not confirmed.
The words after it say what the shown step means, and follow the pointer while it hovers.

## `<Grid>`

The entry as it reads, with nothing to type into.
The panel that hosts this decides what is selected and when the editor takes its place.
So the same view serves the library panel, the phonology panel and the taxonomy panel unchanged.

## `<Border x:Name="PDisplayPronunciationSurface" Style="{StaticResource Theme.Pronunciation.Surface}" ...>`

The pronunciation chip the editor also wears, defined once in the theme.
It carries the primary pronunciation's variety as a flag or a label before the brackets, and play after them.
The brackets are named so the lectern can turn them into slashes for a phonemic respelling.
The play button shows only while the primary pronunciation owns a recording still on disk.
After it stand the editor's lookup, download and plus-minus buttons, drawn hidden.
They keep the room the editor's row takes, so the volume tray stands at one x in both views.
The chip collapses when the entry carries no pronunciation.

## `<local:PContour x:Name="PDisplayContour" Style="{StaticResource Theme.Contour.Box}" />`

The tone contour of the primary pronunciation, drawn beneath its chip when the entry's language is tonal.
The Deportment class hands it the original IPA and the tonal flag, whichever form the chip prints.
It hides itself when there is no tone to draw.

## `<ItemsControl x:Name="PDisplayAccent" ItemTemplate="{StaticResource Theme.Accent.Display}" />`

The further pronunciations of the entry, one row each beneath the primary, drawn by the shared accent template.
The Deportment class binds the playback command on it, so a row's play button reaches this view's player.

## `<local:PReflexList x:Name="PDisplayReflex" ItemTemplate="{StaticResource Theme.Reflex.Display}" />`

The reflexes of the entry, one row each at the head of the reading stack under the headword.
The rows are drawn by the shared reflex template.
`PDisplayReflexLoading` under them is the fetching line, shown only while the engine fills the entry.
It is empty for every entry whose language declares no reflex rule and carries none.
`PDisplayReflexTable` holds the rows, the fetching line and the fold toggle in a stack no wider than the rows.
So the toggle centres under the table, as it does in the editor.

## `<ItemsControl x:Name="PDisplayTranscription" ItemTemplate="{StaticResource Theme.Transcription.Display}" />`

The transcriptions of the entry, one row each beneath the pronunciations, drawn by the shared transcription template.

## `<Border x:Name="PDisplayGlyphSection" Style="{StaticResource Theme.Pronunciation.Surface}" Visibility="Collapsed">`

The glyph row beneath the transcriptions.
It holds the scheme label in the shared label column and one chip per Han character.
The chips wrap in a panel.
The entry command bound on it from code carries a chip to this view's host.
It stays collapsed until an entry in a language with a glyph section is shown.

## `<Border x:Name="PPlayback" Style="{StaticResource Theme.Volume.Tray}">`

The volume tray sets the level every row's recording is played at, drawn bare.
It takes its shape from the theme, as the editor's tray does.
The tray hides when no pronunciation row owns a recording.
A volume with nothing to play is a control that does nothing.
The editor draws the same tray.
A recording is played the same way where it is read and where it is chosen.

## `<StackPanel x:Name="PDisplaySpeechSection" Margin="12,14,0,-4" ...>`

The parts of speech stand on a row of their own, as they do in the editor.
Sharing the pronunciation row put them beside a chip that the editor puts a button beside.

## `<StackPanel x:Name="PDisplayFrequencySection" Margin="12,14,0,0" ...>`

The frequency of the headword, one chip under the parts of speech.
It is read-only here and in the editor, because the engine fetches it and the user never types it.
The chip holds the rung name and a row of four stars, filled to the rung.
The chip carries the tooltip, so hovering anywhere on it names the source and the raw figure.
The section collapses until the engine has a value for the entry.

## `<local:PParadigm x:Name="PDisplayParadigm" Margin="0,14,0,0" />`

The inflected forms of the headword, one row each, boxed above the first meaning.
It is read-only here, because the engine fetches the forms and the editor edits them as inflection rows.
The box collapses by itself while the headword has no form to show.

## `<local:PScript x:Name="PDisplayScript" Margin="0,28,0,0" />`

The character styles of the headword, one row per character and style, boxed after the note.
It is read-only, because the engine fetches the pictures and the user never places them.
The same control sits folded in the editor, so both show one thing.
It shows the loading line while a fetch runs and collapses when nothing is stored and nothing runs.

## `<local:PFanqie x:Name="PDisplayFanqie" Margin="0,14,0,0" />`

The rime-book placements of the headword, one block per character, book and source, boxed under the paradigm box.
It is read-only like the script box, because the engine fetches the rows and the user never writes them.
The same control sits folded in the editor.
It shows the loading line while a fetch runs and collapses when nothing is stored and nothing runs.

## `<StackPanel x:Name="PCompass" Width="216" Margin="0,20,26,0" HorizontalAlignment="Right" VerticalAlignment="Top" ...>`

The floating contents stand over the reading surface rather than beside it.
A column of their own would take width from the entry.
The panel is already narrowed by the seam beside it.
Floating also lets the comparison panel carry one on each side without either half paying for it.
The reader who wants that width back folds the panel away with the toggle above it.
The toggle stays, because a fold the reader cannot undo is a feature they have lost.
The toggle stands at the left edge of the panel it opens rather than the right.
The right edge of that row belongs to the delete button every panel holding a Display puts there.
The column keeps its width while folded.
The toggle therefore does not slide under that button when the panel goes away.
The behavior lives in [LCompass](../../../Llyn.UIDeportment/Display/LCompass.comment.md).

## `<StackPanel x:Name="PDisplayStampSection" Margin="2,36,2,24">`

The entry's creation and last update times, closing the page beneath one hairline.
Two bare rows, no card, because the stamps are a footnote and not content.
The times come from the stored entry rather than the draft, since the draft carries no clock.
Each is shown in local time in the short general format of the current culture.

## `<Ellipse x:Name="PDisplayLanguageGlobe"`

The globe stands where no flag is known.
The lectern shows the flag or the globe, so no trigger here watches the image.
