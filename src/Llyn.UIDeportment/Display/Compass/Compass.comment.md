# Compass

This folder owns the floating entry-view contents and the row objects bound to it.
Conduct supplies wording and row structure, while the driver resolves visible sections and generated card containers.

## `QCompass`

Row rebuilding waits for dispatcher priority Loaded, allowing card containers to exist before targets are resolved.
Only sections with visible local visibility are offered to Conduct.
Rows without a resolved target are omitted rather than retaining an unusable navigation entry.

The open state lives in `_qCompassOpened`, so positioning does not read the toggle's checked state.
The column needs scrolling content and more than one resolved row.
Its surface additionally requires the contents to be open.
Header overlap moves the column below the header instead of covering it.

Offsets are measured from current target geometry rather than cached across layout changes.
Hidden targets, detached targets and failed ancestor transforms supply no offset.
Navigation then leaves the scroll unchanged.

The current row is the last distinct target at or above `QCompassEdge`.
A row sharing the current target does not replace it, preserving the first row for that anchor.
When no target qualifies, the first resolved row is selected.

Row navigation and card spotlighting share the same scroll helper and `QCompassLead` offset.
Spotlighting waits for layout, resolves the card through Conduct and animates only a generated container.

## `QCompassItem`

Each row retains its target element instead of a stale position.
Name, number, indentation and muted state are supplied once during row construction.
Only current state raises property changes because scrolling updates it after construction.
An unchanged current value raises no event.

## Stamp

Commit: `c2f335c331fe`
- `QCompass.cs` `27667124d0102300`
- `QCompassItem.cs` `45e54ad36956b2ae`
