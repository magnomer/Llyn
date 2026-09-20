using System;
using System.Collections.Generic;
using System.IO;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public void LEngineRecordingSweep()
    {
        lock (_lEngineGate)
        {
            HashSet<string> kept = new(StringComparer.OrdinalIgnoreCase);
            foreach (string file in _lEnginePronunciations.LPronunciationAudioScan())
            {
                LEngineRecordingPlace(kept, file);
            }

            foreach (LDraft draft in _lEngineDrafts.LDraftScan())
            {
                foreach (LPronunciationDraft spoken in draft.LDraftContent.LEntryDraftPronunciations)
                {
                    LEngineRecordingPlace(kept, spoken.LPronunciationDraftAudio);
                }
            }

            _lEngineRecordings.LRecordingSweep(kept);
        }
    }

    private void LEngineRecordingPlace(HashSet<string> kept, string file)
    {
        if (file.Length == 0)
        {
            return;
        }

        if (LEngineLocationResolve(file, _lEngineWorkspace) is { IsFile: true } resolved)
        {
            kept.Add(Path.GetFullPath(resolved.LocalPath));
        }
    }
}
