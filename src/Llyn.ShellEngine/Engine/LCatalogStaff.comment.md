# LCatalogStaff.cs
Hash: `23816a2bb79aced1`

## `internal sealed record LCatalogStaff(...)`

The group of clerks for the catalog rows a card, a mention or a citation names.
It is built first, since it reads no other group.

**Parameters**

- `LCatalogStaffTag` manages tags.
- `LCatalogStaffRegister` manages registers.
- `LCatalogStaffTranslation` manages translation links.
- `LCatalogStaffReference` manages references.
- `LCatalogStaffExample` manages examples.
- `LCatalogStaffSituation` manages situations.
- `LCatalogStaffMention` manages text mentions.
- `LCatalogStaffUsage` manages usage records.
- `LCatalogStaffAuthor` manages authors.
- `LCatalogStaffFavorite` manages favorites.

## `internal static LCatalogStaff LCatalogStaffBuild(LRig rig, LLanguageCache cache, LRevisionClerk revision)`

Builds the catalog clerks over one rig.
The translation clerk stamps through the shared revision clerk, so history stays one list.
The example clerk reads through the reference clerk built beside it.
