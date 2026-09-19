using System.Collections.Generic;
using System.IO;

namespace Llyn.Core;

public interface LLocalizationVault
{
    IReadOnlyList<string> LLocalizationScan();

    TextReader LLocalizationOpen(string language);
}
