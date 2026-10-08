using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LMentionFacade : LMentionPort
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
            return LMentionFacadeStaff.LEngineStaffCatalog.LCatalogStaffMention.LMentionClerkFind(exampleId, offset);
        }
    }

    public LMentionResult LEngineMentionFind(LEntryDraft shown, long sentence, int offset)
    {
        lock (_lMentionFacadeGate)
        {
            return LMentionFacadeStaff.LEngineStaffCatalog.LCatalogStaffMention
                .LMentionClerkFind(shown, sentence, offset);
        }
    }

    public LMentionResult LEngineEtymologyFind(LEntryDraft shown, int offset)
    {
        lock (_lMentionFacadeGate)
        {
            return LMentionFacadeStaff.LEngineStaffCatalog.LCatalogStaffMention.LMentionEtymologyFind(shown, offset);
        }
    }

    public long? LEngineLinkRead(long? link)
    {
        return link is long id && id != 0 ? id : null;
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

    public IReadOnlyList<LMentionLabel> LEngineEtymologyResolve(LTenure held)
    {
        ArgumentNullException.ThrowIfNull(held);

        LDraft? draft = held.LTenureRead();
        lock (_lMentionFacadeGate)
        {
            return LMentionFacadeStaff.LEngineStaffCatalog.LCatalogStaffMention.LMentionEtymologyResolve(draft);
        }
    }

    public IReadOnlyList<LMentionLabel> LEngineMentionResolve(LTenure held, long card, long sentence)
    {
        ArgumentNullException.ThrowIfNull(held);

        LDraft? draft = held.LTenureRead();
        lock (_lMentionFacadeGate)
        {
            return LMentionFacadeStaff.LEngineStaffCatalog.LCatalogStaffMention
                .LMentionClerkResolve(draft, card, sentence);
        }
    }

    public IReadOnlyDictionary<long, IReadOnlyList<LMentionLabel>> LEngineMentionResolve(LTenure held)
    {
        ArgumentNullException.ThrowIfNull(held);

        LDraft? draft = held.LTenureRead();
        lock (_lMentionFacadeGate)
        {
            return LMentionFacadeStaff.LEngineStaffCatalog.LCatalogStaffMention.LMentionClerkResolve(draft);
        }
    }
}
