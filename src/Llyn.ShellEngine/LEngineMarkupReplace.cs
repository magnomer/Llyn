using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private void LEngineMarkupDetach(long entryId, List<LMarkupOmission> omissions)
    {
        LMentionArchive mentions = new(_lEngineDatabase);
        HashSet<long> affected = [];
        foreach (LMeaning meaning in new LMeaningArchive(_lEngineDatabase).LMeaningRead(entryId))
        {
            foreach (long exampleId in mentions.LMentionSenseClear(meaning.LMeaningId))
            {
                if (affected.Add(exampleId))
                {
                    omissions.Add(new LMarkupOmission(0, $"sense of example {exampleId}"));
                }
            }
        }
    }
}
