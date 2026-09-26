# QBootstrap.cs

## `public sealed class QBootstrap`

The start of the program as the user sees it.
It applies the resources and holds every dialog a start can raise and the fault handlers.
The veneer's application is running already, and this class reaches it through `Application.Current` alone.
The host builds the engine and hands each step here as a seam, so this class names no engine type.
A failed step shows its dialog and answers false or null, and the host decides how the run ends.
So nothing here calls `Shutdown` while the program is starting.

## `private Func<Exception, string?> _qBootstrapAudit = _ => null;`

Records a fault and names the log it went to.
It stays silent until the host attaches the engine's audit, since no fault handler runs before that.

## `public QBootstrap()`

Attaches the visual-state handlers once, before any window loads.

## `public bool QBootstrapThemeApply(`

Applies the default catalog, the theme colors, and the shared control resources in one step.
It answers whether the program can show anything at all.
The catalog is what would have translated the failure message, so that one message stays inline in English.
It is the documented emergency fallback, shown only when the interface text is unavailable.

## `public QBootstrapEngine? QBootstrapBuild<QBootstrapEngine>(`

Reads the workspace folder and builds the engine over it, answering null when either step fails.
An unwritable folder must reach the user as a message naming the folder, not as a crash inside a constructor.
A corrupt database or a file from a newer build no longer stops the launch.
`LDoctor` sets it aside inside the engine and the rescue notice reports it afterwards.
A database another program holds is not set aside, and the busy seam lets the message say so.
Closing that program is the cure.
The engine is a type parameter because the deportment may not name the engine itself.

## `public void QBootstrapCatalogApply(`

Applies the language the workspace stores before any window or notice is shown.
Every panel therefore attaches in that language, and the rescue notice speaks it too.
A stored language the interface does not carry leaves English, which is already applied, in place.
A catalog that fails to load is reported and English stays, since the program can still run in it.

## `public void QBootstrapRescueShow(bool done, string? backup, string? reason)`

Tells the user their database was set aside and a clean one started, and stays silent when it was not.
The program launches either way.
Without this the workspace would appear empty and the old data would look lost.
The message names the file that was kept, so the user can see nothing was thrown away.

## `public void QBootstrapFaultAttach(Func<Exception, string?> auditSeam)`

Hooks the dispatcher and the task scheduler once the engine exists to record what they catch.
A fault before this point is a failed start, which its own dialog already reports.

## `public void QBootstrapWindowShow(PWindow window)`

Shows the main window the host built over the deportment, never over the engine itself.

## `private void QBootstrapFaultHandle(object sender, DispatcherUnhandledExceptionEventArgs e)`

Catches every exception that reaches the dispatcher unhandled.
That is where an `async void` handler's fault lands.
The fault is written to the audit log, shown to the user, and marked handled so the program stays up.
Without this any such fault ends the process with the user's unsaved drafts still open.
The dialog names the log, so the user can hand over what happened.
When no window is left showing, the program ends after the dialog instead of lingering unseen.
A fault while the main window is first shown leaves it unopened, and an unopened window never closes.
Such a process held the installed executable and blocked the next build.

## `private static bool QBootstrapWindowCheck()`

Whether any window of the program is showing, so a fault with none is the end of the run.

## `private void QBootstrapStrayHandle(object? sender, UnobservedTaskExceptionEventArgs e)`

Records a task fault nobody awaited and marks it observed.
Such a fault reaches no dispatcher, so this is the only place it is seen at all.
