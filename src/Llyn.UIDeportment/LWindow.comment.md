# LWindow.cs

## `public sealed class LWindow`

The deportment of the main window, shared by every panel through the window.
It forwards the engine facts a panel needs that belong to no one panel.
Those are fonts, flags, the languages, the respelling switches, the reflex rules and the media locations.
It also answers the pack facts the editor rows ask: schemes, tones, parts of speech, glyphs and recordings.
The settings panel and the window itself read and save the settings, the workspace and its state through it.
The leftover and recording sweeps, the audit record and the portrait export run through it as well.
The static veneer helpers take it instead of the engine, so no panel holds an engine for them.
It builds every panel's deportment over the ports it holds, so no panel names the engine or a port.
A panel hands in only its own seams and the editor it shares, and receives its deportment built.
It keeps no state of its own.
Panel state stays on each panel's deportment, and window geometry stays on the posture.
