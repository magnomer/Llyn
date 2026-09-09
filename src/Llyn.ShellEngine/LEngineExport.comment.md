# LEngineExport.cs

## `public sealed partial class LEngine`

The one entry point every export goes through.
The shell chooses a path and a format, and the engine does the rest.
No writer is reachable from the interface layer.

## `public void LEnginePressApply(LPress press)`

The shell hands in the platform's printing surface at startup.
Without it every format still works except PDF.

## `public void LEngineMarkupExport(IReadOnlyList<string> entryIds, string path)`

Writes any number of entries into one Llyn Markup document.

One file holds many entries and one catalog they share, so a set is the natural unit.
A single entry is that set with one member and needs no path of its own.
The shell has no place to ask for a set yet, and this seam is here so that it can.

**Parameters**

- `entryIds` — The entries to write, in the order the document should declare them.
- `path` — The `.llx` file to write.

## `public async Task LEnginePortraitExport(string entryId, string path, LPortraitFormat format, LPortraitLabel label)`

Markup is written from the stored draft, because it must keep what the display never shows.
The other formats are written from the portrait, so they show exactly what the panel shows.
PDF is the rendered page printed, which is why it needs no layout of its own.
An unconfigured press is an error rather than a silent empty file.

## `private string LEngineMarkupFormat(IReadOnlyList<string> entryIds)`

Loads the entries, names every row they reach, and hands the document to the writer.

The drafts are loaded through `LEntryLoader` rather than through `LEngineEntryLoad`.
The engine's own loader resolves a recording to a path inside this workspace.
Exporting that path would carry one workspace's folder into another's store.
The stored name is what the file must say, and the loader is where it is still stored.

Entry keys are derived first, so an entry is named after its headword rather than after a suffix.
The keys map every stored row id to the key the document declares it under.
The writer needs it because a draft carries stored ids and a file may name nothing outside itself.

## `private LMarkup.LMarkupCatalog LEngineCatalogCreate(IReadOnlyList<LEntryDraft> drafts, Dictionary<string, string> keys, HashSet<string> taken)`

Builds the catalog the exported entries need, and names every row in it.

Examples, situations, registers, images and videos are the rows the cards reach.
They are gathered off the drafts, because a draft already carries every field a row has.
Reading them back out of the store would ask the same question twice.

Sources and authors are taken whole from the workspace rather than reached from the cards.
Section 3 of the format spec keeps a source nothing quotes and an author credited on nothing.
A row reachable from no card is exactly the row that would be lost if reach decided.
So the file writes them all, and an import of that file loses none of them.

The kinds are named in a fixed order, and each kind's rows in a fixed order within it.
Authors are named before sources, because a source writes the key of each author it credits.
Sources are named before examples, because an example writes the key of the source it cites.
Rows of one kind are sorted by their own content, and ties broken by their stored id.
Both orders exist so that one workspace exported twice is one file twice.

An example's source citation is rewritten from a stored id to a document key here.
It is the one field of a gathered row that names another row.

## `private static LStateValue LEngineCitationRead(LStateValue citation, IReadOnlyDictionary<string, string> keys)`

Turns one stored citation into the key the document declares that row under.
An unspecified or unknown citation names no row and is carried through unchanged.
A row the document does not declare becomes unknown, because the file cannot say what it was.

## `private static void LEngineCatalogRead(LEngineCatalog harvest, IReadOnlyList<LCardDraft> cards)`

Gathers the rows the given cards cite, without repeats, sub-senses included.
A row is kept under its stored id, so a row two cards cite is gathered once.
That is what makes the file declare it once and cite it twice.

## `private static IReadOnlyList<TRow> LEngineSort<TRow>(IEnumerable<TRow> rows, Func<TRow, string> seed, Func<TRow, string> identify)`

Puts one kind of row into the order its keys are handed out in.
Content first, so a key reads as its row.
Stored id second, so two rows of one content are still named in the same order every time.

## `private sealed class LEngineCatalog`

The rows gathered from the cards, one dictionary per kind, each keyed by stored id.
It exists so that the walk over the cards has one thing to fill rather than five.
