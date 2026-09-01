using System;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LNoteArchive
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
        ArgumentException.ThrowIfNullOrWhiteSpace(note.LNoteEntryId);

        using LDatabaseSession session = _lNoteArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO note (entry_id, text) VALUES ($entry, $text)
                ON CONFLICT (entry_id) DO UPDATE SET text = excluded.text;
                """;
            command.Parameters.AddWithValue("$entry", note.LNoteEntryId);
            command.Parameters.AddWithValue("$text", note.LNoteText);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public LNote? LNoteRead(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using LDatabaseSession session = _lNoteArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = "SELECT entry_id, text FROM note WHERE entry_id = $entry;";
        command.Parameters.AddWithValue("$entry", entryId);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? new LNote(reader.GetString(0), reader.GetString(1)) : null;
    }

    public void LNoteDelete(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using LDatabaseSession session = _lNoteArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = "DELETE FROM note WHERE entry_id = $entry;";
            command.Parameters.AddWithValue("$entry", entryId);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }
}
