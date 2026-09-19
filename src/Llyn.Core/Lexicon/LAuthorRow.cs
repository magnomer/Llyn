using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LAuthorRow(
    long LAuthorRowId,
    string LAuthorRowName,
    int LAuthorRowPosition,
    bool LAuthorRowEarlier,
    bool LAuthorRowLater)
{
    public static IReadOnlyList<LAuthorRow> LAuthorRowCreate(IReadOnlyList<LAuthor> credits)
    {
        ArgumentNullException.ThrowIfNull(credits);

        List<LAuthorRow> rows = new(credits.Count);
        for (int index = 0; index < credits.Count; index++)
        {
            LAuthor author = credits[index];
            rows.Add(new LAuthorRow(
                author.LAuthorId,
                author.LAuthorName,
                index,
                index > 0,
                index < credits.Count - 1));
        }

        return rows;
    }
}
