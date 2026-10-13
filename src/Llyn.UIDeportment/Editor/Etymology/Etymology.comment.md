# Etymology

Etymology holds the editor's driver for the etymology field.
It drives the whole field, chips and prose, because the field draws both sides.
Every change goes through one card gate.

## `QEtymologyEditor`

Resolution goes through the prospect menu as a translation does, and the engine trims the typed word.
The mention pick is bound on the field itself, so only a pick anchored at the narrative box reaches it.

### `QEtymologyRefine`

The draft holds ids alone, so the engine resolves the source links.
Each link becomes a chip item here, so the field holds no Conduct record.
A link whose target could not be read is left out rather than drawn blank.

### `QEtymologyMentionRefine`

The spans are painted as ready chips after the narrative and its sources.
It answers the same draft bulletin.

### `QEtymologyWriteObserve`

The narrative is deferred like every other typed field, so one keystroke is not one request.

### `QEtymologyAddRefine`

Only the entry's Return key raises the command, so its source is the entry's box.
The offer is read from that box's text, not from the command parameter.
The pick is heard by `QProspectPickObserve`, which finds the entry from the box the menu stands at.

### `QEtymologyPickObserve`

The box is the command's source and the anchor the menu stood at.
Its text and selection are read raw.
The parameter is the raw nullable id, and the gate decides what null means.

### `QEtymologyLinkCheck`

Linking needs words under the selection, not only spaces.
The mention span verdict owns that blank rule.

### `QEtymologyUnlinkCheck`

Unlinking needs a span the selection lies inside.

## Stamp

Commit: `c2f335c331fe`
- `QEtymologyEditor.cs` `5c4288f7bf9d2410`
