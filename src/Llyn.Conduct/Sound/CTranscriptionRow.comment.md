# CTranscriptionRow.cs
Hash: `0011cc7ba8719efc`

## `public sealed record CTranscriptionRow(`

One row of the editor's transcription block, ready to paint with its scheme dropdown.

**Parameters**

- `CTranscriptionRowDraft`: the row's id, scheme and text.
- `CTranscriptionRowSchemes`: every scheme of the pack, each marked when another row holds it.
