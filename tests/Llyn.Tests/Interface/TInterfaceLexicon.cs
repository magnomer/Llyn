using Llyn.Core;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LFellow TFellowCreate(long id, string name, int shared) =>
        new(id, name, shared);

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
        LStateValue? title,
        LStateValue? expression,
        LStateValue? meaning) =>
        new(
            id,
            entryId,
            position,
            title ?? LStateValue.LStateValueUnspecified,
            expression ?? LStateValue.LStateValueUnspecified,
            meaning ?? LStateValue.LStateValueUnspecified);

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
        int grasp,
        string? addedUtc,
        string? updatedUtc) =>
        new(id, headword, language, grasp, addedUtc, updatedUtc);

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
        IReadOnlyList<LTranscriptionDraft>? transcriptions = null,
        IReadOnlyList<LReflexDraft>? reflexes = null) =>
        new(
            headword,
            language,
            TPronunciationListCreate(TPronunciationDraftCreate(pronunciation, audio: audio, source: source)),
            note,
            meanings,
            collocations,
            TSpeechDraftCreate(speeches),
            LEntryDraftTranscriptions: transcriptions,
            LEntryDraftReflexes: reflexes);

    internal static IReadOnlyList<LPronunciationDraft> TPronunciationListCreate(LPronunciationDraft pronunciation) =>
        pronunciation.LPronunciationDraftEmpty ? [] : [pronunciation];

    internal static LPronunciationDraft TPronunciationDraftCreate(
        string ipa,
        string variety = "",
        string audio = "",
        string? source = null,
        long id = 0,
        string respelling = "") =>
        new(ipa, null, audio, source, id, variety, LPronunciationDraftRespelling: respelling);

    internal static LTranscriptionDraft TTranscriptionDraftCreate(
        string scheme, string text, long id = 0, bool seeded = false) =>
        new(scheme, text, id, seeded);

    internal static LReflexDraft TReflexDraftCreate(
        string language,
        string kind,
        string text,
        bool main = false,
        long id = 0,
        string romanization = "",
        string meaning = "",
        string note = "",
        string respelling = "",
        string region = "") =>
        new(language, kind, text, main, id, romanization, meaning, false, note, respelling, region);

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
        LStateValue? text,
        LStateValue? translation,
        LStateAnchor? source) =>
        new(
            id,
            language,
            text ?? LStateValue.LStateValueUnspecified,
            source ?? LStateAnchor.LStateAnchorUnspecified,
            translation is null || translation.LStateValueEmpty ? [] : [TGlossCreate(0, string.Empty, translation)]);

    internal static LGloss TGlossCreate(long id, string language, LStateValue text) =>
        new(id, language, text);

    internal static LMention TMentionCreate(long id, int start, int length, long entryId, long senseId = 0) =>
        new(id, start, length, entryId, senseId);

    internal static (int LMentionSpanOffset, int LMentionSpanLength) TMentionSpanResolve(
        string text, int offset, bool separated) =>
        LMentionSpan.LMentionSpanResolve(text, offset, separated);

    internal static IReadOnlyList<LMentionPiece> TMentionSpanDivide(string text, IReadOnlyList<LMention> mentions) =>
        LMentionSpan.LMentionSpanDivide(text, mentions);

    internal static int TMentionOffsetRead(string text, int unit) =>
        LMentionSpan.LMentionOffsetRead(text, unit);

    internal static LMentionDraft TMentionSpanRead(string text, int start, int length) =>
        LMentionSpan.LMentionSpanRead(text, start, length);

    internal static LMentionDraft? TEtymologyDraftFind(LEtymologyDraft etymology, LMentionDraft span) =>
        etymology.LEtymologyDraftFind(span);

    internal static LMentionDraft? TExampleDraftFind(LExampleDraft example, LMentionDraft span) =>
        example.LExampleDraftFind(span);

    internal static LExampleDraft? TDraftExampleRead(LDraft draft, long cardId, long sentenceId) =>
        draft.LDraftExampleRead(cardId, sentenceId);

    internal static int TMentionUnitRead(string text, int offset) =>
        LMentionSpan.LMentionUnitRead(text, offset);

    internal static LMentionLabel TMentionLabelCreate(long id, string word, long entryId, string name, string sense) =>
        new(id, word, entryId, name, sense);

    internal static LMentionDraft TMentionDraftCreate(long id, int start, int length, long entryId, long senseId = 0) =>
        new(id, start, length, entryId, senseId);

    internal static LMention TMentionDraftResolve(LMentionDraft draft) =>
        draft.LMentionDraftResolve();

    internal static LExampleDraft TExampleDraftCreate(
        LStateValue text,
        long id,
        LStateAnchor reference,
        string language = "") =>
        new(text, id, reference, language);

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
        new(
            LExampleDraft.LExampleDraftCreate(text),
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified);

    internal static LSentenceDraft TSentenceDraftCreate(
        LStateValue? particle, LStateValue? dependence) =>
        new(
            null,
            particle ?? LStateValue.LStateValueUnspecified,
            dependence ?? LStateValue.LStateValueUnspecified);

    internal static LSentence TSentenceCreate(
        long id,
        LExample? example,
        LStateValue? particle,
        LStateValue? dependence) =>
        new(
            id,
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

    internal static LParadigm TParadigmCreate(
        long speechCode,
        IReadOnlyList<long> morphology,
        IReadOnlyList<LParadigmRule>? regular = null,
        IReadOnlyList<long>? except = null) =>
        new(speechCode, morphology, regular, except);

    internal static LParadigmRule TParadigmRuleCreate(string pattern, string replacement) =>
        new(pattern, replacement);

    internal static string? TParadigmRuleResolve(LParadigmRule rule, string headword) =>
        rule.LParadigmRuleResolve(headword);

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
        LStateValue? title,
        LStateValue? year,
        LReferenceKind kind,
        LStateValue? note,
        LStateValue? url,
        LStateMark authorState) =>
        new(
            id,
            title ?? LStateValue.LStateValueUnspecified,
            year ?? LStateValue.LStateValueUnspecified,
            kind,
            note ?? LStateValue.LStateValueUnspecified,
            url ?? LStateValue.LStateValueUnspecified,
            authorState);

    internal static LMeaning TMeaningCreate(
        long id,
        long entryId,
        long? parentId,
        int position,
        LStateValue? title,
        LStateValue? definition) =>
        new(
            id,
            entryId,
            parentId,
            position,
            title ?? LStateValue.LStateValueUnspecified,
            definition ?? LStateValue.LStateValueUnspecified);

    internal static LRegister TRegisterCreate(long id, string name) =>
        new(id, LStateValue.LStateValueRead(name));

    internal static LRegisterDraft TRegisterDraftCreate(string text) =>
        new(LStateValue.LStateValueRead(text), 0);

    internal static LRegisterDraft TRegisterDraftCreate(LStateValue name, long id) =>
        new(name, id);

    internal static LSituation TSituationCreate(
        long id,
        LStateValue? title,
        LStateValue? description,
        LStateValue? kind) =>
        new(
            id,
            title ?? LStateValue.LStateValueUnspecified,
            description ?? LStateValue.LStateValueUnspecified,
            kind ?? LStateValue.LStateValueUnspecified);

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
        new(
            LStateValue.LStateValueRead(text),
            0,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified);

    internal static LSpeech TSpeechCreate(long? valueId, string? custom = null) =>
        new(valueId, custom);

    internal static LSpeechValue TSpeechValueCreate(string language, long packId, string name, int position) =>
        new(0, language, packId, name, position);

    internal static LStateValue TStateValueCreate(string text) =>
        LStateValue.LStateValueCreate(text);

    internal static LStateValue TStateValueResolve(string? text, bool unknown) =>
        new LStateWritten(text, unknown).LStateWrittenResolve();

    internal static LStateWritten TStateWrittenRead(this LStateValue value) =>
        value is null
            ? LStateWritten.LStateWrittenEmpty
            : new LStateWritten(value.LStateValueShow(), value.LStateValueState == LState.LStateUnknown);

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

    internal static LTranslation TTranslationCreate(long entryId) =>
        new(entryId);

    internal static bool TGraspCheck(int grasp) =>
        LGrasp.LGraspCheck(grasp);
}
