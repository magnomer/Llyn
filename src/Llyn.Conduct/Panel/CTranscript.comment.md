# CTranscript.cs
Hash: `82b887d339d72c5b`

## `public sealed class CTranscript`

The corpus transcript's held Example and the gates that edit its Mentions.
It is split from `CCorpus`, since none of its gates asks the leave question.
The corpus owns the desk and the anthology, and this class only reads and edits through them.
Every gate past the open addresses the transcript as card 0 and sentence 0, the Example's single text.
It reports failures through the envoy, never through a seam a driver hands up.

## `internal CTranscript(CDesk desk, CAnthology anthology, LDraftPort drafts, LSettingsPort settings, CEnvoy envoy)`

Holds the corpus desk and anthology, the draft port Mentions are read through, and the failure reporters.
Building it is no user action, so `CCorpus` builds it right after the anthology and no driver does.
Every argument is required, so a missing one fails at once.

## `public event Action<CExample>? CTranscriptDraftChanged;`

Raised with the held Example after an edit to its draft, through the marshal the corpus attached.
`CCorpusTranscriptChanged` fills the transcript anew, while this one only refreshes what the edit changed.

## `public event Action<CProspect>? CTranscriptMentionOffered;`

The transcript's mention offer, ready to show.
The picker belongs to the corpus editor, so its Refine is wired to this event rather than handed a record.

## `internal void LTranscriptObserverAttach(Action<Action> marshal)`

Attaches the desk's draft notice to `LTranscriptDraftResonate` through `marshal`, which only the medium knows.
The corpus calls it once at build, so no driver wires the desk.

## `internal CExample? LTranscriptRead()`

Reads the Example the desk holds, which persists the tenure first and so can throw.
A throw reports `Example.HoldFailed` through the envoy and reads as null.
The Example carries the anthology panel's tally, which `LAnthologyDraftRead` reads itself.
`CTranscriptDraftChanged` hands it on each draft bulletin, so the desk never hands a driver a draft.
Its callers are the transcript's own draft notice and the corpus's session-held handler.

## `private void LTranscriptDraftResonate()`

Answers the desk's draft notice with the held Example read afresh.
The anthology's `LAnthologyTranscriptRead` marks whether the sentence field already shows its text.
A notice with no Example to show raises nothing, so the driver never redraws from nothing.

## `public void CTranscriptMentionOpen(string word)`

The gate that opens the transcript's mention picker, for the selected word as the user gave it.
It reads the offer through `CMention.LMentionProspectRead`, and a failed read shows `Mention.FindFailed`.
The offer is raised as `CTranscriptMentionOffered`, which the corpus editor's picker paints.

## `public void CTranscriptMentionAdd(string text, int start, int length, long? entryId)`

The gate for an Entry picked for the transcript's selection, with the box's raw text and selection.
The span rule and the request are ShellEngine's.
The driver hands the raw pick, a null id for a fresh row.
The engine has no new Entry path for a Mention.
So a null id shows `Refusal.TargetMissing` through the envoy and links nothing.

## `public void CTranscriptSilenceSet(string text, int start, int length)`

The gate for the silence command on the transcript's selection.
It marks the selection as standing for nothing.
It has its own gate, so a fresh pick's null id is never read as silence.
Conduct maps the silence to the engine's Entry 0 here, so the driver never sends it.

## `public void CTranscriptSenseSet(string text, int start, int length, long senseId)`

The gate for a Meaning picked in the sense menu, with the field's raw text and selection at the pick.
The Mention under the selection is found and narrowed below Conduct, and a selection in no Mention changes nothing.

## `public void CTranscriptMentionRemove(long mentionId)`

The gate for a chip's unlink, with the chip's own Mention id.

## `public void CTranscriptMentionRemove(string text, int start, int length)`

The gate for an unlink from the field, with its raw text and selection.
The Mention under the selection is found below Conduct, and a selection in no Mention changes nothing.

## `public bool CTranscriptMentionCheck(string text, int start, int length)`

Whether a Mention lies under the field's selection, so the unlink command may run.
It answers from the raw selection and never persists.

## `public bool CTranscriptSenseCheck(string text, int start, int length)`

Whether a Mention linked to an Entry lies under the field's selection, so the sense command may run.
It answers from the raw selection and never persists.

## `public CMentionSense? CTranscriptSenseRead(string text, int start, int length)`

The sense menu, ready to show, for the Entry the selection's Mention links to.
It answers null when no linked Mention lies there, so no menu opens.
A failed read shows `Mention.FindFailed`, through the shared read in `CMention`.

## `public IReadOnlyList<CMentionLabel> CTranscriptMentionRead()`

The chip line under the transcript, ready to paint, from the draft the transcript's desk holds.
A failed read shows `Mention.FindFailed` and answers no chips.
