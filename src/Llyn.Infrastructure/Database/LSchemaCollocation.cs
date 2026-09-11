using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaCollocation
{
    public static void LSchemaCollocationCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS collocation (
                id INTEGER PRIMARY KEY,
                entry_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                title_state TEXT NOT NULL DEFAULT 'unspecified',
                title TEXT,
                expression_state TEXT NOT NULL DEFAULT 'unspecified',
                expression TEXT,
                meaning_state TEXT NOT NULL DEFAULT 'unspecified',
                meaning TEXT,
                CHECK (title_state = 'specified' OR title IS NULL),
                CHECK (expression_state = 'specified' OR expression IS NULL),
                CHECK (meaning_state = 'specified' OR meaning IS NULL),
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS collocation_synonym (
                id INTEGER PRIMARY KEY,
                collocation_id INTEGER NOT NULL,
                position INTEGER NOT NULL,
                target_entry_id INTEGER,
                target_sense_id INTEGER,
                CHECK ((target_entry_id IS NULL) <> (target_sense_id IS NULL)),
                FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE,
                FOREIGN KEY (target_entry_id) REFERENCES entry (id),
                FOREIGN KEY (target_sense_id) REFERENCES sense (id)
            );

            CREATE TABLE IF NOT EXISTS note (
                entry_id INTEGER NOT NULL PRIMARY KEY,
                text TEXT NOT NULL,
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }
}
