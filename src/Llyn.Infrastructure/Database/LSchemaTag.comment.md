# LSchemaTag.cs

## `public static class LSchemaTag`

Version 19.
A tag's text is its whole identity, so the shared tag table and the ids pointing at it are retired.
The two link tables carry the text itself, and the tag table is dropped.

## `public static void LSchemaTagNormalize(SqliteConnection connection)`

Rebuilds sense_tag and collocation_tag around a text column and drops the tag table.
A link table already carrying text is the new shape, so the step returns untouched.
The carried rows are grouped per owner, so a card that named one tag twice keeps it once.
A tag whose text was null or empty is dropped, because such a row named nothing.
Positions are renumbered from zero in the order the owner first held each text.
