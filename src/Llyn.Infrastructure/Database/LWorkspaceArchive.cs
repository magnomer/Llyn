using System;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LWorkspaceArchive
{
    private const string LWorkspaceArchiveRow = "workspace";
    private const string LWorkspaceArchiveEditor = "Editor";
    private const string LWorkspaceArchiveDisplay = "Display";

    private const string LWorkspaceArchiveColumn =
        "id, left_entry, right_entry, mode, split, revision, " +
        "library_order, phonology_order, favorite_order, taxonomy_order, " +
        "repertoire_order, reference_order, corpus_order, tenor_order";

    private readonly LDatabase _lWorkspaceArchiveDatabase;

    public LWorkspaceArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lWorkspaceArchiveDatabase = database;
    }

    public LWorkspaceState LWorkspaceStateRead()
    {
        using LDatabaseSession session = _lWorkspaceArchiveDatabase.LDatabaseSessionStart();
        SqliteConnection connection = session.LDatabaseSessionConnection;

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText =
                $"""
                INSERT INTO workspace ({LWorkspaceArchiveColumn})
                VALUES (
                    $id, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
                    NULL)
                ON CONFLICT (id) DO NOTHING;
                """;
            command.Parameters.AddWithValue("$id", LWorkspaceArchiveRow);
            command.ExecuteNonQuery();
        }

        LWorkspaceState state;
        using (SqliteCommand read = connection.CreateCommand())
        {
            read.CommandText =
                $"""
                SELECT {LWorkspaceArchiveColumn}
                FROM workspace WHERE id = $id;
                """;
            read.Parameters.AddWithValue("$id", LWorkspaceArchiveRow);

            using SqliteDataReader reader = read.ExecuteReader();

            if (!reader.Read())
            {
                throw new InvalidOperationException(
                    "The workspace row is missing immediately after it was created.");
            }

            LWorkspaceState fallback = new(reader.GetString(0));

            state = fallback with
            {
                LWorkspaceStateLeft = LWorkspaceArchiveResolve(reader, 1),
                LWorkspaceStateRight = LWorkspaceArchiveResolve(reader, 2),
                LWorkspaceStateMode = LWorkspaceArchiveRead(reader, 3),
                LWorkspaceStateSplit = LWorkspaceArchiveRead(reader, 4) == LWorkspaceArchiveEditor,
                LWorkspaceStateRevision = LWorkspaceArchiveResolve(reader, 5),
                LWorkspaceStateOrder = LCatalog.LCatalogOrderParse(
                    LWorkspaceArchiveRead(reader, 6), fallback.LWorkspaceStateOrder),
                LWorkspaceStateSequence = LCatalog.LCatalogOrderParse(
                    LWorkspaceArchiveRead(reader, 7), fallback.LWorkspaceStateSequence),
                LWorkspaceStateSeries = LCatalog.LCatalogOrderParse(
                    LWorkspaceArchiveRead(reader, 8), fallback.LWorkspaceStateSeries),
                LWorkspaceStateFunnel = LCatalog.LCatalogOrderParse(
                    LWorkspaceArchiveRead(reader, 9), fallback.LWorkspaceStateFunnel),
                LWorkspaceStateTier = LCatalog.LCatalogOrderParse(
                    LWorkspaceArchiveRead(reader, 10), fallback.LWorkspaceStateTier),
                LWorkspaceStateGrade = LCatalog.LCatalogOrderParse(
                    LWorkspaceArchiveRead(reader, 11), fallback.LWorkspaceStateGrade),
                LWorkspaceStateRank = LCatalog.LCatalogOrderParse(
                    LWorkspaceArchiveRead(reader, 12), fallback.LWorkspaceStateRank),
                LWorkspaceStateDegree = LCatalog.LCatalogOrderParse(
                    LWorkspaceArchiveRead(reader, 13), fallback.LWorkspaceStateDegree),
            };
        }

        session.LDatabaseSessionCommit();
        return state;
    }

    public void LWorkspaceStateSave(LWorkspaceState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        using LDatabaseSession session = _lWorkspaceArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO workspace (
                    id, left_entry, right_entry, mode, split, revision,
                    library_order, phonology_order, favorite_order, taxonomy_order,
                    repertoire_order, reference_order, corpus_order, tenor_order)
                VALUES (
                    $id, $left, $right, $mode, $split, $revision,
                    $library, $phonology, $favorite, $taxonomy,
                    $repertoire, $reference, $corpus, $tenor)
                ON CONFLICT (id) DO UPDATE SET
                    left_entry = excluded.left_entry,
                    right_entry = excluded.right_entry,
                    mode = excluded.mode,
                    split = excluded.split,
                    revision = excluded.revision,
                    library_order = excluded.library_order,
                    phonology_order = excluded.phonology_order,
                    favorite_order = excluded.favorite_order,
                    taxonomy_order = excluded.taxonomy_order,
                    repertoire_order = excluded.repertoire_order,
                    reference_order = excluded.reference_order,
                    corpus_order = excluded.corpus_order,
                    tenor_order = excluded.tenor_order;
                """;
            command.Parameters.AddWithValue("$id", LWorkspaceArchiveRow);
            command.Parameters.AddWithValue("$left", (object?)state.LWorkspaceStateLeft ?? DBNull.Value);
            command.Parameters.AddWithValue("$right", (object?)state.LWorkspaceStateRight ?? DBNull.Value);
            command.Parameters.AddWithValue("$mode", (object?)state.LWorkspaceStateMode ?? DBNull.Value);
            command.Parameters.AddWithValue(
                "$split", state.LWorkspaceStateSplit ? LWorkspaceArchiveEditor : LWorkspaceArchiveDisplay);
            command.Parameters.AddWithValue("$revision", (object?)state.LWorkspaceStateRevision ?? DBNull.Value);
            command.Parameters.AddWithValue("$library", LCatalog.LCatalogOrderFormat(state.LWorkspaceStateOrder));
            command.Parameters.AddWithValue("$phonology", LCatalog.LCatalogOrderFormat(state.LWorkspaceStateSequence));
            command.Parameters.AddWithValue("$favorite", LCatalog.LCatalogOrderFormat(state.LWorkspaceStateSeries));
            command.Parameters.AddWithValue("$taxonomy", LCatalog.LCatalogOrderFormat(state.LWorkspaceStateFunnel));
            command.Parameters.AddWithValue("$repertoire", LCatalog.LCatalogOrderFormat(state.LWorkspaceStateTier));
            command.Parameters.AddWithValue("$reference", LCatalog.LCatalogOrderFormat(state.LWorkspaceStateGrade));
            command.Parameters.AddWithValue("$corpus", LCatalog.LCatalogOrderFormat(state.LWorkspaceStateRank));
            command.Parameters.AddWithValue("$tenor", LCatalog.LCatalogOrderFormat(state.LWorkspaceStateDegree));
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    private static string? LWorkspaceArchiveRead(SqliteDataReader reader, int column)
    {
        return reader.IsDBNull(column) ? null : reader.GetString(column);
    }

    private static long? LWorkspaceArchiveResolve(SqliteDataReader reader, int column)
    {
        return reader.IsDBNull(column) ? null : reader.GetInt64(column);
    }
}
