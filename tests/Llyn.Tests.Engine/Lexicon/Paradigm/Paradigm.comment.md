# Paradigm

This folder holds the tests of paradigms and inflections.
They cover the rule book, differences, cells and pieces.

## `TInflectionBook`

Covers the inflection rule engine on a book built in code from the design's Spanish side file.
The engine itself names no language, so the test hands it the Spanish data as the pack file does.
Mark assertions compare predictions with actual forms through the book's folds, mirroring stored-analysis computation.

Each regular row is one tense of recibir or hablar across its six person columns.
Every prediction matches the design table, and every regular form gets no marks.
A dash column has no ending, so that cell is uncovered.
The hand checks carry their marks written as stored text.
Tener marks its irregular stem, construir its inserted y, and seguir its changed vowel.
Spelling alternations such as c to qu, z to c and g to j fold away unmarked.

A headword no kind matches leaves every cell uncovered.
The first singular imperative has a null ending, so it is uncovered.
A partial cell is uncovered, while an out-of-order complete imperative cell remains covered.
The vowel raising rule names the ir kind, so comer keeps its e and vivir raises it.

Each design template marks its root, so the root survives the final marker rule.
A template without the marker still predicts its text.
Its root is null, so a view keeps the stored marks for such a pack.

### `TInflectionBookScan`

Predicts the cell once and compares the actual form with the prediction through the book's folds.
Null means the cell is uncovered, and an empty list means the form is regular.
The loader facts share it, so every mark assertion reads the same two steps.

### `TInflectionBookCreate`

Builds the design's book, zipping each stem's endings with the six columns and skipping null endings.
Prediction tests omit layout because prediction and difference scanning do not consume it.
The non-ASCII kind identifier verifies that class names remain opaque keys.

## `TInflectionDifference`

Covers the comparison of a predicted form with the actual one, and the stored text of marks.
The folds come from the design's book in `TInflectionBook`.
The qu of bosque folds to one c, so an unmatched c marks both original letters.
Case, accents and the dieresis fold away, so they never count as irregular.
Two inserted letters touch, so they merge into one range over the accented form.
The predicted forms of tener keep the root ten, given by hand.
The inserted g of tengo and the inserted i of tienes fall into the root.
A regular tenemos keeps the predicted split.
Marks are written as offset and length pairs, and no marks write the empty string.
Parsing the written text gives back the same marks.
Non-numbers, negatives, missing or extra parts and empty pieces are skipped, and good pairs survive.

## `TParadigm`

Covers what a language pack declares about inflection.
That is which part specialises which, and which forms a part takes.
The loader tests write a pack of their own into a temporary language folder and load it.
So the vocabulary loader is read as it reads a real pack, and nothing shipped is touched.
The other tests read the shipped packs or build rows in memory.

A part naming its parent carries that code, and one naming none or naming it badly carries `0`.
A paradigm keeps its values in the order the pack lists them, because that order is the display order.
A regular-form rule keeps its pattern as written.
A rule with a bad pattern or shape is skipped, and the paradigm is kept.
A row whose part or any value is not a positive integer is dropped, and its neighbours are kept.
A malformed regular or except entry is ignored and the row is kept.

The shipped English pack states paradigms over bare roles.
So the loader is read against disk and not a fixture alone.
No part names a parent, and the noun paradigm declares no exceptions.
The shipped Latin and Greek packs each declare three paradigms.
The noun asks for the genitive alone with no exceptions, and the verb declares the expected principal-part count.
The shipped Spanish verb paradigm declares 64 cells, each a distinct set of values.
Every value a paradigm or cell names is a morphology value the same pack declares.
So each Spanish cell resolves to a slot and has a key no other cell shares.

Slots sharing one form fold into one row, and the first row of each part of speech leads that part.
An uncertain first slot marks its row, and no slots give no rows.
The table prints a part name once per part, so a lost or doubled lead would drop or repeat it.

The status of a slot follows a fixed precedence.
A written form answers text and an empty one answers absent, whatever the pending and morphology verdicts say.
An unknown slot without a form answers unknown before the pending and morphology verdicts are weighed.
An unanswered slot answers pending while the fetch runs, then lost when morphology is on, else absent.

### `TParadigmPackLoad`

Writes one vocabulary file under the application's language folder and removes it after the load.

### `TParadigmStatusRead`

Builds a one-slot row and asks its first slot for its status through the `TParadigmSlotCheck` relay.

## `TParadigmCell`

Covers paradigm cells, the forms a pack names by several values at once.
The loader facts write a vocabulary into an unlisted fixture language, so no other test sees it.
The slot facts build rows in memory.

A row's values come first as one-value cells, then its stated cells in pack order.
A row may state cells alone, and it then has no one-value cell.
A values-only row retains legacy one-cell-per-value loading without requiring explicit cells.
A cell holding a non-number, a non-positive code or nothing at all drops its row.
So does a cell list that is not a list of lists, even beside valid values.
The well-formed neighbour is kept.

The slot key sorts the codes, so the -ra value listed third still sorts last.
The slot name keeps the pack order, and the row label shows it whole.
A one-value slot keys on its bare code and names itself by its morphology.
Equal-text slots still combine morphology names with comma separation.

### `TParadigmCellCreate`

Builds a Spanish first singular imperfect subjunctive slot of the -ra form, its values listed out of code order.

## `TParadigmPiece`

Covers how Core cuts a paradigm cell's text into the pieces every reader paints.
The app box and the Livery paint these pieces, so their own tests feed pieces and pin only the painting.
A form marked at offset 1 for 2 letters cuts into three pieces around the mark.
Only the middle piece is marked, and no piece carries a cut.
A form split at 4 with its root marked cuts into the marked root and the plain ending.
Only the root carries the cut, so a reader draws one hyphen between them.

## Stamp

Commit: `c2f335c331fe`
- `TInflectionBook.cs` `c7bde04a10d9255d`
- `TInflectionDifference.cs` `fb24deb66540ca5e`
- `TParadigm.cs` `bc4fc56ee1db2d47`
- `TParadigmCell.cs` `9448ea67ee03840b`
- `TParadigmPiece.cs` `1fe5571e95a62e29`
