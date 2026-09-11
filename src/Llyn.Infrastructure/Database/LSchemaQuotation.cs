using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaQuotation
{
    public static void LSchemaQuotationCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS example (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                origin_realm BLOB NOT NULL DEFAULT X'',
                origin_id INTEGER NOT NULL DEFAULT 0,
                language TEXT NOT NULL,
                text_state TEXT NOT NULL DEFAULT 'unspecified',
                text TEXT,
                translation_state TEXT NOT NULL DEFAULT 'unspecified',
                translation TEXT,
                source_state TEXT NOT NULL DEFAULT 'unspecified',
                source_id INTEGER,
                CHECK (text_state = 'specified' OR text IS NULL),
                CHECK (translation_state = 'specified' OR translation IS NULL),
                CHECK (source_state = 'specified' OR source_id IS NULL),
                FOREIGN KEY (source_id) REFERENCES source (id),
                CHECK (length(origin_realm) = 16 OR origin_id = 0)
            );

            CREATE TABLE IF NOT EXISTS sense_example (
                id INTEGER PRIMARY KEY,
                sense_id INTEGER NOT NULL,
                example_id INTEGER,
                position INTEGER NOT NULL,
                particle_state TEXT NOT NULL DEFAULT 'unspecified',
                particle TEXT,
                dependence_state TEXT NOT NULL DEFAULT 'unspecified',
                dependence TEXT,
                CHECK (particle_state = 'specified' OR particle IS NULL),
                CHECK (dependence_state = 'specified' OR dependence IS NULL),
                CHECK (example_id IS NOT NULL
                       OR particle_state <> 'unspecified'
                       OR dependence_state <> 'unspecified'),
                FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE,
                FOREIGN KEY (example_id) REFERENCES example (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_example_position
                ON sense_example (sense_id, position);

            CREATE TABLE IF NOT EXISTS collocation_example (
                id INTEGER PRIMARY KEY,
                collocation_id INTEGER NOT NULL,
                example_id INTEGER,
                position INTEGER NOT NULL,
                particle_state TEXT NOT NULL DEFAULT 'unspecified',
                particle TEXT,
                dependence_state TEXT NOT NULL DEFAULT 'unspecified',
                dependence TEXT,
                CHECK (particle_state = 'specified' OR particle IS NULL),
                CHECK (dependence_state = 'specified' OR dependence IS NULL),
                CHECK (example_id IS NOT NULL
                       OR particle_state <> 'unspecified'
                       OR dependence_state <> 'unspecified'),
                FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE,
                FOREIGN KEY (example_id) REFERENCES example (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_example_position
                ON collocation_example (collocation_id, position);
            """;
        command.ExecuteNonQuery();
    }
}
