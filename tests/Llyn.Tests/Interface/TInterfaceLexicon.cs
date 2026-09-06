using Llyn.Core;

using LMarkupEntry = Llyn.Core.LMarkup.LMarkupEntry;
using LMarkupReference = Llyn.Core.LMarkup.LMarkupReference;
using LMarkupToken = Llyn.Core.LMarkup.LMarkupToken;

namespace Llyn.Tests;

internal static partial class TInterface
{
    internal static LAuthor TAuthorCreate(string id, string name) =>
        new(id, name);

    internal static LCardDraft TCardDraftCreate(
        LStateValue title,
        LStateValue expression,
        LStateValue meaning,
        IReadOnlyList<LExampleDraft> example,
        IReadOnlyList<LSituationDraft> situation,
        IReadOnlyList<string> translation,
        string synonym,
        IReadOnlyList<string> tag,
        IReadOnlyList<LStateValue> image,
        int position,
        string id = "",
        IReadOnlyList<LStateValue>? video = null) =>
        new(
            title, expression, meaning, example, situation, translation, synonym, tag, image,
            video ?? [], position, id);

    internal static LCollocation TCollocationCreate(
        string id,
        string entryId,
        int position,
        LStateValue title,
        LStateValue expression,
        LStateValue meaning) =>
        new(id, entryId, position, title, expression, meaning);

    internal static LCourtLink TCourtLinkCreate(
        string id,
        string owner,
        string target,
        string headword,
        string language) =>
        new(id, owner, target, headword, language);

    internal static LDraft TDraftCreate(
        string id,
        string origin,
        string entry,
        LEntryDraft content,
        DateTimeOffset moment) =>
        new(id, origin, entry, content, moment);

    internal static LEntry TEntryCreate(
        string id,
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
        new(headword, language, pronunciation, note, meanings, collocations, audio, source, speeches);

    internal static LExample TExampleCreate(
        string id,
        string language,
        LStateValue text,
        LStateValue translation,
        LStateValue source) =>
        new(id, language, text, translation, source);

    internal static LExampleDraft TExampleDraftCreate(
        LStateValue text,
        string id,
        LStateValue reference,
        LStateValue? particle = null,
        LStateValue? dependence = null,
        LExampleDraft? revision = null) =>
        new(
            text,
            id,
            reference,
            particle ?? LStateValue.LStateValueUnspecified,
            dependence ?? LStateValue.LStateValueUnspecified,
            revision);

    internal static LSentence TSentenceCreate(
        string id,
        string meaningId,
        int position,
        LExample example,
        LExample? revision,
        LStateValue particle,
        LStateValue dependence) =>
        new(id, meaningId, position, example, revision, particle, dependence);

    internal static LExampleDraft TExampleDraftCreate(string text) =>
        LExampleDraft.LExampleDraftCreate(text);

    internal static LFeature TFeatureCreate(string id, string valueId) =>
        new(id, valueId);

    internal static LForm TFormCreate(
        string entryId,
        int position,
        string text,
        string? local,
        string role) =>
        new(entryId, position, text, local, role);

    internal static LInflection TInflectionCreate(
        string entryId,
        int position,
        string text,
        string? local,
        string? speechId,
        IReadOnlyList<LFeature> features) =>
        new(entryId, position, text, local, speechId, features);

    internal static LCardDraft TMarkupCardRead(IReadOnlyList<LMarkupToken> tokens, int position) =>
        LMarkup.LMarkupCardRead(tokens, position);

    internal static IReadOnlyList<LMarkupEntry> TMarkupEntryRead(string text) =>
        LMarkup.LMarkupEntryRead(text);

    internal static IReadOnlyList<LEntryDraft> TMarkupRead(string text) =>
        LMarkup.LMarkupRead(text);

    internal static LMarkupReference TMarkupReferenceRead(IReadOnlyList<LMarkupToken> tokens) =>
        LMarkup.LMarkupReferenceRead(tokens);

    internal static IReadOnlyList<LMarkupToken> TMarkupScan(string text) =>
        LMarkup.LMarkupScan(text);

    internal static LMorphology TMorphologyCreate(
        string language,
        string speechId,
        string featureId,
        string featureName,
        string valueId,
        string valueName,
        int position) =>
        new(language, speechId, featureId, featureName, valueId, valueName, position);

    internal static LNote TNoteCreate(string entryId, string text) =>
        new(entryId, text);

    internal static LPronunciation TPronunciationCreate(
        string id,
        string entryId,
        string? level,
        string? ipa,
        IReadOnlyList<LSyllable> syllables,
        IReadOnlyList<LRepresentation> representations) =>
        new(id, entryId, level, ipa, syllables, representations);

    internal static LReference TReferenceCreate(
        string id,
        LStateValue title,
        LStateValue program,
        LStateValue channel,
        LStateValue year,
        LStateValue url,
        LState authorState) =>
        new(id, title, program, channel, year, url, authorState);

    internal static LRelation TRelationCreate(
        string id,
        string meaningId,
        int position,
        string type,
        string? label,
        string? labels,
        string? targetEntry,
        string? targetMeaning) =>
        new(id, meaningId, position, type, label, labels, targetEntry, targetMeaning);

    internal static LMeaning TMeaningCreate(
        string id,
        string entryId,
        string? parentId,
        int position,
        LStateValue title,
        string? gloss,
        string? definitionLanguage,
        LStateValue definition,
        string labels) =>
        new(id, entryId, parentId, position, title, gloss, definitionLanguage, definition, labels);

    internal static LSituation TSituationCreate(
        string id,
        LStateValue title,
        LStateValue description,
        LStateValue kind) =>
        new(id, title, description, kind);

    internal static LSituationDraft TSituationDraftCreate(LStateValue text, string id) =>
        new(text, id);

    internal static LSituationDraft TSituationDraftCreate(string text) =>
        LSituationDraft.LSituationDraftCreate(text);

    internal static LSpeech TSpeechCreate(
        string entryId,
        int position,
        string? valueId,
        string? custom = null) =>
        new(entryId, position, valueId, custom);

    internal static LSpeechValue TSpeechValueCreate(string language, string id, string name, int position) =>
        new(language, id, name, position);

    internal static LStateValue TStateValueCreate(string text) =>
        LStateValue.LStateValueCreate(text);

    internal static string TStateValueShow(this LStateValue stateValue) =>
        stateValue.LStateValueShow();

    internal static LSynonym TSynonymCreate(
        string id,
        string collocationId,
        int position,
        string? targetEntry,
        string? targetMeaning) =>
        new(id, collocationId, position, targetEntry, targetMeaning);

    internal static LTag TTagCreate(string text) =>
        new(text);

    internal static LTranslation TTranslationCreate(string entryId, int position) =>
        new(entryId, position);
}
