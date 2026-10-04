# QEditorCard.cs
Hash: `43b74f38ef246e93`

## `internal sealed class QEditorCard`

The editor's card half: the meaning and collocation cards, their fields, and the etymology field.
The Situation, Register and Tag fields answer to `QContext`, `QRegister` and `QLabel`, the Tag dropdown to `QSlate`.
The Situation, Register and citation dropdown lives in `QProffer`, the Translation and mention dropdown in `QProspect`.
The Translation field answers to `QLink`, the language toggle to `QSpeaker` and a sentence's Gloss rows to `QGloss`.
`QSentence` holds a sentence row's gestures, mentions and frame.
`QCitation` holds its Source field and the shared Source list.
The card's header, badge, eraser and rendering live in `QCard`, and its header drag in `QCardDrag`.
The two lists answer to `QMeaning` and `QCollocation`, and the picture and film rows to `QImage` and `QVideo`.
It keeps the two card lists, which the card drivers render and the field drivers walk.
It keeps the one list of loaded languages, which the language menu fills and every card's Gloss picker shows.

## `internal QEditorCard(FrameworkElement surface)`

Builds the card drivers over the editor scope in the order the editor once built them.
The routed text handler is added where the editor once added it, after the language toggle.

## `internal QSentence QEditorCardSentence { get; }`

The sentence driver, handed out so the editor subscribes its frame and mention redraws itself.
The frame must draw before the cards and the mention chips after them, so the editor orders them.

## `internal QProspect QEditorCardProspect { get; }`

The Translation and mention dropdown, handed out so the corpus can open it over its transcript.

## `internal void QEditorCardIntroduce(QWindow host, CEditor editor)`

Hands the Conduct editor to each card driver, in the order the editor once introduced them.
The card, drag and list drivers are handed it last.
The language toggle also takes the window's envoy, which its catalog load reports a failure through.

## `internal void QEditorStartRefine()`

A tenure started, so the card lists start empty.

## `internal void QEditorMeaningRefine(CEntryDraft draft)`

The meaning cards, each reading its own links as it is shown.
`QEditorCollocationRefine` does the same for the collocation cards.
Both hand their list to `QCard`, which pairs each control with its card by id.

## `private void QEditorTextObserve(object sender, TextChangedEventArgs e)`

The one routed handler left, for the fields inside cards that come and go with them.
The spine's own boxes report through their own handlers.
Card, sentence and film fields are hooked by their own drivers, so no box name is read here.
A chip field's entry hands its text to that field's text observer, which calls one gate.

## `private void QEditorFieldObserve(TextBox box)`

A Gloss or picture field changed, so its raw text goes to its driver.
The row's type alone picks the driver, which pairs the row to its record.
Only a field with the keyboard in it reports, since a write from the draft echoes through the same event.
