# Compass

This folder owns the floating entry-view contents and the row objects bound to it.
Conduct supplies wording and row structure, while the driver resolves visible sections and generated card containers.

## `QCompass`

Row rebuilding waits for dispatcher priority Loaded, allowing card containers to exist before targets are resolved.
The driver offers Conduct no section list, since Conduct holds the shown parts and lists rows for those alone.
The driver only maps each shown part to its section panel by name.
A part with no panel here, the etymology and the stamp, resolves no target and so gets no row.
Rows without a resolved target are omitted rather than retaining an unusable navigation entry.

The open state lives in `_qCompassOpened`, so positioning does not read the toggle's checked state.
The column needs scrolling content and more than one resolved row.
Its surface additionally requires the contents to be open.
Header overlap moves the column below the header instead of covering it.

Offsets are measured from current target geometry rather than cached across layout changes.
Hidden targets, detached targets and failed ancestor transforms supply no offset.
Navigation then leaves the scroll unchanged.

The current row is the one `CCompass.CCompassLitRead` names.
It reads each row's muted flag and whether its target stands at or above `QCompassEdge`.
The console contents reads the same rule, so both views light the same row.
A muted heading row is never current, and when no target qualifies the first unmuted row is.

Row navigation and card spotlighting share the same scroll helper and `QCompassLead` offset.
Spotlighting starts from the compass's sought event, which Conduct raises with the part and the card place.
It waits for layout and animates only a generated container.
The driver no longer resolves the card, so a stale id finds no place and moves nothing.
The lectern's see notice and the Library reach it through the same event.

## `QCompassItem`

Each row retains its target element instead of a stale position.
Name, number, indentation and muted state are supplied once during row construction.
Only current state raises property changes because scrolling updates it after construction.
An unchanged current value raises no event.

## Stamp

Commit: `36e31eb71c98`
- `QCompass.cs` `c645021bb1068ef7`
- `QCompassItem.cs` `45e54ad36956b2ae`
