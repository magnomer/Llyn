# PCard.cs
Hash: `9b0fcb9fe2596caa`

## `internal sealed class PCard : INotifyPropertyChanged`

One presentation item represents a meaning or collocation without resolving Conduct's worded values.
It retains card identity while changed values notify the row driver.

## `internal PCard(string prefix, ObservableCollection<QCitationItem> catalog, ObservableCollection<string> particles, ObservableCollection<string> dependences, ObservableCollection<PLanguageItem> languages, Func<PCard, CStateWording> peek, CCardDraft draft)`

Shared sentence-menu lists keep cards consistent without copying their catalogs.
The initial draft supplies identity, the stored verdict and worded fields.
The caller's `peek` selects an existing card wording, avoiding a second copy of folded text.

## `public PCardSentence PCardSentence { get; }`

One sentence presentation item survives for the card's lifetime.

## `public ObservableCollection<QImageItem> PCardImage { get; }`

The image collection starts empty and receives only supplied draft rows.
The panel never invents a blank image row.

## `public ObservableCollection<QVideoItem> PCardVideo { get; }`

The video collection follows the same empty-start policy as images.

## `internal void PCardImageShow(IReadOnlyList<CImageDraft> rows)`

Image rows are reconciled by id, allowing retained items to accept fresh ready values.
Their presentation needs no storage lookup.

## `internal void PCardVideoShow(IReadOnlyList<CVideoDraft> rows)`

Video rows use the same identity-based reconciliation as images.
Their drafts already supply the values needed for presentation.

## `public PCaret<PContext> PCardContext { get; }`

Situation chips retain their existing item when identity and wording agree.
A changed wording uses the supplied fresh chip.

## `public PCaret<PRegister> PCardRegister { get; }`

Register chips preserve unchanged wording and replace changed presentation values.

## `public PCaret<QLinkChip> PCardLink { get; }`

Translation chips preserve existing items only while both headword and language agree.

## `public PCaret<PLabelChip> PCardLabel { get; }`

Tag chips preserve existing items while their names agree.

## `public long PCardId { get; }`

The draft supplies immutable card identity, separating a card's address from its current list position.

## `public int PCardPosition`

Position changes notify both the badge and generated header title.
Unchanged positions produce no notification.

## `public string PCardPositionText`

Badge text comes from the position using invariant culture, never from partially typed text.

## `public bool PCardPositionActive`

Badge-edit visibility is presentation state, independent of the stored position.
Only a changed active verdict notifies the row driver.

## `internal void PCardPositionHide()`

Closing the badge changes its active verdict, letting the row driver restore the position text.

## `public CStateWording PTitle`

The title remains Conduct's worded value rather than a locally resolved state.

## `internal void PCardTitleShow(CStateWording value)`

Equal title values are left alone, avoiding redundant redraws that could disturb the caret.

## `internal void PCardDefinitionShow(CStateWording value)`

Definition echoes follow the title's unchanged-value policy.

## `internal void PCardExpressionShow(CStateWording value)`

Expression echoes follow the same unchanged-value policy.

## `public string PCardTitle`

The header caption combines the fixed prefix and current position, independently of the worded title.

## `public CStateWording PCardDefinition`

The definition remains the supplied worded value.

## `public CStateWording PCardExpression`

The expression remains the supplied worded value.

## `public bool PCardFolded`

The last supplied fold verdict drives presentation, not a locally chosen fold policy.

## `public bool PCardStored { get; }`

The initial draft supplies the stored verdict, which lets the row driver decide hinge visibility.
It is fixed for the card's lifetime, since the verdict follows the immutable id and rows are reconciled by id.

## `public string PCardPeek`

Folded text is read from the selected current wording, so field updates cannot leave a separate peek copy stale.
Its wording's notification lets the row driver repaint it.

## `internal void PCardFoldShow(bool folded)`

Only a changed fold verdict notifies the row driver.
The notification names `PCardFolded`, and the row driver repaints the fold and the fixed stored verdict together.

## `public event PropertyChangedEventHandler? PropertyChanged;`

Changed presentation values notify the row driver, while unchanged values preserve their existing controls.
