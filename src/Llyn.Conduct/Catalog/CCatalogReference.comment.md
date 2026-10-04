# CCatalogReference.cs
Hash: `6655818475029685`

## `public sealed record CCatalogReference(long CCatalogReferenceId, string CCatalogReferenceName, string CCatalogReferenceByline, CStateWording CCatalogReferenceCredit, CStateWording CCatalogReferenceYear, int CCatalogReferenceUsage, bool CCatalogReferenceChosen)`

One reference a catalog lists, as the shelf, the Proffer dropdown and the citations read it.

**Parameters**

- `CCatalogReferenceId`: the stored reference.
- `CCatalogReferenceName`: the reference's title.
- `CCatalogReferenceByline`: the title with its authors, as a citation names it.
- `CCatalogReferenceCredit`: the authors ready to show, or the unknown or unset key in their place.
- `CCatalogReferenceYear`: the year ready to show, or the unknown or unset key in its place.
- `CCatalogReferenceUsage`: how many examples cite the reference.
- `CCatalogReferenceChosen`: whether the catalog's selection holds the reference.
