# ScanTest.ps1
Hash: `35250015e7980ffc`

Audits rule, and scans only suspect.
An audit hit is a violation that a ceiling holds or that fails the run.
A scan hit is a lead that a reader judges.
A test the scan reports has broken no rule.
That holds for the letter of the rule and for its spirit.
That is why the scans form a family of their own and not part of the audits.
The pages show the same split.
They keep the audits' counters, kinds and lines.
They have no ceiling, no ledger and no failing status.

The kinds do not weigh the same.
A Blind Test and a Dead Target come closest to a defect.
Such a test protects nothing, because it passes whatever its target does or it guards behaviour that is gone.
They still break no rule, but they earn the closest look.

Every finding is a candidate.
At the end of a wave the supervisor reads the scan.
The supervisor sends each true finding to a Janitor, or to a Surgeon when it needs a real fix.
Nothing gates on a count.

## Kinds

The twelve kinds are written in a fixed order.
The detail tag of each line names the sub-rule, so a reader can filter without a new kind.
The helper reads syntax only, without binding, so every rule works on names and shapes.

- Dead Target flags a test whose subject no longer exists.
  The test name up to its first underscore must end some declared name.
  A one-letter prefix on the declared name is dropped first.
  The names come from every declaration in the source and every non-test declaration in the tests.
  Only a name with an underscore is checked.
  A name is loose and not dead when the body uses a related member, so it is left out.
  The member is one the part ends with, or one that holds the part.
  Detail `renamed` means a removed line of the source history holds the part behind a one or two letter prefix.
  Detail `unmatched` means no such removed name exists, and this is often naming drift and not a dead target.
  Detail `literal` means a dotted string in an assert occurs in none of the pool folders.
  The pool is the text files of the configured pool folders.
  A key that only a test names is therefore flagged.

- Skipped Test flags a test that does not run.
  Detail `skip` is a `Skip` argument on a test attribute.
  Detail `commented` is a line that holds a commented-out `[Fact` or `[Theory`.
  Detail `not-executed` is a trx result with the outcome `NotExecuted`.

- Slow Test lists the tests whose trx duration reaches `testSeconds`.
  Only the newest run of each project counts, by the earliest start time in the file.
  The line shows the duration, the setup estimate and the body.
  The setup estimate is the fastest test of the class, since the trx cannot split setup from body.
  The kind also writes a table by class with the count, sum, span, gap, floor and body.
  The gap is the span minus the sum, and a class is marked slow when its sum reaches `classSeconds`.
  The lines are sorted by duration and the class table by sum, both descending.
  A cold first test in a class carries start-up cost, and the floor understates setup when every test is heavy.

- Flaky Result flags a test that passed in one run and failed in another run of the same tree.
  The trx file name carries the project and the git tree.
  Only runs of one project and one tree are compared.
  A file whose name does not follow that form never joins another run.
  Detail `flip` lists the outcomes in start-time order.
  A change of code between two runs would show as a flip.
  That is why the tree is part of the name.

- Blind Test flags a test that may pass while its target is broken.
  Every detail is a static proxy, since one passing run cannot tell a correct target from a blind test.
  Detail `tautology` is `Assert.True(true)` or `Assert.False(false)`.
  Detail `self-compare` is `Equal`, `Same` or `StrictEqual` with the same text on both sides.
  Detail `notnull-new` is `NotNull` of a freshly created object.
  Detail `broad-throws` is `Throws`, `ThrowsAny` or their async forms with a plain `Exception`, which any fault satisfies.
  Detail `guarded-asserts` means every assert sits under an `if`, a conditional or a switch section.
  Detail `loop-only-asserts` means every assert sits in a loop and no assert outside a loop pins the count.
  It also needs one loop that reads from production.
  An assert of kind `Single`, `NotEmpty`, `Equal`, `Collection` or `Contains` outside a loop pins the count.
  A loop over a literal count or a collection literal is no production read.
  Nor is a loop over `Enum.GetValues` or over `Enumerable.Range` of literals.
  Detail `swallowing-catch` is a catch that neither rethrows, asserts nor reads its captured exception.
  Detail `early-return` is a return inside an `if` that holds no assert, with an assert after it.
  Detail `theory-no-data` is a test whose attribute ends with `theoryMarker` and none ends with `dataSuffix`.
  A broad throw may be deliberate where the exception type depends on the operating system.
  An early return that escapes a missing tool leaves the test passing silently, and that is the real catch.

