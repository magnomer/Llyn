# LAuthorPort.cs
Hash: `6f38609535b6a579`

## `public interface LAuthorPort`

The slice of the engine the authors panel and the source editor see when they read or fold Authors.
`LAuthorFacade` implements it.

## `IReadOnlyList<LCatalogAuthor> LEngineRollFind(LVista? vista);`

The Authors of the authors panel's roll, and none while the vista is missing.
With no query typed, the uncredited row heads them when some Source has no Author.

## `IReadOnlyList<LCatalogAuthor> LEngineUnionFind(LVista? roll, string typed);`

The Authors the stored Author of the roll may be folded into, matched by `typed` and capped.

## `(string LUnionDropped, string LUnionKept) LEngineUnionRead(LTenure? held, long kept);`

The held draft's written Author name and the kept Author's name, as the union question shows them.

## `LVita LEngineVitaRead(LVista? roll);`

The read sheet of the Author the roll stands on, or the sheet of nobody.

## `void LEngineAuthorAbsorb(long kept, long dropped);`

Folds the Author `dropped` into `kept` and deletes the dropped row.
The credits of the dropped Author move to the kept one.
Observers hear both ids.

## `IReadOnlyList<LCatalogReference> LEngineOeuvreFind(LVista? roll, LVista? oeuvre);`

The Sources of the authors panel's oeuvre, and none while either vista is missing.

## `string LEngineWorkFormat(int count);`

An Author's work count as the roll shows it.

## `IReadOnlyList<LAuthorRow> LEngineCreditRead(LTenure? held);`

The credit rows of the held Source draft, after its deferred requests are applied.
