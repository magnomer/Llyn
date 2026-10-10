using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LStemPage(
    string LStemPageLanguage,
    string LStemPageKey,
    IReadOnlyList<string> LStemPageCharacters,
    IReadOnlyList<LStemMember>? LStemPageMembers = null)
{
    public static readonly LStemPage LStemPageBlank = new(string.Empty, string.Empty, []);

    public IReadOnlyList<LStemMember> LStemPageMembers { get; init; } =
        LStemPageMembers ?? LStemMember.LStemBareScan(LStemPageCharacters);

    public bool LStemPageEmpty => LStemPageCharacters.Count == 0;
}
