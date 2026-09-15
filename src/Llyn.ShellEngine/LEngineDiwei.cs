using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LDiwei> LEngineDiweiRead(string language, string kind)
    {
        lock (_lEngineGate)
        {
            return new LDiweiArchive(_lEngineDatabase).LDiweiRead(language, kind);
        }
    }

    public LDiwei? LEngineDiweiFind(string language, string kind, string key)
    {
        lock (_lEngineGate)
        {
            return new LDiweiArchive(_lEngineDatabase).LDiweiFind(language, kind, key);
        }
    }

    public IReadOnlyList<long> LEngineDiweiScan(string language, IReadOnlyList<long> diweiIds)
    {
        lock (_lEngineGate)
        {
            return new LDiweiArchive(_lEngineDatabase).LDiweiEntryScan(language, diweiIds);
        }
    }

    public void LEngineDiweiRebuild()
    {
        lock (_lEngineGate)
        {
            LEngineDiweiApply();
        }
    }

    private void LEngineDiweiApply()
    {
        foreach (string language in LLanguageLoader.LLanguageLoaderScan())
        {
            if (LEngineBookRead(language).Count > 0)
            {
                new LDiweiArchive(_lEngineDatabase).LDiweiRebuild(language, LEngineHypothesisRead(language));
            }
        }
    }
}
