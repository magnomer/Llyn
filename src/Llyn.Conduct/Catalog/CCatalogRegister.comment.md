# CCatalogRegister.cs
Hash: `970abba4732aab26`

## `public sealed record CCatalogRegister(CRegister CCatalogRegisterStored, int CCatalogRegisterUsage, string CCatalogRegisterIcon, bool CCatalogRegisterChosen)`

One row of a register catalog, as the tenor panel lists it.

**Parameters**

- `CCatalogRegisterStored`: the stored register, its name already shown.
- `CCatalogRegisterUsage`: how many entries carry the register.
- `CCatalogRegisterIcon`: the icon key the row wears, chosen by Core from the register's name.
- `CCatalogRegisterChosen`: whether the catalog's vista has this register chosen.
