using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LRigFactoryKeeping
{
    public static LRigKeeping LRigKeepingBuild(LDatabase database) =>
        new(
            new LDoctor(database),
            new LRevisionArchive(database),
            new LWorkspaceArchive(database),
            new LTombstoneArchive(database),
            new LFavoriteArchive(database),
            new LFoldArchive(database),
            new LNoteArchive(database));
}
