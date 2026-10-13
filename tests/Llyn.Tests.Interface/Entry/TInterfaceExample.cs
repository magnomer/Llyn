using Llyn.Core;

namespace Llyn.Tests;

internal static class TInterfaceExample
{
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
            TExampleDraftCreate(text),
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
        new(LStateValue.LStateValueRead(text), 0, LStateAnchor.LStateAnchorUnspecified);
}
