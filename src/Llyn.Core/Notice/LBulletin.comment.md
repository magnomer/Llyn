# LBulletin.cs

## `public sealed record LBulletin(LSubject LBulletinSubject, string LBulletinId)`

One announcement that stored data changed.
It names the kind of record and the id of the one that changed.
It carries no content, because a subscriber re-reads what it needs through the engine.
It names no origin, because a subscriber that acted on its own change is announced to like any other.

An empty id means the kind changed as a whole rather than one named record.
That is an import, a rename across cards, or the workspace moving.
