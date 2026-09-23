# LWindow.cs

## `public sealed class LWindow`

The deportment of the main window, shared by every panel through the window.
It forwards the engine facts a panel needs that belong to no one panel.
Those are fonts, flags, the languages, the respelling switches, the reflex rules and the media locations.
It also answers the pack facts the editor rows ask: schemes, tones, parts of speech, glyphs and recordings.
The settings panel and the window itself read and save the settings, the workspace and its state through it.
The leftover and recording sweeps, the audit record and the portrait export run through it as well.
Since plan 13 it also carries the posture.
The veneer reads and saves the layout, mode, volume and bounds here.
It hands the veneer the pure text facts too: mention pieces, unit and offset conversion, anchors and markdown blocks.
It words a refusal for the notice and opens a folder or a link through the engine's usher.
It disposes the posture when the window closes.
The static veneer helpers take it instead of the engine, so no panel holds an engine for them.
It builds every panel's deportment over the ports it holds, so no panel names the engine or a port.
A panel hands in only its own seams and the editor it shares, and receives its deportment built.
It keeps no state of its own.
Panel state stays on each panel's deportment, and window geometry stays on the posture.

## `public (int LWindowSpanOffset, int LWindowSpanLength) LWindowSpanRead(string text, int start, int length)`

A field's selection as a Mention span, measured by the engine in code points.

## `public static IReadOnlyList<(long LMeaningId, string LMeaningName, int LMeaningDepth)> LWindowMeaningSort(IReadOnlyList<LMeaning> meanings, string unknown)`

The Meanings of one Entry as sense-menu rows in reading order, each with its depth.
A Meaning is named by its title, or by its definition when it has no title.
The unknown label names it when it has neither.

## `private static void LWindowMeaningAppend(...)`

Walks one level of the tree and recurses under each Meaning it lists.
The engine returns the Meanings grouped by parent, and the menu wants them in reading order.
So the children of each parent are picked out and sorted by position before their own children follow.
A row that names itself as its parent is skipped rather than followed.
So a bad row cannot loop the walk.
