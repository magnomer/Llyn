# PDisplay.xaml

## `<UserControl.Resources>`

The read-only card shapes are merged in from [PDisplayCard.xaml](PDisplayCard.xaml.comment.md).
A Meaning card and a Collocation card are one card (LCardDraft), so the two templates share their tail.
They are kept apart from this file.
The card is its own shape, not part of the page around it.
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
The chip collapses when the entry carries no pronunciation, and the playback tray then takes its place at the margin.

## `<Border x:Name="PPlayback" Height="37" Background="Transparent" BorderThickness="0" ...>`

The playback row: the play button and the volume it is played at, drawn bare.
It wears no surface or border, so the controls read as part of the pronunciation row.
The whole tray hides when the entry owns no recording.
A volume with nothing to play is a control that does nothing.
The editor draws the same tray.
A recording is played the same way where it is read and where it is chosen.

## `<StackPanel x:Name="PDisplaySpeechSection" Margin="0,14,0,0" ...>`

The parts of speech stand on a row of their own, as they do in the editor.
Sharing the pronunciation row put them beside a chip that the editor puts a button beside.
