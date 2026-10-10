# TInterfaceInflection.cs
Hash: `2917b71e5a0d84ca`
Hash: `7ce4ad409c79448e`

## `internal static class TInterfaceInflection`

The relays that build the inflection records of an entry.
That is the forms, the inflections, the paradigms, the features and the morphology values.
It also builds the view tables, lines, forms and marks, and a rule book with its stems and endings.
The paradigm rule resolve and the mark text are relayed here too.
So are the book's prediction and division, and the difference scan and division.
Each relay is transparent and carries no test logic of its own.

## `internal static LForm TFormCreate(long entryId, int position, string text, string? local, string role)`

Tests reach the form record through this relay rather than its constructor.

## `internal static LInflection TInflectionCreate(long entryId, int position, string text, string? local, long? speechValueId, IReadOnlyList<long> morphology)`

A constructed row is unstored and unanalysed, leaving production code to assign identity and derive regularity.

## `internal static LParadigm TParadigmCreate(long speechCode, IReadOnlyList<IReadOnlyList<long>> cells, IReadOnlyList<LParadigmRule>? regular = null, IReadOnlyList<long>? except = null)`

A paradigm without stated rules or exceptions is the common fixture, so both default to null.

## `internal static LParadigmTable TParadigmTableCreate(IReadOnlyList<string> headers, IReadOnlyList<LParadigmLine> lines)`

Tests build an expected view table through this relay rather than its constructor.

## `internal static LParadigmView TParadigmViewCreate(LParadigmTable collapsed, LParadigmTable expanded)`

Tests build a view of two tables through this relay rather than its constructor.

## `internal static LParadigmLine TParadigmLineCreate(string group, string label, IReadOnlyList<LParadigmForm> forms)`

Tests build an expected view line through this relay rather than its constructor.

## `internal static LParadigmForm TParadigmFormCreate(string text, IReadOnlyList<LInflectionMark> marks, string? tip, int split = 0)`

A test passes the ready text and tip a cell shows, as the Core view build answers them.

The split defaults to 0, so a cell built without one stands for an undivided form.
A test of a divided form passes where its ending starts.

## `internal static LInflectionMark TInflectionMarkCreate(int offset, int length)`

Expected marks are built here, so assertions compare against production records.

## `internal static string TInflectionMarkFormat(IReadOnlyList<LInflectionMark> marks)`

The stored mark text comes from the production writer rather than a test-side format.

## `internal static IReadOnlyList<LInflectionMark> TInflectionMarkParse(string text)`

The marks read back come from the production reader rather than a test-side parse.

## `internal static IReadOnlyList<LInflectionMark> TInflectionDifferenceScan(IReadOnlyList<LInflectionRule> folds, string predicted, string actual)`

Difference ranges come from the production scanner rather than a test-side comparison.

## `internal static int TInflectionDifferenceDivide(IReadOnlyList<LInflectionRule> folds, string predicted, int root, string actual)`

The ending start comes from the production alignment rather than a test-side walk.

## `internal static string? TInflectionBookResolve(LInflectionBook book, string headword, IReadOnlyList<long> codes)`

The predicted text comes from the production pipeline rather than a test-side rewrite.

## `internal static string? TInflectionBookDivide(LInflectionBook book, string headword, IReadOnlyList<long> codes, out int? root)`

Prediction and root come from the production pipeline in one call.

## `internal static LInflectionBook TInflectionBookCreate(IReadOnlyList<LInflectionKind> kinds, IReadOnlyList<LInflectionStem> stems, IReadOnlyList<LInflectionRule> rules, IReadOnlyList<LInflectionRule> folds, LInflectionLayout? layout, string stamp)`

Tests control the layout and stamp independently, allowing analysis identity and display shape to be exercised separately.

## `internal static LInflectionStem TInflectionStemCreate(IReadOnlyList<long> values, IReadOnlyDictionary<string, string> templates, IReadOnlyList<LInflectionEnding> endings)`

Tests build a book's stem rows in code through this relay rather than a pack file.

## `internal static LInflectionEnding TInflectionEndingCreate(IReadOnlyList<long> values, string text)`

Tests build a stem's endings in code through this relay rather than a pack file.

## `internal static string? TParadigmRuleResolve(LParadigmRule rule, string headword)`

The fallback prediction comes from the production rule rather than a test-side rewrite.

## `internal static LFeature TFeatureCreate(long speechValueId, long packId, string name, int position)`

A constructed feature is unstored, so production code assigns its identity.

## `internal static LMorphology TMorphologyCreate(long featureId, long packId, string name, int position)`

A constructed morphology value is unstored, so production code assigns its identity.
