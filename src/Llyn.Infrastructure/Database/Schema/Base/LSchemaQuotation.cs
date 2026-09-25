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
                example_id INTEGER PRIMARY KEY AUTOINCREMENT,
                origin_realm BLOB NOT NULL DEFAULT X'',
                origin_id INTEGER NOT NULL DEFAULT 0,
                language TEXT NOT NULL,
                text_state TEXT NOT NULL DEFAULT 'unspecified',
                text TEXT,
                reference_state TEXT NOT NULL DEFAULT 'unspecified',
                reference_ref INTEGER,
                CHECK (text_state = 'specified' OR text IS NULL),
                CHECK (reference_state = 'specified' OR reference_ref IS NULL),
                FOREIGN KEY (reference_ref) REFERENCES reference (reference_id),
                CHECK (length(origin_realm) = 16 OR origin_id = 0)
            );

            CREATE TABLE IF NOT EXISTS example_translation (
                example_translation_id INTEGER PRIMARY KEY AUTOINCREMENT,
                example_parent INTEGER NOT NULL,
                position INTEGER NOT NULL,
                language TEXT NOT NULL,
                text_state TEXT NOT NULL DEFAULT 'unspecified',
                text TEXT,
                CHECK (text_state = 'specified' OR text IS NULL),
                FOREIGN KEY (example_parent) REFERENCES example (example_id) ON DELETE CASCADE
            );

            CREATE UNIQUE INDEX IF NOT EXISTS example_translation_position
                ON example_translation (example_parent, position);

            CREATE TABLE IF NOT EXISTS example_mention (
                example_mention_id INTEGER PRIMARY KEY AUTOINCREMENT,
                example_parent INTEGER NOT NULL,
                start INTEGER NOT NULL,
                length INTEGER NOT NULL,
                entry_ref INTEGER,
                sense_ref INTEGER,
                CHECK (start >= 0),
                CHECK (length > 0),
                CHECK (entry_ref IS NOT NULL OR sense_ref IS NULL),
                FOREIGN KEY (example_parent) REFERENCES example (example_id) ON DELETE CASCADE,
                FOREIGN KEY (entry_ref) REFERENCES entry (entry_id) ON DELETE CASCADE,
                FOREIGN KEY (sense_ref) REFERENCES sense (sense_id) ON DELETE SET NULL
            );

            CREATE UNIQUE INDEX IF NOT EXISTS example_mention_start
                ON example_mention (example_parent, start);

            CREATE TABLE IF NOT EXISTS sense_example (
                sense_example_id INTEGER PRIMARY KEY AUTOINCREMENT,
                sense_parent INTEGER NOT NULL,
                example_ref INTEGER,
                position INTEGER NOT NULL,
                particle_state TEXT NOT NULL DEFAULT 'unspecified',
                particle TEXT,
                dependence_state TEXT NOT NULL DEFAULT 'unspecified',
                dependence TEXT,
                CHECK (particle_state = 'specified' OR particle IS NULL),
                CHECK (dependence_state = 'specified' OR dependence IS NULL),
                CHECK (example_ref IS NOT NULL
                       OR particle_state <> 'unspecified'
                       OR dependence_state <> 'unspecified'),
                FOREIGN KEY (sense_parent) REFERENCES sense (sense_id) ON DELETE CASCADE,
                FOREIGN KEY (example_ref) REFERENCES example (example_id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_example_position
                ON sense_example (sense_parent, position);

            CREATE TABLE IF NOT EXISTS collocation_example (
                collocation_example_id INTEGER PRIMARY KEY AUTOINCREMENT,
                collocation_parent INTEGER NOT NULL,
                example_ref INTEGER,
                position INTEGER NOT NULL,
                particle_state TEXT NOT NULL DEFAULT 'unspecified',
                particle TEXT,
                dependence_state TEXT NOT NULL DEFAULT 'unspecified',
                dependence TEXT,
                CHECK (particle_state = 'specified' OR particle IS NULL),
                CHECK (dependence_state = 'specified' OR dependence IS NULL),
                CHECK (example_ref IS NOT NULL
                       OR particle_state <> 'unspecified'
                       OR dependence_state <> 'unspecified'),
                FOREIGN KEY (collocation_parent) REFERENCES collocation (collocation_id) ON DELETE CASCADE,
                FOREIGN KEY (example_ref) REFERENCES example (example_id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_example_position
                ON collocation_example (collocation_parent, position);
            """;
        command.ExecuteNonQuery();
    }
}
