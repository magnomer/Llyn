# Test Naming Rules

This is the authoritative source for test method names in every suite under `tests/`.
Treat all applicable rules as requirements, not optional style guidance.

## 1. Applicability `(T-AP)`

**Scope `(T-APSC)`** These rules apply to every method carrying `[Fact]` or `[Theory]`.

- Helper methods, fixtures, and relay adapters are ordinary logic names and follow `RulesName.md` instead.
- A test file, its class, and its `.comment.md` follow `RulesName.md` under the `T` prefix.

**Prefix exemption `(T-APPX)`** A test method carries no prefix.

- `TAuditName` exempts test methods from the prefix rule only while no test method uses one.
- A single prefixed test method would therefore make every other test method a violation.
- Never begin a test method name with `T` followed by an uppercase letter, a digit, or an underscore.

## 2. Name Structure `(T-NS)`

**Three parts `(T-NSTP)`** Name a test method `MethodUnderTest_Scenario_ExpectedResult`.

- Separate the three parts with an underscore, and use an underscore nowhere else.
- Write each part in PascalCase.
- A name with two parts or four is a violation, not a shorthand.

**Method under test `(T-NSMU)`** The first part names the production operation the test drives.

- Take it from the operation itself with its `L` prefix dropped, so `LEngineDraftCommit` gives `DraftCommit`.
- Drop a leading layer word the suite already makes plain, so `LEngineDraftArchiveSave` gives `DraftArchiveSave`.
- Name the operation whose behaviour is asserted, not the one that merely arranges the workspace.
- A test that drives several operations names the one the assertions are about.

**Scenario `(T-NSSC)`** The second part names the condition the test sets up.

- Write a noun phrase, never a clause.
- Omit whatever the first part already said, so `DatabaseCreate` takes `VersionFifteen`, not `VersionFifteenDatabase`.
- Name the condition that distinguishes this test from its neighbours in the same file.

**Expected result `(T-NSER)`** The third part names the observable outcome.

- Open with a verb in the third person, such as `Returns`, `Keeps`, `Drops`, `Refuses`, `Throws`, or `Renumbers`.
- Name the outcome that distinguishes this test, not every assertion the body makes.
- A migration test asserting that rows survive still names only what the migration added, because every migration test keeps its rows.
- Use `Throws` for an exception and `Refuses` for a refusal the engine reports as a value.

## 3. Length `(T-LE)`

**Budget `(T-LEBU)`** Keep each test method name at or below 55 characters.

- Aim for two or three words per part.
- Count the underscores.

**Overflow resolution `(T-LERE)`** A name that will not fit the budget is reporting a test that does two things.

- Split the test, or narrow the expected result to the one outcome that distinguishes it.
- Do not abbreviate a word to fit, and do not drop a part.

## 4. Wording `(T-WO)`

**No articles `(T-WOAR)`** Omit `A`, `An`, and `The` unless the name is unknown without them.

**No pronouns `(T-WOPR)`** Omit `It`, `Its`, `Them`, `Their`, and `That` where the sentence still reads.

- `ReturnsThemInWrittenOrder` becomes `ReturnsWrittenOrder`.

**No quantifier padding `(T-WOQU)`** Prefer `All` to `Every` and drop the quantifier where the plural already carries it.

**Registered vocabulary `(T-WOVO)`** Use the object base the codebase registered for the thing under test.

- A part of speech is `Speech`, a meaning is `Meaning`, and a tentative link is `Court`.
- `ListObject.md` and `ListVerb.md` remain the authority.

## 5. Examples `(T-EX)`

| Rule at work | Name |
|---|---|
| Plain three parts | `EntrySave_NoHeadword_RefusesAndWritesNothing` |
| Scenario drops the repeated subject | `DatabaseCreate_VersionFifteen_AddsCustomSpeech` |
| Outcome names only what distinguishes | `DraftCommit_NamesDeletedEntry_StoresNewEntry` |
| Exception rather than refusal | `MarkupScan_UnclosedBlock_ThrowsNamingPosition` |
| Lifecycle walked in one test | `MeaningCreate_OneRowAtATime_ReadsBackEachStep` |

These are the shapes to copy.
A name that reads as a sentence with articles and pronouns is the shape to replace.
