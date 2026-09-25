using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LRegisterArchive : LRegisterVault
{
    private readonly LDatabase _lRegisterArchiveDatabase;

    public LRegisterArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lRegisterArchiveDatabase = database;
    }

    public LRegister LRegisterCreate(LRegister register)
    {
        ArgumentNullException.ThrowIfNull(register);

        LRegister stored = register;

        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO register (name_state, name, builtin)
                VALUES ($nameState, $name, $builtin)
                RETURNING register_id;
                """;
            LStateColumn.LStateColumnApply(command, "name", stored.LRegisterName);
            command.Parameters.AddWithValue("$builtin", stored.LRegisterBuiltin ? 1 : 0);
            stored = stored with { LRegisterId = (long)command.ExecuteScalar()! };
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public void LRegisterDefaultCreate(IReadOnlyList<LRegister> registers)
    {
        ArgumentNullException.ThrowIfNull(registers);

        if (registers.Count == 0)
        {
            return;
        }

        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        foreach (LRegister register in registers)
        {
            using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO register (name_state, name, builtin)
                VALUES ($nameState, $name, 1)
                ON CONFLICT (name) WHERE name IS NOT NULL
                DO UPDATE SET builtin = 1;
                """;
            LStateColumn.LStateColumnApply(command, "name", register.LRegisterName);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public IReadOnlyList<LRegister> LRegisterLoad(string language)
    {
        return LRegisterLoader.LRegisterLoaderLoad(language);
    }

    public LRegister? LRegisterRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            "SELECT register_id, name_state, name, builtin FROM register WHERE register_id = $id;";
        command.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? LRegisterRowRead(reader) : null;
    }

    public IReadOnlyList<LRegister> LRegisterRead()
    {
        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT register_id, name_state, name, builtin
            FROM register
            ORDER BY builtin DESC, rowid;
            """;

        List<LRegister> registers = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            registers.Add(LRegisterRowRead(reader));
        }

        return registers;
    }

    public IReadOnlyList<LRegister> LRegisterMeaningRead(long meaningId)
    {
        return LRegisterReferrerRead("sense_register", "sense_parent", meaningId);
    }

    public IReadOnlyList<LRegister> LRegisterCollocationRead(long collocationId)
    {
        return LRegisterReferrerRead("collocation_register", "collocation_parent", collocationId);
    }

    public void LRegisterNameUpdate(long registerId, LStateValue name)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(registerId);
        ArgumentNullException.ThrowIfNull(name);

        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                "UPDATE register SET name_state = $nameState, name = $name WHERE register_id = $id AND builtin = 0;";
            LStateColumn.LStateColumnApply(command, "name", name);
            command.Parameters.AddWithValue("$id", registerId);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public IReadOnlyDictionary<long, int> LRegisterReferenceRead()
    {
        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT register_ref, COUNT(*) FROM (
                SELECT register_ref FROM sense_register
                UNION ALL
                SELECT register_ref FROM collocation_register
            )
            GROUP BY register_ref;
            """;

        Dictionary<long, int> counts = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            counts[reader.GetInt64(0)] = reader.GetInt32(1);
        }

        return counts;
    }

    public void LRegisterMeaningAttach(long meaningId, long registerId, int position)
    {
        LRegisterReferenceAttach("sense_register", "sense_parent", meaningId, registerId, position);
    }

    public void LRegisterCollocationAttach(long collocationId, long registerId, int position)
    {
        LRegisterReferenceAttach("collocation_register", "collocation_parent", collocationId, registerId, position);
    }

    public void LRegisterMeaningDetach(long meaningId, long registerId)
    {
        LRegisterReferenceDetach("sense_register", "sense_parent", meaningId, registerId);
    }

    public void LRegisterCollocationDetach(long collocationId, long registerId)
    {
        LRegisterReferenceDetach("collocation_register", "collocation_parent", collocationId, registerId);
    }

    private static LRegister LRegisterRowRead(SqliteDataReader reader)
    {
        return new LRegister(
            reader.GetInt64(0),
            LStateColumn.LStateColumnRead(reader, 1),
            reader.GetInt64(3) != 0);
    }

    private void LRegisterReferenceAttach(
        string table,
        string column,
        long referrerId,
        long registerId,
        int position)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(registerId);

        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string scope = $"{column} = $owner";
        IReadOnlyList<long> current = LDatabaseOrder.LDatabaseOrderRead(
            connection, table, scope, referrerId, "register_ref");

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                INSERT INTO {table} ({column}, register_ref, position)
                VALUES ($referrer, $register, $position)
                ON CONFLICT ({column}, register_ref) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$register", registerId);
            command.Parameters.AddWithValue("$position", current.Count);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "register_ref",
            LDatabaseOrder.LDatabaseOrderInsert(current, registerId, position));

        session.LDatabaseSessionCommit();
    }

    private void LRegisterReferenceDetach(string table, string column, long referrerId, long registerId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(registerId);

        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        string scope = $"{column} = $owner";

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"DELETE FROM {table} WHERE {column} = $referrer AND register_ref = $register;";
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$register", registerId);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "register_ref",
            LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, referrerId, "register_ref"));

        session.LDatabaseSessionCommit();
    }

    private IReadOnlyList<LRegister> LRegisterReferrerRead(string table, string column, long referrerId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referrerId);

        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"""
            SELECT register.register_id, register.name_state, register.name, register.builtin
            FROM {table} link
            JOIN register ON register.register_id = link.register_ref
            WHERE link.{column} = $referrer
            ORDER BY link.position;
            """;
        command.Parameters.AddWithValue("$referrer", referrerId);

        List<LRegister> registers = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            registers.Add(LRegisterRowRead(reader));
        }

        return registers;
    }
}
