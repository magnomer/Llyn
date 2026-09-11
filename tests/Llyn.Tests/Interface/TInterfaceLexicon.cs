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
        string synonym,
        IReadOnlyList<string> tag,
        IReadOnlyList<LImageDraft> image,
        int position,
        long id = 0,
        IReadOnlyList<LVideoDraft>? video = null,
        IReadOnlyList<LRegisterDraft>? register = null) =>
        new(
            title, expression, meaning, sentence, situation, register ?? [], translation, synonym, tag,
            image, video ?? [], position, id);

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

    internal static LCourtLink TCourtLinkCreate(
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
        IReadOnlyList<string>? speeches = null) =>
        new(
            headword,
            language,
            LPronunciationDraft.LPronunciationDraftCreate(pronunciation, audio, source),
            note,
            meanings,
            collocations,
            TSpeechDraftCreate(speeches));

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

    internal static LFeature TFeatureCreate(string id, string valueId) =>
        new(id, valueId);

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
        string? speechId,
        IReadOnlyList<LFeature> features) =>
        new(entryId, position, text, local, speechId, features);

    internal static LMorphology TMorphologyCreate(
        string language,
        string speechId,
        string featureId,
        string featureName,
        string valueId,
        string valueName,
        int position) =>
        new(language, speechId, featureId, featureName, valueId, valueName, position);

    internal static LNote TNoteCreate(long entryId, string text) =>
        new(entryId, text);

    internal static LPronunciation TPronunciationCreate(
        long id,
        long entryId,
        string? level,
        string? ipa,
        IReadOnlyList<LSyllable> syllables,
        IReadOnlyList<LRepresentation> representations) =>
        new(id, entryId, level, ipa, syllables, representations);

    internal static LReference TReferenceCreate(
        long id,
        LStateValue title,
        LStateValue year,
        LReferenceKind kind,
        LStateValue note,
        LStateValue url,
        LState authorState) =>
        new(id, title, year, kind, note, url, authorState);

    internal static LRelation TRelationCreate(
        long id,
        long meaningId,
        int position,
        string type,
        string? label,
        string? labels,
        long? targetEntry,
        long? targetMeaning) =>
        new(id, meaningId, position, type, label, labels, targetEntry, targetMeaning);

    internal static LMeaning TMeaningCreate(
        long id,
        long entryId,
        long? parentId,
        int position,
        LStateValue title,
        string? gloss,
        string? definitionLanguage,
        LStateValue definition,
        string labels) =>
        new(id, entryId, parentId, position, title, gloss, definitionLanguage, definition, labels);

    internal static LRegister TRegisterCreate(long id, string name) =>
        new(id, LStateValue.LStateValueRead(name), string.Empty, false);

    internal static LRegisterDraft TRegisterDraftCreate(string text) =>
        LRegisterDraft.LRegisterDraftCreate(text);

    internal static LRegisterDraft TRegisterDraftCreate(
        LStateValue name, long id, string language = "", bool builtin = false) =>
        new(name, id, language, builtin);

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
        string? valueId,
        string? custom = null) =>
        new(entryId, position, valueId, custom);

    internal static LSpeechValue TSpeechValueCreate(string language, string id, string name, int position) =>
        new(language, id, name, position);

    internal static LStateValue TStateValueCreate(string text) =>
        LStateValue.LStateValueCreate(text);

    internal static LStateValue TStateValueResolve(string? text, bool unreadable) =>
        LStateValue.LStateValueResolve(text, unreadable);

    internal static string TStateValueShow(this LStateValue stateValue) =>
        stateValue.LStateValueShow();

    internal static long TStateAnchorShow(this LStateAnchor stateAnchor) =>
        stateAnchor.LStateAnchorShow();

    internal static LStateAnchor TStateAnchorRead(long? id) =>
        LStateAnchor.LStateAnchorRead(id);

    internal static LSynonym TSynonymCreate(
        long id,
        long collocationId,
        int position,
        long? targetEntry,
        long? targetMeaning) =>
        new(id, collocationId, position, targetEntry, targetMeaning);

    internal static LTag TTagCreate(string text) =>
        new(text);

    internal static LTranslation TTranslationCreate(long entryId, int position) =>
        new(entryId, position);
}
