# CPlaywright.cs
Hash: `51aeb1220e132b2c`

## `public sealed class CPlaywright`

The editor of the Situation scenario, as `CEditor` is the entry side's editor.
It owns the scenario desk, the scenario's typed edits, its picture and video rows and its scenario notices.
The repertoire keeps the panel's two sides and the moves between them, and reaches the desk through it.
It reports failures through the envoy, never through a seam a driver hands up.

## `internal CPlaywright(CAtelier atelier, CEnvoy envoy, Action<Action> marshal)`

Builds the desk under the `Situation` scope.
The desk starts by subject rather than by a vista, with the `Repertoire` origin.
It attaches the desk's observers through the marshal, so draft notices reach drivers on their own thread.
It keeps no port, and reads the atelier's ports on each use.

## `public event Action<CScenario>? CPlaywrightScenarioChanged`

Raised with the scenario of the Situation the desk holds after a start.
A cancel or a refused start raises the blank scenario, so the driver keeps no default of its own.

## `public event Action<CScenario>? CPlaywrightDraftChanged`

Raised with the held Situation's scenario after an edit, through the marshal the playwright was built with.
`CPlaywrightScenarioChanged` fills the scenario anew, while this one only refreshes what the edit changed.

## `internal CDesk LPlaywrightDesk { get; }`

The scenario desk, which the repertoire's atlas and session run on.
The repertoire cancels it whenever the occurrence side takes the front or the scenario scribe closes.

## `public CImage CPlaywrightImage`

The scenario's picture gates, built fresh over the playwright's desk as the editor builds its own.

## `public CVideo CPlaywrightVideo`

The scenario's video gates, built fresh over the playwright's desk.

## `private LQuillSituation? LPlaywrightQuill`

The scenario's typed edits over the desk's live tenure, or null while no tenure runs or the desk fills.

## `internal CSituationDraft? LPlaywrightScenarioRead()`

Reads the Situation the desk holds, which persists the tenure first and so can throw.
A throw reports `Situation.HoldFailed` through the envoy and reads as null.
`CPlaywrightDraftChanged` hands it on each draft bulletin, so the desk never hands a driver a draft.
It stays internal because the test relays read the held Situation through it.

## `internal void LPlaywrightHeldResonate()`

Answers the session's held notice by raising `CPlaywrightScenarioChanged`.
A failed or empty read raises the blank scenario, so the driver always repaints.

## `private void LPlaywrightDraftResonate()`

Answers the desk's draft notice with the held Situation read afresh.
A notice with no Situation to show raises nothing, so the driver never redraws from nothing.

## `public CScenarioLine CPlaywrightTitleSet(string text)`

Hands the typed title to the scenario's own edit on the tenure, which defers it.
It answers the typed line, whose hint is the untitled text, since typing ends an unknown mark.

## `public CScenarioLine CPlaywrightKindSet(string text)`

Hands the typed kind on as the title gate does, and answers the kind's typed line.

## `public CScenarioLine CPlaywrightDescriptionSet(string text)`

Hands the typed description on as the title gate does, and answers the description's typed line.
