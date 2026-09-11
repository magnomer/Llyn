# LSchemaRealm.cs

## `public static class LSchemaRealm`

Creates the `realm` table and seeds the row standing for this workspace.

A realm is the identity of one workspace's database.
Every row a workspace creates is stamped with the realm that first created it.
Identifiers are sequential within a database, so they say nothing on their own once two databases meet.
The realm is what tells them apart, so a merge never mistakes one workspace's row for another's.

The table holds two columns because a realm has two forms.
`value` is the global form: sixteen bytes no other workspace mints.
`id` is the local form: a small number this database alone assigns.
Rows stamp the local form, so the stamp costs a byte or two rather than sixteen.

## `public const long LSchemaRealmOwn`

The local surrogate this workspace's own realm always takes.

Row one is reserved for it and seeded when the database is made.
Imported realms take the numbers after it, in the order they arrive.

## `public static void LSchemaRealmCreate(SqliteConnection connection)`

Creates the table when it is absent and seeds the own-realm row when that is absent.

The seeded value is minted once and never again, because the insert yields to the row already there.
Minting it twice would give one workspace two identities and break every stamp already written.
