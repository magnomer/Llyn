using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists bibliographic References — independent data no Entry, Example, or Author owns. A Reference
/// is created once with an opaque id and is then cited two ways: an Entry cites any number of them in
/// its own order through <c>entry_source</c>, and an Example cites at most one through its
/// <c>example.source_id</c> column. Both are pointers — detaching a citation leaves the Reference
/// standing, and <see cref="LReferenceDelete"/> refuses to run while any citation remains.
/// <para>
/// Every field is stored as a state column beside its value column so "never filled in", "recorded as
/// unknown", and "this value" stay three different facts; the value column is written only when the
/// state is specified. The Authors a Reference credits are attached in order through
/// <c>source_author</c>: attaching and detaching write association rows only, deleting the Reference
/// drops those rows and never an Author, and the Authors themselves live in
/// <see cref="LAuthorArchive"/>.
/// </para>
/// </summary>
public sealed class LReferenceArchive
{
    private readonly LDatabase _lReferenceArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens connections through.</summary>
    public LReferenceArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lReferenceArchiveDatabase = database;
    }

    /// <summary>
    /// Inserts <paramref name="reference"/> with a fresh opaque id and returns the stored Reference with
    /// that id filled in. Each field is written as its state plus, for a specified field alone, its
    /// value. The new Reference is cited by nothing and credits no Author until one is attached.
    /// </summary>
    public LReference LReferenceCreate(LReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);

        LReference stored = reference with { LReferenceId = LIdentity.LIdentityCreate() };

        using SqliteConnection connection = _lReferenceArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO source (
                id,
                title_state, title,
                program_name_state, program_name,
                channel_name_state, channel_name,
                year_state, year,
                url_state, url,
                author_state)
            VALUES (
                $id,
                $titleState, $title,
                $programState, $program,
                $channelState, $channel,
                $yearState, $year,
                $urlState, $url,
                $authorState);
            """;
        command.Parameters.AddWithValue("$id", stored.LReferenceId);
        LReferenceValueApply(command, "title", stored.LReferenceTitle);
        LReferenceValueApply(command, "program", stored.LReferenceProgram);
        LReferenceValueApply(command, "channel", stored.LReferenceChannel);
        LReferenceValueApply(command, "year", stored.LReferenceYear);
        LReferenceValueApply(command, "url", stored.LReferenceUrl);
        command.Parameters.AddWithValue("$authorState", LReferenceStateFormat(stored.LReferenceAuthorState));
        command.ExecuteNonQuery();

        return stored;
    }

    /// <summary>Reads the Reference identified by <paramref name="id"/>, or <c>null</c> when none exists.</summary>
    public LReference? LReferenceRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using SqliteConnection connection = _lReferenceArchiveDatabase.LDatabaseRead();
        return LReferenceSingleRead(connection, id);
    }

    /// <summary>
    /// Reads the References an Entry cites, in the order that Entry gives them. Another Entry citing the
    /// same References may order them differently — the order lives on the association row.
    /// </summary>
    public IReadOnlyList<LReference> LReferenceEntryRead(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using SqliteConnection connection = _lReferenceArchiveDatabase.LDatabaseRead();

        List<string> ids = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                "SELECT source_id FROM entry_source WHERE entry_id = $entry ORDER BY position;";
            command.Parameters.AddWithValue("$entry", entryId);
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ids.Add(reader.GetString(0));
            }
        }

        List<LReference> references = [];
        foreach (string id in ids)
        {
            LReference? reference = LReferenceSingleRead(connection, id);
            if (reference is not null)
            {
                references.Add(reference);
            }
        }

        return references;
    }

    /// <summary>
    /// Reads the single Reference an Example cites, or <c>null</c> when the Example cites none or does
    /// not exist.
    /// </summary>
    public LReference? LReferenceExampleRead(string exampleId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        using SqliteConnection connection = _lReferenceArchiveDatabase.LDatabaseRead();

        string id;
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "SELECT source_id FROM example WHERE id = $example;";
            command.Parameters.AddWithValue("$example", exampleId);
            using SqliteDataReader reader = command.ExecuteReader();
            if (!reader.Read() || reader.IsDBNull(0))
            {
                return null;
            }

            id = reader.GetString(0);
        }

        return LReferenceSingleRead(connection, id);
    }

    /// <summary>
    /// Rewrites every field of the Reference identified by <paramref name="reference"/>'s id, states and
    /// values alike. The id, the Authors credited, and every citation pointing at it are untouched, so an
    /// update never changes where the Reference appears or in what order.
    /// </summary>
    public void LReferenceUpdate(LReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentException.ThrowIfNullOrWhiteSpace(reference.LReferenceId);

        using SqliteConnection connection = _lReferenceArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE source
            SET title_state = $titleState, title = $title,
                program_name_state = $programState, program_name = $program,
                channel_name_state = $channelState, channel_name = $channel,
                year_state = $yearState, year = $year,
                url_state = $urlState, url = $url,
                author_state = $authorState
            WHERE id = $id;
            """;
        LReferenceValueApply(command, "title", reference.LReferenceTitle);
        LReferenceValueApply(command, "program", reference.LReferenceProgram);
        LReferenceValueApply(command, "channel", reference.LReferenceChannel);
        LReferenceValueApply(command, "year", reference.LReferenceYear);
        LReferenceValueApply(command, "url", reference.LReferenceUrl);
        command.Parameters.AddWithValue("$authorState", LReferenceStateFormat(reference.LReferenceAuthorState));
        command.Parameters.AddWithValue("$id", reference.LReferenceId);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Deletes the Reference identified by <paramref name="id"/> together with the author links it owns.
    /// Guarded: while any Entry or Example still cites the Reference, nothing is deleted and an
    /// <see cref="InvalidOperationException"/> is thrown — remove those citations first. The Authors it
    /// credited survive; only the links between them and this Reference go.
    /// </summary>
    public void LReferenceDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using SqliteConnection connection = _lReferenceArchiveDatabase.LDatabaseRead();

        using (SqliteCommand guard = connection.CreateCommand())
        {
            guard.CommandText =
                """
                SELECT
                    (SELECT COUNT(*) FROM entry_source WHERE source_id = $id)
                    + (SELECT COUNT(*) FROM example WHERE source_id = $id);
                """;
            guard.Parameters.AddWithValue("$id", id);
            long citations = Convert.ToInt64(guard.ExecuteScalar());
            if (citations > 0)
            {
                throw new InvalidOperationException(
                    $"Reference {id} is still cited {citations} time(s); remove every citation before deleting it.");
            }
        }

        // source_author cascades from source, so the author links go with the row and the Authors stay.
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM source WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Credits an existing Author on this Reference at <paramref name="position"/> in the Reference's own
    /// author order. The Author row itself is untouched and stays available to every other Reference.
    /// </summary>
    public void LReferenceAuthorAttach(string referenceId, string authorId, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceId);
        ArgumentException.ThrowIfNullOrWhiteSpace(authorId);

        using SqliteConnection connection = _lReferenceArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "INSERT INTO source_author (source_id, author_id, position) VALUES ($reference, $author, $position);";
        command.Parameters.AddWithValue("$reference", referenceId);
        command.Parameters.AddWithValue("$author", authorId);
        command.Parameters.AddWithValue("$position", position);
        command.ExecuteNonQuery();
    }

    /// <summary>Removes this Reference's credit for an Author. The Author and its other credits survive.</summary>
    public void LReferenceAuthorDetach(string referenceId, string authorId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceId);
        ArgumentException.ThrowIfNullOrWhiteSpace(authorId);

        using SqliteConnection connection = _lReferenceArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM source_author WHERE source_id = $reference AND author_id = $author;";
        command.Parameters.AddWithValue("$reference", referenceId);
        command.Parameters.AddWithValue("$author", authorId);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Cites this Reference from an Entry at <paramref name="position"/> in that Entry's citation order.
    /// An Entry may cite any number of References; each keeps its own position there.
    /// </summary>
    public void LReferenceEntryAttach(string entryId, string referenceId, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceId);

        using SqliteConnection connection = _lReferenceArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            "INSERT INTO entry_source (entry_id, source_id, position) VALUES ($entry, $reference, $position);";
        command.Parameters.AddWithValue("$entry", entryId);
        command.Parameters.AddWithValue("$reference", referenceId);
        command.Parameters.AddWithValue("$position", position);
        command.ExecuteNonQuery();
    }

    /// <summary>Removes an Entry's citation of a Reference. The Reference and its other citations survive.</summary>
    public void LReferenceEntryDetach(string entryId, string referenceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceId);

        using SqliteConnection connection = _lReferenceArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM entry_source WHERE entry_id = $entry AND source_id = $reference;";
        command.Parameters.AddWithValue("$entry", entryId);
        command.Parameters.AddWithValue("$reference", referenceId);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Makes this Reference the single one an Example cites, replacing whatever it cited before — an
    /// Example holds one source column, so there is nothing to order and nothing to duplicate. The
    /// Reference is only pointed at, never owned.
    /// </summary>
    public void LReferenceExampleAttach(string exampleId, string referenceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceId);

        LReferenceExampleSave(exampleId, referenceId);
    }

    /// <summary>
    /// Clears the Reference an Example cites. Only the pointer is cleared: both the Example and the
    /// Reference stay exactly as they were.
    /// </summary>
    public void LReferenceExampleDetach(string exampleId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        LReferenceExampleSave(exampleId, null);
    }

    private void LReferenceExampleSave(string exampleId, string? referenceId)
    {
        using SqliteConnection connection = _lReferenceArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE example SET source_id = $reference WHERE id = $example;";
        command.Parameters.AddWithValue("$reference", (object?)referenceId ?? DBNull.Value);
        command.Parameters.AddWithValue("$example", exampleId);
        command.ExecuteNonQuery();
    }

    // Every field is written the same way: its state always, its text only while the state is specified.
    // Binding both from one place is what keeps "unknown" and "unspecified" free of stray values, which
    // the source table's check constraints then hold to.
    private static void LReferenceValueApply(SqliteCommand command, string field, LReferenceValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        command.Parameters.AddWithValue($"${field}State", LReferenceStateFormat(value.LReferenceValueState));
        command.Parameters.AddWithValue(
            $"${field}",
            value.LReferenceValueState == LState.LStateSpecified && value.LReferenceValueText is not null
                ? value.LReferenceValueText
                : DBNull.Value);
    }

    private static LReference? LReferenceSingleRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT
                title_state, title,
                program_name_state, program_name,
                channel_name_state, channel_name,
                year_state, year,
                url_state, url,
                author_state
            FROM source
            WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);
        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LReference(
            id,
            LReferenceValueRead(reader, 0),
            LReferenceValueRead(reader, 2),
            LReferenceValueRead(reader, 4),
            LReferenceValueRead(reader, 6),
            LReferenceValueRead(reader, 8),
            LReferenceStateParse(reader.GetString(10)));
    }

    private static LReferenceValue LReferenceValueRead(SqliteDataReader reader, int state)
    {
        return new LReferenceValue(
            LReferenceStateParse(reader.GetString(state)),
            reader.IsDBNull(state + 1) ? null : reader.GetString(state + 1));
    }

    // The state text is a persisted data-contract value, so it stays the lowercase word the schema
    // documents rather than the member name of the enum.
    private static string LReferenceStateFormat(LState state)
    {
        return state switch
        {
            LState.LStateUnknown => "unknown",
            LState.LStateSpecified => "specified",
            _ => "unspecified",
        };
    }

    private static LState LReferenceStateParse(string state)
    {
        return state switch
        {
            "unknown" => LState.LStateUnknown,
            "specified" => LState.LStateSpecified,
            _ => LState.LStateUnspecified,
        };
    }
}
