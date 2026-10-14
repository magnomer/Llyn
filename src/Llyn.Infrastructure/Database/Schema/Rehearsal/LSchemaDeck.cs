using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaDeck
{
    public static void LSchemaDeckCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS deck_folder (
                deck_folder_id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL UNIQUE,
                position INTEGER NOT NULL CHECK (position >= 0)
            );

            CREATE TABLE IF NOT EXISTS stencil (
                stencil_id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL DEFAULT '',
                saved INTEGER NOT NULL DEFAULT 0 CHECK (saved IN (0, 1))
            );

            CREATE TABLE IF NOT EXISTS stencil_slot (
                stencil_parent INTEGER NOT NULL,
                column_kind INTEGER NOT NULL CHECK (column_kind BETWEEN 1 AND 7),
                column_index INTEGER NOT NULL,
                body TEXT NOT NULL DEFAULT '',
                PRIMARY KEY (stencil_parent, column_kind, column_index),
                CHECK ((column_kind IN (1, 2, 3) AND column_index BETWEEN 1 AND 5)
                    OR (column_kind IN (4, 5) AND column_index = 1)
                    OR (column_kind IN (6, 7) AND column_index BETWEEN 1 AND 4)),
                FOREIGN KEY (stencil_parent) REFERENCES stencil (stencil_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS deck (
                deck_id INTEGER PRIMARY KEY AUTOINCREMENT,
                folder_ref INTEGER,
                position INTEGER NOT NULL CHECK (position >= 0),
                name TEXT NOT NULL,
                grain INTEGER NOT NULL CHECK (grain BETWEEN 1 AND 5),
                stencil_ref INTEGER NOT NULL,
                side_first INTEGER NOT NULL DEFAULT 1 CHECK (side_first BETWEEN 1 AND 3),
                study_order INTEGER NOT NULL DEFAULT 4 CHECK (study_order BETWEEN 1 AND 4),
                round_goal INTEGER NOT NULL DEFAULT 10 CHECK (round_goal > 0),
                round_spaced INTEGER NOT NULL DEFAULT 20 CHECK (round_spaced > 0),
                set_min_goal INTEGER CHECK (set_min_goal IS NULL OR set_min_goal >= 0),
                new_pool INTEGER NOT NULL DEFAULT 10 CHECK (new_pool > 0),
                target_streak INTEGER NOT NULL DEFAULT 10 CHECK (target_streak > 0),
                target_hours INTEGER NOT NULL DEFAULT 2160 CHECK (target_hours > 0),
                deck_code TEXT NOT NULL COLLATE NOCASE,
                basic INTEGER NOT NULL DEFAULT 0 CHECK (basic IN (0, 1)),
                synced_utc TEXT,
                added_utc TEXT NOT NULL,
                updated_utc TEXT NOT NULL,
                FOREIGN KEY (folder_ref) REFERENCES deck_folder (deck_folder_id) ON DELETE SET NULL,
                FOREIGN KEY (stencil_ref) REFERENCES stencil (stencil_id)
            );

            CREATE TABLE IF NOT EXISTS bearing (
                deck_parent INTEGER NOT NULL,
                direction INTEGER NOT NULL CHECK (direction IN (1, 2)),
                front_sides INTEGER NOT NULL CHECK (front_sides BETWEEN 1 AND 31),
                back_sides INTEGER NOT NULL CHECK (back_sides BETWEEN 1 AND 31),
                statistics_set INTEGER NOT NULL DEFAULT 1 CHECK (statistics_set BETWEEN 1 AND 4),
                PRIMARY KEY (deck_parent, direction),
                FOREIGN KEY (deck_parent) REFERENCES deck (deck_id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS cue (
                cue_id INTEGER PRIMARY KEY AUTOINCREMENT,
                deck_parent INTEGER NOT NULL,
                position INTEGER NOT NULL CHECK (position >= 0),
                grain INTEGER NOT NULL CHECK (grain BETWEEN 0 AND 5),
                entry_ref INTEGER,
                sense_ref INTEGER,
                collocation_ref INTEGER,
                example_ref INTEGER,
                target_ref INTEGER,
                flag INTEGER NOT NULL DEFAULT 0 CHECK (flag IN (0, 1)),
                excluded INTEGER NOT NULL DEFAULT 0 CHECK (excluded IN (0, 1)),
                text1 TEXT NOT NULL DEFAULT '',
                text2 TEXT NOT NULL DEFAULT '',
                added_utc TEXT NOT NULL,
                CHECK (entry_ref IS NULL OR grain = 1),
                CHECK (sense_ref IS NULL OR grain IN (2, 5)),
                CHECK (collocation_ref IS NULL OR grain IN (3, 5)),
                CHECK (example_ref IS NULL OR grain = 4),
                CHECK (target_ref IS NULL OR grain = 5),
                CHECK (sense_ref IS NULL OR collocation_ref IS NULL),
                FOREIGN KEY (deck_parent) REFERENCES deck (deck_id) ON DELETE CASCADE,
                FOREIGN KEY (entry_ref) REFERENCES entry (entry_id) ON DELETE SET NULL,
                FOREIGN KEY (sense_ref) REFERENCES sense (sense_id) ON DELETE SET NULL,
                FOREIGN KEY (collocation_ref) REFERENCES collocation (collocation_id) ON DELETE SET NULL,
                FOREIGN KEY (example_ref) REFERENCES example (example_id) ON DELETE SET NULL,
                FOREIGN KEY (target_ref) REFERENCES entry (entry_id) ON DELETE SET NULL
            );

            CREATE TABLE IF NOT EXISTS cue_cell (
                cue_parent INTEGER NOT NULL,
                column_kind INTEGER NOT NULL CHECK (column_kind BETWEEN 1 AND 7),
                column_index INTEGER NOT NULL,
                value TEXT NOT NULL,
                PRIMARY KEY (cue_parent, column_kind, column_index),
                CHECK ((column_kind = 1 AND column_index BETWEEN 1 AND 9)
                    OR (column_kind IN (2, 3) AND column_index BETWEEN 1 AND 5)
                    OR (column_kind IN (4, 5) AND column_index = 1)
                    OR (column_kind IN (6, 7) AND column_index BETWEEN 1 AND 4)),
                FOREIGN KEY (cue_parent) REFERENCES cue (cue_id) ON DELETE CASCADE
            );
            """;
        command.ExecuteNonQuery();
    }

    public static void LSchemaCueSettle(SqliteConnection connection, string into)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrWhiteSpace(into);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            $"""
            UPDATE {into}.cue SET entry_ref = NULL
            WHERE entry_ref IS NOT NULL AND entry_ref NOT IN (SELECT entry_id FROM {into}.entry);
            UPDATE {into}.cue SET sense_ref = NULL
            WHERE sense_ref IS NOT NULL AND sense_ref NOT IN (SELECT sense_id FROM {into}.sense);
            UPDATE {into}.cue SET collocation_ref = NULL
            WHERE collocation_ref IS NOT NULL
              AND collocation_ref NOT IN (SELECT collocation_id FROM {into}.collocation);
            UPDATE {into}.cue SET example_ref = NULL
            WHERE example_ref IS NOT NULL AND example_ref NOT IN (SELECT example_id FROM {into}.example);
            UPDATE {into}.cue SET target_ref = NULL
            WHERE target_ref IS NOT NULL AND target_ref NOT IN (SELECT entry_id FROM {into}.entry);
            """;
        command.ExecuteNonQuery();
    }
}
