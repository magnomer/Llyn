# LRealm.cs

## `public sealed record LRealm`

One realm: the identity of a workspace's database, in both the forms it takes.

`LRealmId` is the local surrogate this database assigns, which is what rows actually stamp.
`LRealmValue` is the global identity, which is what survives the trip between databases.
The pair is held together because a surrogate means nothing outside the database that assigned it.
