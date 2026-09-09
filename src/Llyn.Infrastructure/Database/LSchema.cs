using System;
using Microsoft.Data.Sqlite;

namespace Llyn.Infrastructure;

public static class LSchema
{
    public static void LSchemaCreate(SqliteConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        LSchemaEntry.LSchemaEntryCreate(connection);
        LSchemaInflection.LSchemaInflectionCreate(connection);
        LSchemaMeaning.LSchemaMeaningCreate(connection);
        LSchemaRelation.LSchemaRelationCreate(connection);
        LSchemaPronunciation.LSchemaPronunciationCreate(connection);
        LSchemaCollocation.LSchemaCollocationCreate(connection);
        LSchemaReference.LSchemaReferenceCreate(connection);
        LSchemaQuotation.LSchemaQuotationCreate(connection);
        LSchemaMarker.LSchemaMarkerCreate(connection);
        LSchemaRegister.LSchemaRegisterCreate(connection);
        LSchemaImage.LSchemaImageCreate(connection);
        LSchemaVideo.LSchemaVideoCreate(connection);
        LSchemaFavorite.LSchemaFavoriteCreate(connection);

        LSchemaRevision.LSchemaRevisionCreate(connection);

        LSchemaMigration.LSchemaMigrationApply(connection);
        LSchemaIndex.LSchemaIndexCreate(connection);
    }
}
