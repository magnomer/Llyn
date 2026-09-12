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
                collocation_id INTEGER PRIMARY KEY AUTOINCREMENT,
                entry_parent INTEGER NOT NULL,
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
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS note (
                entry_parent INTEGER NOT NULL PRIMARY KEY,
                text TEXT NOT NULL,
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }
}
