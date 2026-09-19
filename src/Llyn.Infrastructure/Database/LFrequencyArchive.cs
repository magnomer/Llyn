using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LFrequencyArchive : LFrequencyVault
{
    private readonly LDatabase _lFrequencyArchiveDatabase;

    public LFrequencyArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lFrequencyArchiveDatabase = database;
    }

    public IReadOnlyList<LFrequency> LFrequencyRead(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lFrequencyArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            "SELECT source, raw, band FROM frequency WHERE entry_parent = $id ORDER BY rowid;";
        command.Parameters.AddWithValue("$id", entryId);

        List<LFrequency> rows = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new LFrequency(
                reader.GetString(0),
                reader.GetString(1),
                reader.IsDBNull(2) ? null : reader.GetString(2)));
        }

        return rows;
    }

    public void LFrequencySet(long entryId, IReadOnlyList<LFrequency> rows)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(rows);

        string now = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);

        using LDatabaseSession session = _lFrequencyArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        LFrequencyDelete(connection, entryId);
        foreach (LFrequency row in rows)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT OR REPLACE INTO frequency (entry_parent, source, raw, band, fetched_utc)
                VALUES ($id, $source, $raw, $band, $fetched);
                """;
            command.Parameters.AddWithValue("$id", entryId);
            command.Parameters.AddWithValue("$source", row.LFrequencySource);
            command.Parameters.AddWithValue("$raw", row.LFrequencyRaw);
            command.Parameters.AddWithValue("$band", (object?)row.LFrequencyBand ?? DBNull.Value);
            command.Parameters.AddWithValue("$fetched", now);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public void LFrequencyClear(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lFrequencyArchiveDatabase.LDatabaseSessionStart();
        LFrequencyDelete(session.LDatabaseSessionConnection, entryId);
        session.LDatabaseSessionCommit();
    }

    public void LFrequencyBandSet(long entryId, string source, string? band)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(source);

        using LDatabaseSession session = _lFrequencyArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                "UPDATE frequency SET band = $band WHERE entry_parent = $id AND source = $source;";
            command.Parameters.AddWithValue("$band", (object?)band ?? DBNull.Value);
            command.Parameters.AddWithValue("$id", entryId);
            command.Parameters.AddWithValue("$source", source);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    private static void LFrequencyDelete(SqliteConnection connection, long entryId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM frequency WHERE entry_parent = $id;";
        command.Parameters.AddWithValue("$id", entryId);
        command.ExecuteNonQuery();
    }
}
