# Invocation

Tests of strict mode.
They cover the grammar, the verbs and the envoy's answers.

## `TInvocation`

Runs the whole console start through `BBootstrapIntroduce`, with a real workspace and the English catalog.
A test that names a language loads that catalog instead.
The helper also stores that language in the engine, so the run reads the same language the catalog speaks.
The whole run executes through `TConsoleFake.TConsoleFakeRun`, which gives the loop a thread of its own and a time limit.
The synchronization context is restored on that thread after the run.
Each grammar error asserts its sentence, exit 2 and an empty stdout.
The help listing counts the `-show` line, so every later line moves down by one.
The grammar cases include the write and export verbs, their qualifiers, the id forms and the missing `-to`.
The build seam throws in those cases, so exit 2 also proves the workspace never opened.
The verbs assert their stdout lines and exit 0, and a refused move exits 1.
The `-find` tests seed entries through the engine and dispose it before the run opens the workspace.
The text test sorts the lines first, since the headword order follows the current culture.
Its CJK headword is shorter in characters but wider in cells, so padding by characters would fail.
The tab and JSON tests assert the invariant culture fields, and a query with no match prints nothing.
The layout test runs a verb through the bootstrap and asserts the posture file stays untouched.
The leased find test holds the workspace lease in another engine, and `-find` still prints, since it takes no lease.

## `TInvocationWrite`

Covers `-new` and `-delete` through the whole console start, with a real workspace.
`-new` prints the saved id alone, and a find then lists the entry.
`-new` with no headword exits 1 with the headword refusal.
`-new` with only a language exits 1 with the empty sentence.
`-delete` with `-yes`, or with a terminal `y`, removes the entry.
A `-no`, or no answer without a terminal, keeps the entry and exits 1 with nothing on stderr.
A missing id exits 1 with the missing-entry sentence and keeps the other entries.
Each leased option exits 3 with the busy sentence when another engine holds the lease.
  Those are `-new`, `-delete`, `-import` and `-workspace` with a value.
The entry stays in that case, as a find confirms.

## `TInvocationLectern`

Covers `-show` through the whole console start, with a real workspace.
`-show` prints the entry's reading view on stdout, from the headword through the meanings to the note.
It exits 0 with nothing on stderr.
Every printed line is free of trailing blanks, so a script compares lines exactly.
`-help:show` prints the one line for the verb.
A missing id exits 1 with the missing-entry sentence and prints nothing on stdout.

## `TInvocationPortrait`

Covers `-import` and `-export` through the whole console start, with a real workspace.
`-import` with `-yes` lists each entry on stderr, prints the count and appends to the entry of that name.
The list shows a merge target when an entry of that name exists, and the omission follows.
A terminal `y` shows the row, then the question on stderr, and imports.
A `-no`, or no answer without a terminal, imports nothing and exits 1 after showing the row.
`-export` with `-to` and `-as` prints the path and writes the entry.
An existing file is replaced only under `-yes`, and `-no` keeps it and exits 1.
A missing id exits 1 with the missing-entry sentence and writes no file.

## `TInvocationRun`

Starts the bootstrap with the given arguments, building the atelier from the engine and the detached flag the bootstrap passes.
The build seam gets the lease flag.
A further overload takes a language, and the bootstrap then loads that catalog in place of English.
The last overload also takes the theme seam, and the others build it over the real `LThemeLoader`.
The bootstrap is built over the real `ABootstrap` in every overload.
The default helper builds a leased engine over the workspace rig when it is set.
The presence seam is `File.Exists`.
A test may replace the atelier seam to watch the detached flag.
The bootstrap installs its loop as the thread's context, so the helper restores the previous context.
The busy seam answers busy, so a failed build words the busy refusal.

## `TEnvoyStrict`

Asks the envoy one question each, under `-yes`, `-no`, a terminal line or no terminal.
The envoy gets a presence seam that finds no file.
Each asserts the answer, what stderr shows and whether the run would exit 1.
The leave question gets no answer and refuses the run.
A failure prints its sentence even under `-quiet`, and an omission then prints nothing.

## Stamp

Commit: `9377e657b395`
- `TEnvoyStrict.cs` `5fa5ccbee034a359`
- `TInvocation.cs` `a4886a3e424979be`
- `TInvocationLectern.cs` `785dafc086674fba`
- `TInvocationPortrait.cs` `e92be20c3c887d24`
- `TInvocationWrite.cs` `2eb9347f889f8f81`
