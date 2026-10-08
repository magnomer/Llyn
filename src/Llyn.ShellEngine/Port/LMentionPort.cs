using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LMentionPort
{
    LMentionResult LEngineMentionFind(long exampleId, int offset);

    LMentionResult LEngineMentionFind(LEntryDraft shown, long sentence, int offset);

    LMentionResult LEngineEtymologyFind(LEntryDraft shown, int offset);

    long? LEngineLinkRead(long? link);
}
