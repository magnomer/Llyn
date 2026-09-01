using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LReferenceArchive
{
    private readonly LDatabase _lReferenceArchiveDatabase;

    public LReferenceArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lReferenceArchiveDatabase = database;
    }

    public LReference LReferenceCreate(LReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);

        LReference stored = reference with { LReferenceId = LIdentity.LIdentityCreate() };

        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
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
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public LReference? LReferenceRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        return LReferenceSingleRead(session.LDatabaseSessionConnection, id);
    }

    public IReadOnlyList<LReference> LReferenceEntryRead(string entryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);

        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT source.id,
                   source.title_state, source.title,
                   source.program_name_state, source.program_name,
                   source.channel_name_state, source.channel_name,
                   source.year_state, source.year,
                   source.url_state, source.url,
                   source.author_state
            FROM entry_source link
            JOIN source ON source.id = link.source_id
            WHERE link.entry_id = $entry
            ORDER BY link.position;
            """;
        command.Parameters.AddWithValue("$entry", entryId);

        List<LReference> references = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            references.Add(LReferenceRowRead(reader, reader.GetString(0), 1));
        }

        return references;
    }

    public LReference? LReferenceExampleRead(string exampleId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

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

    public void LReferenceUpdate(LReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentException.ThrowIfNullOrWhiteSpace(reference.LReferenceId);

        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
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
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Reference carries the id '{reference.LReferenceId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LReferenceDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

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

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM source WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public void LReferenceAuthorAttach(string referenceId, string authorId, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceId);
        ArgumentException.ThrowIfNullOrWhiteSpace(authorId);

        LReferenceLinkAttach("source_author", "source_id", "author_id", referenceId, authorId, position);
    }

    public void LReferenceAuthorDetach(string referenceId, string authorId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceId);
        ArgumentException.ThrowIfNullOrWhiteSpace(authorId);

        LReferenceLinkDetach("source_author", "source_id", "author_id", referenceId, authorId);
    }

    public void LReferenceEntryAttach(string entryId, string referenceId, int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceId);

        LReferenceLinkAttach("entry_source", "entry_id", "source_id", entryId, referenceId, position);
    }

    public void LReferenceEntryDetach(string entryId, string referenceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entryId);
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceId);

        LReferenceLinkDetach("entry_source", "entry_id", "source_id", entryId, referenceId);
    }

    public void LReferenceExampleAttach(string exampleId, string referenceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceId);

        LReferenceExampleSave(exampleId, referenceId);
    }

    public void LReferenceExampleDetach(string exampleId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        LReferenceExampleSave(exampleId, null);
    }

    private void LReferenceExampleSave(string exampleId, string? referenceId)
    {
        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = "UPDATE example SET source_id = $reference WHERE id = $example;";
            command.Parameters.AddWithValue("$reference", (object?)referenceId ?? DBNull.Value);
            command.Parameters.AddWithValue("$example", exampleId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Example carries the id '{exampleId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    private void LReferenceLinkAttach(
        string table, string ownerColumn, string memberColumn, string ownerId, string memberId, int position)
    {
        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string scope = $"{ownerColumn} = $owner";
        IReadOnlyList<string> current = LDatabaseOrder.LDatabaseOrderRead(
            connection, table, scope, ownerId, memberColumn);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                INSERT INTO {table} ({ownerColumn}, {memberColumn}, position)
                VALUES ($owner, $member, $position)
                ON CONFLICT ({ownerColumn}, {memberColumn}) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$owner", ownerId);
            command.Parameters.AddWithValue("$member", memberId);
            command.Parameters.AddWithValue("$position", current.Count);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, ownerId, memberColumn,
            LDatabaseOrder.LDatabaseOrderInsert(current, memberId, position));

        session.LDatabaseSessionCommit();
    }

    private void LReferenceLinkDetach(
        string table, string ownerColumn, string memberColumn, string ownerId, string memberId)
    {
        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        string scope = $"{ownerColumn} = $owner";

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"DELETE FROM {table} WHERE {ownerColumn} = $owner AND {memberColumn} = $member;";
            command.Parameters.AddWithValue("$owner", ownerId);
            command.Parameters.AddWithValue("$member", memberId);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, ownerId, memberColumn,
            LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, ownerId, memberColumn));

        session.LDatabaseSessionCommit();
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

        return LReferenceRowRead(reader, id, 0);
    }

    // One Reference from a reader positioned on its row, with the field block starting at `first` — 0
    // when the query selects the fields alone, 1 when it selects the id ahead of them.
    private static LReference LReferenceRowRead(SqliteDataReader reader, string id, int first)
    {
        return new LReference(
            id,
            LReferenceValueRead(reader, first),
            LReferenceValueRead(reader, first + 2),
            LReferenceValueRead(reader, first + 4),
            LReferenceValueRead(reader, first + 6),
            LReferenceValueRead(reader, first + 8),
            LReferenceStateParse(reader.GetString(first + 10)));
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
