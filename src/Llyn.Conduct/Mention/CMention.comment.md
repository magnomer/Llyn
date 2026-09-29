# CMention.cs

## `public sealed class CMention`

The mention gates: how a text divides into Mention pieces, and how its chips are named.
It also converts between code points and UTF-16 units for a driver's caret and selection.
It stands on the atelier's ports, and `CAtelier` builds the one instance every driver shares.
It holds no state of its own.
Its click gate opens what a found word names through the atelier's navigation.

## `internal CMention(CAtelier atelier)`

Only the atelier builds it, so each session has one.

## `public event Action<long>? CMentionSenseChosen;`

Raised with a stored Mention's sense once its entry has opened, so the driver brings that sense card into view.

## `public IReadOnlyList<CTranslationTarget> CMentionResultOpen(CMentionResult result)`

Decides what a click on a text opens next.
A stored Mention opens its linked entry through the navigation's entry open, and a link to nothing opens nothing.
Its sense is raised only when the open went through, so a declined leave spotlights nothing.
A single matching entry opens at once.
Several matching entries open nothing and come back as the candidates the driver offers in a menu.
Every other answer is empty, so the driver's menu shows nothing.

## `public IReadOnlyList<CMentionLabel> CMentionResolve(`

The chips of a mention line, each named by its linked headword.
An unlinked Mention takes `silent`, the label the driver looked up for a word standing for nothing.

## `public IReadOnlyList<CMentionPiece> CMentionDivide(string text, IReadOnlyList<CMentionMark> mentions)`

The whole text as pieces, so a driver draws every character and no gap splits into words.
A linked piece answers true, a silent piece false, and a gap nothing.

## `internal static CProspect LMentionProspectRead(CDesk desk, string word, CEnvoy envoy, LSettingsPort settings)`

The mention picker's interaction, shared by the editor and the corpus transcript.
Each area passes its own desk, so no driver chooses which draft is searched.
A desk without a live draft offers nothing.
A failed search shows `Mention.FindFailed` and offers nothing, so no picker opens.

## `public int CMentionUnitRead(string text, int offset)`

The UTF-16 unit a code-point offset starts at, for placing a caret or a popup.

## `public int CMentionOffsetRead(string text, int unit)`

The code-point offset a UTF-16 unit falls in, for turning a click into a Mention offset.

## `public (int CMentionSpanOffset, int CMentionSpanLength) CMentionSpanRead(string text, int start, int length)`

A field's selection as a Mention span, measured in code points.

## `public bool CMentionSpanCheck(string text, int start, int length)`

Whether a field's selection spans any code point, so a link command may run.
A driver asks this rather than measuring the span it read itself.

## `internal static IReadOnlyList<CMentionMark> CMentionMarkRead(IReadOnlyList<LMentionDraft>? drafts)`

The Mentions a draft Example carries, copied field for field as they would be stored.
No draft list answers no Mentions.

## `internal static IReadOnlyList<CMentionMark> CMentionRead(IReadOnlyList<LMention> mentions)`

The stored Mentions of a text as a driver holds them.
The overload taking the marks maps them back for the engine's piece and find calls.
Both stay internal, since they name engine types.

## `internal static CMentionResult CMentionResultRead(LMentionResult result)`

Maps what a click on a text found to its shape, and a stored Mention under the click with it.
The corpus excerpt and the lectern both hand the window this shape, so the lectern calls it too.

## `private static IReadOnlyList<CMentionLabel> CMentionLabelRead(IReadOnlyList<LMentionLabel> labels, string silent)`

Names each label by its headword while linked, and by `silent` otherwise.

## `public CMentionDraft? CMentionFind(CDesk desk, long cardId, long sentenceId, string text, int start, int length)`

The Mention a selection in a sentence field lies inside, read from the desk's held draft.
The selection arrives as the field gives it, in UTF-16 units, and the engine measures it.
The find answers from what the engine holds, never from a list the shell keeps.
It reads the draft without persisting, so a context menu asking many times sends nothing.
The tenure keeps the draft it read until the next change, so command checks read no file per keystroke.
A command about to act on the answer persists first, so pending typing cannot shift the offsets.

## `private static CMentionDraft? CMentionRead(LMentionDraft? found)`

Shapes the found Mention as its Conduct copy, or none when the selection lies in no Mention.
