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
            "SELECT morphology_value_ref, cell FROM lacuna WHERE entry_parent = $id ORDER BY rowid;";
        command.Parameters.AddWithValue("$id", entryId);

        List<LLacuna> rows = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new LLacuna(reader.IsDBNull(0) ? null : reader.GetInt64(0), reader.GetString(1)));
        }

        return rows;
    }

    public void LLacunaSave(long entryId, IReadOnlyList<LLacuna> lacunae)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(lacunae);

        string now = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);

        using LDatabaseSession session = _lLacunaArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        LLacunaClear(connection, entryId);
        foreach (LLacuna lacuna in lacunae)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT OR REPLACE INTO lacuna (entry_parent, morphology_value_ref, cell, fetched_utc)
                VALUES ($id, $morphology, $cell, $fetched);
                """;
            command.Parameters.AddWithValue("$id", entryId);
            command.Parameters.AddWithValue("$morphology", (object?)lacuna.LLacunaMorphologyId ?? DBNull.Value);
            command.Parameters.AddWithValue("$cell", lacuna.LLacunaCell);
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
