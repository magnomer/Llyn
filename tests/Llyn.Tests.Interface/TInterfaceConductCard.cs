using System.Collections.Generic;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceConductCard
{
    internal static CCard TCardCreate(LEngine engine, CDesk desk, CEnvoy envoy) => new(
        desk, new LDraftOutlet(engine), engine.LEngineReference, new LSettingsOutlet(engine), envoy);

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

    internal static CEntryDraft TCardEntryRead(
        LEntryDraft draft, IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> targets) =>
        CFolio.CFolioEntryRead(draft, targets, new HashSet<long>(), TInterfaceConduct.TMediaCreate());
}
