using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Reads the version an existing database was built at, brings it up to the version this build
/// produces, and records the result. It exists because <c>CREATE TABLE IF NOT EXISTS</c> — everything
/// <see cref="LSchema"/> runs — leaves an existing table exactly as it stands: a column, a constraint,
/// or a foreign key added to a table a previous build already created never reaches a database that has
/// been opened before. Stamping a version without acting on the one already stored therefore records a
/// shape the file does not have, which is what this file fixes.
/// <para>
/// A step that has to change an existing table follows SQLite's documented table-rebuild procedure —
/// create the new shape beside the old one, copy, drop, rename — and that requires foreign-key
/// enforcement to be off, which cannot be changed inside a transaction. So the runner is called with a
/// plain connection, before any session is open.
/// </para>
/// </summary>
public static class LSchemaMigration
{
    /// <summary>The schema version this build produces. A later change to an existing table raises it.</summary>
    public const long LSchemaMigrationVersion = 15;

    /// <summary>
    /// Records the version on a database that has never carried one, and otherwise applies every step
    /// above the stored version before recording the new one. Throws when the file was written by a
    /// newer build than this one, rather than writing it backwards.
    /// </summary>
    public static void LSchemaMigrationApply(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        if (!LSchemaVersionExist(connection))
        {
            LSchemaVersionCreate(connection);
            return;
        }

        long stored = LSchemaVersionRead(connection);
        if (stored > LSchemaMigrationVersion)
        {
            throw new InvalidOperationException(
                $"The workspace database is at schema version {stored}, which is newer than the version " +
                $"{LSchemaMigrationVersion} this build understands. Update Llyn before opening it.");
        }

        if (stored >= LSchemaMigrationVersion)
        {
            return;
        }

        // Version 12. Three shapes an older database is missing, none of which a CREATE statement can
        // deliver: the foreign key on example.source_id (declared only once the source table existed,
        // so a database created before that keeps the column bare), duplicate positions in the two
        // ordered sets that had no unique index yet, and a schema_version table that permits more than
        // one row.
        if (stored < 12)
        {
            LSchemaExampleNormalize(connection);
            LSchemaPositionNormalize(connection, "relation", "sense_id");
            LSchemaPositionNormalize(connection, "collocation", "entry_id");
            LSchemaPositionNormalize(connection, "collocation_synonym", "collocation_id");
            LSchemaPositionNormalize(connection, "example_translation", "example_id");
            LSchemaVersionNormalize(connection);
        }

        // Version 13. The collocation card carries a Meaning beside its Expression, so the table needs
        // the column to store it in; without this step the meaning of every collocation saved into an
        // existing database would be dropped on the way to disk.
        if (stored < 13)
        {
            LSchemaCollocationNormalize(connection);
        }

        // Version 14. The pronunciation's downloaded recording gets a table of its own. A database
        // built before this version has no pronunciation_audio at all, so the step creates it; the
        // rows already stored are untouched, and a pronunciation with no recording simply has no row.
        if (stored < 14)
        {
            LSchemaAudioNormalize(connection);
        }

        // Version 15. Both card templates have always carried a Title field that nothing stored. The
        // step gives sense and collocation the same title column, so the two cards end up the same
        // shape for it; sense.gloss is left as it stands, unused by the input form.
        if (stored < 15)
        {
            LSchemaTitleNormalize(connection, "sense");
            LSchemaTitleNormalize(connection, "collocation");
        }

        LSchemaVersionSave(connection);
    }

