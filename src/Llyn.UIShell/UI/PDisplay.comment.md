# PDisplay.xaml

## `<UserControl.Resources>`

The read-only card shapes.
A Meaning card and a Collocation card are one card (LCardDraft).
So the two templates below differ in one element, the Collocation's Expression.
Every member the loaded draft carries is drawn.
A field the card left empty collapses rather than leaving a blank line.

## `<StackPanel x:Name="PDisplayIncomingSection">`

What other entries point at this one.
A card lists its own links, so the entry it points at would otherwise say nothing about them.
Each row opens the entry that carries it, because that is where such a link is edited.
A muted line stands in when nothing translates the entry.

## `<ToggleButton x:Name="PDisplayFavorite" Grid.Column="2" ...>`

The star stands beside the headword, so the mark is made where the entry is read.
Every entry-browsing tab carries it, because the mark belongs to the entry and not to one panel.
A filled star says the entry is marked.

## `<Grid>`

The entry as it reads, with nothing to type into.
The panel that hosts this decides what is selected and when the editor takes its place.
So the same view serves the library panel, the phonology panel and the taxonomy panel unchanged.

## `<Border x:Name="PDisplayPronunciationSurface" Style="{StaticResource Theme.Pronunciation.Surface}" ...>`

The pronunciation chip the editor also wears, defined once in the theme.
The chip collapses when the entry carries no pronunciation, and the play button then takes its place at the margin.

## `<StackPanel x:Name="PDisplaySpeechSection" Margin="0,14,0,0" ...>`

The parts of speech stand on a row of their own, as they do in the editor.
Sharing the pronunciation row put them beside a chip that the editor puts a button beside.
