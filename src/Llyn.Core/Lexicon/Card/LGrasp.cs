using System;
using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Core;

public static class LGrasp
{
    public const int LGraspStep = 10;

    public static bool LGraspCheck(int grasp)
    {
        return grasp is >= 0 and <= LGraspStep;
    }

    public static int LGraspNormalize(int grasp)
    {
        return Math.Clamp(grasp, 0, LGraspStep);
    }

    public static string LGraspKeyRead(int grasp)
    {
        return "Grasp.Level" + grasp.ToString(CultureInfo.InvariantCulture);
    }

    public static IReadOnlyList<int> LGraspStarScan(int grasp)
    {
        int[] stars = new int[(LGraspStep + 1) / 2];
        for (int star = 0; star < stars.Length; star++)
        {
            stars[star] = Math.Clamp(grasp - (star * 2), 0, 2);
        }

        return stars;
    }
}