    // Whether the database has ever carried a version row. Absent means the file is new: LSchema has
    // just created every table at the current shape, so there is nothing to migrate.
    private static bool LSchemaVersionExist(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'schema_version';";
        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    // Creates the version table in its current single-row shape and stamps this build's version.
    private static void LSchemaVersionCreate(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE schema_version (
                id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL
            );

            INSERT INTO schema_version (id, version) VALUES (1, $version);
            """;
        command.Parameters.AddWithValue("$version", LSchemaMigrationVersion);
        command.ExecuteNonQuery();
    }

    // The highest version recorded. An older database may hold several rows, which is one of the things
    // version 12 removes; until it does, the highest is the one that describes the file.
    private static long LSchemaVersionRead(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT ifnull(MAX(version), 0) FROM schema_version;";
        return Convert.ToInt64(command.ExecuteScalar());
    }

    // Rebuilds the version table so exactly one row can exist, and stamps this build's version.
    private static void LSchemaVersionNormalize(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            DROP TABLE IF EXISTS schema_version_next;

            CREATE TABLE schema_version_next (
                id INTEGER NOT NULL PRIMARY KEY CHECK (id = 1),
                version INTEGER NOT NULL
            );

            INSERT INTO schema_version_next (id, version) VALUES (1, $version);

            DROP TABLE schema_version;

            ALTER TABLE schema_version_next RENAME TO schema_version;
            """;
        command.Parameters.AddWithValue("$version", LSchemaMigrationVersion);
        command.ExecuteNonQuery();
    }

    // Records this build's version on a table that already holds exactly one row.
    private static void LSchemaVersionSave(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE schema_version SET version = $version;";
        command.Parameters.AddWithValue("$version", LSchemaMigrationVersion);
        command.ExecuteNonQuery();
    }

    // Creates the pronunciation_audio table on a database that predates it. LSchema's own CREATE
    // statement usually gets there first on startup, but the step is what makes the change explicit at
    // the version that introduced it — and what carries it when the table is created by any other path.
    private static void LSchemaAudioNormalize(SqliteConnection connection)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS pronunciation_audio (
                pronunciation_id TEXT NOT NULL PRIMARY KEY,
                file TEXT NOT NULL,
                source TEXT,
                added_utc TEXT NOT NULL,
                FOREIGN KEY (pronunciation_id) REFERENCES pronunciation (id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }

    // Gives a card table the title column its template has always had a field for. Adding a column
    // needs no table rebuild, so existing rows keep everything they have and read back a NULL title,
    // which is what they had. Skipped when the column is already there.
    private static void LSchemaTitleNormalize(SqliteConnection connection, string table)
    {
        using (SqliteCommand check = connection.CreateCommand())
        {
            check.CommandText =
                $"SELECT COUNT(*) FROM pragma_table_info('{table}') WHERE name = 'title';";
            if (Convert.ToInt64(check.ExecuteScalar()) > 0)
            {
                return;
            }
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"ALTER TABLE {table} ADD COLUMN title TEXT;";
        command.ExecuteNonQuery();
    }

    // Gives the collocation table the meaning column its card has always had a field for. Adding a
    // column needs no table rebuild, so the existing rows — and their expressions — are untouched; the
    // meaning of a collocation written before this version reads back as NULL, which is what it was.
    private static void LSchemaCollocationNormalize(SqliteConnection connection)
    {
        using (SqliteCommand check = connection.CreateCommand())
        {
            check.CommandText =
                "SELECT COUNT(*) FROM pragma_table_info('collocation') WHERE name = 'meaning';";
            if (Convert.ToInt64(check.ExecuteScalar()) > 0)
            {
                return;
            }
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "ALTER TABLE collocation ADD COLUMN meaning TEXT;";
        command.ExecuteNonQuery();
    }

    // Gives example.source_id the foreign key it was declared with only once the source table existed.
    // Skipped when the constraint is already there, so this costs one pragma read on every later start.
    private static void LSchemaExampleNormalize(SqliteConnection connection)
    {
        using (SqliteCommand check = connection.CreateCommand())
        {
            check.CommandText = "SELECT COUNT(*) FROM pragma_foreign_key_list('example');";
            if (Convert.ToInt64(check.ExecuteScalar()) > 0)
            {
                return;
            }
        }

        // Foreign-key enforcement has to be off across a table rebuild, and it cannot be switched
        // inside a transaction — hence the explicit statements rather than a session.
        using (SqliteCommand off = connection.CreateCommand())
        {
            off.CommandText = "PRAGMA foreign_keys = OFF;";
            off.ExecuteNonQuery();
        }

        try
        {
            using SqliteTransaction rebuild = connection.BeginTransaction();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                DROP TABLE IF EXISTS example_next;

                CREATE TABLE example_next (
                    id TEXT NOT NULL PRIMARY KEY,
                    language TEXT NOT NULL,
                    text TEXT NOT NULL,
                    local TEXT,
                    source_id TEXT,
                    FOREIGN KEY (source_id) REFERENCES source (id)
                );

                INSERT INTO example_next (id, language, text, local, source_id)
                    SELECT id, language, text, local, source_id FROM example;

                DROP TABLE example;

                ALTER TABLE example_next RENAME TO example;
                """;
            command.ExecuteNonQuery();
            rebuild.Commit();
        }
        finally
        {
            using SqliteCommand on = connection.CreateCommand();
            on.CommandText = "PRAGMA foreign_keys = ON;";
            on.ExecuteNonQuery();
        }
    }

    // Renumbers an ordered set to 0…n-1 per owner so the unique position index can be created over it.
    // The new positions are computed into a temporary table first: an UPDATE that read the very column
    // it writes would depend on the order the rows happened to be visited in.
    private static void LSchemaPositionNormalize(SqliteConnection connection, string table, string ownerColumn)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            DROP TABLE IF EXISTS temp.schema_position;

            CREATE TEMP TABLE schema_position AS
                SELECT id,
                       ROW_NUMBER() OVER (PARTITION BY {ownerColumn} ORDER BY position, id) - 1 AS position
                FROM {table};

            UPDATE {table} SET position =
                (SELECT position FROM temp.schema_position WHERE temp.schema_position.id = {table}.id);

            DROP TABLE temp.schema_position;
            """;
        command.ExecuteNonQuery();
    }
}
