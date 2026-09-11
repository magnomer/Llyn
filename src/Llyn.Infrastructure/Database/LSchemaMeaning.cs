using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaMeaning
{
    public static void LSchemaMeaningCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS sense (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                entry_id INTEGER NOT NULL,
                parent_id INTEGER,
                position INTEGER NOT NULL,
                title_state TEXT NOT NULL DEFAULT 'unspecified',
                title TEXT,
                gloss TEXT,
                definition_language TEXT,
                definition_state TEXT NOT NULL DEFAULT 'unspecified',
                definition TEXT,
                labels TEXT NOT NULL,
                CHECK (title_state = 'specified' OR title IS NULL),
                CHECK (definition_state = 'specified' OR definition IS NULL),
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE,
                FOREIGN KEY (parent_id) REFERENCES sense (id) ON DELETE CASCADE
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_sibling_position
                ON sense (entry_id, ifnull(parent_id, ''), position);
            """;
        command.ExecuteNonQuery();
    }
}
