using System.Net.Http;
using System.Threading;
using Llyn.Application;
using Llyn.Core;
using Llyn.Core.Windows;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Llyn.UIDeportment;

int code = 1;
Thread thread = new(() =>
{
    HttpClient client = LRigFactory.LRigClientCreate();
    LUsher usher = new LUsherShell(new LUsherFile());
    LPress press = new LPressBrowser();
    LPhonograph phonograph = new LPhonographMedia();
    PBootstrap application = new();
    LBootstrap bootstrap = new(application, application.PBootstrapTextRead);

    bool themed = bootstrap.LBootstrapThemeApply(() => application.PBootstrapThemeApply(
        LThemeLoader.LThemeLoaderLoad().LThemeColorRead,
        LLocalization.LLocalizationLoad(new LLocalizationLoader(), LLocalization.LLocalizationDefault)));
    LEngine? engine = themed
        ? bootstrap.LBootstrapBuild(
            LWorkspaceRoot.LWorkspaceRootRead,
            workspace => new LEngine(LRigFactory.LRigFactoryBuild(workspace, client, usher, press, phonograph)),
            LDoctor.LDoctorBusyCheck)
        : null;
    if (engine is null)
    {
        client.Dispose();
        return;
    }

    bootstrap.LBootstrapFaultAttach(engine.LEngineAuditRecord);
    LSettingsOutlet settings = new(engine);
    bootstrap.LBootstrapCatalogApply(
        () => LLocalization.LLocalizationDefaultCheck(settings.LEngineSettingsRead().LSettingsLocalization),
        () => settings.LEngineLocalizationLoad(
            LLocalization.LLocalizationNormalize(settings.LEngineSettingsRead().LSettingsLocalization)),
        application.PBootstrapCatalogApply);
    LDoctorRescue rescue = engine.LEngineRescueRead();
    bootstrap.LBootstrapRescueShow(rescue.LDoctorRescueDone, rescue.LDoctorRescueBackup, rescue.LDoctorRescueReason);

    application.PBootstrapWindowShow(new PWindow(
        new LWindow(
            new LPosture(engine),
            new LDraftOutlet(engine),
            new LEntryOutlet(engine),
            settings,
            new LPhonologyOutlet(engine),
            new LMediaOutlet(engine),
            new LPortraitOutlet(engine)),
        path =>
        {
            engine.LEngineRigApply(LRigFactory.LRigFactoryBuild(path, client, usher, press, phonograph));
            LWorkspaceRoot.LWorkspaceRootChange(path);
        }));
    code = application.Run();
    engine.Dispose();
    client.Dispose();
});
thread.SetApartmentState(ApartmentState.STA);
thread.Start();
thread.Join();
return code;
