using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LMentionResult LEngineMentionFind(long exampleId, int offset)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffMention.LMentionClerkFind(exampleId, offset);
        }
    }

    public LMentionResult LEngineMentionFind(
        string text, string language, int offset, IReadOnlyList<LMention> mentions)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffMention.LMentionClerkFind(text, language, offset, mentions);
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

    public LMentionDraft LEngineSpanRead(string text, int start, int length)
    {
        return LMentionClerk.LMentionSpanRead(text, start, length);
    }

    public IReadOnlyList<LMentionLabel> LEngineMentionResolve(string text, IReadOnlyList<LMentionDraft> mentions)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffMention.LMentionClerkResolve(text, mentions);
        }
    }
}
