using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LRigFactoryContext
{
    public static LRigContext LRigContextBuild(LDatabase database) =>
        new(
            new LRegisterArchive(database),
            new LSituationArchive(database),
            new LTagArchive(database));
}
