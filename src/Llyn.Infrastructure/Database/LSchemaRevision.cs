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
                target_id INTEGER NOT NULL,
                target_type TEXT NOT NULL,
                kind TEXT NOT NULL,
                summary TEXT,
                PRIMARY KEY (revision_id, position),
                FOREIGN KEY (revision_id) REFERENCES revision (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS tombstone (
                entry_id INTEGER NOT NULL PRIMARY KEY,
                revision_id INTEGER NOT NULL,
                deleted_utc TEXT NOT NULL,
                FOREIGN KEY (revision_id) REFERENCES revision (id)
            );
            """;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS workspace (
                id INTEGER PRIMARY KEY,
                left_entry INTEGER,
                right_entry INTEGER,
                mode TEXT,
                split TEXT,
                revision INTEGER,
                library_order TEXT,
                phonology_order TEXT,
                favorite_order TEXT,
                taxonomy_order TEXT,
                repertoire_order TEXT,
                reference_order TEXT,
                corpus_order TEXT,
                tenor_order TEXT,
                FOREIGN KEY (left_entry) REFERENCES entry (id) ON DELETE SET NULL,
                FOREIGN KEY (right_entry) REFERENCES entry (id) ON DELETE SET NULL,
                FOREIGN KEY (revision) REFERENCES revision (id) ON DELETE SET NULL
            );
            """;
        command.ExecuteNonQuery();
    }
}
