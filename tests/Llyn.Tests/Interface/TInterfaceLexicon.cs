using Llyn.Core;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LAuthor TAuthorCreate(long id, string name) =>
        new(id, name);

    internal static LCardDraft TCardDraftCreate(
        LStateValue title,
        LStateValue expression,
        LStateValue meaning,
        IReadOnlyList<LSentenceDraft> sentence,
        IReadOnlyList<LSituationDraft> situation,
        IReadOnlyList<long> translation,
        IReadOnlyList<string> tag,
        IReadOnlyList<LImageDraft> image,
        int position,
        long id = 0,
        IReadOnlyList<LVideoDraft>? video = null,
        IReadOnlyList<LRegisterDraft>? register = null) =>
        new(
            title, expression, meaning, sentence, situation, register ?? [], translation,
            [.. tag.Select(LTagDraft.LTagDraftCreate)], image, video ?? [], position, id);

    internal static IReadOnlyList<string> TTagDraftRead(IReadOnlyList<LTagDraft> tags) =>
        [.. tags.Select(tag => tag.LTagDraftText)];

    internal static IReadOnlyList<LTagDraft> TTagDraftCreate(params string[] texts) =>
        [.. texts.Select(LTagDraft.LTagDraftCreate)];

    internal static LVideoDraft TVideoDraftCreate(string location, string span = "", long id = 0) =>
        new(LStateValue.LStateValueRead(location), LStateValue.LStateValueRead(span), id);

    internal static LImageDraft TImageDraftCreate(string location, long id = 0) =>
        new(LStateValue.LStateValueRead(location), id);

    internal static LCollocation TCollocationCreate(
        long id,
        long entryId,
        int position,
        LStateValue title,
        LStateValue expression,
        LStateValue meaning) =>
        new(id, entryId, position, title, expression, meaning);

    internal static LCourt TCourtLinkCreate(
        long id,
        long owner,
        long target,
        string headword,
        string language) =>
        new(id, owner, target, headword, language);

    internal static LDraft TDraftCreate(
        long id,
        string origin,
        long entry,
        LEntryDraft content,
        DateTimeOffset moment) =>
        new(id, origin, entry, content, moment);

    internal static LEntry TEntryCreate(
        long id,
        string headword,
        string language,
        string? proficiency,
        string? frequency,
        string? addedUtc,
        string? updatedUtc) =>
        new(id, headword, language, proficiency, frequency, addedUtc, updatedUtc);

    internal static LEntryDraft TEntryDraftCreate(
        string headword,
        string language,
        string pronunciation,
        string note,
        IReadOnlyList<LCardDraft> meanings,
        IReadOnlyList<LCardDraft> collocations,
        string audio = "",
        string? source = null,
        IReadOnlyList<string>? speeches = null,
        IReadOnlyList<LTranscriptionDraft>? transcriptions = null) =>
        new(
            headword,
            language,
            TPronunciationListCreate(LPronunciationDraft.LPronunciationDraftCreate(pronunciation, audio, source)),
            note,
            meanings,
            collocations,
            TSpeechDraftCreate(speeches),
            LEntryDraftTranscriptions: transcriptions);

    internal static IReadOnlyList<LPronunciationDraft> TPronunciationListCreate(LPronunciationDraft? pronunciation) =>
        pronunciation is null ? [] : [pronunciation];

    internal static LPronunciationDraft TPronunciationDraftCreate(
        string ipa,
        string variety = "",
        string audio = "",
        string? source = null,
        long id = 0) =>
        new(ipa, null, audio, source, id, variety);

    internal static LTranscriptionDraft TTranscriptionDraftCreate(string scheme, string text, long id = 0) =>
        new(scheme, text, id);

    internal static IReadOnlyList<LSpeechDraft> TSpeechDraftCreate(IReadOnlyList<string>? speeches)
    {
        if (speeches is null)
        {
            return [];
        }

        List<LSpeechDraft> drafts = new(speeches.Count);
        foreach (string speech in speeches)
        {
            drafts.Add(LSpeechDraft.LSpeechDraftCreate(speech));
        }

        return drafts;
    }

    internal static LSpeechDraft TSpeechDraftCreate(long valueId, string name) =>
        LSpeechDraft.LSpeechDraftCreate(valueId, name);

    internal static IReadOnlyList<string> TSpeechNameRead(LEntryDraft draft)
    {
        List<string> named = new(draft.LEntryDraftSpeeches.Count);
        foreach (LSpeechDraft speech in draft.LEntryDraftSpeeches)
        {
            named.Add(speech.LSpeechDraftName);
        }

        return named;
    }

    internal static LExample TExampleCreate(
        long id,
        string language,
        LStateValue text,
        LStateValue translation,
        LStateAnchor source) =>
        new(id, language, text, translation, source);

    internal static LMention TMentionCreate(long id, int start, int length, long entryId, long senseId = 0) =>
        new(id, start, length, entryId, senseId);

    internal static (int LMentionSpanOffset, int LMentionSpanLength) TMentionSpanResolve(string text, int offset, bool separated) =>
        LMentionSpan.LMentionSpanResolve(text, offset, separated);

    internal static IReadOnlyList<LMentionPiece> TMentionSpanDivide(string text, IReadOnlyList<LMention> mentions) =>
        LMentionSpan.LMentionSpanDivide(text, mentions);

    internal static int TMentionOffsetRead(string text, int unit) =>
        LMentionSpan.LMentionOffsetRead(text, unit);

    internal static int TMentionUnitRead(string text, int offset) =>
        LMentionSpan.LMentionUnitRead(text, offset);

    internal static LMentionDraft TMentionDraftCreate(long id, int start, int length, long entryId, long senseId = 0) =>
        new(id, start, length, entryId, senseId);

    internal static LMention TMentionDraftResolve(LMentionDraft draft) =>
        draft.LMentionDraftResolve();

    internal static LExampleDraft TExampleDraftCreate(
        LStateValue text,
        long id,
        LStateAnchor reference,
        LStateValue? translation = null,
        string language = "") =>
        new(text, id, reference, translation ?? LStateValue.LStateValueUnspecified, language);

    internal static LSentenceDraft TSentenceDraftCreate(
        LStateValue text,
        long id,
        LStateAnchor reference,
        LStateValue? particle = null,
        LStateValue? dependence = null,
        long row = 0) =>
        new(
            TExampleDraftCreate(text, id, reference),
            particle ?? LStateValue.LStateValueUnspecified,
            dependence ?? LStateValue.LStateValueUnspecified,
            row);

    internal static LSentenceDraft TSentenceDraftCreate(string text) =>
        LSentenceDraft.LSentenceDraftCreate(text);

    internal static LSentenceDraft TSentenceDraftCreate(
        LStateValue? particle, LStateValue? dependence) =>
        new(
            null,
            particle ?? LStateValue.LStateValueUnspecified,
            dependence ?? LStateValue.LStateValueUnspecified);

    internal static LSentence TSentenceCreate(
        long id,
        long ownerId,
        int position,
        LExample? example,
        LStateValue? particle,
        LStateValue? dependence) =>
        new(
            id,
            ownerId,
            position,
            example,
            particle ?? LStateValue.LStateValueUnspecified,
            dependence ?? LStateValue.LStateValueUnspecified);

    internal static LExampleDraft TExampleDraftCreate(string text) =>
        LExampleDraft.LExampleDraftCreate(text);

    internal static LForm TFormCreate(
        long entryId,
        int position,
        string text,
        string? local,
        string role) =>
        new(entryId, position, text, local, role);

    internal static LInflection TInflectionCreate(
        long entryId,
        int position,
        string text,
        string? local,
        long? speechValueId,
        IReadOnlyList<long> morphology) =>
        new(0, entryId, position, text, local, speechValueId, morphology);

    internal static LFeature TFeatureCreate(
        long speechValueId,
        long packId,
        string name,
        int position) =>
        new(0, speechValueId, packId, name, position);

    internal static LMorphology TMorphologyCreate(
        long featureId,
        long packId,
        string name,
        int position) =>
        new(0, featureId, packId, name, position);

    internal static LNote TNoteCreate(long entryId, string text) =>
        new(entryId, text);

    internal static LPronunciation TPronunciationCreate(
        long id,
        long entryId,
        string ipa,
        IReadOnlyList<LSyllable> syllables,
        string? variety = null,
        int position = 0) =>
        new(id, entryId, position, variety, ipa, syllables);

    internal static LTranscription TTranscriptionCreate(long id, long entryId, string scheme, string text) =>
        new(id, entryId, 0, scheme, text);

    internal static LReference TReferenceCreate(
        long id,
        LStateValue title,
        LStateValue year,
        LReferenceKind kind,
        LStateValue note,
        LStateValue url,
        LStateMark authorState) =>
        new(id, title, year, kind, note, url, authorState);

    internal static LMeaning TMeaningCreate(
        long id,
        long entryId,
        long? parentId,
        int position,
        LStateValue title,
        LStateValue definition) =>
        new(id, entryId, parentId, position, title, definition);

    internal static LRegister TRegisterCreate(long id, string name) =>
        new(id, LStateValue.LStateValueRead(name), string.Empty);

    internal static LRegisterDraft TRegisterDraftCreate(string text) =>
        LRegisterDraft.LRegisterDraftCreate(text);

    internal static LRegisterDraft TRegisterDraftCreate(
        LStateValue name, long id, string language = "") =>
        new(name, id, language);

    internal static LSituation TSituationCreate(
        long id,
        LStateValue title,
        LStateValue description,
        LStateValue kind) =>
        new(id, title, description, kind);

    internal static LSituationDraft TSituationDraftCreate(
        LStateValue text,
        long id,
        LStateValue? description = null,
        LStateValue? kind = null) =>
        new(
            text,
            id,
            description ?? LStateValue.LStateValueUnspecified,
            kind ?? LStateValue.LStateValueUnspecified);

    internal static LSituationDraft TSituationDraftCreate(string text) =>
        LSituationDraft.LSituationDraftCreate(text);

    internal static LSpeech TSpeechCreate(
        long entryId,
        int position,
        long? valueId,
        string? custom = null) =>
        new(entryId, position, valueId, custom);

    internal static LSpeechValue TSpeechValueCreate(string language, long packId, string name, int position) =>
        new(0, language, packId, name, position);

    internal static LStateValue TStateValueCreate(string text) =>
        LStateValue.LStateValueCreate(text);

    internal static LStateValue TStateValueResolve(string? text, bool unknown) =>
        new LStateWritten(text, unknown).LStateWrittenResolve();

    internal static LStateWritten TStateWrittenRead(this LStateValue value) =>
        LStateWritten.LStateWrittenRead(value);

    internal static string TStateValueShow(this LStateValue stateValue) =>
        stateValue.LStateValueShow();

    internal static long TStateAnchorShow(this LStateAnchor stateAnchor) =>
        stateAnchor.LStateAnchorShow();

    internal static LStateAnchor TStateAnchorRead(long? id) =>
        LStateAnchor.LStateAnchorRead(id);

    internal static LTag TTagCreate(string text) =>
        new(0, text);

    internal static LTag TTagCreate(long id, string text) =>
        new(id, text);

    internal static LTranslation TTranslationCreate(long entryId, int position) =>
        new(entryId, position);
}