- No Assert flags a test with no assert, direct or through a helper.
  A helper asserts when its body calls `Assert`, or calls a helper that does, found by name until nothing changes.
  Detail `helper-throws` marks a test whose only check is a helper that throws.
  Detail `none` marks the rest.
  Two helpers that share a name can hide a hit.

- Self Echo flags a test whose asserts only compare what the test set itself.
  Every argument of every assert must be a literal.
  A local bound once to a literal that only asserts read counts as a literal.
  The test must have direct asserts and no asserting helper, and some argument must be a local.
  A value written through a fake and read back is not caught, since the helper does no binding.

- Wrong Layer flags a test that boots the interface to check one logic fact.
  A booting helper constructs one of the configured `bootTypes`, directly or through another helper of the same project.
  Only the configured projects are checked.
  The test must call a booting helper and assert at least one name that matches `logicPrefix`.
  The test is left out when its body uses a name that matches `uiPrefixes`.
  It is also left out when it uses a name that matches `drawPattern`.
  The configured `drawPattern` decides which names count as drawing code.
  The asserted names are those that match `assertedPattern`.
  A call of a lambda parameter leaves it out too.
  Detail `hops-n` gives the number of distinct logic names asserted.
  The layer letters come from the config, in `logicPrefix`, `uiPrefixes` and `assertedPattern`.

- Fixed Sleep flags a wait with a fixed length.
  Detail `sleep` is `Thread.Sleep`.
  Detail `delay` is `Task.Delay` outside a `WhenAny` call.
  Comment lines are skipped.
  Some sleeps separate two timestamps on purpose, but they are still listed.

- Long Wait flags a wait that can take long or that a guard bounds.
  Detail `timeout` is `WaitAsync`, `CancelAfter`, a timed `CancellationTokenSource` or a `Timeout =` setting.
  Detail `spin` is any `SpinWait`.
  Detail `blocking-wait` is `Wait`, `WaitOne` or `Join` with a time argument.
  Detail `whenany-delay` is a `Task.Delay` inside `WhenAny`, so it counts here and not as a sleep.
  Detail `polling` is the innermost loop that holds a sleep, delay, yield, spin, wait, dispatcher or pump call.
  Detail `deadline` is a `DateTime.Now` or `UtcNow` line, and it counts only in a method that has another wait line.
  When a trx run is read, a line may gain a note.
  The note appears if its test took at least as long as the guard's literal time.
  The note reads the time from a `From...` call or from a millisecond argument.
  Whether a wait is long is the reader's call.

- Copied Body flags a test body that repeats an earlier one.
  Bodies under `minTokens` tokens are skipped.
  Detail `exact` means the token stream is identical.
  Detail `same-shape` means only string, number and char literals differ, and a pair already exact is not listed twice.
  The first test in source order is the original, and each later copy names it in the message.
  Many same-shape pairs are candidates for one theory, and that is a finding and not an error.

- Unused Helper flags a helper that nothing uses.
  It covers non-test methods, properties, fields and types in the tests.
  The identifier must occur once across all test sources and nowhere as a word in test markup or the pool.
  Overrides, explicit interface members, `Dispose`, `DisposeAsync`, `InitializeAsync` and `Main` are exempt.
  A type that holds a test or an extension method is exempt.
  Matching is by name, so an unused overload whose name another member shares is missed.

## Configuration

