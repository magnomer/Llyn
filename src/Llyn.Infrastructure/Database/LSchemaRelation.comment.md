# LSchemaRelation.cs

## `public static class LSchemaRelation`

Creates the lexical relations originating from a Meaning.

## `public static void LSchemaRelationCreate(SqliteConnection connection)`

Each relation hangs from its origin meaning and points at exactly one target.
The target is reached through a checked reference row.
It is an Entry (relation_entry) XOR another Meaning (relation_sense).
The relation_id primary key on each target table allows at most one target row per relation.
The store enforces the XOR across the two tables.
A relation cascades when its origin meaning is deleted, and its target row cascades with it.
The referenced Entry or Meaning is never touched.
