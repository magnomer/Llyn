# TCardSense.cs
Hash: `9221ebe024afd394`

## `public sealed class TCardSense`

Covers the links from a collocation to meanings, as Conduct hands them to the editor and the read view.
Most cases save one entry with a parent meaning and two children, a second top-level meaning, and two collocations.
One collocation links the first child, then the second top-level meaning.
The other collocation links nothing.

## `public void CardSenseRead_NestedMeanings_ReturnsDepthAndPath()`

The link menu lists every meaning in tree order with its depth and dotted path.
Children sit at depth one under their parent.

## `public void DisplayCardRead_LinkedCollocation_HomesUnderFirstLink()`

A first link to a child homes the collocation under that child's top-level parent in the read view.
The homed row reads its phrase, and the second top-level meaning, though linked too, holds no collocation.

## `public void DisplayCardRead_FurtherLink_AddsSeeLine()`

The further link adds one "see" line under its own meaning, and the home meaning gets none.
The line names the expression and the full path of the nested home.
It carries the homed row's id for the click.

## `public void DisplayCardRead_UnlinkedCollocation_ListsOther()`

A collocation without links is listed in the Other part, which then shows.

## `public void DisplayCompassRead_HomedCollocation_AddsMutedCountRow()`

This case shows its own draft, with one collocation homed at the first meaning and two at the second.
Each meaning with homed collocations gets one muted depth-two row after its own row.
It never gets one row per collocation.
The row points at its meaning's card and carries no number.
It is worded with `Display.CollocationCountOne` for one and `Display.CollocationCount` for more, the count filled in.
No collocation is unhomed, so the collocation part is absent.

## `public void DisplayCompassRead_EqualCounts_KeepsCountsUnnumbered()`

Two meanings each home one collocation, so their count rows read alike.
A count is not a name, so the twin rule leaves both unnumbered.

## `public void DisplayCardRead_NoStoredFold_ReadsRowOpen()`

No fold is stored for the shared entry, so the homed row and the Other row both read open.

## `public void DisplayCardRead_HomedCollocation_WordsHeadingCount()`

The first meaning, home of one collocation, carries the caption worded with a count of one.
The second meaning homes none, so its caption stays empty.

## `public void DisplayCardRead_HomedRow_MapsMarkers()`

The homed row carries the layout's phrase and its "also 2" marker, for the further link to the second meaning.
It has no example and no translation, so those markers stay empty, and the meaning leaf carries none.
The verdicts agree, so the row is shared but not quoted.

## `public void CollocationLayoutRead_HomedCollocation_FillsMarkers()`

This case shows its own draft, whose collocation has a title but no expression, two examples and three targets.
The phrase falls back to the title.
The flags name each language once, in first-seen order.
The example count is two.
The further paths name only the second meaning, though the first is linked twice.

## `private static LEntryDraft TCompassDraftCreate(params long[][] senses)`

An unsaved draft for the compass cases: the meanings 41 and 42, and one collocation per `senses` item.
Each collocation, numbered from 51, links the meanings its item names.

## `private static LEntry TCardSenseSave(LEngine engine)`

Saves the shared entry, with meaning ids minted for the draft so the links can name them.
