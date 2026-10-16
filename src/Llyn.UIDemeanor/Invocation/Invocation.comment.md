# Invocation

Strict mode: one action from the command line, printed, ending with an exit code.
It never draws a screen and never reads keys, only an answer line on a terminal.
The folder holds the parsed command line, its global options and the envoy that answers Conduct's questions.

## `BInvocation`

One parsed command line, its options kept in the order typed.
The bootstrap parses it before the workspace opens and plays it on the loop once the atelier exists.

## `_bInvocationOptions`

Each option's name without its dash, with its value or null when no colon was typed.
The typed order is the order the run plays the options.

## `BInvocationGlobal`

The global options a question or a print reads, held apart from the verb.
It is a property named after its type, as the naming rule asks.

## `BInvocationParse`

Reads the arguments by the grammar of plan `08` section 2, and every failure is a usage error.
An option is a dash, a lowercase ASCII name starting with a letter, and an optional colon and value.
The value is everything after the first colon, so `-workspace:C:\Notes` keeps its drive colon.
A bare word, a double dash, a slash, an equals sign or a capital letter is not an option.
An abbreviation is an unknown option, since no name is matched by its prefix.
An empty value counts as missing, since no option here takes empty text.
`-color` and `-format` are checked against their words and not stored, since no verb reads them yet.
`-yes` with `-no` is a usage error, since no answer could honour both.
A missing verb reuses `Invocation.Usage`, the line every argument printed before strict mode existed.
The first failure in typed order wins, then the yes and no clash, then the missing verb.
The error speaks the default catalog, since the stored language is unknown before the workspace opens.
The caller prints the returned sentence and exits 2, so a typo never opens or migrates a database.

## `BInvocationRun`

Plays the options in typed order, one handler per option that has work of its own.
Global options have no handler, since the envoy and the printers read them where a verb needs them.
It returns 1 when the envoy recorded a refusal, else 0.

## `BInvocationValueRead`

The value typed for one option, or null when that option is absent.
The envoy reads the `-workspace` value through it.

## `BInvocationHelpPrint`

Prints the usage line and one line per option on stdout, or one verb's line for `-help:verb`.
Help reads no gate, so the run hands it the option directly, as a look-only event goes to a refine.
Option names are printed literally, since they are a data contract and never translated.
Names are ASCII, so padding by characters aligns the column by cells.

## `BInvocationVersionObserve`

Calls `CAtelierAboutRead`, the gate the GUI's about box calls, and hands its key on.

## `BInvocationVersionPrint`

Prints the product, the about text and the build on one stdout line.
The build is the assembly version to three parts, as the GUI's about box shows it.

## `BInvocationWorkspaceObserve`

Without a value it reads `CAtelierPathRead`, with a value it moves through `CWorkspaceChange`, as the settings page does.
A refused move shows its failure through the envoy and returns the unchanged path.
A folder the engine does not accept returns the unchanged path with no failure, as in the GUI.

## `BInvocationWorkspacePrint`

Prints the workspace path the gate returned on stdout.

## `BEnvoyStrict`

Implements `CEnvoy` for strict mode, answering every question without a screen.
Conduct passes each key, and the envoy only looks up its text, so no question is worded here.
A question with no answering option and no terminal is answered no, so a script never hangs.

## `BEnvoyStrictRefusal`

Set when a failure was shown or a question was refused, so the run exits 1.
A question that strict mode cannot answer yet also refuses, so a skipped action never reports success.

## `CEnvoyConfirm`

A yes or no question, answered by `BEnvoyStrictConsult`.
The tally form joins the question, the tally text and the number on one line.

## `CEnvoyUnionConfirm`

The merge question, with the dropped and the kept name joined by an arrow, as the GUI shows them.

## `CEnvoyFailureShow`

Prints the sentence after `llyn: ` on stderr and records the refusal.
The notice form adds the notice sentence, and the label and path when both exist.
Failures print under `-quiet` too, since quiet keeps stderr to failures.

## `CEnvoyLeaveConfirm`

`-yes` saves and `-no` discards the unsaved work.
With neither, the workspace stays, which refuses.

## `CEnvoyOmissionShow`

One stderr line per omission, line number then text, unless `-quiet` is given.

## `CEnvoyWorkspaceRead`

The `-workspace` value, or a refusal when none was typed.

## `CEnvoyMarkupRead`

Refuses until the `-import` verb lands.
The same holds for the customs, print, file, folder, coinage and media questions.
Each waits for the verb whose option answers it, as plan `08` section 5 lists.

## `BEnvoyStrictConsult`

`-yes` or `-no` answers first.
Otherwise a terminal gets the question and `[y/N]` on stderr, and one line read from stdin.
Only `y` or `yes`, in any case and trimmed, answers yes.
Without a terminal the answer is no, unasked.
The `[y/N]` mark is a fixed affordance like `llyn: `, so no catalog carries it.

## Stamp

Commit: `ed7d18d90760`
- `BEnvoyStrict.cs` `14232a0a5a7bbef8`
- `BInvocation.cs` `2477fb526127beaf`
- `BInvocationGlobal.cs` `af318fc2fd36320e`
