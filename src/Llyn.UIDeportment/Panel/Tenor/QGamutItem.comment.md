# QGamutItem.cs

## `internal sealed class QGamutItem`

One Register as a row of the tenor panel's catalog.
It carries the id the panel browses by, which is never the name, because a Register may be renamed.
It carries no language, because a Register belongs to none.
The count of cards marked with it is held as a number and shown as text.
Every find rebuilds the rows, so no field changes after the row is made.

## `internal QGamutItem(CCatalogRegister row)`

Maps one ready catalog row onto the item, field by field.
The icon key arrives chosen by Core, so the item only resolves it into an image.

## `public ImageSource QGamutItemIcon`

The register's icon, drawn at the row's full size.

## `public bool QGamutItemChosen`

Whether the catalog's vista has this row chosen, which the row template tints.
