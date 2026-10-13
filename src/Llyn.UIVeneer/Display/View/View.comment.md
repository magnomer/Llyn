# View

This folder holds the read-only entry view that the browse panels place.
The panel that hosts it decides what is selected and when the editor takes its place.
So the same view serves the library, phonology and taxonomy panels unchanged.
No element here names a handler, a command or a binding.
`QDisplay` and the lectern's sections pull each named part by contract ID and wire it.

## `PDisplay`

The C# shell only calls `InitializeComponent()`.
The card shapes merge from [PDisplayCard.xaml](../Template/PDisplayCard.comment.md), the incoming rows and stamps from [PDisplayUsage.xaml](../Template/PDisplayUsage.comment.md), and the contents row from [PDisplayCompass.xaml](../Compass/PDisplayCompass.comment.md).
The merge is declared here, so the list reads the row template dynamically.
A Meaning and a Collocation are one card, so the meaning card and the collocation row share their body.
A field the card left empty collapses rather than leaving a blank line.

## Inline notes

### PDisplay.xaml: `PDisplayIncomingSection`

It lists what other entries point at this one, because a card lists only its own links.
Each row opens the entry that carries it, because that is where such a link is edited.
The heading says "Links here", which names the direction without claiming the source entry is a translation.
Each source is a standalone link card with a direction icon and the source headword leading.
The referring Meaning or Collocation is a compact badge.
The whole section collapses when no entry links here.

### PDisplay.xaml: `Theme.Favorite.Row`

The heart, the stars and their words take their shape from the theme.
So the two views cannot place them apart.

### PDisplay.xaml: `PDisplayFavorite`

The heart stands beside the headword, so the mark is made where the entry is read.
Every entry-browsing tab carries it, because the mark belongs to the entry and not to one panel.
The icon is named `PFavoriteMark`, so the toggle's `QLook` rows fill it hollow or filled.

### PDisplay.xaml: `PDisplayGrasp`

The star row shows how well the user says they know the word, and a click sets it there.
It draws the stored step alone, never a value the store has not confirmed.
The words after it say what the shown step means, and follow the pointer while it hovers.

### PDisplay.xaml: `PDisplayEmpty`

It is the prompt shown while no entry is chosen.
`QLectern` shows either this or `PDisplayContents`, never both.
The contents start collapsed, so a fresh view shows the prompt.

### PDisplay.xaml: `PDisplayReading`

The line under the headword holds the reading taken from the rime-book placements.
It stays empty until those are fetched, and an empty reading takes no room.

### PDisplay.xaml: `PDisplayPronunciationSurface`

It is the pronunciation chip the editor also wears, defined once in the theme.
It shows the primary pronunciation's variety before the brackets and play after them.
The brackets are named so the lectern can turn them into slashes for a phonemic respelling.
Play shows only while the primary pronunciation owns a recording still on disk.
The editor's lookup, download and plus-minus buttons stand after it, drawn hidden.
They keep the room the editor's row takes, so the volume tray stands at one x in both views.
The chip collapses when the entry carries no pronunciation.

### PDisplay.xaml: `PDisplayContour`

It draws the tone contour of the primary pronunciation when the entry's language is tonal.
`QLecternAccent` hands it the syllables of the shown contour, whichever form the chip prints.
It hides itself when there is no tone to draw.

### PDisplay.xaml: `PDisplayAccent`

It lists the further pronunciations, one row each beneath the primary, by the shared accent template.
`QLecternPlayback` binds the playback command on it, so a row's play button reaches this view's player.

### PDisplay.xaml: `PDisplayReflex`

It lists the reflexes, one row each at the head of the reading stack, by the shared reflex template.
The list sits in a `QReflexFrame`, which reports only its fields and buttons to accessibility.
`PDisplayReflexLoading` is the fetching line, shown only while the engine fills the entry.
It is empty for every entry whose language declares no reflex rule.
`PDisplayReflexTable` holds the rows, the fetching line and `PDisplayReflexHinge` in a stack no wider than the rows.
So the toggle centres under the table, as in the editor.

