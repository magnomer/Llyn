using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LRigFactoryRehearsal
{
    public static LRigRehearsal LRigRehearsalBuild(LDatabase database, string root) =>
        new(
            new LDeckArchive(database),
            new LDeckFolderArchive(database),
            new LCueArchive(database),
            new LStencilArchive(database),
            new LDepotFolder(),
            new LDeckManifestFile(new LKeepFile(root)));
}
