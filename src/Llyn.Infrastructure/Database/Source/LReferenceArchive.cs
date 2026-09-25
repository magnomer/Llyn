using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LReferenceArchive : LReferenceVault
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

        LReference stored = reference;

        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO reference (
                    title_state, title,
                    year_state, year,
                    kind,
                    note_state, note,
                    url_state, url,
                    author_state)
                VALUES (
                    $titleState, $title,
                    $yearState, $year,
                    $kind,
                    $noteState, $note,
                    $urlState, $url,
                    $authorState)
                RETURNING reference_id;
                """;
            LStateColumn.LStateColumnApply(command, "title", stored.LReferenceTitle);
            LStateColumn.LStateColumnApply(command, "year", stored.LReferenceYear);
            command.Parameters.AddWithValue("$kind", LReference.LReferenceKindFormat(stored.LReferenceKind));
            LStateColumn.LStateColumnApply(command, "note", stored.LReferenceNote);
            LStateColumn.LStateColumnApply(command, "url", stored.LReferenceUrl);
            LStateColumn.LStateColumnApply(command, "author", stored.LReferenceAuthorState);
            stored = stored with { LReferenceId = (long)command.ExecuteScalar()! };
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public LReference? LReferenceRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        return LReferenceSingleRead(session.LDatabaseSessionConnection, id);
    }

    public IReadOnlyList<LReference> LReferenceAllRead()
    {
        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT reference_id,
                   title_state, title,
                   year_state, year,
                   kind,
                   note_state, note,
                   url_state, url,
                   author_state
            FROM reference
            ORDER BY title;
            """;

        List<LReference> references = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            references.Add(LReferenceRowRead(reader, reader.GetInt64(0), 1));
        }

        return references;
    }

    public IReadOnlyDictionary<long, int> LReferenceUsageRead()
    {
        return new LReferenceUsage(_lReferenceArchiveDatabase).LReferenceUsageRead();
    }

    public IReadOnlyList<LUsage> LReferenceUsageRead(long id)
    {
        return new LReferenceUsage(_lReferenceArchiveDatabase).LReferenceUsageRead(id);
    }

    public void LReferenceUpdate(LReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(reference.LReferenceId);

        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                UPDATE reference
                SET title_state = $titleState, title = $title,
                    year_state = $yearState, year = $year,
                    kind = $kind,
                    note_state = $noteState, note = $note,
                    url_state = $urlState, url = $url,
                    author_state = $authorState
                WHERE reference_id = $id;
                """;
            LStateColumn.LStateColumnApply(command, "title", reference.LReferenceTitle);
            LStateColumn.LStateColumnApply(command, "year", reference.LReferenceYear);
            command.Parameters.AddWithValue("$kind", LReference.LReferenceKindFormat(reference.LReferenceKind));
            LStateColumn.LStateColumnApply(command, "note", reference.LReferenceNote);
            LStateColumn.LStateColumnApply(command, "url", reference.LReferenceUrl);
            LStateColumn.LStateColumnApply(command, "author", reference.LReferenceAuthorState);
            command.Parameters.AddWithValue("$id", reference.LReferenceId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Reference carries the id '{reference.LReferenceId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LReferenceDelete(long id, bool detach)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

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
            command.CommandText = "DELETE FROM reference WHERE reference_id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public void LReferenceAuthorAttach(long referenceId, long authorId, int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referenceId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(authorId);

        LReferenceLinkAttach("reference_author", "reference_parent", "author_ref", referenceId, authorId, position);
    }

    public void LReferenceAuthorDetach(long referenceId, long authorId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referenceId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(authorId);

        LReferenceLinkDetach("reference_author", "reference_parent", "author_ref", referenceId, authorId);
    }

    private void LReferenceLinkAttach(
        string table, string ownerColumn, string memberColumn, long ownerId, long memberId, int position)
    {
        using LDatabaseSession session = _lReferenceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string scope = $"{ownerColumn} = $owner";
        IReadOnlyList<long> current = LDatabaseOrder.LDatabaseOrderRead(
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
        string table, string ownerColumn, string memberColumn, long ownerId, long memberId)
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

    private static LReference? LReferenceSingleRead(SqliteConnection connection, long id)
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
            FROM reference
            WHERE reference_id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);
        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return LReferenceRowRead(reader, id, 0);
    }

    private static LReference LReferenceRowRead(SqliteDataReader reader, long id, int first)
    {
        return new LReference(
            id,
            LStateColumn.LStateColumnRead(reader, first),
            LStateColumn.LStateColumnRead(reader, first + 2),
            LReference.LReferenceKindParse(reader.GetString(first + 4)),
            LStateColumn.LStateColumnRead(reader, first + 5),
            LStateColumn.LStateColumnRead(reader, first + 7),
            LStateColumn.LStateColumnLoad(reader, first + 9));
    }

}
