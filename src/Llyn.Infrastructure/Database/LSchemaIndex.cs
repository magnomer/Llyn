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
            CREATE INDEX IF NOT EXISTS collocation_entry ON collocation (entry_id);
            CREATE INDEX IF NOT EXISTS example_source ON example (source_ref);
            """;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            CREATE INDEX IF NOT EXISTS sense_example_member ON sense_example (example_ref);
            CREATE INDEX IF NOT EXISTS sense_video_member ON sense_video (video_ref);
            CREATE INDEX IF NOT EXISTS collocation_video_member ON collocation_video (video_ref);
            CREATE INDEX IF NOT EXISTS sense_image_member ON sense_image (image_ref);
            CREATE INDEX IF NOT EXISTS collocation_image_member ON collocation_image (image_ref);
            CREATE INDEX IF NOT EXISTS collocation_example_member ON collocation_example (example_ref);
            CREATE INDEX IF NOT EXISTS sense_tag_member ON sense_tag (tag_ref);
            CREATE INDEX IF NOT EXISTS collocation_tag_member ON collocation_tag (tag_ref);
            CREATE INDEX IF NOT EXISTS sense_situation_member ON sense_situation (situation_ref);
            CREATE INDEX IF NOT EXISTS collocation_situation_member ON collocation_situation (situation_ref);
            CREATE INDEX IF NOT EXISTS sense_register_member ON sense_register (register_ref);
            CREATE INDEX IF NOT EXISTS collocation_register_member ON collocation_register (register_ref);
            CREATE INDEX IF NOT EXISTS source_author_member ON source_author (author_ref);
            CREATE INDEX IF NOT EXISTS part_of_speech_value ON part_of_speech (speech_value_ref);
            CREATE INDEX IF NOT EXISTS morphology_feature_speech ON morphology_feature (speech_value_id);
            CREATE INDEX IF NOT EXISTS morphology_value_feature ON morphology_value (morphology_feature_id);
            CREATE INDEX IF NOT EXISTS inflection_speech ON inflection (speech_value_ref);
            CREATE INDEX IF NOT EXISTS inflection_feature_value ON inflection_feature (morphology_value_ref);
            """;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            CREATE INDEX IF NOT EXISTS tombstone_revision ON tombstone (revision_ref);
            CREATE INDEX IF NOT EXISTS workspace_left ON workspace (left_entry_ref);
            CREATE INDEX IF NOT EXISTS workspace_right ON workspace (right_entry_ref);
            CREATE INDEX IF NOT EXISTS workspace_revision ON workspace (revision_ref);
            """;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            CREATE UNIQUE INDEX IF NOT EXISTS collocation_position ON collocation (entry_id, position);
            CREATE UNIQUE INDEX IF NOT EXISTS pronunciation_position ON pronunciation (entry_id, position);
            CREATE UNIQUE INDEX IF NOT EXISTS transcription_position ON transcription (entry_id, position);
            CREATE UNIQUE INDEX IF NOT EXISTS sense_translation_position
                ON sense_translation (sense_id, position);
            CREATE UNIQUE INDEX IF NOT EXISTS collocation_translation_position
                ON collocation_translation (collocation_id, position);
            """;
        command.ExecuteNonQuery();
    }
}
