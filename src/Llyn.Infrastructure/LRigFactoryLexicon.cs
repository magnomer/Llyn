using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LRigFactoryLexicon
{
    public static LRigLexicon LRigLexiconBuild(LDatabase database) =>
        new(
            new LEtymologyArchive(database),
            new LCollocationArchive(database),
            new LGlossArchive(database),
            new LInflectionArchive(database),
            new LLacunaArchive(database),
            new LMeaningArchive(database),
            new LMorphologyArchive(database),
            new LSpeechArchive(database));
}
