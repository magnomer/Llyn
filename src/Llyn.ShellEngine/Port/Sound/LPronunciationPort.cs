using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LPronunciationPort
{
    IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(LVista vista);

    LFrequencyGauge? LEngineFrequencyResolve(long entryId, string once);

    LArticulation LEngineConsonantRead();

    LArticulation LEngineVowelRead();
}
