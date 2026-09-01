using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Creates the database schema idempotently. Each job adds its own <c>CREATE TABLE IF NOT EXISTS</c>
/// statements here (or in a per-area file it owns) so the schema grows one job at a time without ever
/// dropping what an earlier job built.
/// <para>
/// Creating a table is only half of it. <c>IF NOT EXISTS</c> leaves an existing table untouched, so a
/// change to a table an earlier build already created is carried by <see cref="LSchemaMigration"/>,
/// which also owns the version row; the lookup indexes live in <see cref="LSchemaIndex"/> and are
/// created after the migration, because one of them cannot exist until the migration has cleaned the
/// rows it indexes.
/// </para>
/// </summary>
public static class LSchema
{
    /// <summary>
    /// Creates every table that does not yet exist, migrates a database built by an earlier version,
    /// and creates the lookup indexes. Safe to run on each startup: existing tables and an existing
    /// version row are left as they are.
    /// </summary>
    public static void LSchemaCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        // Data-contract names (table and column identifiers) are persisted keys, so they stay
        // lowercase and independent of code member names.
        //
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
        // The pronunciation also owns at most one downloaded recording: pronunciation_audio has no id of
        // its own, since pronunciation_id is its primary key, and its file is stored relative to the
        // workspace folder so a moved or copied workspace keeps its audio.
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

            CREATE TABLE IF NOT EXISTS pronunciation_audio (
                pronunciation_id TEXT NOT NULL PRIMARY KEY,
                file TEXT NOT NULL,
                source TEXT,
                added_utc TEXT NOT NULL,
                FOREIGN KEY (pronunciation_id) REFERENCES pronunciation (id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();

        // The collocations an entry owns and the single Note it owns. A collocation is a stable-id row
        // ordered within its entry, carrying both an expression and the meaning that explains it — the
        // card has a field for each — where a sense carries a definition; reordering
        // rewrites position only. A collocation's synonym is an interlink, not owned text: it uses the same
        // discriminated target model as a job05 relation — an Entry XOR a Meaning — with the XOR enforced by
        // a check constraint and each target column a checked foreign key, so the referenced row must exist
        // and is never touched by the link. The note table has no id and no position: entry_id is its
        // primary key, which makes at-most-one Note per entry a schema fact. Everything here cascades when
        // its entry is deleted, and a synonym cascades when its collocation is deleted.
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS collocation (
                id TEXT NOT NULL PRIMARY KEY,
                entry_id TEXT NOT NULL,
                position INTEGER NOT NULL,
                expression TEXT,
                meaning TEXT,
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

        // The independent Author and the independent bibliographic Reference, created before the Example
        // block below so example.source_id can carry its foreign key. Both are owned by nothing: an
        // Author is shared by any number of References, and a Reference is cited by any number of
        // Entries and Examples without belonging to any of them. Every Reference field is stored as a
        // state column plus a value column: the state says whether the field was never filled in, was
        // recorded as unknown, or holds a value, and the value column is NULL unless the state is
        // 'specified' — the check constraints make that a schema fact rather than a store convention.
        // author_state has no value column of its own because the authors themselves are the value,
        // attached in order through source_author. That table cascades from its Reference, so deleting
        // a Reference drops its author links and never an Author; entry_source cascades from its Entry
        // only, so the source_id and author_id foreign keys deliberately have no cascade and the stores
        // refuse to delete a Reference or an Author while anything still points at it.
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS author (
                id TEXT NOT NULL PRIMARY KEY,
                name TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS source (
                id TEXT NOT NULL PRIMARY KEY,
                title_state TEXT NOT NULL,
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

        // The independent Example and the references that reach it. An Example is owned by nothing: it
        // carries its own opaque id, its language and display text, and at most one Source reference —
        // and it is reached through the three association tables below, one per referrer kind. Each
        // association carries the position the Example takes *for that referrer*, so one Example may be
        // first under an Entry and third under a Meaning; the unique index per referrer keeps those
        // orderings free of duplicates. Deleting a referrer removes only its own association rows
        // (ON DELETE CASCADE on the referrer side); the example_id foreign keys deliberately have no
        // cascade, so an Example survives every detach and the store refuses to delete one while any
        // reference still points at it. Translations are owned text and cascade with their Example.
        //
        // example.source_id points at the independent Reference the block above creates. Its foreign key
        // could only be declared once that source table existed — SQLite refuses to prepare any statement
        // writing to a child table whose parent is missing, even for NULL keys — which is why the source
        // block runs first. A database created before this version keeps the column without the
        // constraint: CREATE TABLE IF NOT EXISTS leaves the existing table as it stands.
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS example (
                id TEXT NOT NULL PRIMARY KEY,
                language TEXT NOT NULL,
                text TEXT NOT NULL,
                local TEXT,
                source_id TEXT,
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

        // The independent Tag and Situation and the references that reach them. Like the Example above,
        // both are owned by nothing: each carries its own opaque id, its visible data is never identity,
        // and it is reached only through the association tables below — one per referrer kind. Each
        // association carries the position the Tag or Situation takes *for that referrer*, so the same
        // one may be first under a Meaning and third under a Collocation; the unique index per referrer
        // keeps those orderings free of duplicates. Deleting a referrer removes only its own association
        // rows (ON DELETE CASCADE on the referrer side); the tag_id and situation_id foreign keys
        // deliberately have no cascade, so the entity survives every detach and the store refuses to
        // delete one while any reference still points at it.
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS tag (
                id TEXT NOT NULL PRIMARY KEY,
                text TEXT NOT NULL
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
                title TEXT NOT NULL,
                description TEXT,
                kind TEXT
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

        // The operational and history tables close the schema: they carry no lexical ownership and the
        // workspace row points at both entry and revision, so they are created last, in their own file.
        LSchemaRevision.LSchemaRevisionCreate(connection);

        // Every table now exists. What an earlier build left in the wrong shape is corrected next, and
        // only then are the indexes created: the unique position indexes cannot be built over rows that
        // still hold the duplicates the migration removes.
        LSchemaMigration.LSchemaMigrationApply(connection);
        LSchemaIndex.LSchemaIndexCreate(connection);
    }
}
