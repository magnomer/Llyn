using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LDeckFolderArchive : LDeckFolderVault
{
    private readonly LDatabase _lDeckFolderDatabase;

    public LDeckFolderArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lDeckFolderDatabase = database;
    }

    public LDeckFolder LDeckFolderCreate(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        using LDatabaseSession session = _lDeckFolderDatabase.LDatabaseSessionStart();
        LDeckFolder created;
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO deck_folder (name, position)
                VALUES ($name, (SELECT ifnull(max(position) + 1, 0) FROM deck_folder))
                RETURNING deck_folder_id, name, position;
                """;
            command.Parameters.AddWithValue("$name", name);
            using SqliteDataReader reader = command.ExecuteReader();
            reader.Read();
            created = new LDeckFolder(reader.GetInt64(0), reader.GetString(1), reader.GetInt32(2));
        }

        session.LDatabaseSessionCommit();
        return created;
    }

    public IReadOnlyList<LDeckFolder> LDeckFolderRead()
    {
        using LDatabaseSession session = _lDeckFolderDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            "SELECT deck_folder_id, name, position FROM deck_folder ORDER BY position, deck_folder_id;";

        List<LDeckFolder> folders = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            folders.Add(new LDeckFolder(reader.GetInt64(0), reader.GetString(1), reader.GetInt32(2)));
        }

        return folders;
    }

    public void LDeckFolderUpdate(LDeckFolder folder)
    {
        ArgumentNullException.ThrowIfNull(folder);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(folder.LDeckFolderId);
        ArgumentException.ThrowIfNullOrWhiteSpace(folder.LDeckFolderName);

        using LDatabaseSession session = _lDeckFolderDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                "UPDATE deck_folder SET name = $name, position = $position WHERE deck_folder_id = $id;";
            command.Parameters.AddWithValue("$name", folder.LDeckFolderName);
            command.Parameters.AddWithValue("$position", folder.LDeckFolderPosition);
            command.Parameters.AddWithValue("$id", folder.LDeckFolderId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No deck folder carries the id '{folder.LDeckFolderId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LDeckFolderDelete(long folderId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(folderId);

        using LDatabaseSession session = _lDeckFolderDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = "DELETE FROM deck_folder WHERE deck_folder_id = $id;";
            command.Parameters.AddWithValue("$id", folderId);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }
}
