using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaScript
{
    public const long LSchemaScriptNoted = 71;

    public static void LSchemaScriptCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS script (
                script_id INTEGER PRIMARY KEY AUTOINCREMENT,
                language TEXT NOT NULL,
                character TEXT NOT NULL,
                style TEXT NOT NULL,
                position INTEGER NOT NULL,
                caption TEXT NOT NULL,
                gloss TEXT NOT NULL,
                data BLOB NOT NULL,
                epoch TEXT NOT NULL DEFAULT '',
                UNIQUE (language, character, style, position)
            );
            """;
        command.ExecuteNonQuery();
    }

    public static void LSchemaScriptSettle(SqliteConnection connection, string schema, long stored)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);

        if (stored >= LSchemaScriptNoted)
        {
            return;
        }

        Dictionary<string, IReadOnlyList<LEpoch>> tables = new(StringComparer.Ordinal);
        foreach ((long row, string language, string style, string caption)
            in LSchemaScriptRead(connection, schema))
        {
            (string epoch, string rest) = LEpoch.LEpochResolve(
                LSchemaEpochRead(tables, language, style), caption);
            if (epoch.Length > 0)
            {
                LSchemaScriptApply(connection, schema, row, epoch, rest);
            }
        }
    }

    private static IReadOnlyList<(long LSchemaRow, string LSchemaLanguage, string LSchemaStyle, string LSchemaCaption)>
        LSchemaScriptRead(SqliteConnection connection, string schema)
    {
        List<(long, string, string, string)> rows = [];
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            SELECT script_id, language, style, caption
            FROM {schema}.script
            WHERE epoch = '' AND caption <> ''
            ORDER BY script_id;
            """;
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add((reader.GetInt64(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
        }

        return rows;
    }

    private static void LSchemaScriptApply(
        SqliteConnection connection, string schema, long row, string epoch, string caption)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"UPDATE {schema}.script SET epoch = $epoch, caption = $caption WHERE script_id = $row;";
        command.Parameters.AddWithValue("$epoch", epoch);
        command.Parameters.AddWithValue("$caption", caption);
        command.Parameters.AddWithValue("$row", row);
        command.ExecuteNonQuery();
    }

    private static IReadOnlyList<LEpoch> LSchemaEpochRead(
        Dictionary<string, IReadOnlyList<LEpoch>> tables, string language, string style)
    {
        string key = language + '\n' + style;
        if (tables.TryGetValue(key, out IReadOnlyList<LEpoch>? held))
        {
            return held;
        }

        IReadOnlyList<LEpoch> found = [];
        foreach (LScriptStyle row in LLanguageLoader.LLanguageLoaderLoad(language).LLanguageScripts)
        {
            if (string.Equals(row.LScriptStyleName, style, StringComparison.Ordinal))
            {
                found = row.LScriptStyleEpoch;
                break;
            }
        }

        tables[key] = found;
        return found;
    }
}
