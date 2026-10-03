# TEngineTenureVista.cs
Hash: `4f996cf9289b7ea7`

## `public sealed class TEngineTenureVista`

Covers the tenures a vista starts from one given row, on a fresh workspace.
The occurrence, quotation, footnote, membership and cohort starts each open a fresh entry that carries the row.
A start without its row opens a blank entry that reads unchanged.
An occurrence is storable only once its headword is written.
Only the storable fact holds the queue, through `TEngineTenure.TTenureHold`.

## `public void OccurrenceStart_SituationGiven_StartsAFreshEntryAlreadyLinked()`

A fresh occurrence start links the Situation to the first card before it returns.
The draft is unstored, so the link is the only change.

## `public void OccurrenceStart_NoSituation_StartsABlankEntry()`

Without a Situation the start is a plain fresh entry, unchanged and with no link.

## `public void TenureStorable_UnnamedOccurrence_AnswersFalseUntilTheHeadwordIsWritten()`

A linked occurrence has changed but refuses its store while it has no headword.
The storable verdict never writes the queue, so a waiting headword counts only after the change check writes it.

## `public void QuotationStart_ExampleGiven_StartsAFreshEntryCitingIt()`

A fresh quotation start cites the Example in the first sentence of the first card.

## `public void FootnoteStart_SourceGiven_StartsAFreshEntryCitingIt()`

A fresh footnote start cites the Source in the first sentence of the first card.

## `public void FootnoteStart_NoSource_StartsABlankEntry()`

A fresh footnote start without a Source is a blank entry that reads unchanged.

## `public void MembershipStart_TagGiven_StartsAFreshEntryCarryingIt()`

A fresh membership start puts the Tag on the first card.

## `public void MembershipStart_NoTag_StartsABlankEntry()`

A fresh membership start without a Tag is a blank entry that reads unchanged.

## `public void CohortStart_RegisterGiven_StartsAFreshEntryCarryingIt()`

A fresh cohort start puts the Register on the first card.

## `public void CohortStart_NoRegister_StartsABlankEntry()`

A fresh cohort start without a Register is a blank entry that reads unchanged.
