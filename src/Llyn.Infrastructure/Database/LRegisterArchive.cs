using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LRegisterArchive
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

        LRegister stored = register with
        {
            LRegisterId = string.IsNullOrWhiteSpace(register.LRegisterId)
                ? LIdentity.LIdentityCreate()
                : register.LRegisterId,
        };

        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO register (id, name_state, name, language, builtin)
                VALUES ($id, $nameState, $name, $language, $builtin);
                """;
            command.Parameters.AddWithValue("$id", stored.LRegisterId);
            LStateColumn.LStateColumnApply(command, "name", stored.LRegisterName);
            LRegisterLanguageApply(command, stored);
            command.ExecuteNonQuery();
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
                INSERT INTO register (id, name_state, name, language, builtin)
                VALUES ($id, $nameState, $name, $language, $builtin)
                ON CONFLICT (id) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$id", register.LRegisterId);
            LStateColumn.LStateColumnApply(command, "name", register.LRegisterName);
            LRegisterLanguageApply(command, register);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public LRegister? LRegisterRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            "SELECT id, name_state, name, language, builtin FROM register WHERE id = $id;";
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
            SELECT id, name_state, name, language, builtin
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

    public IReadOnlyList<LRegister> LRegisterMeaningRead(string meaningId)
    {
        return LRegisterReferrerRead("sense_register", "sense_id", meaningId);
    }

    public IReadOnlyList<LRegister> LRegisterCollocationRead(string collocationId)
    {
        return LRegisterReferrerRead("collocation_register", "collocation_id", collocationId);
    }

    public void LRegisterNameUpdate(string registerId, LStateValue name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(registerId);
        ArgumentNullException.ThrowIfNull(name);

        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                "UPDATE register SET name_state = $nameState, name = $name WHERE id = $id AND builtin = 0;";
            LStateColumn.LStateColumnApply(command, "name", name);
            command.Parameters.AddWithValue("$id", registerId);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public int LRegisterReferenceRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        return LRegisterReferenceRead(session.LDatabaseSessionConnection, id);
    }

    public IReadOnlyDictionary<string, int> LRegisterReferenceRead()
    {
        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT register_id, COUNT(*) FROM (
                SELECT register_id FROM sense_register
                UNION ALL
                SELECT register_id FROM collocation_register
            )
            GROUP BY register_id;
            """;

        Dictionary<string, int> counts = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            counts[reader.GetString(0)] = reader.GetInt32(1);
        }

        return counts;
    }

    public void LRegisterDelete(string id)
    {
        LRegisterDelete(id, false);
    }

    public void LRegisterDelete(string id, bool detach)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        if (detach)
        {
            LRegisterLinkDelete(connection, "sense_register", "sense_id", id);
            LRegisterLinkDelete(connection, "collocation_register", "collocation_id", id);
        }

        int references = LRegisterReferenceRead(connection, id);
        if (references > 0)
        {
            throw new InvalidOperationException(
                $"Register {id} is still referenced {references} time(s); detach every reference before deleting it.");
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM register WHERE id = $id AND builtin = 0;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public void LRegisterMeaningAttach(string meaningId, string registerId, int position)
    {
        LRegisterReferenceAttach("sense_register", "sense_id", meaningId, registerId, position);
    }

    public void LRegisterCollocationAttach(string collocationId, string registerId, int position)
    {
        LRegisterReferenceAttach("collocation_register", "collocation_id", collocationId, registerId, position);
    }

    public void LRegisterMeaningDetach(string meaningId, string registerId)
    {
        LRegisterReferenceDetach("sense_register", "sense_id", meaningId, registerId);
    }

    public void LRegisterCollocationDetach(string collocationId, string registerId)
    {
        LRegisterReferenceDetach("collocation_register", "collocation_id", collocationId, registerId);
    }

    private static void LRegisterLanguageApply(SqliteCommand command, LRegister register)
    {
        command.Parameters.AddWithValue(
            "$language",
            register.LRegisterLanguage.Length == 0 ? DBNull.Value : register.LRegisterLanguage);
        command.Parameters.AddWithValue("$builtin", register.LRegisterBuiltin ? 1 : 0);
    }

    private static LRegister LRegisterRowRead(SqliteDataReader reader)
    {
        return new LRegister(
            reader.GetString(0),
            LStateColumn.LStateColumnRead(reader, 1),
            reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
            reader.GetInt32(4) != 0);
    }

    private static int LRegisterReferenceRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT
                (SELECT COUNT(*) FROM sense_register WHERE register_id = $id)
                + (SELECT COUNT(*) FROM collocation_register WHERE register_id = $id);
            """;
        command.Parameters.AddWithValue("$id", id);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static void LRegisterLinkDelete(
        SqliteConnection connection,
        string table,
        string column,
        string registerId)
    {
        List<string> referrers = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"SELECT {column} FROM {table} WHERE register_id = $register;";
            command.Parameters.AddWithValue("$register", registerId);
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                referrers.Add(reader.GetString(0));
            }
        }

        if (referrers.Count == 0)
        {
            return;
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = $"DELETE FROM {table} WHERE register_id = $register;";
            command.Parameters.AddWithValue("$register", registerId);
            command.ExecuteNonQuery();
        }

        string scope = $"{column} = $owner";
        foreach (string referrer in referrers)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, table, scope, referrer, "register_id",
                LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, referrer, "register_id"));
        }
    }

    private void LRegisterReferenceAttach(
        string table,
        string column,
        string referrerId,
        string registerId,
        int position)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(registerId);

        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        string scope = $"{column} = $owner";
        IReadOnlyList<string> current = LDatabaseOrder.LDatabaseOrderRead(
            connection, table, scope, referrerId, "register_id");

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                INSERT INTO {table} ({column}, register_id, position)
                VALUES ($referrer, $register, $position)
                ON CONFLICT ({column}, register_id) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$register", registerId);
            command.Parameters.AddWithValue("$position", current.Count);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "register_id",
            LDatabaseOrder.LDatabaseOrderInsert(current, registerId, position));

        session.LDatabaseSessionCommit();
    }

    private void LRegisterReferenceDetach(string table, string column, string referrerId, string registerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(registerId);

        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;
        string scope = $"{column} = $owner";

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"DELETE FROM {table} WHERE {column} = $referrer AND register_id = $register;";
            command.Parameters.AddWithValue("$referrer", referrerId);
            command.Parameters.AddWithValue("$register", registerId);
            command.ExecuteNonQuery();
        }

        LDatabaseOrder.LDatabaseOrderNormalize(
            connection, table, scope, referrerId, "register_id",
            LDatabaseOrder.LDatabaseOrderRead(connection, table, scope, referrerId, "register_id"));

        session.LDatabaseSessionCommit();
    }

    private IReadOnlyList<LRegister> LRegisterReferrerRead(string table, string column, string referrerId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referrerId);

        using LDatabaseSession session = _lRegisterArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            $"""
            SELECT register.id, register.name_state, register.name,
                   register.language, register.builtin
            FROM {table} link
            JOIN register ON register.id = link.register_id
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
