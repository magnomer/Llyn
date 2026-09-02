using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchema
{
    public static void LSchemaCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS entry (
                id TEXT NOT NULL PRIMARY KEY,
                headword TEXT NOT NULL,
                language TEXT NOT NULL,
                proficiency TEXT,
                frequency TEXT,
                added_utc TEXT,
                updated_utc TEXT
            );

            CREATE TABLE IF NOT EXISTS form (
                entry_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                text TEXT NOT NULL,
                local TEXT,
                role TEXT NOT NULL,
                PRIMARY KEY (entry_id, position),
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS part_of_speech (
                entry_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                value_id TEXT,
                custom_name TEXT,
                PRIMARY KEY (entry_id, position),
                CHECK ((value_id IS NULL) <> (custom_name IS NULL)),
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS part_of_speech_value (
                language TEXT NOT NULL,
                value_id TEXT NOT NULL,
                display_name TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (language, value_id)
            );
            """;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS inflection (
                entry_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                text TEXT NOT NULL,
                local TEXT,
                part_of_speech_id TEXT,
                PRIMARY KEY (entry_id, position),
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS inflection_feature (
                entry_id TEXT NOT NULL,
                inflection_position INTEGER NOT NULL,
                position INTEGER NOT NULL,
                feature_id TEXT NOT NULL,
                value_id TEXT NOT NULL,
                PRIMARY KEY (entry_id, inflection_position, position),
                FOREIGN KEY (entry_id, inflection_position)
                    REFERENCES inflection (entry_id, position) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS morphology_value (
                language TEXT NOT NULL,
                part_of_speech_id TEXT NOT NULL,
                feature_id TEXT NOT NULL,
                feature_display_name TEXT NOT NULL,
                value_id TEXT NOT NULL,
                value_display_name TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (language, part_of_speech_id, feature_id, value_id)
            );
            """;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS sense (
                id TEXT NOT NULL PRIMARY KEY,
                entry_id TEXT NOT NULL,
                parent_id TEXT,
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

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS relation (
                id TEXT NOT NULL PRIMARY KEY,
                sense_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                relation_type TEXT NOT NULL,
                label TEXT,
                labels TEXT,
                FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS relation_entry (
                relation_id TEXT NOT NULL PRIMARY KEY,
                entry_id TEXT NOT NULL,
                FOREIGN KEY (relation_id) REFERENCES relation (id) ON DELETE CASCADE,
                FOREIGN KEY (entry_id) REFERENCES entry (id)
            );

            CREATE TABLE IF NOT EXISTS relation_sense (
                relation_id TEXT NOT NULL PRIMARY KEY,
                sense_id TEXT NOT NULL,
                FOREIGN KEY (relation_id) REFERENCES relation (id) ON DELETE CASCADE,
                FOREIGN KEY (sense_id) REFERENCES sense (id)
            );
            """;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS pronunciation (
                id TEXT NOT NULL PRIMARY KEY,
                entry_id TEXT NOT NULL UNIQUE,
                level TEXT,
                ipa TEXT,
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS syllable (
                pronunciation_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                orthography TEXT,
                local TEXT,
                onset TEXT,
                medial TEXT,
                nucleus TEXT NOT NULL,
                coda TEXT,
                tone_number INTEGER,
                tone_local TEXT,
                tone_points TEXT,
                PRIMARY KEY (pronunciation_id, position),
                FOREIGN KEY (pronunciation_id) REFERENCES pronunciation (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS representation (
                pronunciation_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                system TEXT NOT NULL,
                role TEXT NOT NULL,
                text TEXT NOT NULL,
                local_tone TEXT,
                PRIMARY KEY (pronunciation_id, position),
                FOREIGN KEY (pronunciation_id) REFERENCES pronunciation (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS pronunciation_audio (
                pronunciation_id TEXT NOT NULL PRIMARY KEY,
                file TEXT NOT NULL,
                source TEXT,
                added_utc TEXT NOT NULL,
                FOREIGN KEY (pronunciation_id) REFERENCES pronunciation (id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS collocation (
                id TEXT NOT NULL PRIMARY KEY,
                entry_id TEXT NOT NULL,
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
                id TEXT NOT NULL PRIMARY KEY,
                collocation_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                target_entry_id TEXT,
                target_sense_id TEXT,
                CHECK ((target_entry_id IS NULL) <> (target_sense_id IS NULL)),
                FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE,
                FOREIGN KEY (target_entry_id) REFERENCES entry (id),
                FOREIGN KEY (target_sense_id) REFERENCES sense (id)
            );

            CREATE TABLE IF NOT EXISTS note (
                entry_id TEXT NOT NULL PRIMARY KEY,
                text TEXT NOT NULL,
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS author (
                id TEXT NOT NULL PRIMARY KEY,
                name TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS source (
                id TEXT NOT NULL PRIMARY KEY,
                title_state TEXT NOT NULL DEFAULT 'unspecified',
                title TEXT,
                program_name_state TEXT NOT NULL,
                program_name TEXT,
                channel_name_state TEXT NOT NULL,
                channel_name TEXT,
                year_state TEXT NOT NULL,
                year TEXT,
                url_state TEXT NOT NULL,
                url TEXT,
                author_state TEXT NOT NULL,
                CHECK (title_state = 'specified' OR title IS NULL),
                CHECK (program_name_state = 'specified' OR program_name IS NULL),
                CHECK (channel_name_state = 'specified' OR channel_name IS NULL),
                CHECK (year_state = 'specified' OR year IS NULL),
                CHECK (url_state = 'specified' OR url IS NULL)
            );

            CREATE TABLE IF NOT EXISTS source_author (
                source_id TEXT NOT NULL,
                author_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (source_id, author_id),
                FOREIGN KEY (source_id) REFERENCES source (id) ON DELETE CASCADE,
                FOREIGN KEY (author_id) REFERENCES author (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS source_author_position
                ON source_author (source_id, position);

            CREATE TABLE IF NOT EXISTS entry_source (
                entry_id TEXT NOT NULL,
                source_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (entry_id, source_id),
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE,
                FOREIGN KEY (source_id) REFERENCES source (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS entry_source_position
                ON entry_source (entry_id, position);
            """;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS example (
                id TEXT NOT NULL PRIMARY KEY,
                language TEXT NOT NULL,
                text_state TEXT NOT NULL DEFAULT 'unspecified',
                text TEXT,
                local TEXT,
                source_state TEXT NOT NULL DEFAULT 'unspecified',
                source_id TEXT,
                CHECK (text_state = 'specified' OR text IS NULL),
                CHECK (source_state = 'specified' OR source_id IS NULL),
                FOREIGN KEY (source_id) REFERENCES source (id)
            );

            CREATE TABLE IF NOT EXISTS example_translation (
                id TEXT NOT NULL PRIMARY KEY,
                example_id TEXT NOT NULL,
                language TEXT NOT NULL,
                text TEXT NOT NULL,
                position INTEGER NOT NULL,
                FOREIGN KEY (example_id) REFERENCES example (id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS entry_example (
                entry_id TEXT NOT NULL,
                example_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (entry_id, example_id),
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE,
                FOREIGN KEY (example_id) REFERENCES example (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS entry_example_position
                ON entry_example (entry_id, position);

            CREATE TABLE IF NOT EXISTS sense_example (
                sense_id TEXT NOT NULL,
                example_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_id, example_id),
                FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE,
                FOREIGN KEY (example_id) REFERENCES example (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_example_position
                ON sense_example (sense_id, position);

            CREATE TABLE IF NOT EXISTS collocation_example (
                collocation_id TEXT NOT NULL,
                example_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_id, example_id),
                FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE,
                FOREIGN KEY (example_id) REFERENCES example (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_example_position
                ON collocation_example (collocation_id, position);
            """;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS tag (
                id TEXT NOT NULL PRIMARY KEY,
                text_state TEXT NOT NULL DEFAULT 'unspecified',
                text TEXT,
                CHECK (text_state = 'specified' OR text IS NULL)
            );

            CREATE TABLE IF NOT EXISTS sense_tag (
                sense_id TEXT NOT NULL,
                tag_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_id, tag_id),
                FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE,
                FOREIGN KEY (tag_id) REFERENCES tag (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_tag_position
                ON sense_tag (sense_id, position);

            CREATE TABLE IF NOT EXISTS collocation_tag (
                collocation_id TEXT NOT NULL,
                tag_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_id, tag_id),
                FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE,
                FOREIGN KEY (tag_id) REFERENCES tag (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_tag_position
                ON collocation_tag (collocation_id, position);

            CREATE TABLE IF NOT EXISTS situation (
                id TEXT NOT NULL PRIMARY KEY,
                title_state TEXT NOT NULL DEFAULT 'unspecified',
                title TEXT,
                description_state TEXT NOT NULL DEFAULT 'unspecified',
                description TEXT,
                kind_state TEXT NOT NULL DEFAULT 'unspecified',
                kind TEXT,
                source_state TEXT NOT NULL DEFAULT 'unspecified',
                source_id TEXT,
                CHECK (title_state = 'specified' OR title IS NULL),
                CHECK (description_state = 'specified' OR description IS NULL),
                CHECK (kind_state = 'specified' OR kind IS NULL),
                CHECK (source_state = 'specified' OR source_id IS NULL),
                FOREIGN KEY (source_id) REFERENCES source (id)
            );

            CREATE TABLE IF NOT EXISTS sense_situation (
                sense_id TEXT NOT NULL,
                situation_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (sense_id, situation_id),
                FOREIGN KEY (sense_id) REFERENCES sense (id) ON DELETE CASCADE,
                FOREIGN KEY (situation_id) REFERENCES situation (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_situation_position
                ON sense_situation (sense_id, position);

            CREATE TABLE IF NOT EXISTS collocation_situation (
                collocation_id TEXT NOT NULL,
                situation_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                PRIMARY KEY (collocation_id, situation_id),
                FOREIGN KEY (collocation_id) REFERENCES collocation (id) ON DELETE CASCADE,
                FOREIGN KEY (situation_id) REFERENCES situation (id)
            );

            CREATE UNIQUE INDEX IF NOT EXISTS collocation_situation_position
                ON collocation_situation (collocation_id, position);
            """;
        command.ExecuteNonQuery();

        LSchemaRevision.LSchemaRevisionCreate(connection);

        LSchemaMigration.LSchemaMigrationApply(connection);
        LSchemaIndex.LSchemaIndexCreate(connection);
    }
}
