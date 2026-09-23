using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LMentionFacade
{
    private readonly LEngine _lMentionFacadeEngine;
    private readonly object _lMentionFacadeGate;

    public LMentionFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lMentionFacadeEngine = engine;
        _lMentionFacadeGate = engine.LEngineGate;
    }

    private LEngineStaff LMentionFacadeStaff => _lMentionFacadeEngine.LEngineStaffHeld;

    public LMentionResult LEngineMentionFind(long exampleId, int offset)
    {
        lock (_lMentionFacadeGate)
        {
            return LMentionFacadeStaff.LEngineStaffMention.LMentionClerkFind(exampleId, offset);
        }
    }

    public LMentionResult LEngineMentionFind(
        string text, string language, int offset, IReadOnlyList<LMention> mentions)
    {
        lock (_lMentionFacadeGate)
        {
            return LMentionFacadeStaff.LEngineStaffMention.LMentionClerkFind(text, language, offset, mentions);
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
        lock (_lMentionFacadeGate)
        {
            return LMentionFacadeStaff.LEngineStaffMention.LMentionClerkResolve(text, mentions);
        }
    }
}
