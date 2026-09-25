using System.Globalization;

namespace Llyn.Core;

public static class LGrasp
{
    public const int LGraspStep = 10;

    public static bool LGraspCheck(int grasp)
    {
        return grasp is >= 0 and <= LGraspStep;
    }

    public static string LGraspKeyRead(int grasp)
    {
        return "Grasp.Level" + grasp.ToString(CultureInfo.InvariantCulture);
    }
}
