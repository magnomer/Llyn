# Pattern

The drivers of the stencil editor.

## `QStencil`

Drives the template editor `PStencil` of the chosen deck, copying `QEtymology` and `QCharter`.
`CDeckChanged` and `CStencilChanged` repaint it from one `CStencilRead`, and a null state collapses it.
Each slot row is a `QStencilItem` holding one `CStencilSlot`.
  The splice keeps the held rows while column and index match, and only swaps their slot.
  So a body edit repaints in place, and the box keeps its focus and caret.
  The caption takes the slot's key as a resource reference, and the number shows the slot's number.
A body edit hands the slot's column and index and the box text to `CStencilSlotSet`.
  The handler is detached while a row paints, so painting sends no setter.
The save button calls `CStencilSave`, and the gate asks the name.
The saved templates fill a choice box of `QStencilChoice` rows.
  The apply button hands the chosen row's id to `CStencilApply`, and with no choice it sends nothing.
  Applying from the button, not from the selection, keeps the repaint out of the box's own selection event.
Each card side is a `QStencilSide` holding one `CStencilSide`, spliced in place by its index.
  A custom side shows its mark and no chips, and the other shows its chips and the field box.
  The field box offers every `CStencilField` by its key.
  A choice hands the side and the offer's index to `CStencilFieldAppend`.
  A chip's remove button hands the side and the place to `CStencilFieldRemove`.
  A chip's join box hands the side, the place and the choice to `CStencilJoinSet`.
The picture and sound boxes offer none and each side number, and a choice goes to `CStencilMediaSet`.
The text toggle calls `CStencilTextToggle`, and the slot rows show only while it is checked.
The grain line shows `CStencilStateGrain`.

## `QStencilItem`

One painted slot row of the template editor.
Its slot changes in place and notifies, so the row repaints.

## `QStencilSide`

One painted card side, holding its `CStencilSide` with the field and join offers.
Its row changes in place and notifies, so the side repaints.
It lists its chips as `QStencilChip` rows, each with its side and place.

## `QStencilChip`

One field chip of a side, holding its `CStencilChip`, its side, its place and the join offers.
It never changes, so a repaint of the side replaces it.

## `QStencilChoice`

One saved template in the choice box, holding its `CStencilRow` as `QEtymologyChoice` holds its choice.
It shows the template's name.

## Stamp

Commit: `121691f60459`
- `QStencil.cs` `b5e40e28c0c534f3`
- `QStencilChip.cs` `11064b09a5cae31e`
- `QStencilChoice.cs` `6649f5c85b7533e1`
- `QStencilItem.cs` `b9b9d7221a2189b5`
- `QStencilSide.cs` `05f49f888365e813`
