using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CStemPage(
    string CStemPageLanguage,
    string CStemPageKey,
    IReadOnlyList<string> CStemPageCharacters,
    bool CStemPageEmpty);
