using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LRigFactoryCitation
{
    public static LRigCitation LRigCitationBuild(LDatabase database) =>
        new(
            new LAuthorArchive(database),
            new LImageArchive(database),
            new LReferenceArchive(database),
            new LVideoArchive(database));
}
