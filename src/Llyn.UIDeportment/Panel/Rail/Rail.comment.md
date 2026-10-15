# Rail

The command rail shared by the entry panels.
It stands apart so no panel repeats the rail's icons, visibility and button states.

## `QPanelRail`

Drives the shared command rail `PPanelRail` inside whichever entry panel holds it.
The entry clicks leave as notices, because only the owning panel knows its gates.
The trail and chronicle clicks need no panel, so the rail answers them itself.
The constructor takes the placed rail and the panel's bin, which stands over the entry and not on the rail.
It points export and print at their routed commands, so the owner's command bindings decide them.
It sets every icon and subscribes every click.
`fresh` shows the new-record button and `portrait` shows the export button.
`QPanelRailCreated` is raised by the new-record button and `QPanelRailStored` by the store button.
`QPanelRailToggled` is raised by a mode button, with true for the editor and false for the reader.
`QPanelRailDeleted` is raised by the bin.
`QPanelRailIntroduce` hands the rail the navigation whose trail it steps and the editor whose chronicle it walks.
It subscribes the navigation's changes, so the trail buttons light without the window's help.
`QPanelRailRefine` paints the mode the owner reads from its shared panel state.
The trail pair shows with the reader and the chronicle pair with the editor.
`QEntryStorableRefine` lights the store button while the draft can be stored.
`QChronicleRefine` lights each chronicle button only while the editor has a step to walk that way.
`QVoyageRefine` lights the two trail buttons from the voyage state the navigation raises.
The navigation owns the trail, so the rail only shows what it is told.

## Stamp

Commit: `2f9a06b8eb95`
- `QPanelRail.cs` `c84dd564c308a7aa`
