using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LEntry LEngineEntrySave(LEntryDraft draft)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffOutcome.LOutcomeEntrySave(draft, []);
        }
    }

    internal LEntry LEngineEntryUpdate(long id, LEntryDraft draft)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffOutcome.LOutcomeEntryUpdate(id, draft, []);
        }
    }

    private void LEngineUpdatedSet(long entryId)
    {
        _lEngineStaff.LEngineStaffEntry.LEntryUpdatedSet(entryId);
    }

    private void LEngineUpdatedSet(long ownerId, bool collocation)
    {
        _lEngineStaff.LEngineStaffCard.LCardUpdatedSet(ownerId, collocation);
    }
}