### PDisplay.xaml: `PDisplayTranscription`

It lists the transcriptions, one row each beneath the pronunciations, by the shared transcription template.

### PDisplay.xaml: `PDisplayGlyphSection`

It holds the scheme label in the shared label column and one chip per Han character.
The chips wrap in a panel.
The entry command bound on it from code carries a chip to this view's host.
It stays collapsed until an entry in a language with a glyph section is shown.

### PDisplay.xaml: `PPlayback`

The volume tray sets the level every row's recording is played at, and takes its shape from the theme.
It hides when no pronunciation row owns a recording, since a volume with nothing to play does nothing.
The editor draws the same tray, so a recording plays the same way where it is read and chosen.

### PDisplay.xaml: `PDisplaySpeechSection`

The parts of speech stand on a row of their own, as in the editor.
`PDisplayUnit` names the lexical unit above them, collapsed until a unit is chosen.
`PDisplayFreedom` follows it on the same line, collapsed until a freedom is chosen.
Sharing the pronunciation row put them beside a chip that the editor puts a button beside.

### PDisplay.xaml: `PDisplayFrequencySection`

It shows the headword's frequency as one chip under the parts of speech.
It is read-only here and in the editor, because the engine fetches it.
The chip holds the rung name and four stars filled to the rung.
The chip carries the tooltip, so hovering anywhere on it names the source and the raw figure.
The section collapses until the engine has a value for the entry.

### PDisplay.xaml: `PDisplayParadigm`

It boxes the inflected forms above the first meaning.
It is read-only here, because the engine fetches the forms and the editor edits them as inflection rows.
The box collapses by itself while the headword has no form to show.

### PDisplay.xaml: `PDisplayScript`

It boxes the character styles of the headword, one row per character and style, after the note.
It is read-only, because the engine fetches the pictures.
The same control sits folded in the editor and here.
Its opening is the entry's stored state, which both modes read and write.
It shows the loading line while a fetch runs and collapses when nothing is stored and nothing runs.

### PDisplay.xaml: `PDisplayFanqie`

It boxes the rime-book placements, one block per character, book and source, under the paradigm box.
Fetched placements are displayed here, but their category and representative controls can still request changes.
The same control sits folded in the editor and here, with its opening shared as stored state.
It shows the loading line while a fetch runs and collapses when nothing is stored and nothing runs.

### PDisplay.xaml: `PCompass`

The floating contents stand over the reading surface, because a column of their own would take width from the entry.
Floating also lets the comparison panel carry one on each side without either half paying for it.
The toggle stays above it, because a fold the reader cannot undo is a feature they have lost.
It stands at the left edge, because the right edge belongs to the delete button every Display panel carries.
The column keeps its width while folded, so the toggle does not slide under that button.
The behaviour lives in [QCompass](../../../Llyn.UIDeportment/Display/QCompass.comment.md).

### PDisplay.xaml: `PDisplayCollocationSection`

It lists collocations that link to no meaning, under the title `Display.CollocationOther`.
Linked collocations show inside their home meaning, so this section lists only the rest.
They are the same folding rows as under a meaning, in one bordered list.
It collapses when every collocation is linked.

### PDisplay.xaml: `PDisplayStampSection`

It shows the creation and last update times, closing the page beneath one hairline.
They are two bare rows, no card, because the stamps are a footnote.
The times come from the stored entry rather than the draft, since the draft carries no clock.
Each is shown in local time in the short general format of the current culture.

### PDisplay.xaml: `PDisplaySwath`

The drag band shares the page's grid cell and lies on top of it.
It sits inside the scroll viewer, so it scrolls and clips with the text it highlights.
`QDisplay` attaches it to `PDisplayContents`, so a press anywhere on the page reaches it first.

### PDisplay.xaml: `PDisplayLanguageGlobe`

The globe stands where no flag is known.
The lectern shows the flag or the globe, so no trigger here watches the image.

## Stamp

Commit: `c2f335c331fe`
- `PDisplay.xaml` `9187c7b59e6b8a37`
- `PDisplay.xaml.cs` `bc75ac1cd4702294`
