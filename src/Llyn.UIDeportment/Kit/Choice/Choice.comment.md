# Choice

This folder holds shared filter and ordering drivers, with their row building and reading in `QChoice`.
The drivers keep picker wiring apart from browse panels and pass choices to their supplied aperture.

## `QChoiceFilter`

The filter menu is anchored to its own button, keeping placement separate from the ordering picker's bar anchor.
The aperture is introduced separately from construction, so the placed picker can be wired before its owner supplies state.
Rows wait for an explicit build call with languages or reference kinds.
A click reads the whole sibling checkbox list, then refreshes the mark from the aperture's answer.
The mark follows `CApertureFiltered`, rather than deciding whether a filter matters locally.

## `QChoiceOrder`

The caller supplies the bar anchor, letting the menu align with more than the dropper button.
The introduced title supplies both the tooltip key and row label prefix.
A click passes the parsed order to the aperture and unchecks the dropper, closing the menu.
Refresh checks the aperture's current ordering, keeping restored state separate from click handling.

## `QChoice`

Ordering words tag the rows and form the final component of their label keys.
The supplied ordering list owns which choices exist and their order.
Order rows need no explicit group name because they share one parent panel.
Kind menu rows use a group name separately from ordering rows.

### `QChoiceDropperAttach`

The popup opens on the press, not the release.
Mouse capture keeps it open until the dropper releases the mouse.
The handled release avoids a second toggle from the button's normal click path.
After capture is lost, the popup can close on an outside click.
The popup's close event unchecks the dropper, keeping both states aligned.

### `QChoiceOrderApply`

No row is marked when the supplied order's word is absent from the popup.

### `QChoiceFilterBuild`

Language and kind rows take their checked state from the supplied filter, keeping hidden-value decisions outside row construction.
Language badges use a ring when no flag image is available.

### `QChoiceFilterRead`

Every unchecked or indeterminate checkbox contributes its string tag to the hidden list.
The clicked box's parent supplies the rows, so no separate view field feeds the aperture.

### `QChoiceOrderRead`

An unknown string tag yields the ordering enum's default through the dictionary lookup.
A sender that is not a tagged radio button yields null instead.

### `QChoiceButtonScan`

Walking the logical tree keeps callers independent of the border and panel wrappers surrounding option rows.

## Stamp

Commit: `c2f335c331fe`
- `QChoice.cs` `bb3bf58d7960e558`
- `QChoiceFilter.cs` `12cb4ec034881da9`
- `QChoiceOrder.cs` `db7a0d6fc08dfa63`
