# Lexicon

This folder holds the records the lexicon's editors, dropdowns and read pages receive from Conduct.
They stand apart from the draft records because they carry engine verdicts, menus and ready lines rather than held fields.
The engine decides every flag, so a driver only looks keys up and paints.
Where a record names a key, Conduct chooses it and the driver looks it up.

## `CCategory`

The category menu of the part of speech field, ready to show.
The rows are the parts the typed text matches, in catalog order.
The declared, matched and shown flags are the engine's verdicts.
`CCategoryHint` is the key the menu shows in place of rows, or null while rows match.
A language that names no part and a text that matches none each have their own key.

## `CCategoryRow`

One part of speech the category menu offers.
`CCategoryRowTaken` is the engine's verdict that the draft already holds the part.

## `CExample`

One stored Example, as the corpus transcript and excerpt read it.
The text is uncertain when it is unknown.
The citation is the ready line the cited reference is shown under, empty when none.
The tally is the chip's sentence for the chosen Example, worded by the engine.
The pieces divide the text at the Mentions the excerpt links, plain unless the text reads soundly.
`CExampleTextHint` is the key of the sentence field's placeholder.
`CExampleWording` is the key the excerpt words instead of the text.
An unknown text reads the unknown mark, and a never-written one reads `Example.Unwritten`.
It is null while the text itself shows.
`CExampleTextKept` says the transcript's sentence field already shows this text, as the engine would store it.
The anthology sets it on a draft bulletin, so the driver keeps the field and never moves the caret.
It stays false on every other read, so the driver paints the text.
`CExampleMuted` is true only for a never-written text.
An unknown text keeps the ink, since the unknown mark is a written state.
`LExampleHintRead` asks a never-written sentence for a sentence, and an unknown one reads the unknown mark.
The text gate answers the typed form, since typing ends an unknown mark.
`LExampleBlankRead` is the empty Example the transcript shows while no draft is held, carrying the tally.
The corpus raises it instead of null, so the driver keeps no default of its own.

## `CMeaning`

One Meaning as the sense menu, the link picker or a collocation's link chip shows it.
The menu and the picker rows come in reading order, the chips in the card's link order.
The name is the title, the definition, or the unknown label.
The depth is zero on a link chip.
The path is its outline number under its parents, such as `2.1`.
So a link chip names where the Meaning sits.

## `CSentenceOrder`

Where a language places the particle and the dependence around an example.
`CSentenceOrderPlain` puts the particle first and the dependence second.
It stands where the pack states no valid order, and before any order arrives.

## `CStateValue`

A written value, as a driver shows it with its state.
It carries the engine's verdicts, so a driver never judges the state itself.
The text is empty when nothing legible is written.
`CStateValueEmpty` is the value of a field nothing is written in.
`CStateValueShown` is the text, or null when it is empty.

## `CStateWording`

A written value as a list row, a read page or an edit field shows it, ready.
It carries its text, or the key the driver looks up instead.
The driver shows the text while the key is null, and never judges the value's state itself.
`CStateWordingMuted` is true when the value was never written, so a page mutes or hides it.
The hint is the key an empty edit field shows in the value's place, null when it shows nothing.
`LStateWordingRead` has an unknown value read the unknown mark.
A value with no text reads the key its owner names.
An owner that words nothing for an unwritten value names no key, and its driver hides the value instead.
An unknown value's field shows the unknown mark, and any other field shows the hint its owner names.
The hint holds while text is written, so a field emptied by typing shows it at once.
The state verdicts arrive from the engine on the value, so only the choice of key happens here.

## `CTranslationTarget`

One entry a card or an etymology links to, as its chip shows it.
It holds the linked entry's id, headword and language.

## `CUsage`

One place that cites what a vita or the lectern shows, as their citation lists read it.
The id is the citing side itself, an Example when the place quotes one.
The name is the headword that names the place, and the epithet is printed after it.
The title is uncertain when unknown.
`CUsageQuoted` is whether the place quotes an Example rather than naming an Entry.
`CUsageCollocated` is whether a Collocation, not a Meaning, holds the place.
`CUsageOwnerKey` names the citing side as a Collocation, an Example or a Meaning.
Both drivers word the place by it, so neither picks the key.
`CUsageTitleKey` is the unknown mark's key while the title is uncertain, and null while the title's own text stands.

## Stamp

Commit: `c2f335c331fe`
- `CCategory.cs` `5a81b26b3cc96c5d`
- `CCategoryRow.cs` `b8ed65076bb7a62e`
- `CExample.cs` `7317a90f9bef8534`
- `CMeaning.cs` `dd7aa1487d775214`
- `CSentenceOrder.cs` `54e18d6ed1fbafa9`
- `CStateValue.cs` `7834e3a4c04a214e`
- `CStateWording.cs` `dfa5e26486b97164`
- `CTranslationTarget.cs` `b0005076d06bed8a`
- `CUsage.cs` `4618200229fd8db0`
