using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LFoldArchive : LFoldVault
{
    private readonly LDatabase _lFoldArchiveDatabase;

    public LFoldArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lFoldArchiveDatabase = database;
    }

    public void LFoldSave(long cardId)
    {
        using LDatabaseSession session = _lFoldArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO sense_fold (sense_parent)
                SELECT sense_id FROM sense WHERE sense_id = $card
                ON CONFLICT (sense_parent) DO NOTHING;

                INSERT INTO collocation_fold (collocation_parent)
                SELECT collocation_id FROM collocation WHERE collocation_id = $card
                ON CONFLICT (collocation_parent) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$card", cardId);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public void LFoldDelete(long cardId)
    {
        using LDatabaseSession session = _lFoldArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                DELETE FROM sense_fold WHERE sense_parent = $card;
                DELETE FROM collocation_fold WHERE collocation_parent = $card;
                """;
            command.Parameters.AddWithValue("$card", cardId);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public IReadOnlySet<long> LFoldRead(long entryId)
    {
        using LDatabaseSession session = _lFoldArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT sense_fold.sense_parent
            FROM sense_fold
            JOIN sense ON sense.sense_id = sense_fold.sense_parent
            WHERE sense.entry_parent = $entry
            UNION
            SELECT collocation_fold.collocation_parent
            FROM collocation_fold
            JOIN collocation ON collocation.collocation_id = collocation_fold.collocation_parent
            WHERE collocation.entry_parent = $entry;
            """;
        command.Parameters.AddWithValue("$entry", entryId);

        HashSet<long> folded = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            folded.Add(reader.GetInt64(0));
        }

        return folded;
    }

    public void LFoldReflexSpread(long entryId, bool opened)
    {
        using LDatabaseSession session = _lFoldArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = opened
                ? """
                  INSERT INTO reflex_fold (entry_parent)
                  SELECT entry_id FROM entry WHERE entry_id = $entry
                  ON CONFLICT (entry_parent) DO NOTHING;
                  """
                : "DELETE FROM reflex_fold WHERE entry_parent = $entry;";
            command.Parameters.AddWithValue("$entry", entryId);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public bool LFoldReflexCheck(long entryId)
    {
        using LDatabaseSession session = _lFoldArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM reflex_fold WHERE entry_parent = $entry;";
        command.Parameters.AddWithValue("$entry", entryId);
        return Convert.ToInt64(command.ExecuteScalar(), CultureInfo.InvariantCulture) > 0;
    }

    public void LFoldBoxSpread(long entryId, LFoldBox box, bool opened)
    {
        string table = LFoldTableSelect(box);
        using LDatabaseSession session = _lFoldArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = opened
                ? $"""
                  INSERT INTO {table} (entry_parent)
                  SELECT entry_id FROM entry WHERE entry_id = $entry
                  ON CONFLICT (entry_parent) DO NOTHING;
                  """
                : $"DELETE FROM {table} WHERE entry_parent = $entry;";
            command.Parameters.AddWithValue("$entry", entryId);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public bool LFoldBoxCheck(long entryId, LFoldBox box)
    {
        string table = LFoldTableSelect(box);
        using LDatabaseSession session = _lFoldArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM {table} WHERE entry_parent = $entry;";
        command.Parameters.AddWithValue("$entry", entryId);
        return Convert.ToInt64(command.ExecuteScalar(), CultureInfo.InvariantCulture) > 0;
    }

    public void LFoldStemSpread(long entryId, string key, bool opened)
    {
        ArgumentNullException.ThrowIfNull(key);

        using LDatabaseSession session = _lFoldArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = opened
                ? """
                  INSERT INTO stem_fold (entry_parent, key)
                  SELECT entry_id, $key FROM entry WHERE entry_id = $entry
                  ON CONFLICT (entry_parent, key) DO NOTHING;
                  """
                : "DELETE FROM stem_fold WHERE entry_parent = $entry AND key = $key;";
            command.Parameters.AddWithValue("$entry", entryId);
            command.Parameters.AddWithValue("$key", key);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public bool LFoldStemCheck(long entryId, string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        using LDatabaseSession session = _lFoldArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM stem_fold WHERE entry_parent = $entry AND key = $key;";
        command.Parameters.AddWithValue("$entry", entryId);
        command.Parameters.AddWithValue("$key", key);
        return Convert.ToInt64(command.ExecuteScalar(), CultureInfo.InvariantCulture) > 0;
    }

    private static string LFoldTableSelect(LFoldBox box) => box switch
    {
        LFoldBox.LFoldBoxFanqie => "fanqie_fold",
        LFoldBox.LFoldBoxScript => "script_fold",
        _ => throw new ArgumentOutOfRangeException(nameof(box), box, null),
    };
}
