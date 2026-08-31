using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists the single pronunciation an entry owns, together with its ordered syllables and
/// representations. A pronunciation's id is assigned here on creation; its children are written as
/// ordered rows whose <c>pronunciation_id</c> and <c>position</c> come from that id and list order, so
/// reordering rewrites positions only. One pronunciation per entry is enforced by the unique
/// <c>entry_id</c> column — a second create for the same entry fails at the database. Reading returns
/// the whole aggregate by entry id; updating replaces the level, IPA, and both child lists; deleting
/// removes the syllables and representations through the foreign-key cascade.
/// </summary>
public sealed class LPronunciationArchive
{
    private readonly LDatabase _lPronunciationArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens connections through.</summary>
    public LPronunciationArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lPronunciationArchiveDatabase = database;
    }

    /// <summary>
    /// Inserts <paramref name="pronunciation"/> with a fresh opaque id, writing its syllables and
    /// representations as ordered child rows (their pronunciation id and position are assigned from the
    /// new id and list order). Returns the stored pronunciation with its id and children filled in. The
    /// whole write is one transaction. The entry's unique constraint rejects a second pronunciation for
    /// the same entry.
    /// </summary>
    public LPronunciation LPronunciationCreate(LPronunciation pronunciation)
    {
        ArgumentNullException.ThrowIfNull(pronunciation);
        ArgumentException.ThrowIfNullOrWhiteSpace(pronunciation.LPronunciationEntryId);

        string id = LIdentity.LIdentityCreate();
        LPronunciation stored = pronunciation with
        {
            LPronunciationId = id,
            LPronunciationSyllables = LPronunciationSyllableBuild(id, pronunciation.LPronunciationSyllables),
            LPronunciationRepresentations =
                LPronunciationRepresentationBuild(id, pronunciation.LPronunciationRepresentations),
        };

        using SqliteConnection connection = _lPronunciationArchiveDatabase.LDatabaseRead();
        using SqliteTransaction transaction = connection.BeginTransaction();

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO pronunciation (id, entry_id, level, ipa)
                VALUES ($id, $entry, $level, $ipa);
                """;
            command.Parameters.AddWithValue("$id", stored.LPronunciationId);
            command.Parameters.AddWithValue("$entry", stored.LPronunciationEntryId);
            command.Parameters.AddWithValue("$level", (object?)stored.LPronunciationLevel ?? DBNull.Value);
            command.Parameters.AddWithValue("$ipa", (object?)stored.LPronunciationIpa ?? DBNull.Value);
            command.ExecuteNonQuery();
        }

        LPronunciationSyllableInsert(connection, id, stored.LPronunciationSyllables);
        LPronunciationRepresentationInsert(connection, id, stored.LPronunciationRepresentations);

        transaction.Commit();
        return stored;
    }

    /// <summary>
    /// Reads the entry's pronunciation with its ordered syllables and representations, or <c>null</c>
    /// when the entry has none.
    /// </summary>
    public LPronunciation? LPronunciationRead(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using SqliteConnection connection = _lPronunciationArchiveDatabase.LDatabaseRead();

        string id;
        string? level;
        string? ipa;
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                "SELECT id, level, ipa FROM pronunciation WHERE entry_id = $entry;";
            command.Parameters.AddWithValue("$entry", entryId);

            using SqliteDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            id = reader.GetString(0);
            level = reader.IsDBNull(1) ? null : reader.GetString(1);
            ipa = reader.IsDBNull(2) ? null : reader.GetString(2);
        }

        return new LPronunciation(
            id,
            entryId,
            level,
            ipa,
            LPronunciationSyllableRead(connection, id),
            LPronunciationRepresentationRead(connection, id));
    }

    /// <summary>
    /// Replaces the level, IPA, and both child lists of the pronunciation identified by
    /// <paramref name="pronunciation"/>'s id. Existing syllable and representation rows are cleared and
    /// the supplied lists written in order, so the pronunciation id, entry link, and identity stay
    /// fixed. The whole write is one transaction.
    /// </summary>
    public void LPronunciationUpdate(LPronunciation pronunciation)
    {
        ArgumentNullException.ThrowIfNull(pronunciation);
        ArgumentException.ThrowIfNullOrWhiteSpace(pronunciation.LPronunciationId);

        string id = pronunciation.LPronunciationId;

        using SqliteConnection connection = _lPronunciationArchiveDatabase.LDatabaseRead();
        using SqliteTransaction transaction = connection.BeginTransaction();

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                "UPDATE pronunciation SET level = $level, ipa = $ipa WHERE id = $id;";
            command.Parameters.AddWithValue("$level", (object?)pronunciation.LPronunciationLevel ?? DBNull.Value);
            command.Parameters.AddWithValue("$ipa", (object?)pronunciation.LPronunciationIpa ?? DBNull.Value);
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        LPronunciationChildClear(connection, "syllable", id);
        LPronunciationChildClear(connection, "representation", id);
        LPronunciationSyllableInsert(connection, id, pronunciation.LPronunciationSyllables);
        LPronunciationRepresentationInsert(connection, id, pronunciation.LPronunciationRepresentations);

        transaction.Commit();
    }

    /// <summary>
    /// Deletes the pronunciation identified by <paramref name="id"/>. Its syllables and representations
    /// are removed by the foreign-key cascade.
    /// </summary>
    public void LPronunciationDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using SqliteConnection connection = _lPronunciationArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM pronunciation WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    private static IReadOnlyList<LSyllable> LPronunciationSyllableBuild(
        string id, IReadOnlyList<LSyllable> syllables)
    {
        ArgumentNullException.ThrowIfNull(syllables);

        List<LSyllable> rebased = [];
        for (int position = 0; position < syllables.Count; position++)
        {
            rebased.Add(syllables[position] with
            {
                LSyllablePronunciationId = id,
                LSyllablePosition = position,
            });
        }

        return rebased;
    }

    private static IReadOnlyList<LRepresentation> LPronunciationRepresentationBuild(
        string id, IReadOnlyList<LRepresentation> representations)
    {
        ArgumentNullException.ThrowIfNull(representations);

        List<LRepresentation> rebased = [];
        for (int position = 0; position < representations.Count; position++)
        {
            rebased.Add(representations[position] with
            {
                LRepresentationPronunciationId = id,
                LRepresentationPosition = position,
            });
        }

        return rebased;
    }

    private static void LPronunciationSyllableInsert(
        SqliteConnection connection, string id, IReadOnlyList<LSyllable> syllables)
    {
        for (int position = 0; position < syllables.Count; position++)
        {
            LSyllable syllable = syllables[position];
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO syllable (pronunciation_id, position, orthography, local, onset, medial,
                    nucleus, coda, tone_number, tone_local, tone_points)
                VALUES ($pronunciation, $position, $orthography, $local, $onset, $medial,
                    $nucleus, $coda, $toneNumber, $toneLocal, $tonePoints);
                """;
            command.Parameters.AddWithValue("$pronunciation", id);
            command.Parameters.AddWithValue("$position", position);
            command.Parameters.AddWithValue("$orthography", (object?)syllable.LSyllableOrthography ?? DBNull.Value);
            command.Parameters.AddWithValue("$local", (object?)syllable.LSyllableLocal ?? DBNull.Value);
            command.Parameters.AddWithValue("$onset", (object?)syllable.LSyllableOnset ?? DBNull.Value);
            command.Parameters.AddWithValue("$medial", (object?)syllable.LSyllableMedial ?? DBNull.Value);
            command.Parameters.AddWithValue("$nucleus", syllable.LSyllableNucleus);
            command.Parameters.AddWithValue("$coda", (object?)syllable.LSyllableCoda ?? DBNull.Value);
            command.Parameters.AddWithValue("$toneNumber", (object?)syllable.LSyllableToneNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("$toneLocal", (object?)syllable.LSyllableToneLocal ?? DBNull.Value);
            command.Parameters.AddWithValue("$tonePoints", (object?)syllable.LSyllableTonePoints ?? DBNull.Value);
            command.ExecuteNonQuery();
        }
    }

    private static void LPronunciationRepresentationInsert(
        SqliteConnection connection, string id, IReadOnlyList<LRepresentation> representations)
    {
        for (int position = 0; position < representations.Count; position++)
        {
            LRepresentation representation = representations[position];
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO representation (pronunciation_id, position, system, role, text, local_tone)
                VALUES ($pronunciation, $position, $system, $role, $text, $localTone);
                """;
            command.Parameters.AddWithValue("$pronunciation", id);
            command.Parameters.AddWithValue("$position", position);
            command.Parameters.AddWithValue("$system", representation.LRepresentationSystem);
            command.Parameters.AddWithValue("$role", representation.LRepresentationRole);
            command.Parameters.AddWithValue("$text", representation.LRepresentationText);
            command.Parameters.AddWithValue("$localTone", (object?)representation.LRepresentationLocalTone ?? DBNull.Value);
            command.ExecuteNonQuery();
        }
    }

    private static IReadOnlyList<LSyllable> LPronunciationSyllableRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT pronunciation_id, position, orthography, local, onset, medial, nucleus, coda,
                tone_number, tone_local, tone_points
            FROM syllable WHERE pronunciation_id = $pronunciation ORDER BY position;
            """;
        command.Parameters.AddWithValue("$pronunciation", id);

        List<LSyllable> syllables = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            syllables.Add(new LSyllable(
                reader.GetString(0),
                reader.GetInt32(1),
                reader.IsDBNull(2) ? null : reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                reader.IsDBNull(5) ? null : reader.GetString(5),
                reader.GetString(6),
                reader.IsDBNull(7) ? null : reader.GetString(7),
                reader.IsDBNull(8) ? null : reader.GetInt32(8),
                reader.IsDBNull(9) ? null : reader.GetString(9),
                reader.IsDBNull(10) ? null : reader.GetString(10)));
        }

        return syllables;
    }

    private static IReadOnlyList<LRepresentation> LPronunciationRepresentationRead(
        SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT pronunciation_id, position, system, role, text, local_tone
            FROM representation WHERE pronunciation_id = $pronunciation ORDER BY position;
            """;
        command.Parameters.AddWithValue("$pronunciation", id);

        List<LRepresentation> representations = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            representations.Add(new LRepresentation(
                reader.GetString(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.IsDBNull(5) ? null : reader.GetString(5)));
        }

        return representations;
    }

    private static void LPronunciationChildClear(SqliteConnection connection, string table, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = $"DELETE FROM {table} WHERE pronunciation_id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }
}
