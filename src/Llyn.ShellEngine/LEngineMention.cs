using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LMentionResult LEngineMentionFind(long exampleId, int offset)
    {
        lock (_lEngineGate)
        {
            return _lEngineMentionClerk.LMentionClerkFind(exampleId, offset);
        }
    }

    public LMentionResult LEngineMentionFind(
        string text, string language, int offset, IReadOnlyList<LMention> mentions)
    {
        lock (_lEngineGate)
        {
            return _lEngineMentionClerk.LMentionClerkFind(text, language, offset, mentions);
        }
    }

    public IReadOnlyList<LMentionPiece> LEngineMentionDivide(string text, IReadOnlyList<LMention> mentions)
    {
        return LMentionClerk.LMentionClerkDivide(text, mentions);
    }

    public int LEngineUnitRead(string text, int offset)
    {
        return LMentionClerk.LMentionUnitRead(text, offset);
    }

    public int LEngineOffsetRead(string text, int unit)
    {
        return LMentionClerk.LMentionOffsetRead(text, unit);
    }

    public IReadOnlyList<LMentionLabel> LEngineMentionResolve(string text, IReadOnlyList<LMentionDraft> mentions)
    {
        lock (_lEngineGate)
        {
            return _lEngineMentionClerk.LMentionClerkResolve(text, mentions);
        }
    }
}
