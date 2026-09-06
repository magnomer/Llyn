# LEngineState.cs

## `public sealed partial class LEngine`

The shell's own view state as the engine keeps it.
The shell reads the whole state once when a window attaches, and pushes one field back each time the user changes it.
Every push is a field of its own, so two panels changing two different things in one session both keep their change.
A shell that read the state, changed a field and wrote the whole record back would lose whatever another panel had written in between.

## `public LWorkspaceState LEngineStateRead()`

Reads the stored view state of the open workspace.
It is the one call a window makes on attach, before any panel is shown.

## `public void LEngineLeftSave(string? id)`

Stores the Entry the left duplex side stands on, or nothing when that side is cleared.

## `public void LEngineRightSave(string? id)`

Stores the Entry the right duplex side stands on, or nothing when that side is cleared.

## `public void LEngineModeSave(string mode)`

Stores the name of the tab standing open.

## `public void LEngineSplitSave(bool split)`

Stores whether the open tab shows its editor rather than its read area.

## `public void LEngineOrderSave(LCatalogOrder order)`

Stores the ordering the library panel lists entries in.

## `public void LEngineSequenceSave(LCatalogOrder order)`

Stores the ordering the phonology panel lists pronunciations in.

## `public void LEngineSeriesSave(LCatalogOrder order)`

Stores the ordering the favorites panel lists favorite entries in.

## `public void LEngineFunnelSave(LCatalogOrder order)`

Stores the ordering the taxonomy panel lists tags in.

## `public void LEngineTierSave(LCatalogOrder order)`

Stores the ordering the repertoire panel lists situations in.

## `public void LEngineGradeSave(LCatalogOrder order)`

Stores the ordering the sources panel lists sources in.

## `public void LEngineRankSave(LCatalogOrder order)`

Stores the ordering the corpus panel lists examples in.

## `private void LEngineStateChange(Func<LWorkspaceState, LWorkspaceState> change)`

Reads the stored state, applies `change` to it, and writes it back, all under the engine gate.
The read and the write are one step, so a field pushed from one panel never overwrites a field pushed from another.
