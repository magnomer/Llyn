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
                sense_id INTEGER PRIMARY KEY AUTOINCREMENT,
                entry_parent INTEGER NOT NULL,
                sense_parent INTEGER,
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
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE,
                FOREIGN KEY (sense_parent) REFERENCES sense (sense_id) ON DELETE CASCADE
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_sibling_position
                ON sense (entry_parent, ifnull(sense_parent, ''), position);
            """;
        command.ExecuteNonQuery();
    }
}
