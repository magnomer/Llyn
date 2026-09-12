using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaRevision
{
    public static void LSchemaRevisionCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS revision (
                id INTEGER PRIMARY KEY,
                created_utc TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS revision_change (
                revision_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                target_ref INTEGER NOT NULL,
                target_type TEXT NOT NULL,
                kind TEXT NOT NULL,
                summary TEXT,
                PRIMARY KEY (revision_id, position),
                FOREIGN KEY (revision_id) REFERENCES revision (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS tombstone (
                entry_id INTEGER NOT NULL PRIMARY KEY,
                revision_ref INTEGER NOT NULL,
                deleted_utc TEXT NOT NULL,
                FOREIGN KEY (revision_ref) REFERENCES revision (id)
            );
            """;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS workspace (
                id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                left_entry_ref INTEGER,
                right_entry_ref INTEGER,
                revision_ref INTEGER,
                identity_floor INTEGER NOT NULL DEFAULT 0,
                mode TEXT,
                split TEXT,
                library_order TEXT,
                phonology_order TEXT,
                favorite_order TEXT,
                taxonomy_order TEXT,
                repertoire_order TEXT,
                reference_order TEXT,
                corpus_order TEXT,
                tenor_order TEXT,
                CHECK (identity_floor <= 0),
                FOREIGN KEY (left_entry_ref) REFERENCES entry (id) ON DELETE SET NULL,
                FOREIGN KEY (right_entry_ref) REFERENCES entry (id) ON DELETE SET NULL,
                FOREIGN KEY (revision_ref) REFERENCES revision (id) ON DELETE SET NULL
            );
            """;
        command.ExecuteNonQuery();
    }
}
