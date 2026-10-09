# TPostureWorkspace.cs
Hash: `37bc217c5bd76de7`

## `public sealed class TPostureWorkspace`

Covers which posture file wins when a workspace starts or another one opens.
A posture file standing beside the legacy one wins.
A workspace moved onto keeps its own posture, and one without any inherits the posture held.
One moved onto with only the legacy settings file yields its mode and volume.
A posture file that will not read leaves the posture held and the file as it was.
A failing save after a workspace opens raises `LPostureSaveFailed` again, since each open clears the failed flag.
