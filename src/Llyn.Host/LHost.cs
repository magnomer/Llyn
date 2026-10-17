using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.Core.Windows;
using Llyn.Guestroom;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UIDeportment;
using ABootstrap = Llyn.UITerminal.ABootstrap;
using PBootstrap = Llyn.UIVeneer.PBootstrap;

Func<LEngine, LSettingsOutlet, Action<Action>, bool, CAtelier> atelier = static (engine, settings, marshal, detached) =>
    new CAtelier(
        new LPosture(engine),
        new LDraftOutlet(engine),
        new CEntryBundle(
            engine.LEngineEntry,
            engine.LEngineEntry,
            engine.LEngineCatalog,
            engine.LEngineVista,
            engine.LEngineCard,
            engine.LEngineCatalog,
            engine.LEngineCatalog,
            engine.LEngineMention,
            engine.LEngineLanguage,
            engine.LEngineSituation,
            engine.LEngineExample,
            engine.LEngineReference,
            engine.LEngineAuthor,
            engine.LEngineDraft,
            engine.LEnginePronunciation,
            engine.LEngineAncestor,
            new LDeckOutlet(engine)),
        settings,
        new CPhonologyBundle(
            engine.LEngineFanqie,
            engine.LEngineFanqie,
            engine.LEngineScript,
            engine.LEngineLanguage,
            engine.LEngineReflex,
            engine.LEngineVocabulary,
            engine.LEngineVocabulary,
            engine.LEngineStem),
        new LMediaOutlet(engine),
        new LPortraitOutlet(engine),
        marshal,
        detached);

Func<int> console = () =>
{
    HttpClient client = LRigFactory.LRigClientCreate();
    LUsher usher = new LUsherShell(new LUsherFile());
    LPress press = new LPressBrowser();
    LPressQueue queue = new LPressSpooler();
    LWarrant warrant = new LWarrantShield();
    LPhonograph phonograph = new LPhonographMedia();
    BBootstrap bootstrap = new(new BConsoleStream(), new ABootstrap());

    int status = bootstrap.BBootstrapIntroduce(
        args,
        name => LThemeLoader.LThemeLoaderLoad(name).LThemeColorRead,
        () => LLocalization.LLocalizationLoad(new LLocalizationLoader(), LLocalization.LLocalizationDefault),
        LWorkspaceRoot.LWorkspaceRootRead,
        (workspace, leased) => new LEngine(
            LRigFactory.LRigFactoryBuild(workspace, client, usher, press, queue, warrant, phonograph),
            path => LRigFactory.LRigFactoryBuild(path, client, usher, press, queue, warrant, phonograph),
            LWorkspaceRoot.LWorkspaceRootChange,
            leased),
        LDoctor.LDoctorBusyCheck,
        engine =>
        {
            bootstrap.BBootstrapFaultIntroduce(engine.LEngineWorkspace.LEngineAuditRecord);
            LSettingsOutlet settings = new(engine);
            bootstrap.BBootstrapCatalogApply(
                () => LLocalization.LLocalizationDefaultCheck(settings.LEngineSettingsRead().LSettingsLocalization),
                () => settings.LEngineLocalizationLoad(
                    LLocalization.LLocalizationNormalize(settings.LEngineSettingsRead().LSettingsLocalization)));
            LDoctorRescue rescue = engine.LEngineWorkspace.LEngineRescueRead();
            bootstrap.BBootstrapRescueConsult(
                rescue.LDoctorRescueDone, rescue.LDoctorRescueBackup, rescue.LDoctorRescueReason);

            Action<Action> marshal = BObserver.BObserverCreate<Action>(static work => work());
            int run = bootstrap.BBootstrapAtelierRun(
                detached => atelier(engine, settings, marshal, detached), File.Exists);
            engine.Dispose();
            return run;
        });
    client.Dispose();
    return status;
};

Func<int> window = () =>
{
    int code = 1;
    Thread thread = new(() =>
    {
        HttpClient client = LRigFactory.LRigClientCreate();
        LUsher usher = new LUsherShell(new LUsherFile());
        LPress press = new LPressBrowser();
        LPressQueue queue = new LPressSpooler();
        LWarrant warrant = new LWarrantShield();
        LPhonograph phonograph = new LPhonographMedia();
        PBootstrap application = new();
        QBootstrap bootstrap = new();

        bootstrap.QBootstrapIntroduce(
            () => LThemeLoader.LThemeLoaderLoad().LThemeColorRead,
            () => LLocalization.LLocalizationLoad(new LLocalizationLoader(), LLocalization.LLocalizationDefault),
            LWorkspaceRoot.LWorkspaceRootRead,
            workspace => new LEngine(
                LRigFactory.LRigFactoryBuild(workspace, client, usher, press, queue, warrant, phonograph),
                path => LRigFactory.LRigFactoryBuild(path, client, usher, press, queue, warrant, phonograph),
                LWorkspaceRoot.LWorkspaceRootChange,
                true),
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

                bootstrap.QBootstrapWindowShow(new QWindow(
                    atelier(engine, settings, QObserver.QObserverCreate<Action>(static run => run()), false)));
                code = application.Run();
                engine.Dispose();
            });
        client.Dispose();
    });
    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();
    thread.Join();
    return code;
};

return LGuestroom.LGuestroomRun(Environment.ProcessPath, new LConsoleParent().LConsoleAttach(), console, window);
