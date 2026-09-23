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
