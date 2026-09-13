using System;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LWorkspaceArchive
{
    private const long LWorkspaceArchiveRow = 1;
    private const string LWorkspaceArchiveEditor = "Editor";
    private const string LWorkspaceArchiveDisplay = "Display";

    private const string LWorkspaceArchiveColumn =
        "workspace_id, left_entry_ref, right_entry_ref, revision_ref, identity_floor, mode, split, " +
        "library_order, phonology_order, favorite_order, taxonomy_order, " +
        "repertoire_order, reference_order, corpus_order, tenor_order, " +
        "library_filter, phonology_filter, favorite_filter, taxonomy_filter, tenor_filter, corpus_filter";

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
                    $id, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL,
                    NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
                ON CONFLICT (workspace_id) DO NOTHING;
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
                FROM workspace WHERE workspace_id = $id;
                """;
            read.Parameters.AddWithValue("$id", LWorkspaceArchiveRow);

            using SqliteDataReader reader = read.ExecuteReader();

            if (!reader.Read())
            {
                throw new InvalidOperationException(
                    "The workspace row is missing immediately after it was created.");
            }

            LWorkspaceState fallback = new(reader.GetInt64(0));

            state = fallback with
            {
                LWorkspaceStateLeft = LWorkspaceArchiveResolve(reader, 1),
                LWorkspaceStateRight = LWorkspaceArchiveResolve(reader, 2),
                LWorkspaceStateRevision = LWorkspaceArchiveResolve(reader, 3),
                LWorkspaceStateFloor = reader.GetInt64(4),
                LWorkspaceStateMode = LWorkspaceArchiveRead(reader, 5),
                LWorkspaceStateSplit = LWorkspaceArchiveRead(reader, 6) == LWorkspaceArchiveEditor,
                LWorkspaceStateOrder = LCatalog.LCatalogOrderParse(
                    LWorkspaceArchiveRead(reader, 7), fallback.LWorkspaceStateOrder),
                LWorkspaceStateSequence = LCatalog.LCatalogOrderParse(
                    LWorkspaceArchiveRead(reader, 8), fallback.LWorkspaceStateSequence),
                LWorkspaceStateSeries = LCatalog.LCatalogOrderParse(
                    LWorkspaceArchiveRead(reader, 9), fallback.LWorkspaceStateSeries),
                LWorkspaceStateFunnel = LCatalog.LCatalogOrderParse(
                    LWorkspaceArchiveRead(reader, 10), fallback.LWorkspaceStateFunnel),
                LWorkspaceStateTier = LCatalog.LCatalogOrderParse(
                    LWorkspaceArchiveRead(reader, 11), fallback.LWorkspaceStateTier),
                LWorkspaceStateGrade = LCatalog.LCatalogOrderParse(
                    LWorkspaceArchiveRead(reader, 12), fallback.LWorkspaceStateGrade),
                LWorkspaceStateRank = LCatalog.LCatalogOrderParse(
                    LWorkspaceArchiveRead(reader, 13), fallback.LWorkspaceStateRank),
                LWorkspaceStateDegree = LCatalog.LCatalogOrderParse(
                    LWorkspaceArchiveRead(reader, 14), fallback.LWorkspaceStateDegree),
                LWorkspaceStateSieve = LCatalog.LCatalogFilterParse(LWorkspaceArchiveRead(reader, 15)),
                LWorkspaceStateLens = LCatalog.LCatalogFilterParse(LWorkspaceArchiveRead(reader, 16)),
                LWorkspaceStateStrainer = LCatalog.LCatalogFilterParse(LWorkspaceArchiveRead(reader, 17)),
                LWorkspaceStateLattice = LCatalog.LCatalogFilterParse(LWorkspaceArchiveRead(reader, 18)),
                LWorkspaceStateGrille = LCatalog.LCatalogFilterParse(LWorkspaceArchiveRead(reader, 19)),
                LWorkspaceStatePrism = LCatalog.LCatalogFilterParse(LWorkspaceArchiveRead(reader, 20)),
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
                    workspace_id, left_entry_ref, right_entry_ref, revision_ref, identity_floor, mode, split,
                    library_order, phonology_order, favorite_order, taxonomy_order,
                    repertoire_order, reference_order, corpus_order, tenor_order,
                    library_filter, phonology_filter, favorite_filter, taxonomy_filter, tenor_filter, corpus_filter)
                VALUES (
                    $id, $left, $right, $revision, $floor, $mode, $split,
                    $library, $phonology, $favorite, $taxonomy,
                    $repertoire, $reference, $corpus, $tenor,
                    $sieve, $lens, $strainer, $lattice, $grille, $prism)
                ON CONFLICT (workspace_id) DO UPDATE SET
                    left_entry_ref = excluded.left_entry_ref,
                    right_entry_ref = excluded.right_entry_ref,
                    revision_ref = excluded.revision_ref,
                    identity_floor = min(identity_floor, excluded.identity_floor),
                    mode = excluded.mode,
                    split = excluded.split,
                    library_order = excluded.library_order,
                    phonology_order = excluded.phonology_order,
                    favorite_order = excluded.favorite_order,
                    taxonomy_order = excluded.taxonomy_order,
                    repertoire_order = excluded.repertoire_order,
                    reference_order = excluded.reference_order,
                    corpus_order = excluded.corpus_order,
                    tenor_order = excluded.tenor_order,
                    library_filter = excluded.library_filter,
                    phonology_filter = excluded.phonology_filter,
                    favorite_filter = excluded.favorite_filter,
                    taxonomy_filter = excluded.taxonomy_filter,
                    tenor_filter = excluded.tenor_filter,
                    corpus_filter = excluded.corpus_filter;
                """;
            command.Parameters.AddWithValue("$id", LWorkspaceArchiveRow);
            command.Parameters.AddWithValue("$left", (object?)state.LWorkspaceStateLeft ?? DBNull.Value);
            command.Parameters.AddWithValue("$right", (object?)state.LWorkspaceStateRight ?? DBNull.Value);
            command.Parameters.AddWithValue("$revision", (object?)state.LWorkspaceStateRevision ?? DBNull.Value);
            command.Parameters.AddWithValue("$floor", state.LWorkspaceStateFloor);
            command.Parameters.AddWithValue("$mode", (object?)state.LWorkspaceStateMode ?? DBNull.Value);
            command.Parameters.AddWithValue(
                "$split", state.LWorkspaceStateSplit ? LWorkspaceArchiveEditor : LWorkspaceArchiveDisplay);
            command.Parameters.AddWithValue("$library", LCatalog.LCatalogOrderFormat(state.LWorkspaceStateOrder));
            command.Parameters.AddWithValue("$phonology", LCatalog.LCatalogOrderFormat(state.LWorkspaceStateSequence));
            command.Parameters.AddWithValue("$favorite", LCatalog.LCatalogOrderFormat(state.LWorkspaceStateSeries));
            command.Parameters.AddWithValue("$taxonomy", LCatalog.LCatalogOrderFormat(state.LWorkspaceStateFunnel));
            command.Parameters.AddWithValue("$repertoire", LCatalog.LCatalogOrderFormat(state.LWorkspaceStateTier));
            command.Parameters.AddWithValue("$reference", LCatalog.LCatalogOrderFormat(state.LWorkspaceStateGrade));
            command.Parameters.AddWithValue("$corpus", LCatalog.LCatalogOrderFormat(state.LWorkspaceStateRank));
            command.Parameters.AddWithValue("$tenor", LCatalog.LCatalogOrderFormat(state.LWorkspaceStateDegree));
            command.Parameters.AddWithValue("$sieve", LWorkspaceArchiveFormat(state.LWorkspaceStateSieve));
            command.Parameters.AddWithValue("$lens", LWorkspaceArchiveFormat(state.LWorkspaceStateLens));
            command.Parameters.AddWithValue("$strainer", LWorkspaceArchiveFormat(state.LWorkspaceStateStrainer));
            command.Parameters.AddWithValue("$lattice", LWorkspaceArchiveFormat(state.LWorkspaceStateLattice));
            command.Parameters.AddWithValue("$grille", LWorkspaceArchiveFormat(state.LWorkspaceStateGrille));
            command.Parameters.AddWithValue("$prism", LWorkspaceArchiveFormat(state.LWorkspaceStatePrism));
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public long LWorkspaceFloorAdjust()
    {
        using LDatabaseSession session = _lWorkspaceArchiveDatabase.LDatabaseSessionStart();
        long floor;
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO workspace (workspace_id, identity_floor)
                VALUES ($id, -1)
                ON CONFLICT (workspace_id) DO UPDATE SET identity_floor = identity_floor - 1
                RETURNING identity_floor;
                """;
            command.Parameters.AddWithValue("$id", LWorkspaceArchiveRow);
            floor = Convert.ToInt64(command.ExecuteScalar());
        }

        session.LDatabaseSessionCommit();
        return floor;
    }

    private static string LWorkspaceArchiveFormat(LCatalogFilter? filter)
    {
        return LCatalog.LCatalogFilterFormat(filter ?? LCatalogFilter.LCatalogFilterEmpty);
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
