# LBootstrap.cs

## `public sealed class LBootstrap`

The start of the program as the user sees it: every dialog a start can raise and the fault handlers.
The host builds the engine and hands each step here as a seam, so this class names no engine type.
A failed step shows its dialog and answers false or null, and the host decides how the run ends.
So nothing here calls `Shutdown` while the program is starting.

## `public LBootstrap(System.Windows.Application application, Func<string, string> textSeam)`

The application is the one whose windows the fault handler counts and whose dispatcher it watches.
The text seam reads the interface catalog, so every dialog speaks the language already applied.

## `private Func<Exception, string?> _lBootstrapAudit = _ => null;`

Records a fault and names the log it went to.
It stays silent until the host attaches the engine's audit, since no fault handler runs before that.

## `public bool LBootstrapThemeApply(Action themeSeam)`

Applies the default catalog and the theme, and answers whether the program can show anything at all.
The catalog is what would have translated the failure message, so that one message stays inline in English.
It is the documented emergency fallback, shown only when the interface text is unavailable.

## `public LBootstrapEngine? LBootstrapBuild<LBootstrapEngine>(`

Reads the workspace folder and builds the engine over it, answering null when either step fails.
An unwritable folder must reach the user as a message naming the folder, not as a crash inside a constructor.
A corrupt database or a file from a newer build no longer stops the launch.
`LDoctor` sets it aside inside the engine and the rescue notice reports it afterwards.
A database another program holds is not set aside, and the busy seam lets the message say so.
Closing that program is the cure.
The engine is a type parameter because the deportment may not name the engine itself.

## `public void LBootstrapCatalogApply(`

Applies the language the workspace stores before any window or notice is shown.
Every panel therefore attaches in that language, and the rescue notice speaks it too.
A stored language the interface does not carry leaves English, which is already applied, in place.
A catalog that fails to load is reported and English stays, since the program can still run in it.

## `public void LBootstrapRescueShow(bool done, string? backup, string? reason)`

Tells the user their database was set aside and a clean one started, and stays silent when it was not.
The program launches either way.
Without this the workspace would appear empty and the old data would look lost.
The message names the file that was kept, so the user can see nothing was thrown away.

## `public void LBootstrapFaultAttach(Func<Exception, string?> auditSeam)`

Hooks the dispatcher and the task scheduler once the engine exists to record what they catch.
A fault before this point is a failed start, which its own dialog already reports.

## `private void LBootstrapFaultHandle(object sender, DispatcherUnhandledExceptionEventArgs e)`

Catches every exception that reaches the dispatcher unhandled.
That is where an `async void` handler's fault lands.
The fault is written to the audit log, shown to the user, and marked handled so the program stays up.
Without this any such fault ends the process with the user's unsaved drafts still open.
The dialog names the log, so the user can hand over what happened.
When no window is left showing, the program ends after the dialog instead of lingering unseen.
A fault while the main window is first shown leaves it unopened, and an unopened window never closes.
Such a process held the installed executable and blocked the next build.

## `private bool LBootstrapWindowCheck()`

Whether any window of the program is showing, so a fault with none is the end of the run.

## `private void LBootstrapStrayHandle(object? sender, UnobservedTaskExceptionEventArgs e)`

Records a task fault nobody awaited and marks it observed.
Such a fault reaches no dispatcher, so this is the only place it is seen at all.
