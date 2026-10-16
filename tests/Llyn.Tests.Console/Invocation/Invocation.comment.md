# Invocation

Tests of strict mode: the grammar, the first verbs and the envoy's answers.

## `TInvocation`

Runs the whole console start through `BBootstrapIntroduce`, with a real workspace and the English catalog.
Each grammar error asserts its sentence, exit 2 and an empty stdout.
The build seam throws in those cases, so exit 2 also proves the workspace never opened.
The verbs assert their stdout lines and exit 0, and a refused move exits 1.

## `TInvocationRun`

Starts the bootstrap with the given arguments, building the atelier from the engine as the host does.
The bootstrap installs its loop as the thread's context, so the helper restores the previous context.
The busy seam answers busy, so a failed build words the busy refusal.

## `TEnvoyStrict`

Asks the envoy one question each, under `-yes`, `-no`, a terminal line or no terminal.
Each asserts the answer, what stderr shows and whether the run would exit 1.

## Stamp

Commit: `ed7d18d90760`
- `TEnvoyStrict.cs` `47bb32b66d48b5c7`
- `TInvocation.cs` `a07485b1adc23eeb`
