# LEntryPort.cs

## `public interface LEntryPort`

The slice of the engine a deportment sees when it reads stored records.
It finds and reads entries, the catalog rows of every tab, and the per-entry marks a reader shows.
The marks are favourite, grasp, frequency, epithet, incoming links, mentions and glyphs.
The catalog side finds and creates tags, registers, situations, examples, references and authors.
Everything here reads or writes a committed record, never a held draft.
`LEngine` implements it today, and the entry clerk takes it over when the parts are dismantled.

## `IReadOnlyList<LVistaRow> LEngineEntryFind(LVista? parent, LVista? child);`

The entry rows of a child list narrowed by the parent catalog's choice, as the footnote and cohort lists read.

## `IReadOnlyDictionary<long, int> LEngineUsageRead(LOwner owner);`

How many places cite each record of one owner kind, keyed by record id.
