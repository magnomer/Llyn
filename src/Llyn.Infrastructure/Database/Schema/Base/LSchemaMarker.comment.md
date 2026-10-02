# LSchemaMarker.cs
Hash: `7f186069510a11a3`

## `public static class LSchemaMarker`

Creates the sets a card attaches to itself.
They are its Tags, its translation links, and the independent Situations it reaches.

## `public static void LSchemaMarkerCreate(SqliteConnection connection)`

A Tag is one shared row whose text is unique.
The association row links a card to it by `tag_ref` and holds the position.
The primary key on (card, tag) stops a card carrying the same Tag twice.
The unique index on (card, position) keeps the order free of duplicates.
The association rows cascade with the card.
The `tag_ref` foreign key has no cascade, so the Tag row survives every detach.
The translation tables beside them are the links a card carries to another Entry.
A link stores the target's id and never its text, so it is one card, one entry, one position.
The primary key on (card, entry) stops a card linking the same Entry twice.
The entry_ref cascade is deliberate, since deleting a target Entry drops the links pointing at it.
A Situation is shared in the same way.
It carries its own opaque id, and its visible data is never identity.
It is reached only through the association tables below it.
Their situation_ref foreign keys deliberately have no cascade.
So the row survives every detach.
