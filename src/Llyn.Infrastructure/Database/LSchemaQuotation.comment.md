# LSchemaQuotation.cs

## `public static class LSchemaQuotation`

Creates the independent Example and the association tables that reach it.

## `public static void LSchemaQuotationCreate(SqliteConnection connection)`

An Example is owned by nothing.
It carries its own opaque id, its language and display text, and at most one Source reference.
It is reached through the two association tables, one per card kind.
Each association carries the position the Example takes for that referrer.
So one Example may be first under a Meaning and third under a Collocation.
The unique index per referrer keeps those orderings free of duplicates.
sense_example and collocation_example carry data of their own, so each row carries its own opaque id.
A row holds the frame its owner reads the Example under, a marker and a role.
Each carries what is known about it.
The frame lives on the association and never on the Example.
So two owners citing one Example keep their own frame.
Its example_ref is nullable, so a row may state a frame and cite no Example at all.
The frame is the owner's, so writing one before any sentence exists must not wait on a sentence.
A row citing nothing and stating no frame says nothing, and the CHECK refuses it.
Deleting a referrer removes only its own association rows, by ON DELETE CASCADE on that side.
The example_ref foreign keys deliberately have no cascade.
So an Example survives every detach.
The store refuses to delete one while any reference still points at it.
The translation is a column on the Example row, so it goes when the row goes.

example.source_ref points at the independent Reference that LSchemaReference creates.
Its foreign key could only be declared once that source table existed.
SQLite refuses to prepare any statement writing to a child table whose parent is missing.
That holds even for NULL keys, which is why the reference tables are created first.
A database created before this version keeps the column without the constraint.
CREATE TABLE IF NOT EXISTS leaves the existing table as it stands.
