using System;
using System.Collections.Generic;
using System.IO;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private const string LEngineRecordingBucket = "audio";

    public void LEngineRecordingSweep()
    {
        lock (_lEngineGate)
        {
            string folder = Path.Combine(_lEngineWorkspace, LEngineRecordingBucket);
            if (!Directory.Exists(folder))
            {
                return;
            }

            HashSet<string> kept = new(StringComparer.OrdinalIgnoreCase);
            foreach (string file in new LPronunciationArchive(_lEngineDatabase).LPronunciationAudioScan())
            {
                LEngineRecordingPlace(kept, file);
            }

            foreach (LDraft draft in LDraftArchive.LDraftArchiveScan(_lEngineWorkspace))
            {
                foreach (LPronunciationDraft spoken in draft.LDraftContent.LEntryDraftPronunciations)
                {
                    LEngineRecordingPlace(kept, spoken.LPronunciationDraftAudio);
                }
            }

            foreach (string file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
            {
                if (kept.Contains(Path.GetFullPath(file)))
                {
                    continue;
                }

                try
                {
                    File.Delete(file);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
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
