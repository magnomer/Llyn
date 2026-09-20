using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LEntry LEngineEntrySave(LEntryDraft draft)
    {
        lock (_lEngineGate)
        {
            return _lEngineOutcomeClerk.LOutcomeEntrySave(draft, []);
        }
    }

    internal LEntry LEngineEntryUpdate(long id, LEntryDraft draft)
    {
        lock (_lEngineGate)
        {
            return _lEngineOutcomeClerk.LOutcomeEntryUpdate(id, draft, []);
        }
    }

    private void LEngineUpdatedSet(long entryId)
    {
        _lEngineEntryClerk.LEntryUpdatedSet(entryId);
    }

    private void LEngineUpdatedSet(long ownerId, bool collocation)
    {
        _lEngineCardClerk.LCardUpdatedSet(ownerId, collocation);
    }
}
