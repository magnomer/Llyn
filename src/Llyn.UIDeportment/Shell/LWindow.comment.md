# LWindow.cs

## `public sealed class LWindow`

The deportment of the main window, shared by every panel through the window.
It forwards the engine facts a panel needs that belong to no one panel.
Those are fonts, the respelling switches, the reflex rules and the pack facts the editor rows ask.
Since plan 13 it also carries the posture.
The veneer reads and saves the layout, mode, volume and bounds here.
It hands the veneer the pure text facts too: mention pieces, unit and offset conversion, anchors and markdown blocks.
It disposes the posture when the window closes.
The static veneer helpers take it instead of the engine, so no panel holds an engine for them.
It builds every panel's deportment over the ports it holds, so no panel names the engine or a port.
A panel hands in only its own seams and the editor it shares, and receives its deportment built.
Since job 8 it is sealed, so its public members name only .NET types, Conduct shapes and controllers.
The settings, the folder and the stored-entry reads went to `QWorkspace`, which it builds and exposes.
Each engine value is mapped once, by an internal static on the controller that owns the shape.
Panel state stays on each panel's deportment, and window geometry stays on the posture.

## `private readonly List<Action> _lWindowVistas`

The vista restore of every panel deportment built here, in the order they were built.
A workspace change runs them all again, so each panel stands on the new workspace's vistas.

## `internal LWindow(...)`

Takes the posture and the ports the host builds over one engine.
It is internal, so only the host, built as `Llyn`, can build a window deportment.

## `public QWorkspace LWindowWorkspace`

The workspace deportment, built over this window deportment and its ports.

## `internal LEntryPort LWindowEntryPort`

The ports the workspace deportment and a duplex wing reach, handed to no driver.

## `public LEditor LWindowInputCreate(Func<bool> unreadableSeam)`

The input panel's editor, standing on the input vista already restored.
A panel's own editor comes from `LWindowEditorCreate` instead, and its panel restores it with the panel's vista.

## `public LCorpus LWindowCorpusCreate(...)`

Every panel deportment is handed back with its vistas already started through the posture.
The restore is kept, so `LWindowVistaRestore` can run it again.

## `public void LWindowVistaRestore()`

Restarts every panel's vistas, which the settings panel asks for after a workspace change.

## `private LWindowHeld LWindowVistaAdd<LWindowHeld>(LWindowHeld held, Action<LWindowHeld, LWindow> restore)`

Runs one deportment's restore and keeps it for the next workspace change.

## `public CLayout? LWindowLayoutRead(string tab)`

The stored widths of one tab, or null when the posture holds none for it.

## `internal LVista LWindowVistaStart(string tab, LSubject? subject, LCatalogOrder fallback, bool blank = false)`

Starts a vista through the posture, for the panel deportments and the duplex wings alone.

## `public void LWindowLeftoverSweep()`

Sweeps the drafts the workspace holds that no panel stands on, on the way in and on the way out.

## `public Action LWindowObserverAttach(CSubject subject, Action<CBulletin> observer)`

Hands a driver every engine notice of one subject, copied into the Conduct shape.
The overload without a subject hands it every notice.
The answer detaches the observer again, so a driver keeps no engine delegate.

## `private Action LWindowObserverAdd(Action<LBulletin> sent)`

Attaches the engine observer and answers its detach.

## `public IReadOnlyList<string> LWindowLocalizationScan()`

The interface languages the build embeds, which the settings panel lists in its language box.

## `public IReadOnlyList<CMentionLabel> LWindowMentionResolve(string text, IReadOnlyList<CMentionDraft> mentions, string silent)`

The chips of a mention line, each named by its linked headword or by `silent` when unlinked.

## `public (int LWindowSpanOffset, int LWindowSpanLength) LWindowSpanRead(string text, int start, int length)`

A field's selection as a Mention span, measured by the engine in code points.

## `internal bool LWindowAnchorCheck(IReadOnlyList<LFanqieRow> rows, string headword)`

The anchor checks stay internal, since only the sound lectern calls them with the display's rows.

## `internal static IReadOnlyList<CMention> LWindowMentionRead(IReadOnlyList<LMention> mentions)`

The stored Mentions of a text as a mention text holds them.
The overload taking the shapes maps them back for the engine's piece and find calls.
