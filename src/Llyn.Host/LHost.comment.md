# LHost.cs
Hash: `8133aed429fed138`

## `int code = 1;`

The file uses top-level statements, since a method named `Main` fails the name audit.
The exit code stays 1 unless the application runs, so a failed start exits with 1.

## `Thread thread = new(() =>`

WPF needs a single-threaded apartment, and top-level statements cannot carry `[STAThread]`.
So the whole run happens on one STA thread the entry point starts and joins.

## `HttpClient client = LRigFactory.LRigClientCreate();`

This file is the composition root: the one place that builds a rig and names every project.
The client is built once and handed to every rig of the session, and disposed after the run.

## `LUsher usher = new LUsherShell(new LUsherFile());`

The file usher answers path facts and the shell usher adds the launch of a folder or a link.

## `LPress press = new LPressBrowser();`

The rig carries the press, so the engine prints through it and the host applies none later.

## `LWarrant warrant = new LWarrantShield();`

The Windows warrant protects the Joplin token, so the engine never sees the operating system.
The rig carries it as a port, and the host is the one place that names the twin.

## `LPhonograph phonograph = new LPhonographMedia();`

The rig carries the phonograph as a port, and the host is the one place that names the twin.
The host builds one for the whole run, so a workspace change keeps the same player.

## `PBootstrap application = new();`

The veneer's application, which the host only creates and runs.
Creating it makes it `Application.Current`, which is how the deportment reaches it.

## `QBootstrap bootstrap = new();`

`QBootstrap` applies resources, raises every dialog of the start, and shows the window.
It pulls the application from `Application.Current`, so the host hands it no application.

## `bootstrap.QBootstrapIntroduce(() => LThemeLoader.LThemeLoaderLoad().LThemeColorRead, () => LLocalization.LLocalizationLoad(new LLocalizationLoader(), LLocalization.LLocalizationDefault), LWorkspaceRoot.LWorkspaceRootRead, workspace => new LEngine(LRigFactory.LRigFactoryBuild(workspace, client, usher, press, warrant, phonograph), path => LRigFactory.LRigFactoryBuild(path, client, usher, press, warrant, phonograph), LWorkspaceRoot.LWorkspaceRootChange), LDoctor.LDoctorBusyCheck, engine => { bootstrap.QBootstrapFaultIntroduce(engine.LEngineWorkspace.LEngineAuditRecord); LSettingsOutlet settings = new(engine); bootstrap.QBootstrapCatalogApply(() => LLocalization.LLocalizationDefaultCheck(settings.LEngineSettingsRead().LSettingsLocalization), () => settings.LEngineLocalizationLoad(LLocalization.LLocalizationNormalize(settings.LEngineSettingsRead().LSettingsLocalization))); LDoctorRescue rescue = engine.LEngineWorkspace.LEngineRescueRead(); bootstrap.QBootstrapRescueConsult(rescue.LDoctorRescueDone, rescue.LDoctorRescueBackup, rescue.LDoctorRescueReason); bootstrap.QBootstrapWindowShow(new QWindow(new CAtelier(new LPosture(engine), new LDraftOutlet(engine), new CEntryBundle(engine.LEngineEntry, engine.LEngineEntry, engine.LEngineCatalog, engine.LEngineVista, engine.LEngineCard, engine.LEngineCatalog, engine.LEngineCatalog, engine.LEngineMention, engine.LEngineLanguage, engine.LEngineSituation, engine.LEngineExample, engine.LEngineReference, engine.LEngineAuthor, engine.LEngineDraft, engine.LEnginePronunciation), settings, new CPhonologyBundle(engine.LEngineFanqie, engine.LEngineFanqie, engine.LEngineScript, engine.LEngineLanguage, engine.LEngineReflex, engine.LEngineVocabulary, engine.LEngineVocabulary, engine.LEngineStem), new LMediaOutlet(engine), new LPortraitOutlet(engine), QObserver.QObserverCreate<Action>(static run => run())))); code = application.Run(); engine.Dispose(); })`

