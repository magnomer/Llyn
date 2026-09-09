# LSchemaMarker.cs

## `public static class LSchemaMarker`

Creates the sets a card attaches to itself.
They are its Tags, its translation links, and the independent Situations it reaches.

## `public static void LSchemaMarkerCreate(SqliteConnection connection)`

A Tag has no table of its own, because its text is its identity.
So the association row is the Tag.
It is one card, one text, and the position that text takes on that card.
The primary key on (card, text) stops a card carrying the same Tag twice.
The unique index on (card, position) keeps the order free of duplicates.
Both cascade with the card, since a Tag no card writes is not data left behind.
The translation tables beside them are the links a card carries to another Entry.
A link stores the target's id and never its text, so it is one card, one entry, one position.
The primary key on (card, entry) stops a card linking the same Entry twice.
The entry_id cascade is deliberate, since deleting a target Entry drops the links pointing at it.
Situation is the opposite and keeps the older shape.
It carries its own opaque id, and its visible data is never identity.
It is reached only through the association tables below it.
Their situation_id foreign keys deliberately have no cascade.
So the row survives every detach.
