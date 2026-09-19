using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LFavoriteArchive : LFavoriteVault
{
    private readonly LDatabase _lFavoriteArchiveDatabase;

    public LFavoriteArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lFavoriteArchiveDatabase = database;
    }

    public void LFavoriteSave(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lFavoriteArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO favorite (entry_parent, marked_utc)
                VALUES ($entry, $marked)
                ON CONFLICT (entry_parent) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$entry", entryId);
            command.Parameters.AddWithValue("$marked", DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public void LFavoriteDelete(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lFavoriteArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = "DELETE FROM favorite WHERE entry_parent = $entry;";
            command.Parameters.AddWithValue("$entry", entryId);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public bool LFavoriteCheck(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lFavoriteArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM favorite WHERE entry_parent = $entry;";
        command.Parameters.AddWithValue("$entry", entryId);
        return Convert.ToInt64(command.ExecuteScalar(), CultureInfo.InvariantCulture) > 0;
    }

    public IReadOnlyList<LFavorite> LFavoriteFind(string query)
    {
        ArgumentNullException.ThrowIfNull(query);
        query = query.Trim();

        using LDatabaseSession session = _lFavoriteArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT entry.entry_id, entry.headword, entry.language, entry.grasp,
                   entry.added_utc, entry.updated_utc, favorite.marked_utc
            FROM favorite
            JOIN entry ON entry.entry_id = favorite.entry_parent
            WHERE lmatch(entry.headword, $query)
            ORDER BY entry.headword;
            """;
        command.Parameters.AddWithValue("$query", query);

        List<LFavorite> favorites = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            favorites.Add(new LFavorite(
                new LEntry(
                    reader.GetInt64(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetInt32(3),
                    reader.IsDBNull(4) ? null : reader.GetString(4),
                    reader.IsDBNull(5) ? null : reader.GetString(5)),
                reader.GetString(6)));
        }

        return favorites;
    }
}
