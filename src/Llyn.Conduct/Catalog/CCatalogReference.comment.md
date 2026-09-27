# CCatalogReference.cs

## `public sealed record CCatalogReference(`

One reference a catalog lists, as the shelf, the candidates and the citations read it.

**Parameters**

- `CCatalogReferenceId`: the stored reference.
- `CCatalogReferenceName`: the reference's title.
- `CCatalogReferenceByline`: the title with its authors, as a citation names it.
- `CCatalogReferenceCredit`: the authors, uncertain when they are unknown.
- `CCatalogReferenceYear`: the year, uncertain when it is unknown.
- `CCatalogReferenceUsage`: how many examples cite the reference.
- `CCatalogReferenceChosen`: whether the catalog's selection holds the reference.
