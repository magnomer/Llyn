namespace Llyn.Core;

public interface LTombstoneVault
{
    LTombstone LTombstoneRecord(long entryId, long revisionId);

    LTombstone? LTombstoneRead(long entryId);
}
