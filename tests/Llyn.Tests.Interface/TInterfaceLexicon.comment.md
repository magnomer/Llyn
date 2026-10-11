# TInterfaceLexicon.cs
Hash: `62c8c9447eaa99fd`

## `internal static partial class TInterface`

The relays for the lexical records of the core layer.
They build the lexical records and drafts a test hands to the engine.
The video film and span reads, the grasp range check and the grasp star scan are relayed here too.
The application's reflex find over a draft is relayed here too.
Each relay is transparent and carries no test logic of its own.
The reflex draft builder takes optional anchors, so a scan test can read a row's anchors back.
`TEntryDraftCreate` takes a pronunciation, its audio and part-of-speech names, and builds the draft shapes around them.
`TSpeechNameRead` reads those names back off a loaded draft.

`TInterfaceExample` builds the example, gloss and sentence records and drafts.
`TInterfaceMentionSpan` builds the mentions and reads the spans a test divides.
`TInterfaceInflection` builds the form, inflection, paradigm, feature and morphology records.
`TInterfaceState` builds and reads the state values and anchors.
