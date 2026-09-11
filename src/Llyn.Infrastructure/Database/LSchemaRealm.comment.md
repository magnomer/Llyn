# LSchemaRealm.cs

## `public static class LSchemaRealm`

Creates the `realm` table and seeds the row standing for this workspace.

A realm is the identity of one workspace's database.
Every row a workspace creates is stamped with the realm that first created it.
Identifiers are sequential within a database, so they say nothing on their own once two databases meet.
The realm is what tells them apart, so a merge never mistakes one workspace's row for another's.

The table holds one row.
`value` is sixteen bytes no other workspace mints.
Rows stamp that value itself, so a stamp means the same thing in every workspace it reaches.
A local number in its place would be true only inside the database that assigned it.

## `public const long LSchemaRealmRow`

The id the single realm row always carries.

The table checks that no other id exists.
A second row would give one workspace two identities.

## `public static void LSchemaRealmCreate(SqliteConnection connection)`

Creates the table when it is absent and seeds the realm row when that is absent.

The seeded value is minted once and never again, because the insert names the one row and yields to the row already there.
Minting it twice would give one workspace two identities and break every stamp already written.
