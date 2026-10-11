using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public sealed record LGraspStar(
    IReadOnlyList<IReadOnlyList<int>> LGraspStarFill,
    IReadOnlyList<bool> LGraspStarUnrated,
    IReadOnlyList<IReadOnlyList<int>> LGraspStarHit)
{
    public static LGraspStar LGraspStarResolve(int limit)
    {
        int last = Math.Max(0, limit);
        int count = Math.Min(LGrasp.LGraspStarScan(0).Count, (last + 1) / 2);
        List<IReadOnlyList<int>> fills = [];
        List<bool> unrated = [];
        for (int step = 0; step <= last; step++)
        {
            fills.Add([.. LGrasp.LGraspStarScan(step).Take(count)]);
            unrated.Add(step == 0);
        }

        List<IReadOnlyList<int>> hits = [];
        for (int star = 0; star < count; star++)
        {
            hits.Add([Math.Min(last, (star * 2) + 1), Math.Min(last, (star * 2) + 2)]);
        }

        return new LGraspStar(fills, unrated, hits);
    }
}
