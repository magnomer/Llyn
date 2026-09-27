using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CLedgerState(
    CSettings CLedgerStateSettings,
    string CLedgerStatePath,
    string CLedgerStateLocalization,
    IReadOnlyDictionary<string, string> CLedgerStateTexts,
    IReadOnlyList<KeyValuePair<string, string>> CLedgerStateLanguages,
    IReadOnlyList<CLedgerPage> CLedgerStatePages);
