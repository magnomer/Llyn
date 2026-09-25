using System.Collections.Generic;

namespace Llyn.Core;

public interface LScriptVault
{
    void LScriptSave(string language, string character, IReadOnlyList<LScriptImage> images);

    IReadOnlyList<LScriptImage> LScriptRead(string language, string character);
}
