using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public IReadOnlyList<LMarkdownBlock> LEngineMarkdownParse(string? text)
    {
        return LMarkdown.LMarkdownParse(text);
    }

    private static IReadOnlyList<LTranscriptionDraft> LEngineTranscriptionReset(
        IReadOnlyList<LTranscriptionDraft> drafts)
    {
        List<LTranscriptionDraft> renewed = new(drafts.Count);
        foreach (LTranscriptionDraft draft in drafts)
        {
            renewed.Add(draft.LTranscriptionDraftId > 0 ? draft with { LTranscriptionDraftId = 0 } : draft);
        }

        return renewed;
    }
}
