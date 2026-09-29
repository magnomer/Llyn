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

    public LMentionResult LEngineMentionFind(
        string text, string language, LEntryDraft? shown, int offset, IReadOnlyList<LMention>? mentions)
    {
        return LEngineMentionFind(
            text,
            language.Length > 0 ? language : shown?.LEntryDraftLanguage ?? string.Empty,
            offset,
            mentions ?? []);
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

    public bool LEngineSpanCheck(string text, int start, int length)
    {
        return LEngineSpanRead(text, start, length).LMentionDraftLength > 0;
    }

    public IReadOnlyList<LMentionLabel> LEngineMentionResolve(string text, IReadOnlyList<LMentionDraft> mentions)
    {
        lock (_lMentionFacadeGate)
        {
            return LMentionFacadeStaff.LEngineStaffMention.LMentionClerkResolve(text, mentions);
        }
    }

    public IReadOnlyList<LMentionLabel> LEngineMentionResolve(LTenure held, long card, long sentence)
    {
        ArgumentNullException.ThrowIfNull(held);

        LDraft? draft = held.LTenureRead();
        lock (_lMentionFacadeGate)
        {
            return LMentionFacadeStaff.LEngineStaffMention.LMentionClerkResolve(draft, card, sentence);
        }
    }

    public IReadOnlyDictionary<long, IReadOnlyList<LMentionLabel>> LEngineMentionResolve(LTenure held)
    {
        ArgumentNullException.ThrowIfNull(held);

        LDraft? draft = held.LTenureRead();
        lock (_lMentionFacadeGate)
        {
            return LMentionFacadeStaff.LEngineStaffMention.LMentionClerkResolve(draft);
        }
    }
}
