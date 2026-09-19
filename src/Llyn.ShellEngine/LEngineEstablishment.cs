using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LEstablishment LEngineEstablishmentRead()
    {
        lock (_lEngineGate)
        {
            int unsaved = 0;
            foreach (long id in _lEngineDraftHeld)
            {
                if (LEngineDraftCheck(id))
                {
                    unsaved++;
                }
            }

            long entries = _lEngineEntries.LEntryCountRead();

            return new LEstablishment(unsaved, entries, _lEngineWorkspaces.LWorkspaceSizeRead());
        }
    }
}
