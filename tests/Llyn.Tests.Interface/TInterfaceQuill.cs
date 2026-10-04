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


    internal static void TQuillGlossRemove(this LQuill quill, long gloss)
    {
        quill.LQuillGlossRemove(0, 0, gloss);
    }

    internal static void TQuillGlossSet(this LQuill quill, long gloss, string? language, string? text)
    {
        quill.LQuillGlossSet(0, 0, gloss, language, text);
    }

    internal static void TQuillEtymologySet(this LQuill quill, string text)
    {
        quill.LQuillEtymologySet(text);
    }

    internal static void TQuillEtymonAdd(this LQuill quill, long entry, int position)
    {
        quill.LQuillEtymonAdd(entry, position);
    }

    internal static void TQuillEtymonRemove(this LQuill quill, long entry)
    {
        quill.LQuillEtymonRemove(entry);
    }

    internal static void TQuillMentionSave(this LQuill quill, int offset, int length, long entry)
    {
        quill.LQuillMentionSave(offset, length, entry);
    }

    internal static void TQuillCitationSet(this LQuill quill, long card, long sentence, long reference)
    {
        quill.LQuillCitationSet(card, sentence, reference);
    }

    internal static LQuillChip TQuillChipCreate(this LTenure tenure, LDraftPort drafts) => new(tenure, drafts);

    internal static void TQuillTranslationRemove(this LQuillChip quill, long card, long entry)
    {
        quill.LQuillTranslationRemove(card, entry);
    }

    internal static LQuillSituation TQuillSituationCreate(this LTenure tenure) => new(tenure);

    internal static void TQuillTitleSet(this LQuillSituation quill, string text)
    {
        quill.LQuillTitleSet(text);
    }

    internal static void TQuillKindSet(this LQuillSituation quill, string text)
    {
        quill.LQuillKindSet(text);
    }

    internal static void TQuillDescriptionSet(this LQuillSituation quill, string text)
    {
        quill.LQuillDescriptionSet(text);
    }
}
