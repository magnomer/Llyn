using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaReflex
{
    public const long LSchemaReflexNoted = 60;

    public const long LSchemaReflexParted = 72;

    public static void LSchemaReflexCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS reflex (
                reflex_id INTEGER PRIMARY KEY AUTOINCREMENT,
                entry_parent INTEGER NOT NULL,
                position INTEGER NOT NULL,
                language TEXT NOT NULL,
                kind TEXT NOT NULL,
                text TEXT NOT NULL,
                main INTEGER NOT NULL DEFAULT 0,
                romanization TEXT NOT NULL DEFAULT '',
                meaning TEXT NOT NULL DEFAULT '',
                owned INTEGER NOT NULL DEFAULT 0,
                note TEXT NOT NULL DEFAULT '',
                respelling TEXT NOT NULL DEFAULT '',
                region TEXT NOT NULL DEFAULT '',
                onset_ipa TEXT NOT NULL DEFAULT '',
                vowel_ipa TEXT NOT NULL DEFAULT '',
                coda_ipa TEXT NOT NULL DEFAULT '',
                tone_ipa TEXT NOT NULL DEFAULT '',
                onset_respelling TEXT NOT NULL DEFAULT '',
                vowel_respelling TEXT NOT NULL DEFAULT '',
                coda_respelling TEXT NOT NULL DEFAULT '',
                tone_respelling TEXT NOT NULL DEFAULT '',
                FOREIGN KEY (entry_parent) REFERENCES entry (entry_id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }

    public static void LSchemaReflexSettle(
        SqliteConnection connection, string schema, string other, long stored)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        ArgumentException.ThrowIfNullOrWhiteSpace(other);

        if (stored < LSchemaReflexNoted)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"DELETE FROM {schema}.reflex;";
            command.ExecuteNonQuery();
            return;
        }

        if (stored < LSchemaReflexParted)
        {
            LReflexPartApply(connection, schema, other);
        }
    }

    private static void LReflexPartApply(SqliteConnection connection, string schema, string other)
    {
        using SqliteCommand carry = connection.CreateCommand();
        carry.CommandText = LReflexColumnCheck(connection, other, "remark")
            ? $"""
                UPDATE {schema}.reflex AS mine SET
                    romanization = mine.note,
                    note = COALESCE((
                        SELECT held.remark FROM {other}.reflex held
                        WHERE held.reflex_id = mine.reflex_id), '');
                """
            : $"UPDATE {schema}.reflex SET romanization = note, note = '';";
        carry.ExecuteNonQuery();
    }

    private static bool LReflexColumnCheck(SqliteConnection connection, string schema, string column)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"PRAGMA {schema}.table_info(\"reflex\");";
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            if (string.Equals(reader.GetString(1), column, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}
