# LSchemaReference.cs

## `public static class LSchemaReference`

Creates the independent Author and the independent bibliographic Reference.
They are created before the Example tables.
So example.reference_ref can carry its foreign key.

## `public static void LSchemaReferenceCreate(SqliteConnection connection)`

Both are owned by nothing.
An Author is shared by any number of References.
A Reference is cited by any number of Entries and Examples without belonging to any.
Every Reference text field is stored as a state column plus a value column.
The state says whether the field was never filled in, was recorded as unknown, or holds a value.
The value column is NULL unless the state is 'specified'.
The check constraints make that a schema fact rather than a store convention.
kind is one column rather than two, because its closed set carries the three states as members of its own.
It stores the word rather than a number, so the file never depends on the order the members are declared in.
author_state has no value column of its own, because the authors themselves are the value.
They are attached in order through reference_author.
That table cascades from its Reference.
So deleting a Reference drops its author links and never an Author.
So the example.reference_ref and author_ref foreign keys deliberately have no cascade.
The stores refuse to delete a Reference or an Author while anything still points at it.
