using System;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LGraspClerk
{
    private readonly LEntryVault _lGraspClerkEntries;

    public LGraspClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lGraspClerkEntries = rig.LRigEntries;
    }

    public static int LGraspClerkStep => LGrasp.LGraspStep;

    public static string LGraspClerkFormat(int step)
    {
        return LLocalization.QLocalizationTextRead(LGrasp.LGraspKeyRead(step));
    }

    public void LGraspClerkSet(long entryId, int grasp)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
        if (!LGrasp.LGraspCheck(grasp))
        {
            throw new ArgumentOutOfRangeException(nameof(grasp));
        }

        _lGraspClerkEntries.LEntryGraspSet(entryId, grasp);
    }
}
