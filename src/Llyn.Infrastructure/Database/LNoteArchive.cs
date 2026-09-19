using System;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LNoteArchive : LNoteVault
{
    private readonly LDatabase _lNoteArchiveDatabase;

    public LNoteArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lNoteArchiveDatabase = database;
    }

    public void LNoteSave(LNote note)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(note.LNoteEntryId);

        using LDatabaseSession session = _lNoteArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO note (entry_parent, text) VALUES ($entry, $text)
                ON CONFLICT (entry_parent) DO UPDATE SET text = excluded.text;
                """;
            command.Parameters.AddWithValue("$entry", note.LNoteEntryId);
            command.Parameters.AddWithValue("$text", note.LNoteText);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public LNote? LNoteRead(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lNoteArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = "SELECT entry_parent, text FROM note WHERE entry_parent = $entry;";
        command.Parameters.AddWithValue("$entry", entryId);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? new LNote(reader.GetInt64(0), reader.GetString(1)) : null;
    }

    public void LNoteDelete(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lNoteArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = "DELETE FROM note WHERE entry_parent = $entry;";
            command.Parameters.AddWithValue("$entry", entryId);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }
}
