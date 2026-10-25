using System.Collections.Generic;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceConductCard
{
    internal static CCard TCardCreate(LEngine engine, CDesk desk, CEnvoy envoy) => new(
        desk, engine.LEngineReference, new LSettingsOutlet(engine), envoy, new CLedgerNoticed());

    internal static CCardList TCardListCreate(CDesk desk) => new(
        desk,
        TEngineFake.TEngineStubCreate<LCardPort>(),
        TInterfaceConduct.TSettingsCreate(),
        TEnvoyFake.TEnvoyCreate(false, []));

    internal static CSentence TSentenceCreate(LEngine engine, CDesk desk) => new(
        desk,
        engine.LEngineVocabulary,
        new LDraftOutlet(engine),
        TInterfaceConduct.TSettingsCreate(),
        TEnvoyFake.TEnvoyCreate(false, []),
        new CLedgerNoticed());

    internal static CStateValue TCardStateRead(LStateValue value) => CFolio.CFolioStateRead(value);

    internal static IReadOnlyList<CImageDraft> TCardImageRead(IReadOnlyList<LImageDraft> images, LMediaPort media) =>
        CFolio.CFolioImageRead(images, media);

    internal static IReadOnlyList<CVideoDraft> TCardVideoRead(IReadOnlyList<LVideoDraft> videos, LMediaPort media) =>
        CFolio.CFolioVideoRead(videos, media);

    internal static CEntryDraft TCardEntryRead(LEntryDraft draft) =>
        TCardEntryRead(
            draft,
            draft.LEntryDraftMeanings.Select(static card => (LCardKind.LCardKindMeaning, card.LCardDraftId))
                .Concat(draft.LEntryDraftCollocations.Select(
                    static card => (LCardKind.LCardKindCollocation, card.LCardDraftId)))
                .ToDictionary(static key => key, static IReadOnlyList<LTranslationTarget> (_) => []));

    internal static CEntryDraft TCardEntryRead(
        LEntryDraft draft, IReadOnlyDictionary<(LCardKind, long), IReadOnlyList<LTranslationTarget>> targets) =>
        CFolio.CFolioEntryRead(
            draft,
            targets,
            new HashSet<(LCardKind, long)>(),
            TInterfaceConduct.TMediaCreate(),
            [],
            TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, System.Func<object?[]?, object?>>
            {
                ["LEngineLineFind"] = static args =>
                    LMentionClerk.LMentionLineFind((LEntryDraft)args![0]!, (LSentenceDraft)args[1]!),
            }));

    internal static IReadOnlyList<CMeaning> TCardSenseRead(this CCardField field) => field.CCardSenseRead();
}
