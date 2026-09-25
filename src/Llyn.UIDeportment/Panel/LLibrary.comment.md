# LLibrary.cs

## `public sealed class LLibrary`

The deportment of the library panel: the shared panel state, the entry rows it browses, and the markup import.
The panel state is held rather than inherited, because a shell type deriving from logic is a custody hit.
It keeps its own handle on the vista.
A vista read off the panel would be an engine answer to branch on.
The panel's loads and clears go straight to the editor's lectern, so the veneer relays no draft.

## `public event Action<string, Exception>? LLibraryFailed;`

An import that failed, named by the key the window localizes and carrying the exception.
The engine imports every entry or none, so nothing needs undoing before it is shown.

## `public LEditor LLibraryEditor { get; }`

The entry editor's deportment, whose desk answers whether the panel may leave and opens the row that is edited.

## `public IReadOnlyList<LVistaRow> LLibraryRowsRead()`

The rows the engine returns for the vista, already filtered, sorted, numbered and marked.

## `private LIndex? _lLibraryIndex;`

The entry list the panel fills, absent until the veneer hands over its controls.
Tests attach none, so the panel reads its rows without a list to fill.

## `public void LLibraryIndexAttach(ItemsControl view, FrameworkElement empty)`

Builds the list over the veneer's controls and refills it whenever the panel announces new rows.

## `private void LLibraryIndexShow()`

Reads the rows and hands them to the list as an answered request.
The list counts what it holds, so the empty notice needs no count kept here.

## `public void LLibraryIndexSelect(object sender)`

Puts the panel on the entry of the clicked row, or on none when the sender carries no row.

## `public long LLibraryVoyageRead()`

The entry the panel shows, as the station the window records before a jump away.
Zero says no entry is shown, so there is no place to come back to.

## `private void LLibraryOrderSet(LCatalogOrder? order)`

Hands a chosen ordering to the vista, which saves and announces it.
A sender that is no order row hands null, which keeps the ordering it has.

## `private void LLibrarySieveSet(LCatalogFilter filter)`

Hands the ticked languages to the vista, which saves and announces them.

## `public void LLibraryOrderHandle(object sender, ToggleButton dropper)`

A clicked order row closes the dropdown and sets the ordering its tag carries.
The row's enum is read here, so no ordering is spelled or parsed in the veneer.

## `public void LLibrarySieveHandle(Panel list, UIElement mark)`

A clicked language row sets the filter the list now stands for, then redraws the mark at once.

## `public void LLibrarySieveShow(UIElement mark)`

Shows the mark on the sieve button while the vista hides any language.

## `public Task LLibraryPortraitPrint(LPortraitLabel label, LPressTicket ticket)`

Prints the chosen entry as the engine portrays it, with the labels the window localized.

## `public async Task LLibraryMarkupStart(`

The whole import, with each question the reader answers asked through a seam.
The path seam picks the file and answers null for a cancelled pick, which leaves the workspace as it was.
The file is read once, and the cargo it yields goes straight into the import as an argument.
Held as a parameter, the cargo is never a value this class keeps between two requests.
A failure in either hop is announced rather than shown, because the deportment holds no window.

## `private async Task LLibraryMarkupImport(`

The customs seam shows what the cargo declares before anything is written.
A seam answering null is a declined declaration, and the workspace is left untouched.
The engine then imports under the declared intakes, and the rows are re-read from what the workspace holds.
What the import could not place goes to the omission seam after the rows already hold the entries.
