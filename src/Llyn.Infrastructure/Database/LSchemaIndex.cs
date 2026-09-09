using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaIndex
{
    public static void LSchemaIndexCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using SqliteCommand command = connection.CreateCommand();

        command.CommandText =
            """
            CREATE INDEX IF NOT EXISTS sense_parent ON sense (parent_id);
            CREATE INDEX IF NOT EXISTS relation_sense_origin ON relation (sense_id);
            CREATE INDEX IF NOT EXISTS collocation_entry ON collocation (entry_id);
            CREATE INDEX IF NOT EXISTS example_source ON example (source_id);
            """;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            CREATE INDEX IF NOT EXISTS relation_entry_target ON relation_entry (entry_id);
            CREATE INDEX IF NOT EXISTS relation_sense_target ON relation_sense (sense_id);
            CREATE INDEX IF NOT EXISTS collocation_synonym_owner ON collocation_synonym (collocation_id);
            CREATE INDEX IF NOT EXISTS collocation_synonym_entry ON collocation_synonym (target_entry_id);
            CREATE INDEX IF NOT EXISTS collocation_synonym_sense ON collocation_synonym (target_sense_id);
            """;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            CREATE INDEX IF NOT EXISTS sense_example_member ON sense_example (example_id);
            CREATE INDEX IF NOT EXISTS sense_video_member ON sense_video (video_id);
            CREATE INDEX IF NOT EXISTS collocation_video_member ON collocation_video (video_id);
            CREATE INDEX IF NOT EXISTS sense_image_member ON sense_image (image_id);
            CREATE INDEX IF NOT EXISTS collocation_image_member ON collocation_image (image_id);
            CREATE INDEX IF NOT EXISTS collocation_example_member ON collocation_example (example_id);
            CREATE INDEX IF NOT EXISTS sense_tag_member ON sense_tag (text);
            CREATE INDEX IF NOT EXISTS collocation_tag_member ON collocation_tag (text);
            CREATE INDEX IF NOT EXISTS sense_situation_member ON sense_situation (situation_id);
            CREATE INDEX IF NOT EXISTS collocation_situation_member ON collocation_situation (situation_id);
            CREATE INDEX IF NOT EXISTS sense_register_member ON sense_register (register_id);
            CREATE INDEX IF NOT EXISTS collocation_register_member ON collocation_register (register_id);
            CREATE INDEX IF NOT EXISTS source_author_member ON source_author (author_id);
            """;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            CREATE INDEX IF NOT EXISTS tombstone_revision ON tombstone (revision_id);
            CREATE INDEX IF NOT EXISTS workspace_left ON workspace (left_entry);
            CREATE INDEX IF NOT EXISTS workspace_right ON workspace (right_entry);
            CREATE INDEX IF NOT EXISTS workspace_revision ON workspace (revision);
            """;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            CREATE UNIQUE INDEX IF NOT EXISTS relation_position ON relation (sense_id, position);
            CREATE UNIQUE INDEX IF NOT EXISTS collocation_position ON collocation (entry_id, position);
            CREATE UNIQUE INDEX IF NOT EXISTS collocation_synonym_position
                ON collocation_synonym (collocation_id, position);
            CREATE UNIQUE INDEX IF NOT EXISTS sense_translation_position
                ON sense_translation (sense_id, position);
            CREATE UNIQUE INDEX IF NOT EXISTS collocation_translation_position
                ON collocation_translation (collocation_id, position);
            """;
        command.ExecuteNonQuery();
    }
}
