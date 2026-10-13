# Choice

The settings panel's choice drivers, one per setting the user ticks or picks.
Each is kept as a setting, so the next run opens with it.
It stands apart from the other panels because every driver here follows one write-back pattern.

Each driver wires its control once, through its markup name, as the settings panel is built.
Its Introduce puts the control to work on the ledger and keeps the window's envoy.
The ledger shows a failed save through that envoy.
Its Refine paints the control from the stored setting the panel hands over.
Setting `IsChecked` or a selection in code raises no user event, so a painted control writes nothing back.
Its Observe only hands the raw value to one ledger save, with the kept envoy.
A failed save raises the ledger again, which paints the control back.
The settings bulletin the engine raises after a save redraws the panel.

## `QFrequency`

The frequency switch.
Nothing already stored is fetched or dropped when it changes.
The next save or first display of an unfilled entry reads the switch, because the engine checks it per fill.

## `QInflection`

The custom analysis switch of the Inflection page.
Turning it on makes the inflection box use the pack's custom sheets and mark letters that differ from the rules.

## `QMorphology`

The morphology switch.
Turning it off cancels any fetch still in flight, and the engine does that under its own gate.
Forms already stored stay, because the switch governs asking, not keeping.
The next display of an unfilled entry reads the switch, because the engine checks it per fetch.

## `QRespelling`

The respelling switch.
Every open reading redraws in the form now picked.
Nothing is refetched and nothing stored changes, because both forms are already kept.

## `QLocalization`

The interface language box.
The language alone is handed downstream, never the whole settings record, so a geometry saved elsewhere is not overwritten.
The box starts empty, since its items wait for the first ledger state.

### `QLocalizationRefine`

The items are rebuilt whenever the ledger's list differs from what the box shows.
The observer is unhooked while the items and the selection are painted, and hooked again after.
So a selection the driver set itself raises no save.

### `QLocalizationChoiceRefine`

The items are kept when each one already matches the list by name, code and order.
Each item shows the native name and carries its language code as `Tag`.
The markup lists no language, so a new catalog file needs no edit to either side.

### `QLocalizationObserve`

The ledger state the save brings back applies the catalog.

### Inline notes

### QLocalization.cs: if (sender is not ComboBox { SelectedValue: string language })

The language is read off the box the event came from, the raw value the observer heard.
A cleared box has nothing to save.

## `QWorkspace`

The workspace folder the user picks, as a path field and its browse button.
A change to that path costs a different database.
So the form, the list and the display all move onto the new workspace.
Conduct's `CWorkspaceChange` gate owns the move, its question, its failure and its restore plan.
This driver hears the field and the button, and paints the folder the gate answers.
It finds its controls through `QContract.QContractFind` on the settings panel, keeping their markup names.

### `QWorkspace`

Escape is heard before Enter, as the panel always wired them.

### `QWorkspacePathRefine`

The panel calls it with the folder in every ledger state.

### `QWorkspaceDialogObserve`

The folder dialog is a question the gate asks, so its failure is the gate's to report.
The field then shows the folder in use, moved or not.

### `QWorkspacePathObserve`

Enter hands the raw typed path to the gate, which judges and moves.
Nothing else applies it, so a half-typed path never becomes a folder on disk.
A declined or failed move puts the folder in use back.

### `QWorkspaceEscapeRefine`

Escape puts the folder in use back into the field and asks nothing.

### `QWorkspaceFocusRefine`

Leaving the field without Enter puts the folder in use back into it.
The field therefore never shows a path that is not the workspace.

## Stamp

Commit: `c2f335c331fe`
- `QFrequency.cs` `0b63620f68a1644a`
- `QInflection.cs` `ee61dd1e41f56e82`
- `QLocalization.cs` `099a4d0f00dfe588`
- `QMorphology.cs` `ca18d5605dce8e31`
- `QRespelling.cs` `467f73e266176961`
- `QWorkspace.cs` `27b24b17e726e96f`
