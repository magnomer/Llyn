# PCard.cs
Hash: `77487827a064361e`

## `internal sealed class PCard : INotifyPropertyChanged`

One meaning or collocation card of the editor, the item the card list draws.
Each chip field is a `PCaret`, and the Example rows are a `PCardSentence`.
It holds the engine's values as they stand, so it never resolves a state.

## `internal PCard(string prefix, ObservableCollection<QCitationItem> catalog, ObservableCollection<string> particles, ObservableCollection<string> dependences, ObservableCollection<PLanguageItem> languages, CCardDraft draft)`

The catalog, particle, dependence and language lists are the editor's own, shared by every card.
They go to the card's `PCardSentence`, whose Example rows read them.
So one write reaches every card at once.
Each caret field sets its hint when it is built.
So an empty card shows its placeholders before any draft arrives.

## `public PCardSentence PCardSentence { get; }`

The Example rows a card shows, built once with the card.

## `public ObservableCollection<QImageItem> PCardImage { get; }`

The Image rows a card shows.
Unlike Example and Situation, a card starts with no Image row at all.
A picture is an addition the user asks for through the card's Extra row.
So an empty card shows no picture field, and one whose last picture is dropped goes back to none.
The engine holds the rows, blank ones included, and the card renders them by id.

## `public ObservableCollection<QVideoItem> PCardVideo { get; }`

The Video rows a card shows, kept the way the Image rows are.
A clip is an addition the user asks for.
So a card starts with no row and offers none it is not given.

## `internal void PCardImageShow(IReadOnlyList<CImageDraft> rows)`

Makes the rows show the engine's Images, matched by id.
A row already reading what the engine holds is left alone.
A new row is built from its ready state alone, since the address arrives resolved.
The row holds the engine's values and nothing typed, so nothing on it is listened to.
What is typed into its fields leaves through the editor's own handler.

## `internal void PCardVideoShow(IReadOnlyList<CVideoDraft> rows)`

Makes the rows show the engine's Videos, matched by id.
A field already reading what the engine holds is left alone.
A new row is built from its draft alone, which already carries the screen it plays.
The row holds the engine's values and nothing typed, so nothing on it is listened to.
What is typed into its fields leaves through the editor's own handler.

## `public PCaret<PContext> PCardContext { get; }`

The Situations a card shows, with the entry beside them.
A Situation can hold spaces and punctuation without a separator being guessed at inside it.
Only a comma ends a Situation.
A chip whose wording changed yields to the fresh chip `QContext.QContextShow` built.
The engine's clerk skips a wording the card already shows, so the field shows what the engine holds.

## `public PCaret<PRegister> PCardRegister { get; }`

The Registers a card shows, with the entry beside them, held the way the Situations are.
A chip whose name changed yields to the fresh chip `QRegister.QRegisterShow` built.
The engine's clerk skips a Register the card already holds, by id and by wording alike.

## `public PCaret<QLinkChip> PCardLink { get; }`

The Translation links a card shows, with the entry beside them, held the way the Situations are.
A chip whose headword or language changed yields to the fresh chip `QLink.QLinkShow` built.

## `public PCaret<PLabelChip> PCardLabel { get; }`

The Tags a card shows, with the entry beside them, held the way the Situations are.
A chip whose name changed yields to the fresh chip `QLabel.QLabelShow` built.

## Inline notes

### `public long PCardId { get; set; }`

Id of the draft card this control shows.
It is negative for a card the engine minted and positive for a stored row.
No control shows it and nothing on screen changes with it.
It is an address into the draft the engine holds, not a value the card owns.
Every request the card raises names it, and every bulletin's render finds the card by it.

### `public int PCardPosition`

The number this card is shown by, counted from one.
It is stored rather than derived, so the card carries the same number the draft was saved with.
Changing it retitles the card, because the header reads the prefix and this number.

### `public string PCardPositionText`

The number as the badge shows it, read from the position alone.
Typed text never enters it, because a half-typed number is not an order.
The box keeps that text until the badge shuts or the card is renumbered.

### `public bool PCardPositionActive`

Whether the badge is open for writing.
The number is read-only otherwise, so a click on it drags the card as the header does.

### `internal void PCardPositionHide()`

Closes the badge, and the row fill puts the stored number back into the box.
So a number typed and then abandoned or refused leaves nothing behind.

### `public CStateWording PTitle => _pTitle;`

The title, definition and expression are the engine's values, shown as they stand.
A field the user types into binds the value one way and reports the typing to the editor itself.
So the card holds no copy of what was typed and never resolves a state.
Each arrives worded by Conduct, which also names the meaning field's hint for the card's kind.

### `internal void PCardTitleShow(CStateWording value)`

Takes the draft's value and redraws the field only when the value differs.
A field already reading what the engine holds is left alone, so the caret survives its own echo.

### `internal void PCardDefinitionShow(CStateWording value)`

Takes the draft's definition and redraws the field only when the value differs.
It answers the same way as the title, so the caret survives its own echo.

### `internal void PCardExpressionShow(CStateWording value)`

Takes the draft's expression and redraws the field only when the value differs.
It answers the same way as the title, so the caret survives its own echo.

### `public string PCardTitle`

The header text, the prefix and the number joined.
It is read-only, so only a renumbering changes it.

### `public CStateWording PCardDefinition => _pCardDefinition;`

The definition as the engine holds it, shown as it stands.

### `public CStateWording PCardExpression => _pCardExpression;`

The expression as the engine holds it, shown as it stands.
