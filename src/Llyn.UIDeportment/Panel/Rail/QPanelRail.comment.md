# QPanelRail.cs
Hash: `5c8013bac30477c3`

## `internal sealed class QPanelRail`

Drives the shared command rail `PPanelRail` inside whichever entry panel holds it.
It owns the rail's icons, visibility and button states, so no panel repeats them.
The entry clicks leave as notices, because only the owning panel knows its gates.
The trail and chronicle clicks need no panel, so the rail answers them itself.

## `internal QPanelRail(UserControl rail, Button bin, QIconImage binIcon, bool fresh, bool portrait)`

Takes the placed rail and the panel's bin, which stands over the entry and not on the rail.
It points export and print at their routed commands, so the owner's command bindings decide them.
It sets every icon and subscribes every click.
`fresh` shows the new-record button and `portrait` shows the export button.

## `internal event Action? QPanelRailCreated`

Raised when the new-record button is clicked.

## `internal event Action? QPanelRailStored`

Raised when the store button is clicked.

## `internal event Action<bool>? QPanelRailToggled`

Raised when a mode button is clicked, with true for the editor and false for the reader.

## `internal event Action? QPanelRailDeleted`

Raised when the bin is clicked.

## `internal void QPanelRailIntroduce(CNavigation navigation, QChronicleHost host)`

Hands the rail the navigation whose trail it steps and the editor whose chronicle it walks.
It subscribes the navigation's changes, so the trail buttons light without the window's help.

## `internal void QPanelRailRefine(bool scribe, bool modeEnabled, bool binEnabled)`

Paints the mode the owner reads from its shared panel state.
The trail pair shows with the reader and the chronicle pair with the editor.

## `internal void QEntryStorableRefine(bool storable)`

Lights the store button while the draft can be stored.

## `internal void QChronicleRefine(bool undo, bool redo)`

Lights each chronicle button only while the editor has a step to walk that way.

## `private void QVoyageRefine(CNavigationState state)`

Lights the two trail buttons from the voyage state the navigation raises.
The navigation owns the trail, so the rail only shows what it is told.
