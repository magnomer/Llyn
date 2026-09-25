# PCorpusExcerpt.cs

## `public partial class PCorpus`

The reading side of the Corpus panel, the page one chosen Example is shown on.
The sentence stands at its head and the cited Source under it.
A click on a word asks the engine what it names.

## `private string? PExcerptTextRead(LStateValue value)`

The text a three-state value reads as, or null when it was never written.
An unknown value reads the unknown mark, and whether it is unknown is the converter's to say.

## `private void PExcerptShow(LExample example)`

Paints the read page of an Example the engine announces.
The tally counts the Example the anthology has chosen.

## `private void PExcerptSentenceShow(LExample example)`

Writes the sentence at the head of the page, as a situation's title stands at the head of its page.
A never-written sentence reads the unwritten text in the muted colour, because the head of the page cannot be empty.
The sentence field is the clickable control, and it takes its text through its own property.
Its Mentions are handed over only when the text shown is the stored sentence.
The unknown mark and the unwritten text have no words for a Mention to lie on.

## `private void PExcerptCitationShow(LStateAnchor value)`

Writes the cited Source under its heading, or hides the heading when no citation was ever written.
A source that is not known is itself a Source on the shelf, and reads under its own line.

## `private void PExcerptMentionHandle(object? sender, PMentionArgument e)`

A click on a word of the open Example asks the engine what stands at that offset.
The engine reads the stored sentence itself, so the panel passes the id and the offset and nothing more.
The window decides what the answer opens, as it does for the display cards.
An open draft is confirmed first, because landing on an Entry leaves the corpus panel.

# PCorpusExcerpt.xaml

## `ResourceDictionary`

The reading side of the Corpus panel: the anthology rows and the excerpt page they open.
The panel keeps its layout and merges these shapes from here, as it merges the editor popups.
Nothing here answers an event, so the dictionary is loose and needs no class of its own.
The quotation row stands here because it is read, never edited.
`Theme.Display.Value` is the face the read Gloss takes, and the theme reaches it by a dynamic reference.

## `<Style x:Key="Theme.Excerpt.Chip" TargetType="Border">`

The language chip, shaped as a speech chip and coloured in the accent, with the flag before the name.
The same shape dresses the language toggle on the editing side.
So the language reads the same whether it is read or chosen.

## `<Style x:Key="Theme.Excerpt.Tally" TargetType="Border">`

The usage count, a chip of the same shape in the raised surface colour.
So it reads as a figure and not a language.
It reads as a sentence, not a bare number.
A bare number beside a sentence says nothing about what it counts.
