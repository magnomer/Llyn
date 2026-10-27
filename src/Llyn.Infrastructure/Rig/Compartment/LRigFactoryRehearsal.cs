using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LRigFactoryRehearsal
{
    public static LRigRehearsal LRigRehearsalBuild(LDatabase database, string root, LClock clock) =>
        new(
            new LDeckArchive(database),
            new LDeckFolderArchive(database),
            new LCueArchive(database),
            new LStencilArchive(database),
            new LLockerEntrance(new LKeepFile(root), clock));
}
