# LWindow.cs

## `public sealed class LWindow`

The deportment of the main window, shared by every panel through the window.
It stands over Conduct's atelier, `CAtelier`, whose gates are the session's medium-free actions.
It also builds `QPosture`, which keeps the GUI-only state through Deportment's Capsule.
It forwards the engine facts a panel needs that belong to no one panel.
Those are fonts, the respelling switches, the reflex rules and the pack facts the editor rows ask.
It hands the veneer the pure text facts too: mention pieces, unit and offset conversion, anchors and markdown blocks.
The static veneer helpers take it instead of the engine, so no panel holds an engine for them.
The panel factories went to `QForge`, which it builds over the root's ports and exposes.
The settings, the folder and the stored-entry reads went to `QWorkspace`, which it builds and exposes.
Each engine value is mapped once, by an internal static on the controller that owns the shape.
Panel state stays on each panel's deportment, and window geometry stays on `QPosture`.
The text services leave for Conduct in job02-02 and the rest in job03-04.

## `private readonly List<Action> _lWindowVistas`

The vista restore of every panel deportment built here, in the order they were built.
A workspace change runs them all again, so each panel stands on the new workspace's vistas.

## `internal LWindow(CAtelier atelier)`

Stands the window deportment over the root the host built.
It is internal, so only the window builds one.

## `public QWorkspace LWindowWorkspace`

The workspace deportment, built over this window deportment and the root's ports.

## `public QForge LWindowForge`

The panel factory, built over this window deportment and the ports every panel stands on.

## `public CAtelier LWindowAtelier { get; }`

Conduct's atelier, whose gates every panel calls directly.

## `public QPosture LWindowPosture { get; }`

The GUI-only state of the window: geometry, panel widths and linked columns.

## `public void LWindowVistaRestore()`

Restarts every panel's vistas, which the settings panel asks for after a workspace change.

## `internal LWindowHeld LWindowVistaAdd<LWindowHeld>(LWindowHeld held, Action<LWindowHeld, LWindow> restore)`

Runs one deportment's restore and keeps it for the next workspace change.
`QForge` calls it for each panel it builds, so the restore list stays here.

## `public IReadOnlyList<string> LWindowLocalizationScan()`

The interface languages the build embeds, which the settings panel lists in its language box.

## `public IReadOnlyList<CMentionLabel> LWindowMentionResolve(string text, IReadOnlyList<CMentionDraft> mentions, string silent)`

The chips of a mention line, each named by its linked headword or by `silent` when unlinked.

## `public (int LWindowSpanOffset, int LWindowSpanLength) LWindowSpanRead(string text, int start, int length)`

A field's selection as a Mention span, measured by the engine in code points.

## `public bool LWindowSpanCheck(string text, int start, int length)`

Whether a field's selection spans any code point, so a link command may run.
A driver asks this rather than measuring the span it read itself.

## `internal bool LWindowAnchorCheck(IReadOnlyList<LFanqieRow> rows, string headword)`

The anchor checks stay internal, since only the sound lectern calls them with the display's rows.

## `internal static IReadOnlyList<CMention> LWindowMentionRead(IReadOnlyList<LMention> mentions)`

The stored Mentions of a text as a mention text holds them.
The overload taking the shapes maps them back for the engine's piece and find calls.
