# Pattern

This folder holds the stencil gates: the side template editor of the chosen deck.

## `CStencil`

The template editor of the chosen deck, built by `CDeck` beside the editor.
It reads the chosen deck from `CDeck` at each call, as `CCue` does.
Each slot set saves at once, so the editor keeps no draft and asks no leave question.
`CStencilChanged` tells the driver to read the template again, as `CEtymologyChanged` does.
A read failure shows `Deck.LoadFailed`, and a change failure shows `Stencil.SaveFailed`.
`CStencilSave` asks the name through the coinage question under `Coinage.Stencil`.
  A declined name saves nothing.
  A save raises `CStencilChanged`, because the saved list in the state grows.
`CStencilApply` copies a saved template's slots into the deck's own template.
`CStencilRead` reads the deck's own template, the saved templates and the deck's grain, so the driver can offer them.
  The reads share one failure notice, and a failure returns a state with every list empty.
`CStencilRead` offers every slot the export writes, filled from the deck's own template.
  The offer follows the export's column order.
  Each of the five sides offers Text, Picture and Sound, then come Notes, Extra Info and four categories.
  A slot the deck does not store reads an empty body.
  A stored slot outside the offer is not listed.
  So the driver paints the rows it is given and builds no slot itself.
`CStencilFieldAppend` and `CStencilFieldRemove` add a field chip to a side's face and drop one by its place.
`CStencilJoinSet` sets a chip's join by side and place to a choice in the state's join list.
  `CStencilFieldAppend` and `CStencilJoinSet` refuse a choice outside their lists.
  `CStencilFieldRemove` passes its place to the engine unchecked.
  The three save at once like a slot set.
`CStencilTextToggle` flips `CStencilTextChecked`, which shows the slot rows as text, and raises `CStencilChanged`.
`CStencilMediaSet` picks the side that holds the picture or the sound, by column.
  `CStencilPictureSet` and `CStencilSoundSet` fix the column, so the driver names no column number.
With no deck chosen it reads null, and the driver collapses the editor, as `CCharterRead` does.

## `CStencilState`

The chosen deck's template as the driver shows it.
`CStencilStateSlots` lists the offered slots with the deck's body in each.
`CStencilStateSaved` lists every saved template by id and name, in the engine's order.
`CStencilStateSides` lists the five card sides.
`CStencilStateFields` lists the fields a chip may add, and `CStencilStateJoins` the two joins.
`CStencilStatePicture` and `CStencilStateSound` are the side that holds each medium, zero for none.
`CStencilStateGrain` is the deck's grain as a wording key, empty when the deck is not found.

## `CStencilSlot`

One slot of the template as the driver shows it.
The column and index are the numbers `CStencilSlotSet` takes back, and the body is the deck's text.
The caption is a wording key, and the number is the index as text, empty for Notes and Extra Info.
  Conduct chooses both, so the driver composes no label.

## `CStencilSide`

One card side as the driver shows it, by index.
A side built from field tokens carries a `CStencilChip` per field, in face order.
A side whose body holds other text is custom and carries no chips.
  So the driver shows no chip editor for it.

## `CStencilChip`

One field chip of a side.
It carries its caption key and whether its join is the all choice.

## `CStencilField`

One field a chip may add, by its caption key.
The driver adds it by its place in the state's list.

## `CStencilJoin`

One join choice, by its caption key and its value, false for the first and true for the all choice.

## `CStencilRow`

One saved template by id and name.
The driver hands the id back to `CStencilApply`.

## Stamp

Commit: `2f9a06b8eb95`
- `CStencil.cs` `97235d4ac96b91dc`
- `CStencilChip.cs` `b6c58dc5500080c5`
- `CStencilField.cs` `3f58ce3434ac694a`
- `CStencilJoin.cs` `b413d4396927e6a1`
- `CStencilRow.cs` `e29bb2f86dfc726e`
- `CStencilSide.cs` `1a0f78eafa4cfbe8`
- `CStencilSlot.cs` `a30bf1a5149e16dc`
- `CStencilState.cs` `817255b845192fda`
