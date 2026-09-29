using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CMarkupEntry(
    string CMarkupEntryLanguage,
    string CMarkupEntryName,
    IReadOnlyList<long> CMarkupEntryTarget);
