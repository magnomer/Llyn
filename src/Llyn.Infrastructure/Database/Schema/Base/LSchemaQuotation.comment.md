# LSchemaQuotation.cs

## `public static class LSchemaQuotation`

Creates the independent Example and the association tables that reach it.

## `public static void LSchemaQuotationCreate(SqliteConnection connection)`

Creates the Example, its own Gloss and Mention lists, and the two association tables that cite it.

An Example is owned by nothing.
It carries its own opaque id, its language and display text, and at most one Source reference.
It is reached through the two association tables, one per card kind.
Each association carries the position the Example takes for that referrer.
An association id counts strictly upward and is never given to a later row once the row is gone.
A draft holds it while editing.
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

example_translation is one Gloss: the sentence rendered as text in one language.
A row holds the language it is written in and the text with its state.
It also holds the position it takes among the Example's Glosses.
The row lives on the Example and goes with it by ON DELETE CASCADE.
A rendering of a sentence says nothing without the sentence.
The unique index per Example keeps the positions free of duplicates, as the association tables do for their referrers.
One Example may carry several rows in one language, since two renderings of one sentence are both worth keeping.
The table is named after the Translation the schema already speaks of, while the code names the row a Gloss.

example_mention is one word of the sentence that stands for an Entry.
A row holds the span the word occupies, as a start and a length counted in code points.
It holds the Entry the word points at, and at most one Meaning of that Entry.
The row lives on the Example and never on the card that quotes it.
So every card citing one sentence reads the same words the same way.
A NULL entry_ref is the explicit mark that the word stands for nothing.
The CHECK refuses a Meaning without its Entry.
Two of its foreign keys cascade although they are references, as sense_translation does.
A Mention without its Entry says nothing, so it goes with the Entry.
A Mention whose Meaning went still says the Entry, so it degrades to the Entry by ON DELETE SET NULL.
The unique index refuses two words starting at one place.
Overlap between two spans is not a SQL rule, and the store refuses it instead.
The table is created before the associations because it is the Example's own list.

example.reference_ref points at the independent Reference that LSchemaReference creates.
Its foreign key could only be declared once that source table existed.
SQLite refuses to prepare any statement writing to a child table whose parent is missing.
That holds even for NULL keys, which is why the reference tables are created first.
A database created before this version keeps the column without the constraint.
CREATE TABLE IF NOT EXISTS leaves the existing table as it stands.
