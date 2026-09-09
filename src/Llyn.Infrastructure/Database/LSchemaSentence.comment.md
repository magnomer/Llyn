# LSchemaSentence.cs

## `public static class LSchemaSentence`

Versions 24 and 25.
A Meaning's or a Collocation's hold on an Example becomes a row with data of its own.
The hold gains its own id and the frame the owner reads the Example under.
The two tables differ only in their name and their owner column, so one rebuild serves both.

## `public static void LSchemaSentenceNormalize(`

Rebuilds a hold table unless it already carries an id column.
Runs with foreign keys off and back on, because the rebuild copies rows before their parents are checked.

## `public static void LSchemaSentenceRebuild(`

SQLite cannot add a primary key or a foreign key to a table that stands.
The table is rebuilt beside itself.
Enforcement is off for the rebuild, because the carried rows are copied before their parents are checked.
A row an older build left pointing at nothing therefore blocks nothing here.
Every older row is carried over with its Example and its place, stating no frame.
