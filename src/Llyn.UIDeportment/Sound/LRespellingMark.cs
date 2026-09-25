using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed record LRespellingMark(bool LRespellingMarkShown, bool LRespellingMarkSlashed)
{
    public static readonly LRespellingMark LRespellingMarkPlain = new(false, false);

    public string LRespellingMarkOpener => LRespellingMarkSlashed ? "/" : "[";

    public string LRespellingMarkCloser => LRespellingMarkSlashed ? "/" : "]";

    public static LRespellingMark LRespellingMarkRead(LWindow window, string language)
    {
        bool shown = window.LWindowRespellingCheck(language);
        return new LRespellingMark(shown, shown && window.LWindowPhonemicCheck(language));
    }

    public string LRespellingMarkResolve(LPronunciationDraft spoken)
    {
        return LRespellingMarkResolve(spoken.LPronunciationDraftIpa, spoken.LPronunciationDraftRespelling);
    }

    public string LRespellingMarkResolve(string phonetic, string? respelling)
    {
        return LRespellingMarkShown && !string.IsNullOrEmpty(respelling) ? respelling : phonetic;
    }
}
