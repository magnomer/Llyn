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

        LReference stored = reference with
        {
            LReferenceId = string.IsNullOrWhiteSpace(reference.LReferenceId)
                ? LIdentity.LIdentityCreate()
                : reference.LReferenceId,
        };

        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO source (
                    id,
                    title_state, title,
                    year_state, year,
                    kind,
                    note_state, note,
                    url_state, url,
                    author_state)
                VALUES (
                    $id,
                    $titleState, $title,
                    $yearState, $year,
                    $kind,
                    $noteState, $note,
                    $urlState, $url,
                    $authorState);
                """;
            command.Parameters.AddWithValue("$id", stored.LReferenceId);
            LStateColumn.LStateColumnApply(command, "title", stored.LReferenceTitle);
            LStateColumn.LStateColumnApply(command, "year", stored.LReferenceYear);
            command.Parameters.AddWithValue("$kind", LReference.LReferenceKindFormat(stored.LReferenceKind));
            LStateColumn.LStateColumnApply(command, "note", stored.LReferenceNote);
            LStateColumn.LStateColumnApply(command, "url", stored.LReferenceUrl);
            command.Parameters.AddWithValue("$authorState", LStateColumn.LStateColumnFormat(stored.LReferenceAuthorState));
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

    public IReadOnlyList<LReference> LReferenceAllRead()
    {
        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT id,
                   title_state, title,
                   year_state, year,
                   kind,
                   note_state, note,
                   url_state, url,
                   author_state
            FROM source
            ORDER BY title;
            """;

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
                    year_state = $yearState, year = $year,
                    kind = $kind,
                    note_state = $noteState, note = $note,
                    url_state = $urlState, url = $url,
                    author_state = $authorState
                WHERE id = $id;
                """;
            LStateColumn.LStateColumnApply(command, "title", reference.LReferenceTitle);
            LStateColumn.LStateColumnApply(command, "year", reference.LReferenceYear);
            command.Parameters.AddWithValue("$kind", LReference.LReferenceKindFormat(reference.LReferenceKind));
            LStateColumn.LStateColumnApply(command, "note", reference.LReferenceNote);
            LStateColumn.LStateColumnApply(command, "url", reference.LReferenceUrl);
            command.Parameters.AddWithValue("$authorState", LStateColumn.LStateColumnFormat(reference.LReferenceAuthorState));
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
        LReferenceDelete(id, false);
    }

    public void LReferenceDelete(string id, bool detach)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        if (detach)
        {
            LReferenceUsage.LReferenceUsageClear(connection, id);
        }

        int citations = LReferenceUsage.LReferenceUsageRead(connection, id);
        if (citations > 0)
        {
            throw new InvalidOperationException(
                $"Reference {id} is still cited {citations} time(s); remove every citation before deleting it.");
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

    public void LReferenceExampleAttach(string exampleId, string referenceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceId);

        LReferenceExampleSave(exampleId, LStateValue.LStateValueCreate(referenceId));
    }

    public void LReferenceExampleDetach(string exampleId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exampleId);

        LReferenceExampleSave(exampleId, LStateValue.LStateValueUnspecified);
    }

    private void LReferenceExampleSave(string exampleId, LStateValue reference)
    {
        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                "UPDATE example SET source_state = $referenceState, source_id = $reference WHERE id = $example;";
            LStateColumn.LStateColumnApply(command, "reference", reference);
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

    private static LReference? LReferenceSingleRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT
                title_state, title,
                year_state, year,
                kind,
                note_state, note,
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

    private static LReference LReferenceRowRead(SqliteDataReader reader, string id, int first)
    {
        return new LReference(
            id,
            LStateColumn.LStateColumnRead(reader, first),
            LStateColumn.LStateColumnRead(reader, first + 2),
            LReference.LReferenceKindParse(reader.GetString(first + 4)),
            LStateColumn.LStateColumnRead(reader, first + 5),
            LStateColumn.LStateColumnRead(reader, first + 7),
            LStateColumn.LStateColumnParse(reader.GetString(first + 9)));
    }

}
