# LEntryPort.cs

## `public interface LEntryPort`

The slice of the engine a deportment sees when it reads stored records.
It finds and reads entries, the catalog rows of every tab, and the per-entry marks a reader shows.
The marks are favourite, grasp, frequency, epithet, incoming links, mentions and glyphs.
The catalog side finds and creates tags, registers, situations, examples, references and authors.
The pure facts a panel draws with travel here too: the grasp step and wording, twin names and markdown blocks.
Everything here reads or writes a committed record, never a held draft.
`LEngine` implements it today, and the entry clerk takes it over when the parts are dismantled.

## `IReadOnlyList<LVistaRow> LEngineEntryFind(LVista? parent, LVista? child);`

The entry rows of a child list narrowed by the parent catalog's choice, as the footnote and cohort lists read.

## `IReadOnlyList<LGlyphCell> LEngineGlyphDivide(LEntryDraft draft);`

The cells of the draft's glyph row, empty when its language declares no glyph section.

## `IReadOnlyDictionary<long, int> LEngineUsageRead(LOwner owner);`

How many places cite each record of one owner kind, keyed by record id.

## `IReadOnlyList<LCatalogAuthor> LEngineRollFind(LVista? vista);`

The Authors of the authors panel's roll, headed by the uncredited row, and none while the vista is missing.

## `IReadOnlyList<LCatalogAuthor> LEngineUnionFind(LVista? roll, string typed);`

The Authors the stored Author of the roll may be folded into, matched by `typed` and capped.

## `(string LUnionDropped, string LUnionKept) LEngineUnionRead(LTenure? held, long kept);`

The held draft's written Author name and the kept Author's name, as the union question shows them.

## `LVita LEngineVitaRead(LVista? roll);`

The read sheet of the Author the roll stands on, or the sheet of nobody.

## `IReadOnlyList<LCatalogReference> LEngineOeuvreFind(LVista? roll, LVista? oeuvre);`

The Sources of the authors panel's oeuvre, and none while either vista is missing.

## `string LEngineWorkFormat(int count);`

An Author's work count as the roll shows it.

## `LColophon LEngineColophonRead(LDraft draft);`

The read sheet of a Source draft, with the citation line of the Source it holds.

## `LImprint LEngineImprintRead(LDraft? draft);`

The edit sheet of a Source draft, or of a blank Source with no draft.

## `IReadOnlyList<LAuthorRow> LEngineCreditRead(LTenure? held);`

The credit rows of the held Source draft, after its deferred requests are applied.

## `string LEngineTallyRead(long? reference);`

The citation line of one Source, worded as none when no Source is given.

## `IReadOnlyList<LCatalogReference> LEngineReferenceFind();`

Every Source as the whole shelf lists it, ordered by author.

## `IReadOnlyList<LCatalogReference> LEngineCitationFind(long draftId, string word);`

The Sources a citation field offers for the typed word, and none for a blank word.
A word already naming the byline the held draft's Example cites offers none.

## `static bool LEngineTextMatch(string field, string shown)`

Whether a field showing `field` already shows the text `shown`, a blank field reading as nothing recorded.
It is static, since it reads no record and a Conduct verdict holds no port for it.
