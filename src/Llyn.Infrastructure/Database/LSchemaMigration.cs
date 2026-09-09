using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchemaMigration
{
    public const long LSchemaMigrationVersion = 32;

    public static void LSchemaMigrationApply(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        if (!LSchemaVersion.LSchemaVersionExist(connection))
        {
            LSchemaVersion.LSchemaVersionCreate(connection);
            return;
        }

        long stored = LSchemaVersion.LSchemaVersionRead(connection);
        if (stored > LSchemaMigrationVersion)
        {
            throw new InvalidOperationException(
                $"The workspace database is at schema version {stored}, which is newer than the version " +
                $"{LSchemaMigrationVersion} this build understands. Update Llyn before opening it.");
        }

        if (stored >= LSchemaMigrationVersion)
        {
            return;
        }

        if (stored < 20)
        {
            LSchemaTranslation.LSchemaTranslationCreate(connection);
        }

        if (stored < 12)
        {
            LSchemaExample.LSchemaExampleNormalize(connection);
            LSchemaPosition.LSchemaPositionNormalize(connection, "relation", "sense_id");
            LSchemaPosition.LSchemaPositionNormalize(connection, "collocation", "entry_id");
            LSchemaPosition.LSchemaPositionNormalize(connection, "collocation_synonym", "collocation_id");
            LSchemaVersion.LSchemaVersionNormalize(connection);
        }

        if (stored < 13)
        {
            LSchemaColumn.LSchemaCollocationNormalize(connection);
        }

        if (stored < 14)
        {
            LSchemaTable.LSchemaAudioNormalize(connection);
        }

        if (stored < 15)
        {
            LSchemaColumn.LSchemaTitleNormalize(connection, "sense");
            LSchemaColumn.LSchemaTitleNormalize(connection, "collocation");
        }

        if (stored < 16)
        {
            LSchemaSpeech.LSchemaSpeechNormalize(connection);
        }

        if (stored < 18)
        {
            LSchemaState.LSchemaStateNormalize(connection);
        }

        if (stored < 19)
        {
            LSchemaTag.LSchemaTagNormalize(connection);
        }

        if (stored < 21)
        {
            LSchemaTranslation.LSchemaTranslationNormalize(connection);
        }

        if (stored < 22)
        {
            LSchemaColumn.LSchemaSituationNormalize(connection);
        }

        if (stored < 23)
        {
            LSchemaTable.LSchemaFavoriteCreate(connection);
        }

        if (stored < 24)
        {
            LSchemaSentence.LSchemaSentenceNormalize(connection, "sense_example", "sense_id", "sense");
        }

        if (stored < 25)
        {
            LSchemaSentence.LSchemaSentenceNormalize(connection, "collocation_example", "collocation_id", "collocation");
        }

        if (stored < 26)
        {
            LSchemaWorkspace.LSchemaWorkspaceNormalize(connection);
        }

        if (stored < 27)
        {
            LSchemaFrame.LSchemaFrameNormalize(connection, "sense_example", "sense_id", "sense");
            LSchemaFrame.LSchemaFrameNormalize(connection, "collocation_example", "collocation_id", "collocation");
        }

        if (stored < 28)
        {
            LSchemaColumn.LSchemaVideoNormalize(connection);
        }

        if (stored < 29)
        {
            LSchemaTable.LSchemaRegisterCreate(connection);
        }

        if (stored < 30)
        {
            LSchemaWorkspace.LSchemaWorkspaceNormalize(connection);
        }

        if (stored < 31)
        {
            LSchemaSource.LSchemaSourceNormalize(connection);
        }

        if (stored < 32)
        {
            LSchemaShortcut.LSchemaShortcutRemove(connection);
        }

        LSchemaVersion.LSchemaVersionSave(connection);
    }
}
