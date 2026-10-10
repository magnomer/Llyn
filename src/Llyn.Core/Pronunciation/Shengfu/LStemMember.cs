using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LStemMember(
    string LStemMemberCharacter,
    IReadOnlyList<string> LStemMemberReadings,
    IReadOnlyList<LReflexDraft> LStemMemberReflexes,
    IReadOnlyList<LReflexGuise>? LStemMemberGuises = null)
{
    public IReadOnlyList<LReflexGuise> LStemMemberGuises { get; init; } = LStemMemberGuises ?? [];

    public bool LStemMemberOpened { get; init; }

    public string LStemMemberReading => LFanqieGroup.LFanqieReadingFormat([LStemMemberReadings]);

    public IReadOnlyList<(LReflexDraft, LReflexGuise?)> LStemMemberRows =>
        LStemMemberReflexes
            .Select((reflex, index) => (reflex, LReflexGuise.LReflexGuiseFind(LStemMemberGuises, index)))
            .ToList();

    public IReadOnlyList<string> LStemMemberLanguages =>
        LStemMemberReflexes.Select(static reflex => reflex.LReflexDraftLanguage).ToList();

    public static IReadOnlyList<LStemMember> LStemBareScan(IReadOnlyList<string> characters)
    {
        ArgumentNullException.ThrowIfNull(characters);

        return characters.Select(static character => new LStemMember(character, [], [])).ToList();
    }

    public static long? LStemMemberFind(IReadOnlyList<LEntry> entries, string character)
    {
        ArgumentNullException.ThrowIfNull(entries);

        return entries
            .FirstOrDefault(entry => string.Equals(entry.LEntryHeadword.Trim(), character, StringComparison.Ordinal))
            ?.LEntryId;
    }
}
