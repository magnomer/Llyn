using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchema
{
    public static void LSchemaCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        LSchemaRealm.LSchemaRealmCreate(connection);
        LSchemaEntry.LSchemaEntryCreate(connection);
        LSchemaInflection.LSchemaInflectionCreate(connection);
        LSchemaMeaning.LSchemaMeaningCreate(connection);
        LSchemaPronunciation.LSchemaPronunciationCreate(connection);
        LSchemaTranscription.LSchemaTranscriptionCreate(connection);
        LSchemaReflex.LSchemaReflexCreate(connection);
        LSchemaCollocation.LSchemaCollocationCreate(connection);
        LSchemaReference.LSchemaReferenceCreate(connection);
        LSchemaQuotation.LSchemaQuotationCreate(connection);
        LSchemaMarker.LSchemaMarkerCreate(connection);
        LSchemaRegister.LSchemaRegisterCreate(connection);
        LSchemaImage.LSchemaImageCreate(connection);
        LSchemaVideo.LSchemaVideoCreate(connection);
        LSchemaFavorite.LSchemaFavoriteCreate(connection);
        LSchemaFrequency.LSchemaFrequencyCreate(connection);
        LSchemaScript.LSchemaScriptCreate(connection);
        LSchemaFanqie.LSchemaFanqieCreate(connection);
        LSchemaDiwei.LSchemaDiweiCreate(connection);
        LSchemaAnchor.LSchemaAnchorCreate(connection);

        LSchemaRevision.LSchemaRevisionCreate(connection);

        LSchemaMigration.LSchemaMigrationApply(connection);
        LSchemaIndex.LSchemaIndexCreate(connection);
        LSchemaStamp.LSchemaStampCreate(connection);
    }
}
