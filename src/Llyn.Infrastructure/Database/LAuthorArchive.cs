using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LAuthorArchive
{
    private const long LAuthorAbsorbShift = 2_000_000_000L;

    private readonly LDatabase _lAuthorArchiveDatabase;

    public LAuthorArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lAuthorArchiveDatabase = database;
    }

    public LAuthor LAuthorCreate(LAuthor author)
    {
        ArgumentNullException.ThrowIfNull(author);

        LAuthor stored = author;

        using LDatabaseSession session = _lAuthorArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                "INSERT INTO author (name) VALUES ($name) RETURNING author_id;";
            command.Parameters.AddWithValue("$name", stored.LAuthorName);
            stored = stored with { LAuthorId = (long)command.ExecuteScalar()! };
        }

        session.LDatabaseSessionCommit();
        return stored;
    }

    public LAuthor? LAuthorRead(long id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lAuthorArchiveDatabase.LDatabaseSessionStart();
        return LAuthorSingleRead(session.LDatabaseSessionConnection, id);
    }

    public IReadOnlyList<LAuthor> LAuthorReferenceRead(long referenceId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(referenceId);

        using LDatabaseSession session = _lAuthorArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT author.author_id, author.name
            FROM reference_author link
            JOIN author ON author.author_id = link.author_ref
            WHERE link.reference_parent = $reference
            ORDER BY link.position;
            """;
        command.Parameters.AddWithValue("$reference", referenceId);

        List<LAuthor> authors = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            authors.Add(new LAuthor(reader.GetInt64(0), reader.GetString(1)));
        }

        return authors;
    }

    public IReadOnlyList<LAuthor> LAuthorAllRead()
    {
        using LDatabaseSession session = _lAuthorArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText = "SELECT author_id, name FROM author ORDER BY name;";

        List<LAuthor> authors = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            authors.Add(new LAuthor(reader.GetInt64(0), reader.GetString(1)));
        }

        return authors;
    }

    public IReadOnlyDictionary<long, IReadOnlyList<LAuthor>> LAuthorReferenceRead()
    {
        using LDatabaseSession session = _lAuthorArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT link.reference_parent, author.author_id, author.name
            FROM reference_author link
            JOIN author ON author.author_id = link.author_ref
            ORDER BY link.reference_parent, link.position;
            """;

        Dictionary<long, IReadOnlyList<LAuthor>> credits = [];
        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            long reference = reader.GetInt64(0);
            if (credits.TryGetValue(reference, out IReadOnlyList<LAuthor>? held))
            {
                ((List<LAuthor>)held).Add(new LAuthor(reader.GetInt64(1), reader.GetString(2)));
                continue;
            }

            credits[reference] = new List<LAuthor> { new(reader.GetInt64(1), reader.GetString(2)) };
        }

        return credits;
    }

    public void LAuthorUpdate(LAuthor author)
    {
        ArgumentNullException.ThrowIfNull(author);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(author.LAuthorId);
        ArgumentException.ThrowIfNullOrWhiteSpace(author.LAuthorName);

        using LDatabaseSession session = _lAuthorArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText = "UPDATE author SET name = $name WHERE author_id = $id;";
            command.Parameters.AddWithValue("$name", author.LAuthorName);
            command.Parameters.AddWithValue("$id", author.LAuthorId);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException($"No Author carries the id '{author.LAuthorId}'.");
            }
        }

        session.LDatabaseSessionCommit();
    }

    public void LAuthorAbsorb(long kept, long dropped)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(kept);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(dropped);
        if (kept == dropped)
        {
            throw new InvalidOperationException("An Author cannot be merged into itself.");
        }

        using LDatabaseSession session = _lAuthorArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        if (LAuthorSingleRead(connection, kept) is null || LAuthorSingleRead(connection, dropped) is null)
        {
            throw new InvalidOperationException($"No Author carries the id '{kept}' or '{dropped}'.");
        }

        List<long> references = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "SELECT reference_parent FROM reference_author WHERE author_ref = $id;";
            command.Parameters.AddWithValue("$id", dropped);
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                references.Add(reader.GetInt64(0));
            }
        }

        const string scope = "reference_parent = $owner";
        foreach (long reference in references)
        {
            IReadOnlyList<long> order =
                LDatabaseOrder.LDatabaseOrderRead(connection, "reference_author", scope, reference, "author_ref");
            List<long> merged = [];
            bool held = false;
            foreach (long author in order)
            {
                held |= author == kept;
                long credited = author == dropped ? kept : author;
                if (!merged.Contains(credited))
                {
                    merged.Add(credited);
                }
            }

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "DELETE FROM reference_author WHERE reference_parent = $owner AND author_ref = $dropped;";
                command.Parameters.AddWithValue("$owner", reference);
                command.Parameters.AddWithValue("$dropped", dropped);
                command.ExecuteNonQuery();
            }

            if (!held)
            {
                using SqliteCommand command = connection.CreateCommand();
                command.CommandText =
                    """
                    INSERT INTO reference_author (reference_parent, author_ref, position)
                    VALUES ($owner, $kept, $position);
                    """;
                command.Parameters.AddWithValue("$owner", reference);
                command.Parameters.AddWithValue("$kept", kept);
                command.Parameters.AddWithValue("$position", order.Count + LAuthorAbsorbShift);
                command.ExecuteNonQuery();
            }

            LDatabaseOrder.LDatabaseOrderNormalize(
                connection, "reference_author", scope, reference, "author_ref", merged);
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM author WHERE author_id = $id;";
            command.Parameters.AddWithValue("$id", dropped);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public void LAuthorDelete(long id)
    {
        LAuthorDelete(id, false);
    }

    public void LAuthorDelete(long id, bool detach)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        using LDatabaseSession session = _lAuthorArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        if (detach)
        {
            LAuthorCreditClear(connection, id);
        }

        using (SqliteCommand guard = connection.CreateCommand())
        {
            guard.CommandText = "SELECT COUNT(*) FROM reference_author WHERE author_ref = $id;";
            guard.Parameters.AddWithValue("$id", id);
            long references = Convert.ToInt64(guard.ExecuteScalar());
            if (references > 0)
            {
                throw new InvalidOperationException(
                    $"Author {id} is still credited on {references} Reference(s); "
                    + "detach every reference before deleting it.");
            }
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM author WHERE author_id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    private static void LAuthorCreditClear(SqliteConnection connection, long id)
    {
        List<long> references = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "SELECT reference_parent FROM reference_author WHERE author_ref = $id;";
            command.Parameters.AddWithValue("$id", id);
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                references.Add(reader.GetInt64(0));
            }
        }

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "DELETE FROM reference_author WHERE author_ref = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        const string scope = "reference_parent = $owner";
        foreach (long reference in references)
        {
            LDatabaseOrder.LDatabaseOrderNormalize(
                connection,
                "reference_author",
                scope,
                reference,
                "author_ref",
                LDatabaseOrder.LDatabaseOrderRead(connection, "reference_author", scope, reference, "author_ref"));
        }
    }

    private static LAuthor? LAuthorSingleRead(SqliteConnection connection, long id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM author WHERE author_id = $id;";
        command.Parameters.AddWithValue("$id", id);
        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? new LAuthor(id, reader.GetString(0)) : null;
    }
}
