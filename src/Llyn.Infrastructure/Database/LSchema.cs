using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Creates the database schema idempotently and records its version. Job01 defines only the runner
/// and the version row; each later job adds its own <c>CREATE TABLE IF NOT EXISTS</c> statements
/// here (or in a per-area file it owns) so the schema grows one job at a time without ever dropping
/// what an earlier job built.
/// </summary>
public static class LSchema
{
    /// <summary>The schema version this build produces. Later jobs raise it as they extend the schema.</summary>
    private const long LSchemaVersion = 6;

    /// <summary>
    /// Creates every table that does not yet exist and stamps the schema version. Safe to run on each
    /// startup: existing tables and an existing version row are left as they are.
    /// </summary>
    public static void LSchemaCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        // Data-contract names (table and column identifiers) are persisted keys, so they stay
        // lowercase and independent of code member names.
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS schema_version (
                version INTEGER NOT NULL
            );
            """;
        command.ExecuteNonQuery();

        // The Entry root, its owned written forms and parts of speech, and the language-controlled POS
        // display vocabulary. Owned child rows carry an (entry_id, position) identity and cascade when
        // their Entry is deleted.
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
                value_id TEXT NOT NULL,
                PRIMARY KEY (entry_id, position),
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

        // Entry-owned inflected forms and their ordered grammatical features, plus the
        // language-controlled morphology display vocabulary. A feature's identity is the two-level
        // owned key (entry_id, inflection_position, position); it cascades when its inflection is
        // deleted, and an inflection cascades when its Entry is deleted. Lexical rows store only stable
        // ids — display names live in morphology_value.
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

        // The Meaning tree an entry owns: each sense is a stable-id node that may nest under another
        // sense in the same entry, carrying a single inline definition field. A sense cascades when its
        // parent sense is deleted and when its Entry is deleted. Sibling ordering is unique within a
        // parent — the ifnull() expression index treats root senses (null parent) as one sibling group.
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS sense (
                id TEXT NOT NULL PRIMARY KEY,
                entry_id TEXT NOT NULL,
                parent_id TEXT,
                position INTEGER NOT NULL,
                gloss TEXT,
                definition_language TEXT,
                definition TEXT,
                labels TEXT NOT NULL,
                FOREIGN KEY (entry_id) REFERENCES entry (id) ON DELETE CASCADE,
                FOREIGN KEY (parent_id) REFERENCES sense (id) ON DELETE CASCADE
            );

            CREATE UNIQUE INDEX IF NOT EXISTS sense_sibling_position
                ON sense (entry_id, ifnull(parent_id, ''), position);
            """;
        command.ExecuteNonQuery();

        // Lexical relations originating from a Meaning. Each relation hangs from its origin sense and
        // points at exactly one target through a checked reference row: an Entry (relation_entry) XOR
        // another Meaning (relation_sense). The relation_id primary key on each target table allows at
        // most one target row per relation there; the store enforces the XOR across the two tables.
        // A relation cascades when its origin sense is deleted, and its target row cascades with it —
        // the referenced Entry/Meaning is never touched.
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

        // The single pronunciation an entry owns and its two owned child structures. An entry carries at
        // most one pronunciation — enforced by the unique entry_id, with no position on the pronunciation
        // itself — and the pronunciation owns ordered syllables and representations keyed by
        // (pronunciation_id, position). A syllable requires only its nucleus; every other syllable field
        // and a representation's local_tone are optional and store NULL when absent (distinct from empty).
        // Children cascade when their pronunciation is deleted, and the pronunciation cascades when its
        // entry is deleted.
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
            """;
        command.ExecuteNonQuery();

        command.CommandText = "SELECT COUNT(*) FROM schema_version;";
        long rows = Convert.ToInt64(command.ExecuteScalar());
        if (rows == 0)
        {
            command.CommandText = "INSERT INTO schema_version (version) VALUES ($version);";
            command.Parameters.AddWithValue("$version", LSchemaVersion);
            command.ExecuteNonQuery();
        }
    }
}
