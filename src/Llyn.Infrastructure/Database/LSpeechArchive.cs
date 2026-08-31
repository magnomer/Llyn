using System;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists and resolves the language-controlled part-of-speech display vocabulary. An entry's POS
/// rows store only the stable value id; the display name for a language lives here and is resolved by
/// <c>(language, value_id)</c>, so a name is never copied onto an entry's rows.
/// </summary>
public sealed class LSpeechArchive
{
    private readonly LDatabase _lSpeechArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens connections through.</summary>
    public LSpeechArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lSpeechArchiveDatabase = database;
    }

    /// <summary>
    /// Adds or replaces one vocabulary entry, keyed by <c>(language, value_id)</c>, with its display
    /// name and display order.
    /// </summary>
    public void LSpeechValueCreate(LSpeechValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        using SqliteConnection connection = _lSpeechArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO part_of_speech_value (language, value_id, display_name, position)
            VALUES ($language, $value, $name, $position)
            ON CONFLICT (language, value_id)
            DO UPDATE SET display_name = excluded.display_name, position = excluded.position;
            """;
        command.Parameters.AddWithValue("$language", value.LSpeechValueLanguage);
        command.Parameters.AddWithValue("$value", value.LSpeechValueId);
        command.Parameters.AddWithValue("$name", value.LSpeechValueName);
        command.Parameters.AddWithValue("$position", value.LSpeechValuePosition);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Resolves the display name for <paramref name="valueId"/> in <paramref name="language"/>, or
    /// <c>null</c> when the vocabulary has no such entry.
    /// </summary>
    public string? LSpeechValueRead(string language, string valueId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(valueId);

        using SqliteConnection connection = _lSpeechArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT display_name FROM part_of_speech_value
            WHERE language = $language AND value_id = $value;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$value", valueId);

        object? result = command.ExecuteScalar();
        return result is string name ? name : null;
    }
}
