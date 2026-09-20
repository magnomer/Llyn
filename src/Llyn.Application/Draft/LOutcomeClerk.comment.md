# LOutcomeClerk.cs

## `public sealed class LOutcomeClerk`

The commit round of an entry draft, producing the outcome the tenure reports.
It composes the claim, court and entry clerks, which is why it is not a part of any of them.
The single entry save and update the engine offers live here too, since the round is made of them.

## `public LOutcomeClerk(LRig rig, LLanguageCache languages, LDraftClerk drafts, LClaimClerk claims, LCourtClerk courts, LEntryClerk entries, LLacunaClerk lacunae, LFrequencyClerk frequencies)`

Reads the vault out of `rig` and keeps the clerks the round writes through.

## `public LEntry LOutcomeEntrySave(LEntryDraft draft, Dictionary<long, long> identity)`

A fresh entry from a draft, its ids minted, its respellings filled and its headword trimmed.
The frequency fetch starts once the entry stands.

## `public LEntry LOutcomeEntryUpdate(long id, LEntryDraft draft, Dictionary<long, long> identity)`

An entry rewritten from a draft, its pending inflection fetch cancelled first.
A headword or language that changed starts the frequency fetch again.

## `public LOutcome LOutcomeClerkCommit(long id, List<long> raised)`

The round over one draft and every draft it links, in one session.
The court is applied once the round settled.
Then the queued draft and court writes run and the held drafts finish.
`raised` receives every entry the round wrote, for the bulletins.

## `private LOutcome LOutcomeClerkCommit(long id, bool held, Dictionary<long, LDraft> loaded, Dictionary<long, LOutcome> settled, List<LCourt> deferred, List<long> finished, List<Action> written)`

One frame of the round.
A linked draft already loaded but not settled is deferred to the court.
A linked draft not yet loaded is committed first, so its entry id can settle the link.
The draft is saved and its court settled after the session, so a failed round leaves both untouched.