`ScanTest.json` holds every project-specific value, and the script names none.
Every key is checked at start.
A missing, malformed or unknown key stops the run with a list of the problems.
The configuration generation must equal the script's.

- `tests` and `source` name the test and source folders.
  `pool` lists the folders whose text files feed Dead Target literals and Unused Helper.
- `markers` lists the test attribute names, with or without the `Attribute` suffix.
- `trx` names the folder read when no `-Trx` is given.
  Only the `.trx` files directly in it are read, never those in a subfolder.
  It is the folder that `Test.ps1 -KeepTrx` writes to.
- `top` is the number of console lines per kind, and `-Top` overrides it.
- `report` gives the page folder, the version file and key, and the file-name prefix.
  The version falls back to `0.0.0` with a console line when the file is missing or unreadable.
- `helper.framework` is the target framework of the embedded scanner.
- `kinds` must hold an object for each of the twelve kinds.
  `DeadTarget` takes `prefixPattern`, `dottedPattern`, `fileEndingPattern`, `poolExtensions` and `looseCapitals`.
  `SkippedTest` takes `commentedPattern`.
  `SlowTest` takes `testSeconds` and `classSeconds`.
  `BlindTest` takes `broadThrows`, `countPins`, `theoryMarker` and `dataSuffix`.
  `WrongLayer` takes `bootTypes`, `projects`, `uiPrefixes`, `logicPrefix`, `assertedPattern` and `drawPattern`.
  `FixedSleep` takes `sleepPattern` and `delayPattern`.
  `LongWait` takes `timeoutPattern`, `spinPattern`, `blockingPattern`, `pollingPattern`, `clockPattern` and `guardPattern`.
  `CopiedBody` takes `minTokens`.
  `UnusedHelper` takes `lifetimeNames`.
  The other kinds take no setting.
  A key missing from the configuration stops the run with its full name.
  Patterns are matched with the invariant culture and word lists by ordinal comparison.

The scanner is a small C# program embedded in the script.
It is built once and cached in the temporary folder.
The cache key is a hash of its text, the framework and the SDK version.
A build for a new hash removes the older cache folders first.
Git lists the test files, tracked or not, so a new test is scanned before it is added.
`bin` and `obj` folders are skipped.
A syntax error is counted as a parse error and does not stop the scan.
A history read or a trx file that fails becomes a notice, and the scan goes on.

## Outputs

The script writes `<prefix><version>.md`, `.json` and `.html` into the report folder.
The `.html` is the template `ScanTest.html` with the same JSON inside, and a missing marker stops the run.
The console shows one `[n/12]` line per kind, then the counters.
Then it shows the first lines of each kind that has hits.
Output pauses at each page unless `-NoPause` is given or the output is redirected.
`Report.ps1` relays the `[n/m]` lines.

The Markdown page holds the header facts, the counters and a hits table by kind with a total.
It then holds one section per kind.
The header names the trx files read and whether the history was read.
Each line reads file, line, name, detail and message.
A line with no source place shows its trx file name and line 0.
A trx result that matches no test is such a line.
The Slow Test section ends with its table by class.
`-Open` opens the Markdown page as well, and the HTML page opens unless `-NoOpen` is given.

The exit code is 0 whenever the scan completes.
It is non-zero only when the run itself breaks.
`ScanTest` never runs tests.

## Parity

The page must come out the same in Windows PowerShell 5.1 and in PowerShell 7, apart from its `generated` stamp.
Every sort is ordinal, since `Sort-Object` compares by culture and `Group-Object` order is not fixed.
The trx file list is made unique and sorted ordinally by hand.
The helper reads the trx files and writes the scan document itself, so both shells share one code path.
Dates and numbers use the invariant culture.
The page JSON is written by hand in ASCII, with `<`, `>`, `&` and every other non-ASCII character escaped.
That also keeps the JSON from closing the script element that holds it.
Files are written as UTF-8 without a byte order mark and with LF line ends.
