using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LQuill TQuillCreate(this LTenure tenure) => new(tenure);

    internal static void TQuillAuthorSet(this LQuill quill, string name)
    {
        quill.LQuillAuthorSet(name);
    }

    internal static void TQuillExampleSet(this LQuill quill, string text)
    {
        quill.LQuillExampleSet(text);
    }

    internal static void TQuillSpeakerSet(this LQuill quill, string language)
    {
        quill.LQuillSpeakerSet(language);
    }

    internal static void TQuillReferenceSet(this LQuill quill, long reference)
    {
        quill.LQuillReferenceSet(reference);
    }

    internal static void TQuillGlossAdd(this LQuill quill, string language, int position)
    {
        quill.LQuillGlossAdd(0, 0, language, position);
    }

    internal static void TQuillGlossRemove(this LQuill quill, long gloss)
    {
        quill.LQuillGlossRemove(0, 0, gloss);
    }

    internal static void TQuillGlossSet(this LQuill quill, long gloss, string? language, string? text)
    {
        quill.LQuillGlossSet(0, 0, gloss, language, text);
    }

    internal static void TQuillMentionAdd(this LQuill quill, int offset, int length, long entry)
    {
        quill.LQuillMentionAdd(0, 0, offset, length, entry);
    }

    internal static void TQuillMentionRemove(this LQuill quill, long mention)
    {
        quill.LQuillMentionRemove(0, 0, mention);
    }

    internal static void TQuillMentionSet(this LQuill quill, long mention, long sense)
    {
        quill.LQuillMentionSet(0, 0, mention, sense);
    }

    internal static void TQuillSituationSet(this LQuill quill, string title, string description, string kind)
    {
        quill.LQuillSituationSet(title, description, kind);
    }
}
