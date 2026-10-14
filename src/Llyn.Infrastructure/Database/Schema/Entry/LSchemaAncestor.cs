using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaAncestor
{
    public static void LSchemaAncestorCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS ancestor_relation (
                ancestor_relation_id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL UNIQUE,
                builtin INTEGER NOT NULL DEFAULT 0,
                CHECK (builtin IN (0, 1))
            );

            CREATE TABLE IF NOT EXISTS ancestor_tongue (
                ancestor_tongue_id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL UNIQUE
            );

            CREATE TABLE IF NOT EXISTS ancestor (
                ancestor_id INTEGER PRIMARY KEY AUTOINCREMENT,
                entry_parent INTEGER NOT NULL,
                child_ref INTEGER,
                position INTEGER NOT NULL,
                entry_ref INTEGER,
                text TEXT NOT NULL DEFAULT '',
                language TEXT NOT NULL DEFAULT '',
                tongue_ref INTEGER,
                gloss TEXT NOT NULL DEFAULT '',
                relation_ref INTEGER,
                CHECK (position >= 0),
                CHECK (language = '' OR tongue_ref IS NULL),
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE,
                FOREIGN KEY (child_ref) REFERENCES ancestor (ancestor_id) ON DELETE CASCADE,
                FOREIGN KEY (entry_ref) REFERENCES entry (entry_id) ON DELETE SET NULL,
                FOREIGN KEY (tongue_ref) REFERENCES ancestor_tongue (ancestor_tongue_id) ON DELETE SET NULL,
                FOREIGN KEY (relation_ref) REFERENCES ancestor_relation (ancestor_relation_id) ON DELETE SET NULL
            );

            INSERT INTO ancestor_relation (name, builtin)
            VALUES ('borrowed', 1), ('compound', 1), ('derived', 1), ('calque', 1), ('blend', 1)
            ON CONFLICT (name) DO UPDATE SET builtin = 1 WHERE builtin = 0;
            """;
        command.ExecuteNonQuery();
    }
}
