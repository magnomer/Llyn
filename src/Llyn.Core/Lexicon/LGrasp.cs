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

    public static string LGraspFormat(int grasp)
    {
        return (grasp / 2.0).ToString("0.#", CultureInfo.InvariantCulture);
    }
}
