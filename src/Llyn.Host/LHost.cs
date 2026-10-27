using System;
using System.IO;
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
    using LRigFactory foundry = new(
        new LUsherShell(new LUsherFile()),
        new LPressBrowser(),
        new LPressSpooler(),
        new LWarrantShield(),
        new LPhonographMedia());
    BBootstrap bootstrap = new(new BConsoleStream(), new ABootstrap());

    int status = bootstrap.BBootstrapIntroduce(
        args,
        name => LThemeLoader.LThemeLoaderLoad(name).LThemeColorRead,
        () => LLocalization.LLocalizationLoad(new LLocalizationLoader(), LLocalization.LLocalizationDefault),
        LWorkspaceRoot.LWorkspaceRootRead,
        (workspace, leased) => new LEngine(
            foundry.LRigFactoryBuild(workspace),
            path => foundry.LRigFactoryBuild(path),
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
                detached => atelier(engine, settings, marshal, detached),
                File.Exists,
                Directory.GetDirectories,
                Directory.GetFiles,
                Directory.GetLogicalDrives);
            engine.Dispose();
            return run;
        });
    return status;
};

Func<int> window = () =>
{
    int code = 1;
    Thread thread = new(() =>
    {
        using LRigFactory foundry = new(
            new LUsherShell(new LUsherFile()),
            new LPressBrowser(),
            new LPressSpooler(),
            new LWarrantShield(),
            new LPhonographMedia());
        PBootstrap application = new();
        QBootstrap bootstrap = new();

        bootstrap.QBootstrapIntroduce(
            () => LThemeLoader.LThemeLoaderLoad().LThemeColorRead,
            () => LLocalization.LLocalizationLoad(new LLocalizationLoader(), LLocalization.LLocalizationDefault),
            LWorkspaceRoot.LWorkspaceRootRead,
            workspace => new LEngine(
                foundry.LRigFactoryBuild(workspace),
                path => foundry.LRigFactoryBuild(path),
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
    });
    thread.SetApartmentState(ApartmentState.STA);
    thread.Start();
    thread.Join();
    return code;
};

return LGuestroom.LGuestroomRun(Environment.ProcessPath, new LConsoleParent().LConsoleAttach(), console, window);
