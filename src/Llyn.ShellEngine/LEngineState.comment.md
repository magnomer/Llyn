# LEngineState.cs

## `public sealed partial class LEngine`

The shell's own view state as the engine keeps it.
The shell reads the whole state once when a window attaches.
It pushes one field back each time the user changes it.
Every push is a field of its own.
Two panels changing two different things in one session both keep their change.
A shell that read the state, changed a field and wrote the whole record back would lose work.
Whatever another panel had written in between would be gone.

The Entry each duplex side stands on names a row of the workspace database, so it is stored beside it.
The open tab, its split, each panel's ordering and its hidden languages name only panels.
Those go to the settings file, one per tab in its layout record, through the same one-field pushes.
The signatures stay one per panel, so a panel never learns which file its choice lands in.

## `private const string LEngineStateLibrary = "library";`

The lowercase tab names the layout records are keyed by, one per browse panel.
They match the names the shell attaches its panel widths under.
So a panel's ordering, filter and widths share one record.

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

## `public void LEngineDegreeSave(LCatalogOrder order)`

Stores the ordering the tenor panel lists registers in.

## `public void LEngineTierSave(LCatalogOrder order)`

Stores the ordering the repertoire panel lists situations in.

## `public void LEngineGradeSave(LCatalogOrder order)`

Stores the ordering the sources panel lists sources in.

## `public void LEngineRankSave(LCatalogOrder order)`

Stores the ordering the corpus panel lists examples in.

## `public void LEngineEchelonSave(LCatalogOrder order)`

Stores the ordering the authors panel lists authors in.

## `public void LEngineLadderSave(LCatalogOrder order)`

Stores the ordering the yunjing panel lists onsets in, under the panel's own record.

## `public void LEngineStairSave(LCatalogOrder order)`

Stores the ordering the yunjing panel lists rimes in.
The rime column has a record of its own, `yunmu`, since one record holds one ordering.

## `public void LEngineSieveSave(LCatalogFilter filter)`

Stores the languages the library panel hides.

## `public void LEngineLensSave(LCatalogFilter filter)`

Stores the languages the phonology panel hides.

## `public void LEngineStrainerSave(LCatalogFilter filter)`

Stores the languages the favorites panel hides.

## `public void LEngineLatticeSave(LCatalogFilter filter)`

Stores the languages the taxonomy panel hides from the entries of the chosen tag.

## `public void LEngineGrilleSave(LCatalogFilter filter)`

Stores the languages the tenor panel hides from the entries of the chosen register.

## `public void LEngineMeshSave(LCatalogFilter filter)`

Stores the languages the repertoire panel hides from the entries referencing the chosen situation.

## `public void LEngineGauzeSave(LCatalogFilter filter)`

Stores the languages the corpus panel hides from the entries quoting the chosen example.

## `public void LEngineTrellisSave(LCatalogFilter filter)`

Stores the languages the sources panel hides from the entries citing the chosen source.

## `public void LEngineLouverSave(LCatalogFilter filter)`

Stores the kinds the authors panel hides from the sources crediting the chosen author.

## `private void LEngineStateChange(Func<LWorkspaceState, LWorkspaceState> change)`

Reads the stored state, applies `change` to it, and writes it back, all under the engine gate.
The read and the write are one step.
A field pushed from one panel never overwrites a field pushed from another.
