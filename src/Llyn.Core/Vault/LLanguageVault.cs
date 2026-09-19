using System.Collections.Generic;

namespace Llyn.Core;

public interface LLanguageVault
{
    IReadOnlyList<string> LLanguageScan();

    LLanguage LLanguageRead(string language);

    bool LLanguageNameValidate(string? language);
}
