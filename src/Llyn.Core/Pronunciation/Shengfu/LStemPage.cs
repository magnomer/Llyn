using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LStemPage(
    string LStemPageLanguage,
    string LStemPageKey,
    IReadOnlyList<string> LStemPageCharacters)
{
    public static readonly LStemPage LStemPageBlank = new(string.Empty, string.Empty, []);

    public bool LStemPageEmpty => LStemPageCharacters.Count == 0;
}
