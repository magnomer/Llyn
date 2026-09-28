# QForge.cs

## `public sealed class QForge`

The factory of every panel's deportment, built over the atelier and reading the ports it holds.
No panel names the engine or a port, since each hands in only its own seams and shared editor.
It keeps no port of its own, so each port has one holder, and job44 sinks it into Conduct.

## `public LEditor QForgeInputCreate(CEnvoy envoy)`

The input panel's editor, standing on the input vista already restored.
A panel's own editor comes from `QForgeEditorCreate` instead, and its panel restores it with the panel's vista.

## `public CCorpus QForgeCorpusCreate(Func<bool> shownSeam, CEnvoy envoy)`

Every panel deportment is handed back with its vistas already started through the posture.
The corpus, favorites, library and authors panels are handed back as their Conduct, which starts its own vistas.
The forge keeps the restore through `QForgeVistaAdd`, so a workspace change can run it again.

## `public void QForgeVistaRestore()`

Restarts every panel's vistas, which the settings panel asks for after a workspace change.

## `private QForgeHeld QForgeVistaAdd<QForgeHeld>(QForgeHeld held, Action<QForgeHeld> restore)`

Runs one deportment's restore and keeps it for the next workspace change.
The list holds GUI callbacks, so it stays in Deportment and never reaches Conduct.
