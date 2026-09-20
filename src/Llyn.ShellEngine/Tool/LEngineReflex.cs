using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<long> LEngineAnchorToggle(IReadOnlyList<long> anchors, long fanqieId, bool anchored)
    {
        return LReflexClerk.LReflexAnchorToggle(anchors, fanqieId, anchored);
    }

    public bool LEngineAnchorMatch(IReadOnlyList<long> one, IReadOnlyList<long> other)
    {
        return LReflexClerk.LReflexAnchorMatch(one, other);
    }

    public IReadOnlyList<LReflexRule> LEngineReflexRead(string language)
    {
        lock (_lEngineGate)
        {
            return _lEngineReflexClerk.LReflexRuleRead(language);
        }
    }

    public IReadOnlyList<LReflex> LEngineReflexRead(long entryId)
    {
        lock (_lEngineGate)
        {
            return _lEngineReflexClerk.LReflexClerkRead(entryId);
        }
    }

    internal IReadOnlyList<LReflex> LEngineReflexSet(long entryId, IReadOnlyList<LReflex> reflexes)
    {
        lock (_lEngineGate)
        {
            return _lEngineReflexClerk.LReflexClerkSet(entryId, reflexes);
        }
    }

    public void LEngineReflexStart(long entryId)
    {
        _lEngineReflexClerk.LReflexClerkStart(entryId);
    }

    public void LEngineReflexRebuild(long entryId)
    {
        _lEngineReflexClerk.LReflexClerkRebuild(entryId);
    }

    public bool LEngineReflexCheck(long entryId)
    {
        return _lEngineReflexClerk.LReflexClerkCheck(entryId);
    }

    internal Task<IReadOnlyList<LReflexDraft>> LEngineReflexFind(
        string headword, string language, CancellationToken cancellation)
    {
        return _lEngineReflexClerk.LReflexClerkFind(headword, language, cancellation);
    }

    public IReadOnlyList<LAnatomyTone> LEngineToneRead(string language)
    {
        return string.IsNullOrWhiteSpace(language) ? [] : LEngineLanguageLoad(language).LLanguageAnatomyTones;
    }
}
