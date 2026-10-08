using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceConductDesk
{
    internal static CDesk TDeskCreate(LEngine engine, string scope, CEnvoy envoy) =>
        new(new LDraftOutlet(engine), new LSettingsOutlet(engine), scope, envoy);

    internal static CDesk TDeskCreate(LEngine engine, string scope, CEnvoy envoy, string origin, CSubject subject) =>
        new(new LDraftOutlet(engine), new LSettingsOutlet(engine), scope, envoy, origin, subject);

    internal static void TDeskOccurrenceStart(this CDesk desk, long? situation) =>
        desk.LDeskOccurrenceStart(situation);

    internal static void TDeskQuotationStart(this CDesk desk, long? example) => desk.LDeskQuotationStart(example);

    internal static void TDeskFootnoteStart(this CDesk desk, long? reference) => desk.LDeskFootnoteStart(reference);

    internal static void TDeskMembershipStart(this CDesk desk, long? tag) => desk.LDeskMembershipStart(tag);

    internal static void TDeskCohortStart(this CDesk desk, long? register) => desk.LDeskCohortStart(register);

    internal static void TDeskVistaRestore(this CDesk desk, LVista vista) => desk.CDeskVistaRestore(vista);

    internal static void TDeskDefer(this CDesk desk, LRequest request) =>
        desk.CDeskTenure?.LTenureRequestDefer(request);

    internal static LDraft? TDeskRead(this CDesk desk) => desk.CDeskRead();

    internal static IReadOnlyList<long> TDeskSentenceRead(this CDesk desk) =>
        desk.CDeskRead() is LDraft draft
            ? draft.LDraftContent.LEntryDraftMeanings.Concat(draft.LDraftContent.LEntryDraftCollocations)
                .SelectMany(static card => card.LCardDraftSentence)
                .Select(static sentence => sentence.LSentenceDraftId)
                .Order()
                .ToList()
            : [];

    internal static CEsteem TEsteemCreate(LEngine engine, LGraspPort grasps, long stored)
    {
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, []);
        CDesk desk = TDeskCreate(engine, "Input", envoy);
        desk.CDeskVistaRestore(engine.TEngineVistaStart("library", LCatalogOrder.LCatalogOrderHeadword));
        desk.CDeskStart(stored);
        return new CEsteem(
            desk,
            new LDisplay(
                new LDraftOutlet(engine),
                engine.LEngineEntry,
                engine.LEngineVista,
                engine.LEngineVista,
                grasps,
                engine.LEnginePronunciation,
                engine.LEngineLanguage,
                engine.LEngineReflex,
                engine.LEngineFanqie,
                engine.LEngineLanguage,
                engine.LEngineVocabulary,
                new LSettingsOutlet(engine),
                TEngineFake.TEngineStubCreate<LMediaPort>(),
                envoy,
                new CLedgerNoticed()));
    }

    internal static LDraft? TDeskHeldRead(this CDesk desk) => desk.CDeskTenure?.LTenureRead();

    internal static void TDeskVarietySet(this CDesk desk, bool primary, long pronunciation, string variety)
    {
        if (desk.CDeskTenure is LTenure held)
        {
            new LQuillPronunciation(held).LQuillVarietySet(primary, pronunciation, variety);
        }
    }

    internal static LForay? TDeskForayStart(this CDesk desk, string scheme) =>
        desk.CDeskTenure?.LTenureErrand.LErrandTranscriptionStart(0, scheme, static (_, _) => { });

    internal static bool TDeskChangeCheck(this CDesk desk) => desk.LDeskChangeCheck();

    internal static CRecording? TErrandRecordingRead(LRecording? recording) => CErrand.CErrandRecordingRead(recording);

    internal static LRecording TErrandRecordingRead(CRecording recording) => CErrand.CErrandRecordingRead(recording);

    internal static CCandidate? TErrandCandidateRead(LCandidate? candidate) => CErrand.CErrandCandidateRead(candidate);

    internal static void TErrandHarvestResonate(this CErrand errand, CHarvestStep step) =>
        errand.LErrandHarvestResonate(step);

    internal static void TErrandLookupResonate(this CErrand errand, CLookupStep step, LForay foray) =>
        errand.LErrandLookupResonate(step, foray);

    internal static CSession TSessionCreate(
        CDesk desk, IReadOnlyList<Func<bool>> pending, Func<bool> readySeam, Action<long> storedSeam) =>
        new(
            desk, pending, null, static () => false, static _ => true, readySeam, storedSeam);

    internal static CSession TSessionCreate(CDesk desk, CDesk editor, Func<bool> shownSeam, List<string> seen) =>
        new(
            desk, [], editor, shownSeam,
            store =>
            {
                seen.Add(store ? "Finish" : "Drop");
                return true;
            },
            static () => true, static _ => { });

    internal static bool TSessionChangeCheck(this CSession session) => session.LSessionChangeCheck();

    internal static bool TSessionFinish(this CSession session, bool store) => session.LSessionFinish(store);
}
