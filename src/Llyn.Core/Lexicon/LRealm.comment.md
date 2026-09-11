# LRealm.cs

## `public sealed record LRealm`

One realm: the identity of a workspace's database.

`LRealmValue` is sixteen bytes minted once, when the database is made, and no other workspace mints the same.
It is the whole identity, so it means the same thing in every workspace it reaches.
No local surrogate stands beside it, because a surrogate would mean nothing outside the database that assigned it.
