using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LRigFactorySentence
{
    public static LRigSentence LRigSentenceBuild(LDatabase database) =>
        new(
            new LExampleArchive(database),
            new LMentionArchive(database),
            new LSentenceArchive(database),
            new LTranslationArchive(database));
}
