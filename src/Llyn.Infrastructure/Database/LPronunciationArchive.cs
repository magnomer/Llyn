using System;
using System.Collections.Generic;
using System.Globalization;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LPronunciationArchive
{
    private readonly LDatabase _lPronunciationArchiveDatabase;

    public LPronunciationArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lPronunciationArchiveDatabase = database;
    }

    public LPronunciation LPronunciationCreate(LPronunciation pronunciation)
    {
        ArgumentNullException.ThrowIfNull(pronunciation);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pronunciation.LPronunciationEntryId);

        using LDatabaseSession session = _lPronunciationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LPronunciation stored = pronunciation with
        {
            LPronunciationPosition = LPronunciationSiblingRead(connection, pronunciation.LPronunciationEntryId).Count,
            LPronunciationVariety = LPronunciationVarietyRead(pronunciation.LPronunciationVariety),
            LPronunciationIpa = LPronunciationVarietyRead(pronunciation.LPronunciationIpa),
        };

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO pronunciation (entry_parent, position, variety, ipa)
                VALUES ($entry, $position, $variety, $ipa)
                RETURNING pronunciation_id;
                """;
            command.Parameters.AddWithValue("$entry", stored.LPronunciationEntryId);
            command.Parameters.AddWithValue("$position", stored.LPronunciationPosition);
            command.Parameters.AddWithValue("$variety", (object?)stored.LPronunciationVariety ?? DBNull.Value);
            command.Parameters.AddWithValue("$ipa", (object?)stored.LPronunciationIpa ?? DBNull.Value);
            stored = stored with { LPronunciationId = (long)command.ExecuteScalar()! };
        }

        long id = stored.LPronunciationId;
        stored = stored with
        {
            LPronunciationSyllables = LPronunciationSyllableBuild(id, stored.LPronunciationSyllables),
        };

        LPronunciationSyllableInsert(connection, id, stored.LPronunciationSyllables);

        session.LDatabaseSessionCommit();
        return stored;
    }

    public IReadOnlyList<LPronunciation> LPronunciationRead(long entryId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);

        using LDatabaseSession session = _lPronunciationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        List<LPronunciation> rows = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                SELECT pronunciation_id, position, variety, ipa
                FROM pronunciation WHERE entry_parent = $entry ORDER BY position;
                """;
            command.Parameters.AddWithValue("$entry", entryId);

            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                rows.Add(new LPronunciation(
                    reader.GetInt64(0),
                    entryId,
                    reader.GetInt32(1),
                    reader.IsDBNull(2) ? null : reader.GetString(2),
                    reader.IsDBNull(3) ? null : reader.GetString(3),
                    []));
            }
        }

        for (int index = 0; index < rows.Count; index++)
        {
            rows[index] = rows[index] with
            {
                LPronunciationSyllables = LPronunciationSyllableRead(connection, rows[index].LPronunciationId),
            };
        }

        return rows;
    }

    public void LPronunciationUpdate(LPronunciation pronunciation)
    {
        ArgumentNullException.ThrowIfNull(pronunciation);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pronunciation.LPronunciationId);

        long id = pronunciation.LPronunciationId;

        using LDatabaseSession session = _lPronunciationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                "UPDATE pronunciation SET variety = $variety, ipa = $ipa WHERE pronunciation_id = $id;";
            command.Parameters.AddWithValue(
                "$variety",
                (object?)LPronunciationVarietyRead(pronunciation.LPronunciationVariety) ?? DBNull.Value);
            command.Parameters.AddWithValue(
                "$ipa", (object?)LPronunciationVarietyRead(pronunciation.LPronunciationIpa) ?? DBNull.Value);
            command.Parameters.AddWithValue("$id", id);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No pronunciation carries the id '{id}'.");
            }
        }

        LPronunciationSyllableClear(connection, id);
        LPronunciationSyllableInsert(connection, id, pronunciation.LPronunciationSyllables);

        session.LDatabaseSessionCommit();
    }

    public void LPronunciationOrderSet(long entryId, IReadOnlyList<long> order)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        ArgumentNullException.ThrowIfNull(order);

        using LDatabaseSession session = _lPronunciationArchiveDatabase.LDatabaseSessionStart();
        LDatabaseOrder.LDatabaseOrderNormalize(
            session.LDatabaseSessionConnection, "pronunciation", "entry_parent = $owner", entryId, "pronunciation_id", order);
        session.LDatabaseSessionCommit();
    }

    public void LPronunciationDelete(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lPronunciationArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        long? entryId = LPronunciationHolderRead(connection, id);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM pronunciation WHERE pronunciation_id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        if (entryId is not null)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, "pronunciation", "entry_parent = $owner", entryId,
                "pronunciation_id", LPronunciationSiblingRead(connection, entryId.Value));
        }

        session.LDatabaseSessionCommit();
    }

    public void LPronunciationAudioSave(long pronunciationId, string file, string? source)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pronunciationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(file);

        using LDatabaseSession session = _lPronunciationArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO pronunciation_audio (pronunciation_parent, file, site, added_utc)
                VALUES ($pronunciation, $file, $source, $added)
                ON CONFLICT (pronunciation_parent) DO UPDATE SET
                    file = excluded.file,
                    site = excluded.site,
                    added_utc = excluded.added_utc;
                """;
            command.Parameters.AddWithValue("$pronunciation", pronunciationId);
            command.Parameters.AddWithValue("$file", file);
            command.Parameters.AddWithValue("$source", (object?)source ?? DBNull.Value);
            command.Parameters.AddWithValue(
                "$added", DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public LPronunciationAudio? LPronunciationAudioRead(long pronunciationId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pronunciationId);

        using LDatabaseSession session = _lPronunciationArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT pronunciation_parent, file, site, added_utc
            FROM pronunciation_audio WHERE pronunciation_parent = $pronunciation;
            """;
        command.Parameters.AddWithValue("$pronunciation", pronunciationId);

        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LPronunciationAudio(
            reader.GetInt64(0),
            reader.GetString(1),
            reader.IsDBNull(2) ? null : reader.GetString(2),
            reader.GetString(3));
    }

    private static long? LPronunciationHolderRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT entry_parent FROM pronunciation WHERE pronunciation_id = $id;";
        command.Parameters.AddWithValue("$id", id);
        object? holder = command.ExecuteScalar();
        return holder is null or DBNull ? null : Convert.ToInt64(holder, CultureInfo.InvariantCulture);
    }

    private static IReadOnlyList<long> LPronunciationSiblingRead(SqliteConnection connection, long entryId)
    {
        return LDatabaseOrder.LDatabaseOrderRead(connection, "pronunciation", "entry_parent = $owner", entryId, "pronunciation_id");
    }

    private static string? LPronunciationVarietyRead(string? text)
    {
        return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
    }

    private static IReadOnlyList<LSyllable> LPronunciationSyllableBuild(
        long id, IReadOnlyList<LSyllable> syllables)
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

    private static void LPronunciationSyllableInsert(
        SqliteConnection connection, long id, IReadOnlyList<LSyllable> syllables)
    {
        for (int position = 0; position < syllables.Count; position++)
        {
            LSyllable syllable = syllables[position];
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO syllable (pronunciation_parent, position, onset, medial,
                    nucleus, coda, tone_number, tone_points)
                VALUES ($pronunciation, $position, $onset, $medial,
                    $nucleus, $coda, $toneNumber, $tonePoints);
                """;
            command.Parameters.AddWithValue("$pronunciation", id);
            command.Parameters.AddWithValue("$position", position);
            command.Parameters.AddWithValue("$onset", (object?)syllable.LSyllableOnset ?? DBNull.Value);
            command.Parameters.AddWithValue("$medial", (object?)syllable.LSyllableMedial ?? DBNull.Value);
            command.Parameters.AddWithValue("$nucleus", syllable.LSyllableNucleus);
            command.Parameters.AddWithValue("$coda", (object?)syllable.LSyllableCoda ?? DBNull.Value);
            command.Parameters.AddWithValue("$toneNumber", (object?)syllable.LSyllableToneNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("$tonePoints", (object?)syllable.LSyllableTonePoints ?? DBNull.Value);
            command.ExecuteNonQuery();
        }
    }

    private static IReadOnlyList<LSyllable> LPronunciationSyllableRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT pronunciation_parent, position, onset, medial, nucleus, coda,
                tone_number, tone_points
            FROM syllable WHERE pronunciation_parent = $pronunciation ORDER BY position;
            """;
        command.Parameters.AddWithValue("$pronunciation", id);

        List<LSyllable> syllables = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            syllables.Add(new LSyllable(
                reader.GetInt64(0),
                reader.GetInt32(1),
                reader.IsDBNull(2) ? null : reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.GetString(4),
                reader.IsDBNull(5) ? null : reader.GetString(5),
                reader.IsDBNull(6) ? null : reader.GetInt32(6),
                reader.IsDBNull(7) ? null : reader.GetString(7)));
        }

        return syllables;
    }

    private static void LPronunciationSyllableClear(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM syllable WHERE pronunciation_parent = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }
}
