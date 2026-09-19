using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

internal sealed record PRespelling(bool PRespellingShown, bool PRespellingSlashed)
{
    internal static readonly PRespelling PRespellingPlain = new(false, false);

    internal string PRespellingOpener => PRespellingSlashed ? "/" : "[";

    internal string PRespellingCloser => PRespellingSlashed ? "/" : "]";

    internal static PRespelling PRespellingRead(LEngine engine, string language)
    {
        bool shown = engine.LEngineRespellingCheck(language);
        return new PRespelling(shown, shown && engine.LEnginePhonemicCheck(language));
    }

    internal string PRespellingTextRead(LPronunciationDraft spoken)
    {
        return PRespellingTextRead(spoken.LPronunciationDraftIpa, spoken.LPronunciationDraftRespelling);
    }

    internal string PRespellingTextRead(string phonetic, string? respelling)
    {
        return PRespellingShown && !string.IsNullOrEmpty(respelling) ? respelling : phonetic;
    }
}
