using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LStencilArchive : LStencilVault
{
    private readonly LDatabase _lStencilArchiveDatabase;

    public LStencilArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lStencilArchiveDatabase = database;
    }

    public LStencil LStencilCreate(LStencil stencil)
    {
        ArgumentNullException.ThrowIfNull(stencil);
        ArgumentNullException.ThrowIfNull(stencil.LStencilSlots);

        using LDatabaseSession session = _lStencilArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LStencil stored;
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "INSERT INTO stencil (name, saved) VALUES ($name, $saved) RETURNING stencil_id;";
            command.Parameters.AddWithValue("$name", stencil.LStencilName);
            command.Parameters.AddWithValue("$saved", stencil.LStencilSaved ? 1 : 0);
            stored = stencil with { LStencilId = (long)command.ExecuteScalar()! };
        }

        LStencilSlotInsert(connection, stored.LStencilId, stored.LStencilSlots);

        session.LDatabaseSessionCommit();
        return stored;
    }

    public IReadOnlyList<LStencil> LStencilRead()
    {
        using LDatabaseSession session = _lStencilArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        List<LStencil> stencils = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "SELECT stencil_id, name, saved FROM stencil WHERE saved = 1 ORDER BY stencil_id;";
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                stencils.Add(new LStencil(reader.GetInt64(0), reader.GetString(1), reader.GetInt32(2) != 0, []));
            }
        }

        return stencils.ConvertAll(stencil => stencil with
        {
            LStencilSlots = LStencilSlotRead(connection, stencil.LStencilId),
        });
    }

    public LStencil? LStencilRead(long stencilId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(stencilId);

        using LDatabaseSession session = _lStencilArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        LStencil? held = null;
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "SELECT stencil_id, name, saved FROM stencil WHERE stencil_id = $id;";
            command.Parameters.AddWithValue("$id", stencilId);
            using SqliteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                held = new LStencil(reader.GetInt64(0), reader.GetString(1), reader.GetInt32(2) != 0, []);
            }
        }

        return held is null ? null : held with { LStencilSlots = LStencilSlotRead(connection, held.LStencilId) };
    }

    public void LStencilUpdate(LStencil stencil)
    {
        ArgumentNullException.ThrowIfNull(stencil);
        ArgumentNullException.ThrowIfNull(stencil.LStencilSlots);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(stencil.LStencilId);

        using LDatabaseSession session = _lStencilArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                """
                UPDATE stencil SET name = $name WHERE stencil_id = $id;
                DELETE FROM stencil_slot WHERE stencil_parent = $id;
                """;
            command.Parameters.AddWithValue("$name", stencil.LStencilName);
            command.Parameters.AddWithValue("$id", stencil.LStencilId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No stencil carries the id '{stencil.LStencilId}'.");
            }
        }

        LStencilSlotInsert(connection, stencil.LStencilId, stencil.LStencilSlots);

        session.LDatabaseSessionCommit();
    }

    private static IReadOnlyList<LStencilSlot> LStencilSlotRead(SqliteConnection connection, long stencilId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT column_kind, column_index, body
            FROM stencil_slot WHERE stencil_parent = $stencil
            ORDER BY column_kind, column_index;
            """;
        command.Parameters.AddWithValue("$stencil", stencilId);

        List<LStencilSlot> slots = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            slots.Add(new LStencilSlot((LCueColumn)reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2)));
        }

        return slots;
    }

    private static void LStencilSlotInsert(
        SqliteConnection connection, long stencilId, IReadOnlyList<LStencilSlot> slots)
    {
        foreach (LStencilSlot slot in slots)
        {
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO stencil_slot (stencil_parent, column_kind, column_index, body)
                VALUES ($stencil, $column, $index, $body);
                """;
            command.Parameters.AddWithValue("$stencil", stencilId);
            command.Parameters.AddWithValue("$column", (int)slot.LStencilSlotColumn);
            command.Parameters.AddWithValue("$index", slot.LStencilSlotIndex);
            command.Parameters.AddWithValue("$body", slot.LStencilSlotBody);
            command.ExecuteNonQuery();
        }
    }
}
