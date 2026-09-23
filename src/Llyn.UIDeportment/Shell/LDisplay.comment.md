# LDisplay.cs

## `public sealed class LDisplay`

The reading view deportment.
It keeps the reflex fold toggle and answers whether the fetched sections are still pending.
Every other member forwards one read or one mark request to the ports, so the view never holds an engine.
The veneer still catches a refused read on its side and shows the section empty.
That is what it did over the engine.

## `private bool _lDisplayOpened;`

Whether the folded reflexes are shown, flipped by the fold toggle alone and read through its verdict.

## `public int LDisplayGraspStep => _lEntryPort.LEngineGraspStep;`

The last grasp step, which the display's star control takes as its limit.

## `public IReadOnlyList<string> LDisplayNameResolve(IReadOnlyList<string> labels)`

The compass labels made distinct, numbered in order where two sections share a name.

## `public string LDisplayGraspFormat(int step)`

The wording of a grasp step, or empty while no entry is shown.

## `public IReadOnlyList<LFanqieRow> LDisplayAnchorRead(long id)`

The fanqie rows of every block the engine divides, in order, for the anchor text of the reflex rows.
The view reads them afresh each time, so the shell holds no fanqie rows of its own.
A failed read gives no rows, and the reflex rows then show no anchors.

## `public string LDisplayReadingRead(long id, string headword)`

The headword's representative reading, formed by the engine from the blocks it divides.
The view prints it under the headword, so the shell holds no reading of its own.

## `public bool LDisplayFanqieCheck(long? id)`

Whether the entry's rime-book rows are still being fetched, false for no entry or when the engine refuses to say.

## `public bool LDisplayScriptCheck(long? id)`

Whether the entry's script images are still being fetched, false for no entry or when the engine refuses to say.

## `public IReadOnlyList<LTranslationTarget> LDisplayEtymonRead(LEntryDraft draft)`

The source links of an etymology, named by the engine.
A read the engine refuses answers no links, so the page still draws.

## `public static bool LDisplayNarrativeCheck(bool editable, string text)`

Whether the read face of the narrative stands: only on the read side, and only with words in it.

## `public static bool LDisplayEtymonCheck(bool editable, int count)`

Whether the row of source links shows: always while editable, else only with a link.

## `public static bool LDisplayEtymologyCheck(string text, int count)`

Whether a read etymology is worth a place at all: a narrative or a link.
