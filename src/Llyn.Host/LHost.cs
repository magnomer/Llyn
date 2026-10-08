using System;
using System.Net.Http;
using System.Threading;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.Core.Windows;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using PBootstrap = Llyn.UIVeneer.PBootstrap;

int code = 1;
Thread thread = new(() =>
{
    HttpClient client = LRigFactory.LRigClientCreate();
    LUsher usher = new LUsherShell(new LUsherFile());
    LPress press = new LPressBrowser();
    LWarrant warrant = new LWarrantShield();
    LPhonograph phonograph = new LPhonographMedia();
    PBootstrap application = new();
    QBootstrap bootstrap = new();

    bootstrap.QBootstrapIntroduce(
        () => LThemeLoader.LThemeLoaderLoad().LThemeColorRead,
        () => LLocalization.LLocalizationLoad(new LLocalizationLoader(), LLocalization.LLocalizationDefault),
        LWorkspaceRoot.LWorkspaceRootRead,
        workspace => new LEngine(
            LRigFactory.LRigFactoryBuild(workspace, client, usher, press, warrant, phonograph),
            path => LRigFactory.LRigFactoryBuild(path, client, usher, press, warrant, phonograph),
            LWorkspaceRoot.LWorkspaceRootChange),
        LDoctor.LDoctorBusyCheck,
        engine =>
        {
            bootstrap.QBootstrapFaultIntroduce(engine.LEngineWorkspace.LEngineAuditRecord);
            LSettingsOutlet settings = new(engine);
            bootstrap.QBootstrapCatalogApply(
                () => LLocalization.LLocalizationDefaultCheck(settings.LEngineSettingsRead().LSettingsLocalization),
                () => settings.LEngineLocalizationLoad(
                    LLocalization.LLocalizationNormalize(settings.LEngineSettingsRead().LSettingsLocalization)));
            LDoctorRescue rescue = engine.LEngineWorkspace.LEngineRescueRead();
            bootstrap.QBootstrapRescueConsult(
                rescue.LDoctorRescueDone, rescue.LDoctorRescueBackup, rescue.LDoctorRescueReason);

            bootstrap.QBootstrapWindowShow(new QWindow(new CAtelier(
                new LPosture(engine),
                new LDraftOutlet(engine),
                new CEntryBundle(
                    engine.LEngineEntry,
                    engine.LEngineEntry,
                    engine.LEngineVista,
                    engine.LEngineVista,
                    engine.LEngineCard,
                    engine.LEngineCard,
                    engine.LEngineCard,
                    engine.LEngineMention,
                    engine.LEngineLanguage,
                    engine.LEngineSituation,
                    engine.LEngineExample,
                    engine.LEngineReference,
                    engine.LEngineAuthor,
                    engine.LEngineDraft,
                    engine.LEnginePronunciation),
                settings,
                new CPhonologyBundle(
                    engine.LEngineFanqie,
                    engine.LEngineFanqie,
                    engine.LEngineLanguage,
                    engine.LEngineLanguage,
                    engine.LEngineReflex,
                    engine.LEngineVocabulary,
                    engine.LEngineVocabulary,
                    engine.LEngineStem),
                new LMediaOutlet(engine),
                new LPortraitOutlet(engine),
                QObserver.QObserverCreate<Action>(static run => run()))));
            code = application.Run();
            engine.Dispose();
        });
    client.Dispose();
});
thread.SetApartmentState(ApartmentState.STA);
thread.Start();
thread.Join();
return code;
