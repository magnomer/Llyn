using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIVeneer;

public partial class PEditor
{
    private IReadOnlyDictionary<long, LTranslationTarget> PEditorTargetRead(LEntryDraft draft)
    {
        Dictionary<long, LTranslationTarget> targets = [];
        try
        {
            foreach (LTranslationTarget target in _lEngine.LEngineTargetRead(PEditorDraft))
            {
                targets[target.LTranslationTargetId] = target;
            }
        }
        catch (Exception)
        {
            targets.Clear();
        }

        return targets;
    }
}