The host hands over the theme, the engine factory and the rest of the run as seams.
`QBootstrap` decides whether the start continues, so the host holds no branch.
A theme or an engine that fails leaves the rest unrun, and the exit code stays 1.

## `() => LLocalization.LLocalizationLoad(new LLocalizationLoader(), LLocalization.LLocalizationDefault),`

The default catalog is applied before any engine exists, so the host opens it through the port itself.
That load lists the embedded languages first, so a stored language the build lacks falls back to the default.

## `workspace => new LEngine(LRigFactory.LRigFactoryBuild(workspace, client, usher, press, warrant, phonograph), path => LRigFactory.LRigFactoryBuild(path, client, usher, press, warrant, phonograph), LWorkspaceRoot.LWorkspaceRootChange)`

The engine reads the settings the rig holds and imports the languages when it opens.
The engine receives every adapter through the rig and references Infrastructure nowhere.

## `path => LRigFactory.LRigFactoryBuild(path, client, usher, press, warrant, phonograph),`

The rig factory the engine keeps for a workspace change.
The engine takes the new rig whole and refuses it whole.
A folder that fails to open leaves the old rig standing.

## `LWorkspaceRoot.LWorkspaceRootChange),`

Records the chosen folder, which the engine calls only after the new rig stands.
A folder that fails to open is therefore never pointed at.

## `LDoctor.LDoctorBusyCheck,`

The deportment may not name Infrastructure, so the host hands the doctor's busy check over as a seam.

## `engine =>`

The rest of the run, which `QBootstrap` calls only with an engine that was built.
It wires the engine into the window, runs it, and disposes the engine at its end.

## `bootstrap.QBootstrapFaultIntroduce(engine.LEngineWorkspace.LEngineAuditRecord);`

The first step once the engine exists, so a fault in any later step is recorded.

## `LSettingsOutlet settings = new(engine);`

Built before the window, because the catalog step reads the stored language through it.
The atelier then takes this same outlet.

## `bootstrap.QBootstrapCatalogApply(() => LLocalization.LLocalizationDefaultCheck(settings.LEngineSettingsRead().LSettingsLocalization), () => settings.LEngineLocalizationLoad(LLocalization.LLocalizationNormalize(settings.LEngineSettingsRead().LSettingsLocalization)))`

It runs before the rescue consult, so the rescue notice speaks the stored language.

## `bootstrap.QBootstrapWindowShow(new QWindow(new CAtelier(new LPosture(engine), new LDraftOutlet(engine), new CEntryBundle(engine.LEngineEntry, engine.LEngineEntry, engine.LEngineCatalog, engine.LEngineVista, engine.LEngineCard, engine.LEngineCatalog, engine.LEngineCatalog, engine.LEngineMention, engine.LEngineLanguage, engine.LEngineSituation, engine.LEngineExample, engine.LEngineReference, engine.LEngineAuthor, engine.LEngineDraft, engine.LEnginePronunciation), settings, new CPhonologyBundle(engine.LEngineFanqie, engine.LEngineFanqie, engine.LEngineScript, engine.LEngineLanguage, engine.LEngineReflex, engine.LEngineVocabulary, engine.LEngineVocabulary, engine.LEngineStem), new LMediaOutlet(engine), new LPortraitOutlet(engine), QObserver.QObserverCreate<Action>(static run => run()))))`

The host builds Conduct's root and hands it to the deportment's window, so `QBootstrap` builds nothing.
The root takes the posture and one outlet per port, all over the one engine.
The entry ports and the sound ports are the engine's own facades, gathered in two bundles with no outlet between.
The deportment receives the root and never sees the engine itself.
The root also takes the marshal, built here on the UI thread, which every editor takes at construction.

## `code = application.Run();`

The window disposes its own deportment and posture when it closes.
The host then disposes the engine the window ran over.

## `client.Dispose();`

The client is disposed in this one place at the end, whether or not the start went through.
