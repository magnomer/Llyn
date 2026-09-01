using System;
using System.Collections.Generic;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

/// <summary>
/// Persists Authors — independent data no Reference owns. An Author is created once with an opaque id
/// and is then <em>referenced</em> by any number of References through <c>source_author</c>, which
/// carries the position the Author takes on that Reference alone. Renaming rewrites the visible name
/// and never the id, so every Reference keeps pointing at the same Author, and
/// <see cref="LAuthorDelete"/> refuses to run while any Reference still credits it.
/// <para>
/// Attaching an Author to a Reference belongs to <see cref="LReferenceArchive"/>, which owns the
/// association rows; this store only creates, reads, renames, and deletes the Authors themselves.
/// </para>
/// </summary>
public sealed class LAuthorArchive
{
    private readonly LDatabase _lAuthorArchiveDatabase;

    /// <summary>Binds the store to the workspace <paramref name="database"/> it opens connections through.</summary>
    public LAuthorArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lAuthorArchiveDatabase = database;
    }

    /// <summary>
    /// Inserts <paramref name="author"/> with a fresh opaque id and returns the stored Author with that
    /// id filled in. The new Author is credited on no Reference until one attaches it.
    /// </summary>
    public LAuthor LAuthorCreate(LAuthor author)
    {
        ArgumentNullException.ThrowIfNull(author);
        ArgumentException.ThrowIfNullOrWhiteSpace(author.LAuthorName);

        LAuthor stored = author with { LAuthorId = LIdentity.LIdentityCreate() };

        using SqliteConnection connection = _lAuthorArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "INSERT INTO author (id, name) VALUES ($id, $name);";
        command.Parameters.AddWithValue("$id", stored.LAuthorId);
        command.Parameters.AddWithValue("$name", stored.LAuthorName);
        command.ExecuteNonQuery();

        return stored;
    }

    /// <summary>Reads the Author identified by <paramref name="id"/>, or <c>null</c> when none exists.</summary>
    public LAuthor? LAuthorRead(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using SqliteConnection connection = _lAuthorArchiveDatabase.LDatabaseRead();
        return LAuthorSingleRead(connection, id);
    }

    /// <summary>
    /// Reads the Authors a Reference credits, in the order that Reference gives them. Another Reference
    /// crediting the same Authors may order them differently — the order lives on the association row.
    /// </summary>
    public IReadOnlyList<LAuthor> LAuthorReferenceRead(string referenceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceId);

        using SqliteConnection connection = _lAuthorArchiveDatabase.LDatabaseRead();

        List<string> ids = [];
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                "SELECT author_id FROM source_author WHERE source_id = $reference ORDER BY position;";
            command.Parameters.AddWithValue("$reference", referenceId);
            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ids.Add(reader.GetString(0));
            }
        }

        List<LAuthor> authors = [];
        foreach (string id in ids)
        {
            LAuthor? author = LAuthorSingleRead(connection, id);
            if (author is not null)
            {
                authors.Add(author);
            }
        }

        return authors;
    }

    /// <summary>
    /// Rewrites the name of the Author identified by <paramref name="author"/>'s id. The id and every
    /// Reference crediting it are untouched, so a rename never changes where the Author appears.
    /// </summary>
    public void LAuthorUpdate(LAuthor author)
    {
        ArgumentNullException.ThrowIfNull(author);
        ArgumentException.ThrowIfNullOrWhiteSpace(author.LAuthorId);
        ArgumentException.ThrowIfNullOrWhiteSpace(author.LAuthorName);

        using SqliteConnection connection = _lAuthorArchiveDatabase.LDatabaseRead();
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE author SET name = $name WHERE id = $id;";
        command.Parameters.AddWithValue("$name", author.LAuthorName);
        command.Parameters.AddWithValue("$id", author.LAuthorId);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Deletes the Author identified by <paramref name="id"/>. Guarded: while any Reference still
    /// credits the Author, nothing is deleted and an <see cref="InvalidOperationException"/> is thrown —
    /// detach the Author from every Reference first. Deleting an Author never deletes a Reference.
    /// </summary>
    public void LAuthorDelete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        using SqliteConnection connection = _lAuthorArchiveDatabase.LDatabaseRead();

        using (SqliteCommand guard = connection.CreateCommand())
        {
            guard.CommandText = "SELECT COUNT(*) FROM source_author WHERE author_id = $id;";
            guard.Parameters.AddWithValue("$id", id);
            long references = Convert.ToInt64(guard.ExecuteScalar());
            if (references > 0)
            {
                throw new InvalidOperationException(
                    $"Author {id} is still credited on {references} Reference(s); detach every reference before deleting it.");
            }
        }

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM author WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    private static LAuthor? LAuthorSingleRead(SqliteConnection connection, string id)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM author WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);
        using SqliteDataReader reader = command.ExecuteReader();
        return reader.Read() ? new LAuthor(id, reader.GetString(0)) : null;
    }
}
