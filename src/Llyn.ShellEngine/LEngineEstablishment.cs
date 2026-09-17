using System.IO;
using Llyn.Core;
using Llyn.Infrastructure;

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

            long entries = new LEntryArchive(_lEngineDatabase).LEntryCountRead();

            FileInfo file = new(_lEngineDatabase.LDatabaseFile);
            long size = file.Exists ? file.Length : 0;

            return new LEstablishment(unsaved, entries, size);
        }
    }
}
