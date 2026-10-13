# Line

This folder turns drafts into the lines, chips, links and plates a page likeness is made of.
It stands apart from `Page`, which holds the page and section shapes these builders fill.
Every writer takes its text from here, so no two formats disagree about an unknown field.
Localized words arrive from the shell, and the engine holds none.

## `LPortraitText`

The one place a stored state becomes shown text.
A specified value shows its text, an unknown one shows the mark, and an unset one shows nothing.
An unreadable value shows its stored text as written.
This is the display's own rule, kept in logic so an export cannot drift from it.

## `LPortraitTitleRead`

The head of a page read from a state, falling back to the vacant word when the state shows nothing.
An example, a source and a situation page all head themselves this way.

## `LPortraitLabel`

The localized words an entry likeness needs, resolved by the caller.
Passing the words in keeps export out of the interface layer without moving translation into logic.
The stub word is the format of a see-line.
It is filled with the expression, else the title, and the home meaning's number.
The heading word is the format of the heading over a meaning's collocation rows, filled with the row count.
The unit and freedom words name each lexical unit and freedom in the speech chips.
Every other member is a heading or label for one kind of section.

## `LPortraitLegend`

The localized words a page likeness is written with.
The entry likeness has its own label record, because it names different things.
The kind words fall back to the stored word when none was handed in.

## `LPortraitTallyFormat`

The usage chip for a count of places, worded as the display words it.

## `LPortraitKindFormat`

The chip text for a source kind, in the reader's language when one was handed in.

## `LPortraitLine`

One line of a page section with an optional label and the text beside it.
A gloss is a line whose label is its language.
The text is bare of any bracket.
The opener and closer are the marks a writer draws around it, such as `[` or `/`.

## `LPortraitLink`

One linked entry on a section, such as a translation chip on a card.
The headword is shown on the chip and the language after it.

## `LPortraitLinkRead`

Collects every entry id the given cards name, without repeats.
The ids come from translations and from the mentions of each example.
Child cards are walked too, so a nested example is found.
One collected list lets the engine read all targets in a single query.
An id of zero or less marks an unset link and is never added.

## `LPortraitMedia`

One picture or video plate on a card.
Both kinds share a shape because both are drawn as a bordered preview.
The span is the played span of a video, or the caption of a stored picture.

## `LPortraitMediaCreate`

Draft overloads skip any plate whose location was never written.
The display draws no empty plate, so the likeness carries none either.
The byte overload makes a plate from picture bytes held in the workspace, such as a fetched character form.
Its location is a data address, so a writer that embeds files embeds these too.
The media type is read off the leading bytes, and anything not PNG or GIF is called JPEG.

## `LPortraitCard`

Turns meaning card drafts into numbered sections, and collocations into rows.
A homed collocation and an unlinked one share the row shape, exactly as they do on screen.
Every child carries the role it plays on screen, so a writer never guesses a role from a shape.

## `LPortraitCardCreate`

One section per card, in stored order, numbered by the card's position.
The heading is the title, else the kind word, and the role says which.
So a writer can mute a heading that is only the kind.
The expression becomes a phrase child standing first, drawn before the meaning as the panel does.
The card's fields follow through `LPortraitBodyAdd`, then the child cards recurse through here.
The meanings are the whole meaning tree, and the collocations are those that may link into it.
Under a meaning, the collocations whose home it is follow its child cards as one list child.
The list is headed by the label's heading word filled with the row count.
After the list come the see-lines of the collocations that link it without it being their home.
The folds are the cards whose fold is stored, handed on to the rows.
An unset field becomes empty text and an unknown one becomes the mark, as the display does.
Media whose location was never written is skipped, because the display draws no empty plate.

## `LPortraitRowsCreate`

One row per collocation, in the given order.
A homed list and the unlinked band share this one row shape.
A row whose collocation is in the folds carries the fold, and the rest stand open.

## `LPortraitRowCreate`

One unnumbered row for a collocation.
The heading is the phrase `LCollocation.LCollocationPhraseRead` gives, and the line is the meaning.
Every filled field follows as a child through `LPortraitBodyAdd`, and the row carries its images and video.
The fold is kept on the row for a writer that folds.
The row's content is the same either way.

## `LPortraitBodyAdd`

Adds the fields a card and a row share, in the display's order.
Situations become a scene child, registers a tone child, and translations a bridge child of links, each skipped when empty.
A translation id with no loaded target is dropped.
Each written sentence becomes a child, then the tags a label child of chips.

## `LPortraitCardAdd`

The child overload adds one unnumbered child of the given role carrying chips or links.
It is skipped when both are empty.
The chip overload adds one chip when the value reads as text.

## `LPortraitStubAdd`

Adds one see-line for each collocation that links the meaning but has its home elsewhere.
The line names the phrase `LCollocation.LCollocationPhraseRead` gives, and the number of the home meaning.
A collocation whose home has no number is skipped.
The line is an unnumbered section with one unlabelled line and no heading.

## `LPortraitReading`

Turns the reading rows of a draft into the labelled lines shown under the title.
A pronunciation shows its variety as the label and one reading as the text, bare.
The display's brackets travel as the line's open and close marks, so a writer draws them in its own style.
The reading is the respelling when the language is set to respell and the row has one, else the IPA.
That is the rule the display panel follows, so a page never prints one sound twice.
A phonemic language takes slashes, and any other takes square brackets, as the panel does.
A transcription shows its scheme as the label and its spelled reading as the text.
A reflex follows the same respelling rule under its own language, as the reflex rows of the panel do.
The label is empty when the row needs none, which a single unlabeled pronunciation never does.

## `LPortraitReadingCreate`

Each overload lists its rows in draft order and skips a row whose reading is blank.
Each reflex belongs to its own language, so the respelling and phonemic questions are asked per language.
A phonemic reflex takes slashes around the reading alone and any other stands bare.
The reflex label is the language, the kind, the region and the main mark, so a page reads `Japanese Kan-on`.
The romanization, meaning, note and anchored rime rows follow the reading, as the panel's reflex row prints them.

## `LPortraitAnchorRead`

The rime rows a reflex is tied to, each as its slashed reading or its book, joined by a dot.
Empty when the reflex anchors nothing.

## `LPortraitMarkRead`

The marks around a reading are slashes when phonemic.
A bracketed row takes square brackets otherwise, and any other row takes none.

## `LPortraitSentence`

Turns one example row of a card into an unnumbered child section.
The heading is the example word, and the role is quote, so writers draw it as an example row.
The first line carries the bracketed frame as its label and the sentence as its text.
Each written gloss follows as a line labelled by its language, or by the gloss word when unnamed.
The cited source follows as a line labelled by the source word, or the mark when unknown.
The source reads as the display cites it, by byline, which the engine hands in.
The example's own language becomes a chip only when it differs from the entry's.
The mentioned entries become a bridge child of links, each once, so a writer keeps their ids.
A row with no example holds only the frame line.

## Stamp

Commit: `c2f335c331fe`
- `LPortraitCard.cs` `75f8fa2f83350454`
- `LPortraitLabel.cs` `0dc4501554f04f66`
- `LPortraitLegend.cs` `a249935f5a5769be`
- `LPortraitLine.cs` `b47739206ec6efc2`
- `LPortraitLink.cs` `07fa6ad3d1cae6d7`
- `LPortraitMedia.cs` `b31070add1850cfd`
- `LPortraitReading.cs` `7087431c92ea45bb`
- `LPortraitSentence.cs` `cc955dc250134028`
- `LPortraitText.cs` `daff705434b56781`
