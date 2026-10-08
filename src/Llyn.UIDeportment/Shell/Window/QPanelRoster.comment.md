# QPanelRoster.cs
Hash: `bdde7d16914ce742`

## `internal sealed class QPanelRoster`

The window's fourteen panels and the shared widths of their seams.
It builds each panel driver over its markup, wires it, and stops it when the window closes.
It takes facets only, never `QWindow`, so no panel reaches the window through it.

## `internal QPanelRoster(Window window, QPosture posture)`

Keeps the loaded window whose name scope holds every panel.
Builds the layout over the posture, then one driver per panel.
Each driver is built here and wired only when the roster introduces it.
The settings panel takes the layout and the posture, since it resets the widths.

## `private readonly Window _qPanelRosterSurface;`

The loaded window whose name scope holds every panel.

## `private readonly QLayout _qLayout;`

The one owner of the seam widths, shared by every seamed tab and by the settings panel.

## `private readonly QInput _qInput;`

Drives the input panel, pulled from the window under the contract ID `PInput`.

## `private readonly QTaxonomy _qTaxonomy;`

Drives the taxonomy panel, pulled from the window under the contract ID `PTaxonomy`.

## `private readonly QTenor _qTenor;`

Drives the tenor panel, pulled from the window under the contract ID `PTenor`.

## `private readonly QRepertoire _qRepertoire;`

Drives the repertoire panel, pulled from the window under the contract ID `PRepertoire`.

## `private readonly QCorpus _qCorpus;`

Drives the corpus panel, pulled from the window under the contract ID `PCorpus`.

## `private readonly QReference _qReference;`

Drives the source panel, pulled from the window under the contract ID `PReference`.

## `private readonly QLibrary _qLibrary;`

Drives the library panel, pulled from the window under the contract ID `PLibrary`.

## `private readonly QPhonology _qPhonology;`

Drives the phonology panel, pulled from the window under the contract ID `PPhonology`.

## `private readonly QXiesheng _qXiesheng;`

Drives the Xiesheng panel, pulled from the window under the contract ID `PXiesheng`.

## `private readonly QYunjing _qYunjing;`

Drives the Yunjing panel, pulled from the window under the contract ID `PYunjing`.

## `private readonly QFavorite _qFavorite;`

Drives the favorites panel, pulled from the window under the contract ID `PFavorite`.

## `private readonly QDuplex _qDuplex;`

Drives the duplex panel, pulled from the window under the contract ID `PDuplex`.

## `private readonly QGuild _qGuild;`

Drives the guild panel, pulled from the window under the contract ID `PGuild`.

## `private readonly QSettings _qSettings;`

Drives the settings panel, pulled from the window under the contract ID `PSettings`.

## `internal void QPanelRosterIntroduce(CAtelier atelier, CEnvoy envoy, QVolume volume, QMentionMenu mentionMenu)`

Introduces every panel with only the facets it uses.
Then each panel's restore answers `CWorkspaceOpened`, and each duplex wing subscribes its own.
The widths answer `CWorkspaceOpened` too, after every panel.
They reset first, then apply the stored widths of the workspace opened.
The window subscribes the posture's root before this runs, so every reload reads the new workspace.

## `internal void QPanelRosterAttach()`

Registers the root grid of every tab that has a seam, under the name its widths are stored by.
Each panel wears its loaded markup as a `UserControl`, so the grid is that surface's content.
The duplex panel is left out.
Its two halves are editors sharing the window, not a catalog beside a display.
The stored widths are applied by the open that follows, before any tab is shown, so nothing jumps.

## `internal void QPanelRosterClose()`

Stops every panel that holds something to stop when the window has closed.
The duplex and settings panels are not stopped here.
The window calls it before the session ends, so every panel stops before the engine goes.

## `private UserControl QInputSurface`

The named panels are read through the window's name scope.
A panel already moved to the veneer is pulled by its contract ID as a plain page.
