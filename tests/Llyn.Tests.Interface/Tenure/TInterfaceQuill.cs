using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LQuillAuthor TQuillAuthorCreate(this LTenure tenure) => new(tenure);

    internal static LQuillExample TQuillExampleCreate(this LTenure tenure) => new(tenure);

    internal static LQuillSentence TQuillSentenceCreate(this LTenure tenure) => new(tenure);

    internal static LQuillEtymology TQuillEtymologyCreate(this LTenure tenure) => new(tenure);

    internal static void TQuillAuthorSet(this LQuillAuthor quill, string name)
    {
        quill.LQuillAuthorSet(name);
    }

    internal static void TQuillExampleSet(this LQuillExample quill, string text)
    {
        quill.LQuillExampleSet(text);
    }

    internal static void TQuillSpeakerSet(this LQuillExample quill, string language)
    {
        quill.LExampleSpeakerSet(language);
    }

    internal static void TQuillReferenceSet(this LQuillExample quill, long reference)
    {
        quill.LExampleReferenceSet(reference);
    }


    internal static void TQuillGlossRemove(this LQuillSentence quill, long gloss)
    {
        quill.LSentenceGlossRemove(0, 0, gloss);
    }

    internal static void TQuillGlossSet(this LQuillSentence quill, long gloss, string? language, string? text)
    {
        quill.LSentenceGlossSet(0, 0, gloss, language, text);
    }

    internal static void TQuillEtymologySet(this LQuillEtymology quill, string text)
    {
        quill.LQuillEtymologySet(text);
    }

    internal static void TQuillEtymonAdd(this LQuillEtymology quill, long entry, int position)
    {
        quill.LEtymonAdd(entry, position);
    }

    internal static void TQuillEtymonRemove(this LQuillEtymology quill, long entry)
    {
        quill.LEtymonRemove(entry);
    }

    internal static void TQuillMentionSave(this LQuillEtymology quill, int offset, int length, long entry)
    {
        quill.LEtymologyMentionSave(offset, length, entry);
    }

    internal static void TQuillCitationSet(this LQuillSentence quill, long card, long sentence, long reference)
    {
        quill.LSentenceCitationSet(card, sentence, reference);
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
