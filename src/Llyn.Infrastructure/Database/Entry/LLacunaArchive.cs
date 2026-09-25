using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LLacunaArchive : LLacunaVault
{
    private readonly LDatabase _lLacunaArchiveDatabase;

    public LLacunaArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lLacunaArchiveDatabase = database;
    }

    public IReadOnlyList<LLacuna> LLacunaRead(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lLacunaArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            "SELECT morphology_value_ref FROM lacuna WHERE entry_parent = $id ORDER BY rowid;";
        command.Parameters.AddWithValue("$id", entryId);

        List<LLacuna> rows = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new LLacuna(reader.IsDBNull(0) ? null : reader.GetInt64(0)));
        }

        return rows;
    }

    public void LLacunaSave(long entryId, IReadOnlyList<long?> morphologyIds)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(morphologyIds);

        string now = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);

        using LDatabaseSession session = _lLacunaArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        LLacunaClear(connection, entryId);
        bool lost = false;
        foreach (long? morphologyId in morphologyIds)
        {
            if (morphologyId is null)
            {
                if (lost)
                {
                    continue;
                }

                lost = true;
            }

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT OR REPLACE INTO lacuna (entry_parent, morphology_value_ref, fetched_utc)
                VALUES ($id, $morphology, $fetched);
                """;
            command.Parameters.AddWithValue("$id", entryId);
            command.Parameters.AddWithValue("$morphology", (object?)morphologyId ?? DBNull.Value);
            command.Parameters.AddWithValue("$fetched", now);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public void LLacunaDelete(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lLacunaArchiveDatabase.LDatabaseSessionStart();
        LLacunaClear(session.LDatabaseSessionConnection, entryId);
        session.LDatabaseSessionCommit();
    }

    private static void LLacunaClear(SqliteConnection connection, long entryId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM lacuna WHERE entry_parent = $id;";
        command.Parameters.AddWithValue("$id", entryId);
        command.ExecuteNonQuery();
    }
}
