using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Conduct;

public sealed record CStemMember(
    string CStemMemberCharacter,
    string CStemMemberReading,
    IReadOnlyList<CReflex> CStemMemberReflexes,
    bool CStemMemberOpened,
    bool CStemMemberFoldable)
{
    internal static IReadOnlyList<CStemMember> LStemMemberRead(IReadOnlyList<LStemMember> members)
    {
        ArgumentNullException.ThrowIfNull(members);

        return members.Select(LStemMemberRead).ToList();
    }

    private static CStemMember LStemMemberRead(LStemMember member)
    {
        IReadOnlyList<CReflex> rows = CRespelling.LRespellingReflexScan(
            CReflexDraft.CReflexDraftRead(member.LStemMemberReflexes), member.LStemMemberGuises);
        return new CStemMember(
            member.LStemMemberCharacter,
            member.LStemMemberReading,
            rows,
            member.LStemMemberOpened,
            LStemFoldCheck(rows));
    }

    private static bool LStemFoldCheck(IReadOnlyList<CReflex> rows)
    {
        return rows.Count > 0;
    }
}
