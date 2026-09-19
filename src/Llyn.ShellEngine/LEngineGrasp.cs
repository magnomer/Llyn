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
            return _lEngineEntries.LEntryRead(entryId)?.LEntryGrasp ?? 0;
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

            _lEngineEntries.LEntryGraspSet(entryId, grasp);
        }

        LEngineBulletinRaise(LSubject.LSubjectGrasp, entryId);
    }
}
