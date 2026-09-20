using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public int LEngineGraspStep => LGrasp.LGraspStep;

    public string LEngineGraspFormat(int step)
    {
        return LLocalization.LLocalizationTextRead(LGrasp.LGraspKeyRead(step));
    }

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
