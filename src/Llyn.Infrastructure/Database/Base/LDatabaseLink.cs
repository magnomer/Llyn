using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LDatabaseLink
{
    private readonly LDatabase _lDatabaseLinkDatabase;
    private readonly string _lDatabaseLinkTable;
    private readonly string _lDatabaseLinkOwner;
    private readonly string _lDatabaseLinkMember;

    public LDatabaseLink(LDatabase database, string table, string owner, string member)
    {
        ArgumentNullException.ThrowIfNull(database);
        ArgumentException.ThrowIfNullOrWhiteSpace(table);
        ArgumentException.ThrowIfNullOrWhiteSpace(owner);
        ArgumentException.ThrowIfNullOrWhiteSpace(member);
        _lDatabaseLinkDatabase = database;
        _lDatabaseLinkTable = table;
        _lDatabaseLinkOwner = owner;
        _lDatabaseLinkMember = member;
    }

    public void LDatabaseLinkAttach(long ownerId, long memberId, int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ownerId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(memberId);

        using LDatabaseSession session = _lDatabaseLinkDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string scope = $"{_lDatabaseLinkOwner} = $owner";
        IReadOnlyList<long> current = LDatabaseOrder.LDatabaseOrderRead(
            connection, _lDatabaseLinkTable, scope, ownerId, _lDatabaseLinkMember);

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                INSERT INTO {_lDatabaseLinkTable} ({_lDatabaseLinkOwner}, {_lDatabaseLinkMember}, position)
                VALUES ($referrer, $member, $position)
                ON CONFLICT ({_lDatabaseLinkOwner}, {_lDatabaseLinkMember}) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$referrer", ownerId);
            command.Parameters.AddWithValue("$member", memberId);
            command.Parameters.AddWithValue("$position", current.Count);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, _lDatabaseLinkTable, scope, ownerId, _lDatabaseLinkMember,
            LDatabaseOrder.LDatabaseOrderInsert(current, memberId, position));

        session.LDatabaseSessionCommit();
    }

    public void LDatabaseLinkDetach(long ownerId, long memberId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ownerId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(memberId);

        using LDatabaseSession session = _lDatabaseLinkDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        string scope = $"{_lDatabaseLinkOwner} = $owner";

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"DELETE FROM {_lDatabaseLinkTable} WHERE {_lDatabaseLinkOwner} = $referrer "
                + $"AND {_lDatabaseLinkMember} = $member;";
            command.Parameters.AddWithValue("$referrer", ownerId);
            command.Parameters.AddWithValue("$member", memberId);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, _lDatabaseLinkTable, scope, ownerId, _lDatabaseLinkMember,
            LDatabaseOrder.LDatabaseOrderRead(connection, _lDatabaseLinkTable, scope, ownerId, _lDatabaseLinkMember));

        session.LDatabaseSessionCommit();
    }

    public void LDatabaseLinkDelete(long memberId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(memberId);

        using LDatabaseSession session = _lDatabaseLinkDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        List<long> referrers = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"SELECT {_lDatabaseLinkOwner} FROM {_lDatabaseLinkTable} WHERE {_lDatabaseLinkMember} = $member;";
            command.Parameters.AddWithValue("$member", memberId);
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                referrers.Add(reader.GetInt64(0));
            }
        }

        if (referrers.Count == 0)
        {
            return;
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"DELETE FROM {_lDatabaseLinkTable} WHERE {_lDatabaseLinkMember} = $member;";
            command.Parameters.AddWithValue("$member", memberId);
            command.ExecuteNonQuery();
        }

        string scope = $"{_lDatabaseLinkOwner} = $owner";
        foreach (long referrer in referrers)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, _lDatabaseLinkTable, scope, referrer, _lDatabaseLinkMember,
                LDatabaseOrder.LDatabaseOrderRead(
                    connection, _lDatabaseLinkTable, scope, referrer, _lDatabaseLinkMember));
        }

        session.LDatabaseSessionCommit();
    }
}
