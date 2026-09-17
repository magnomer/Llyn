using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TExemplar
{
    private const string TExemplarLanguage = "English";

    private const string TExemplarSentence = "An exemplar sets the pattern for others.";

    private static readonly Regex TExemplarKeyPattern = new(
        "\"(\\w+Id|LMentionDraftEntry|LMentionDraftSense|LSpeechDraftValue)\":\\s*-?\\d+",
        RegexOptions.Compiled);

    private static readonly Regex TExemplarListPattern = new(
        "\"(LCardDraftTranslation|LInflectionMorphology|LReflexDraftAnchors)\":\\s*\\[[^\\]]*\\]",
        RegexOptions.Compiled);

    private static readonly JsonSerializerOptions TExemplarJsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    internal static readonly string[] TExemplarHidden =
    [
        "eg-ZEM-plar",
        "ig-ZEM-pluh",
        "exemplar-reflex-respelling",
        "https://example.org/exemplar-audio.ogg",
        "exemplar-source",
        "ks",
        "jw",
        "ɛː",
        "ŋk",
        "exemplar-points",
        "exemplar-situation-description",
        "exemplar-situation-kind",
        "https://example.org/exemplar-image.png",
    ];

    internal static LEntryDraft TExemplarCreate(LEngine engine)
    {
        LEntry translation = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "Vorbild", "German", string.Empty, string.Empty, [TInterface.TCardCreate("exemplar", 1)], []));
        LEntry mention = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "pattern", TExemplarLanguage, string.Empty, string.Empty, [TInterface.TCardCreate("a model", 1)], []));
        long sense = engine.TEngineEntryLoad(mention.LEntryId)!.LEntryDraftMeanings[0].LCardDraftId;

        LReference reference = engine.TEngineReferenceCreate(TInterface.TReferenceCreate(
            0,
            TInterface.TStateValueCreate("Exemplar Sourcebook"),
            TInterface.TStateValueCreate("1999"),
            LReferenceKind.LReferenceKindBook,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateMark.LStateMarkUnspecified));
        LAuthor author = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Exemplar Author"));
        engine.TEngineAuthorAttach(reference.LReferenceId, author.LAuthorId, 0);

        LSpeechValue noun = engine.TEngineSpeechFind(TExemplarLanguage, "Noun")!;
        LSpeechValue verb = engine.TEngineSpeechFind(TExemplarLanguage, "Verb")!;
        LMorphology plural = engine.TEngineMorphologyFind(
            engine.TEngineFeatureFind(noun.LSpeechValueId, "number")!.LFeatureId, "plural")!;
        LMorphology past = engine.TEngineMorphologyFind(
            engine.TEngineFeatureFind(verb.LSpeechValueId, "form")!.LFeatureId, "past")!;

        return new LEntryDraft(
            "exemplar",
            TExemplarLanguage,
            [
                new LPronunciationDraft(
                    "ɛɡˈzɛmplɑr",
                    [
                        new LSyllable(
                            LSyllablePronunciationId: 0,
                            LSyllablePosition: 0,
                            LSyllableOnset: "ks",
                            LSyllableMedial: "jw",
                            LSyllableNucleus: "ɛː",
                            LSyllableCoda: "ŋk",
                            LSyllableToneNumber: 3,
                            LSyllableTonePoints: "exemplar-points"),
                    ],
                    "https://example.org/exemplar-audio.ogg",
                    "exemplar-source",
                    0,
                    "American",
                    LPronunciationDraftRespelling: "eg-ZEM-plar"),
                new LPronunciationDraft(
                    "ɪɡˈzɛmplə",
                    [],
                    string.Empty,
                    null,
                    0,
                    "British",
                    LPronunciationDraftRespelling: "ig-ZEM-pluh"),
            ],
            "exemplar-note",
            [
                new LCardDraft(
                    "exemplar-title",
                    LStateValue.LStateValueUnspecified,
                    "exemplar-meaning",
                    [
                        new LSentenceDraft(
                            new LExampleDraft(
                                TExemplarSentence,
                                0,
                                LExampleDraftReference: LStateAnchor.LStateAnchorCreate(reference.LReferenceId),
                                LExampleDraftLanguage: "Old English",
                                LExampleDraftGloss: [new LGlossDraft(0, "Korean", "exemplar-gloss")],
                                LExampleDraftMention:
                                [
                                    new LMentionDraft(
                                        0,
                                        LMentionDraftOffset: TExemplarSentence.IndexOf(
                                            "pattern", System.StringComparison.Ordinal),
                                        LMentionDraftLength: "pattern".Length,
                                        LMentionDraftEntry: mention.LEntryId,
                                        LMentionDraftSense: sense),
                                ]),
                            LStateValue.LStateValueUnspecified,
                            LStateValue.LStateValueUnspecified),
                        new LSentenceDraft(
                            LExampleDraft.LExampleDraftCreate("Every exemplar earns its name."),
                            "with",
                            "of"),
                    ],
                    [
                        new LSituationDraft(
                            "exemplar-situation-title",
                            0,
                            "exemplar-situation-description",
                            "exemplar-situation-kind"),
                    ],
                    [LRegisterDraft.LRegisterDraftCreate("Formal")],
                    LCardDraftTranslation: [translation.LEntryId],
                    [LTagDraft.LTagDraftCreate("exemplar-tag")],
                    [new LImageDraft("https://example.org/exemplar-image.png")],
                    [new LVideoDraft("https://youtu.be/exemplarvideo", "0:01-0:02")],
                    1,
                    LCardDraftChild:
                    [
                        new LCardDraft(
                            "exemplar-child-title",
                            LStateValue.LStateValueUnspecified,
                            "exemplar-child-meaning",
                            [], [], [], [], [], [], [], 1),
                    ]),
                new LCardDraft(
                    "exemplar-second-title",
                    LStateValue.LStateValueUnspecified,
                    "exemplar-second-meaning",
                    [], [], [], [], [], [], [], 2),
            ],
            [
                new LCardDraft(
                    LStateValue.LStateValueUnspecified,
                    "exemplar-collocation-phrase",
                    "exemplar-collocation-meaning",
                    [], [], [], [], [], [], [], 1),
            ],
            [
                new LSpeechDraft(LSpeechDraftValue: noun.LSpeechValueId, null, noun.LSpeechValueName),
                new LSpeechDraft(LSpeechDraftValue: verb.LSpeechValueId, null, verb.LSpeechValueName),
                new LSpeechDraft(0, LSpeechDraftCustom: "exemplar-speech"),
            ],
            [
                new LForm(0, 0, "exemplar-form-text", "exemplar-form-local", "exemplar-form-role"),
                new LForm(0, 1, "exemplar-form-second", null, "exemplar-form-second-role"),
            ],
            [
                new LInflection(
                    0,
                    0,
                    0,
                    "exemplaria",
                    "exemplar-inflection-local",
                    LInflectionSpeechId: noun.LSpeechValueId,
                    LInflectionMorphology: [plural.LMorphologyId]),
                new LInflection(
                    0,
                    0,
                    1,
                    "exemplarised",
                    null,
                    LInflectionSpeechId: verb.LSpeechValueId,
                    LInflectionMorphology: [past.LMorphologyId]),
            ],
            [new LTranscriptionDraft("exemplar-scheme", "exemplar-transcription")],
            [
                new LReflexDraft(
                    "Korean",
                    "exemplar-kind",
                    "exemplar-reflex-reading",
                    LReflexDraftMain: true,
                    0,
                    "exemplar-reflex-note",
                    "exemplar-reflex-respelling",
                    "exemplar-region",
                    "exemplar-remark"),
            ]);
    }

    internal static IReadOnlyList<long> TExemplarSave(LEngine engine, LEntryDraft draft)
    {
        List<long> ids = [engine.TEngineEntrySave(draft).LEntryId];
        LPortraitLink.LPortraitLinkRead(draft.LEntryDraftMeanings, ids);
        LPortraitLink.LPortraitLinkRead(draft.LEntryDraftCollocations, ids);
        return ids;
    }

    internal static IReadOnlyList<string> TExemplarTextRead(LEntryDraft draft)
    {
        List<string> texts = [draft.LEntryDraftHeadword, draft.LEntryDraftLanguage, draft.LEntryDraftNote];

        foreach (LSpeechDraft speech in draft.LEntryDraftSpeeches)
        {
            texts.Add(speech.LSpeechDraftName);
        }

        foreach (LForm form in draft.LEntryDraftForms)
        {
            TExemplarTextAdd(texts, form.LFormText, form.LFormLocal, form.LFormRole);
        }

        foreach (LInflection inflection in draft.LEntryDraftInflections)
        {
            TExemplarTextAdd(texts, inflection.LInflectionText, inflection.LInflectionLocal);
        }

        foreach (LPronunciationDraft pronunciation in draft.LEntryDraftPronunciations)
        {
            TExemplarTextAdd(
                texts,
                pronunciation.LPronunciationDraftIpa,
                pronunciation.LPronunciationDraftRespelling,
                pronunciation.LPronunciationDraftVariety,
                pronunciation.LPronunciationDraftAudio,
                pronunciation.LPronunciationDraftSource);
            foreach (LSyllable syllable in pronunciation.LPronunciationDraftSyllables)
            {
                TExemplarTextAdd(
                    texts,
                    syllable.LSyllableOnset,
                    syllable.LSyllableMedial,
                    syllable.LSyllableNucleus,
                    syllable.LSyllableCoda,
                    syllable.LSyllableTonePoints);
            }
        }

        foreach (LTranscriptionDraft transcription in draft.LEntryDraftTranscriptions)
        {
            TExemplarTextAdd(texts, transcription.LTranscriptionDraftScheme, transcription.LTranscriptionDraftText);
        }

        foreach (LReflexDraft reflex in draft.LEntryDraftReflexes)
        {
            TExemplarTextAdd(
                texts,
                reflex.LReflexDraftLanguage,
                reflex.LReflexDraftKind,
                reflex.LReflexDraftText,
                reflex.LReflexDraftRespelling,
                reflex.LReflexDraftNote,
                reflex.LReflexDraftRegion,
                reflex.LReflexDraftRemark);
        }

        TExemplarTextAdd(texts, draft.LEntryDraftMeanings);
        TExemplarTextAdd(texts, draft.LEntryDraftCollocations);
        return texts;
    }

    internal static string TExemplarShapeRead(LEntryDraft draft)
    {
        string shape = JsonSerializer.Serialize(draft, TExemplarJsonOptions);
        shape = TExemplarKeyPattern.Replace(shape, match => "\"" + match.Groups[1].Value + "\":0");
        return TExemplarListPattern.Replace(
            shape,
            match => Regex.Replace(match.Value, "(?<=[\\[,\\s])\\d+", "0"));
    }

    private static void TExemplarTextAdd(List<string> texts, IReadOnlyList<LCardDraft> cards)
    {
        foreach (LCardDraft card in cards)
        {
            TExemplarTextAdd(texts, card.LCardDraftTitle, card.LCardDraftExpression, card.LCardDraftMeaning);

            foreach (LSentenceDraft sentence in card.LCardDraftSentence)
            {
                TExemplarTextAdd(texts, sentence.LSentenceDraftParticle, sentence.LSentenceDraftDependence);
                if (sentence.LSentenceDraftExample is not LExampleDraft example)
                {
                    continue;
                }

                TExemplarTextAdd(texts, example.LExampleDraftText);
                TExemplarTextAdd(texts, example.LExampleDraftLanguage);
                foreach (LGlossDraft gloss in example.LExampleDraftGloss)
                {
                    TExemplarTextAdd(texts, gloss.LGlossDraftLanguage);
                    TExemplarTextAdd(texts, gloss.LGlossDraftText);
                }
            }

            foreach (LSituationDraft situation in card.LCardDraftSituation)
            {
                TExemplarTextAdd(
                    texts,
                    situation.LSituationDraftTitle,
                    situation.LSituationDraftDescription,
                    situation.LSituationDraftKind);
            }

            foreach (LRegisterDraft register in card.LCardDraftRegister)
            {
                TExemplarTextAdd(texts, register.LRegisterDraftName);
            }

            foreach (LTagDraft tag in card.LCardDraftTag)
            {
                TExemplarTextAdd(texts, tag.LTagDraftText);
            }

            foreach (LImageDraft image in card.LCardDraftImage)
            {
                TExemplarTextAdd(texts, image.LImageDraftLocation);
            }

            foreach (LVideoDraft video in card.LCardDraftVideo)
            {
                TExemplarTextAdd(texts, video.LVideoDraftLocation, video.LVideoDraftSpan);
            }

            TExemplarTextAdd(texts, card.LCardDraftChild);
        }
    }

    private static void TExemplarTextAdd(List<string> texts, params string?[] values)
    {
        foreach (string? value in values)
        {
            if (!string.IsNullOrEmpty(value))
            {
                texts.Add(value);
            }
        }
    }

    private static void TExemplarTextAdd(List<string> texts, params LStateValue[] values)
    {
        foreach (LStateValue value in values)
        {
            if (value.LStateValueState == LState.LStateSpecified)
            {
                texts.Add(value.LStateValueShow());
            }
        }
    }
}
