using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LRigFactoryDraft
{
    public static LRigDraft LRigDraftBuild(string root) =>
        new(
            new LDraftArchive(root),
            new LClaimArchive(root),
            new LCourtArchive(root));
}
