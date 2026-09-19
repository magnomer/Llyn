using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private void LEngineMarkupDetach(long entryId, List<LMarkupOmission> omissions)
    {
        LMentionVault mentions = _lEngineMentions;
        HashSet<long> affected = [];
        foreach (LMeaning meaning in _lEngineMeanings.LMeaningRead(entryId))
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
