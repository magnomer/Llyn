using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Llyn.Core;

public interface LLanguageVault
{
    IReadOnlyList<string> LLanguageScan();

    LLanguage LLanguageRead(string language);

    bool LLanguageNameValidate(string? language);

    Task<string?> LLanguageFlagRead(string code, CancellationToken cancellation);
}
