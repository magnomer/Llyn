using System;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public int LEngineGraspRead(long entryId)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
            return new LEntryArchive(_lEngineDatabase).LEntryRead(entryId)?.LEntryGrasp ?? 0;
        }
    }

    public void LEngineGraspSave(long entryId, int grasp)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entryId);
            if (!LGrasp.LGraspCheck(grasp))
            {
                throw new ArgumentOutOfRangeException(nameof(grasp));
            }

            new LEntryArchive(_lEngineDatabase).LEntryGraspSet(entryId, grasp);
        }

        LEngineBulletinRaise(LSubject.LSubjectGrasp, entryId);
    }
}
