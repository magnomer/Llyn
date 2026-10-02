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
    LPhonograph phonograph = new LPhonographMedia();
    PBootstrap application = new();
    QBootstrap bootstrap = new();

    bootstrap.QBootstrapIntroduce(
        () => LThemeLoader.LThemeLoaderLoad().LThemeColorRead,
        () => LLocalization.LLocalizationLoad(new LLocalizationLoader(), LLocalization.LLocalizationDefault),
        LWorkspaceRoot.LWorkspaceRootRead,
        workspace => new LEngine(
            LRigFactory.LRigFactoryBuild(workspace, client, usher, press, phonograph),
            path => LRigFactory.LRigFactoryBuild(path, client, usher, press, phonograph),
            LWorkspaceRoot.LWorkspaceRootChange),
        LDoctor.LDoctorBusyCheck,
        engine =>
        {
            bootstrap.QBootstrapFaultIntroduce(engine.LEngineAuditRecord);
            LSettingsOutlet settings = new(engine);
            bootstrap.QBootstrapCatalogApply(
                () => LLocalization.LLocalizationDefaultCheck(settings.LEngineSettingsRead().LSettingsLocalization),
                () => settings.LEngineLocalizationLoad(
                    LLocalization.LLocalizationNormalize(settings.LEngineSettingsRead().LSettingsLocalization)));
            LDoctorRescue rescue = engine.LEngineRescueRead();
            bootstrap.QBootstrapRescueConsult(
                rescue.LDoctorRescueDone, rescue.LDoctorRescueBackup, rescue.LDoctorRescueReason);

            bootstrap.QBootstrapWindowShow(new QWindow(new CAtelier(
                new LPosture(engine),
                new LDraftOutlet(engine),
                new LEntryOutlet(engine),
                settings,
                new LPhonologyOutlet(engine),
                new LMediaOutlet(engine),
                new LPortraitOutlet(engine))));
            code = application.Run();
            engine.Dispose();
        });
    client.Dispose();
});
thread.SetApartmentState(ApartmentState.STA);
thread.Start();
thread.Join();
return code;
