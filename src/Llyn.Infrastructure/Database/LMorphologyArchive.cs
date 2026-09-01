using System;
using Llyn.Core;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public sealed class LMorphologyArchive
{
    private readonly LDatabase _lMorphologyArchiveDatabase;

    public LMorphologyArchive(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lMorphologyArchiveDatabase = database;
    }

    public void LMorphologyCreate(LMorphology morphology)
    {
        ArgumentNullException.ThrowIfNull(morphology);

        using LDatabaseSession session = _lMorphologyArchiveDatabase.LDatabaseSessionStart();
        using (SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand())
        {
            command.CommandText =
                """
                INSERT INTO morphology_value
                    (language, part_of_speech_id, feature_id, feature_display_name,
                     value_id, value_display_name, position)
                VALUES ($language, $speech, $feature, $featureName, $value, $valueName, $position)
                ON CONFLICT (language, part_of_speech_id, feature_id, value_id)
                DO UPDATE SET feature_display_name = excluded.feature_display_name,
                              value_display_name = excluded.value_display_name,
                              position = excluded.position;
                """;
            command.Parameters.AddWithValue("$language", morphology.LMorphologyLanguage);
            command.Parameters.AddWithValue("$speech", morphology.LMorphologySpeechId);
            command.Parameters.AddWithValue("$feature", morphology.LMorphologyFeatureId);
            command.Parameters.AddWithValue("$featureName", morphology.LMorphologyFeatureName);
            command.Parameters.AddWithValue("$value", morphology.LMorphologyValueId);
            command.Parameters.AddWithValue("$valueName", morphology.LMorphologyValueName);
            command.Parameters.AddWithValue("$position", morphology.LMorphologyPosition);
            command.ExecuteNonQuery();
        }

        session.LDatabaseSessionCommit();
    }

    public LMorphology? LMorphologyRead(string language, string speechId, string featureId, string valueId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(speechId);
        ArgumentException.ThrowIfNullOrWhiteSpace(featureId);
        ArgumentException.ThrowIfNullOrWhiteSpace(valueId);

        using LDatabaseSession session = _lMorphologyArchiveDatabase.LDatabaseSessionStart();
        using SqliteCommand command = session.LDatabaseSessionConnection.CreateCommand();
        command.CommandText =
            """
            SELECT feature_display_name, value_display_name, position
            FROM morphology_value
            WHERE language = $language AND part_of_speech_id = $speech
              AND feature_id = $feature AND value_id = $value;
            """;
        command.Parameters.AddWithValue("$language", language);
        command.Parameters.AddWithValue("$speech", speechId);
        command.Parameters.AddWithValue("$feature", featureId);
        command.Parameters.AddWithValue("$value", valueId);

        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new LMorphology(
            language,
            speechId,
            featureId,
            reader.GetString(0),
            valueId,
            reader.GetString(1),
            reader.GetInt32(2));
    }
}
