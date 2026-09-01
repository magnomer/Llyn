using System;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists the single Note an entry owns. The note table is keyed by <c>entry_id</c>, so at most one Note
/// exists per entry as a schema fact: saving a Note for an entry that already has one replaces it rather
/// than adding a second. Deleting the entry removes its Note through the foreign-key cascade.
/// </summary>
public sealed class LNoteArchive
{
    private readonly LDatabase _lNoteArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens connections through.</summary>
    public LNoteArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lNoteArchiveDatabase = database;
    }

    /// <summary>
    /// Writes <paramref name="note"/> as the entry's Note, replacing any Note the entry already had. The
    /// text is stored exactly as given — see the format TODO on <see cref="LNote"/>.
    /// </summary>
    public void LNoteSave(LNote note)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentException.ThrowIfNullOrWhiteSpace(note.LNoteEntryId);

        using SqliteConnection connection = _lNoteArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO note (entry_id, text) VALUES ($entry, $text)
            ON CONFLICT (entry_id) DO UPDATE SET text = excluded.text;
            """;
        command.Parameters.AddWithValue("$entry", note.LNoteEntryId);
        command.Parameters.AddWithValue("$text", note.LNoteText);
        command.ExecuteNonQuery();
    }

    /// <summary>Reads the entry's Note, or <c>null</c> when it has none.</summary>
    public LNote? LNoteRead(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using SqliteConnection connection = _lNoteArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT entry_id, text FROM note WHERE entry_id = $entry;";
        command.Parameters.AddWithValue("$entry", entryId);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? new LNote(reader.GetString(0), reader.GetString(1)) : null;
    }

    /// <summary>Removes the entry's Note, if it has one. The entry itself is untouched.</summary>
    public void LNoteDelete(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using SqliteConnection connection = _lNoteArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM note WHERE entry_id = $entry;";
        command.Parameters.AddWithValue("$entry", entryId);
        command.ExecuteNonQuery();
    }
}
