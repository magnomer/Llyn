using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Creates the lookup indexes the schema needs but does not get for free. SQLite indexes a primary key
/// and a unique constraint, and nothing else — in particular it never indexes the <em>child</em> side of
/// a foreign key. With <c>PRAGMA foreign_keys = ON</c> every parent delete has to prove no child row
/// points at the row going away, so an unindexed child column turns one delete into a full scan of that
/// table, and the cascade of an Entry delete reaches a dozen of them.
/// <para>
/// Only the columns no existing key already covers are listed here. A composite primary key indexes its
/// leading column, so <c>form (entry_id, position)</c> and <c>entry_example (entry_id, example_id)</c>
/// need nothing for their first column — it is the second column, the one an association is searched by
/// from the other direction, that needs an index of its own.
/// </para>
/// </summary>
public static class LSchemaIndex
{
    /// <summary>
    /// Creates every index that does not yet exist. Called by <see cref="LSchema.LSchemaCreate"/> on
    /// each startup; existing indexes are left as they are.
    /// </summary>
    public static void LSchemaIndexCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        // Lexical rows owned by an Entry or a Meaning whose owning column carries no key of its own.
        command.CommandText =
            """
            CREATE INDEX IF NOT EXISTS sense_parent ON sense (parent_id);
            CREATE INDEX IF NOT EXISTS relation_sense_origin ON relation (sense_id);
            CREATE INDEX IF NOT EXISTS collocation_entry ON collocation (entry_id);
            CREATE INDEX IF NOT EXISTS example_translation_example ON example_translation (example_id);
            CREATE INDEX IF NOT EXISTS example_source ON example (source_id);
            """;
        command.ExecuteNonQuery();

        // The target side of every interlink: the column a delete of the referenced row must scan.
        command.CommandText =
            """
            CREATE INDEX IF NOT EXISTS relation_entry_target ON relation_entry (entry_id);
            CREATE INDEX IF NOT EXISTS relation_sense_target ON relation_sense (sense_id);
            CREATE INDEX IF NOT EXISTS collocation_synonym_owner ON collocation_synonym (collocation_id);
            CREATE INDEX IF NOT EXISTS collocation_synonym_entry ON collocation_synonym (target_entry_id);
            CREATE INDEX IF NOT EXISTS collocation_synonym_sense ON collocation_synonym (target_sense_id);
            """;
        command.ExecuteNonQuery();

        // The member side of every association table, and the shared entities those members name.
        command.CommandText =
            """
            CREATE INDEX IF NOT EXISTS entry_example_member ON entry_example (example_id);
            CREATE INDEX IF NOT EXISTS sense_example_member ON sense_example (example_id);
            CREATE INDEX IF NOT EXISTS collocation_example_member ON collocation_example (example_id);
            CREATE INDEX IF NOT EXISTS sense_tag_member ON sense_tag (tag_id);
            CREATE INDEX IF NOT EXISTS collocation_tag_member ON collocation_tag (tag_id);
            CREATE INDEX IF NOT EXISTS sense_situation_member ON sense_situation (situation_id);
            CREATE INDEX IF NOT EXISTS collocation_situation_member ON collocation_situation (situation_id);
            CREATE INDEX IF NOT EXISTS source_author_member ON source_author (author_id);
            CREATE INDEX IF NOT EXISTS entry_source_member ON entry_source (source_id);
            """;
        command.ExecuteNonQuery();

        // Operational rows pointing at lexical rows and at history.
        command.CommandText =
            """
            CREATE INDEX IF NOT EXISTS tombstone_revision ON tombstone (revision_id);
            CREATE INDEX IF NOT EXISTS workspace_left ON workspace (left_entry);
            CREATE INDEX IF NOT EXISTS workspace_right ON workspace (right_entry);
            CREATE INDEX IF NOT EXISTS workspace_revision ON workspace (revision);
            """;
        command.ExecuteNonQuery();

        // The ordered sets that were left without a unique position: a relation within its Meaning, a
        // collocation within its Entry, a synonym within its Collocation, and a translation within its
        // Example. Every other ordered set already has one, and without it a duplicate position is
        // silently possible and the read order becomes arbitrary. Any duplicate an older database still
        // holds is renumbered by the migration that precedes this call.
        command.CommandText =
            """
            CREATE UNIQUE INDEX IF NOT EXISTS relation_position ON relation (sense_id, position);
            CREATE UNIQUE INDEX IF NOT EXISTS collocation_position ON collocation (entry_id, position);
            CREATE UNIQUE INDEX IF NOT EXISTS collocation_synonym_position
                ON collocation_synonym (collocation_id, position);
            CREATE UNIQUE INDEX IF NOT EXISTS example_translation_position
                ON example_translation (example_id, position);
            """;
        command.ExecuteNonQuery();
    }
}
