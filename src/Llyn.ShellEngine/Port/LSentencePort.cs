using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LSentencePort
{
    IReadOnlyList<string> LEngineDependenceRead(string language);

    IReadOnlyList<string> LEngineParticleRead(string language);

    LSentenceOrder LEngineOrderRead(string language);
}
