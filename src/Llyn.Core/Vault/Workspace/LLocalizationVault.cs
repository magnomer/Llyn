using System.Collections.Generic;

namespace Llyn.Core;

public interface LLocalizationVault
{
    IReadOnlyList<string> LLocalizationScan();

    IReadOnlyDictionary<string, string> LLocalizationRead(string language);
}
