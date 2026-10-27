using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LOutpostCargo(
    int LOutpostCargoPort,
    string LOutpostCargoToken,
    string LOutpostCargoNotebook,
    string LOutpostCargoRealm,
    IReadOnlyList<LEntry> LOutpostCargoEntries,
    IReadOnlyList<string> LOutpostCargoLanguages,
    Func<long, LLiveryPage?> LOutpostCargoPage,
    Func<string, LLiveryLanguage> LOutpostCargoSound,
    Func<string, string> LOutpostCargoLookup,
    Action<Exception> LOutpostCargoFault);
