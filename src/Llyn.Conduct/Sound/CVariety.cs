using System;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed record CVariety(string CVarietyName, string CVarietyKey, string CVarietyEnsign)
{
    public static CVariety CVarietyRead(string language, string variety)
    {
        ArgumentNullException.ThrowIfNull(variety);

        return new CVariety(
            variety, string.Concat("Variety.", variety), LSettingsPort.LEngineEnsignFormat(language, variety));
    }
}
