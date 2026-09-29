# QAutograph.cs

## `internal sealed class QAutograph`

The driver of the Author edit area, a veneer page the guild page places.
It holds no state, since `CGuild` holds the autograph desk and the union verdict.
Every Observe hands one raw value to one gate, and every Refine paints what the panel answers.

## `internal QAutograph(UserControl surface)`

Takes the edit page as its surface.
Subscribes the name and union fields and the union list's clicks, and attaches the union row fill.

## `private TextBox QAutographName`

Each part of the page is pulled by its contract ID through `QContract.QContractFind`.

## `internal void QAutographIntroduce(CGuild guild)`

Takes the panel the guild driver built and answers its union clear and its autograph's draft notice.
`CGuild` attaches the desk's own bulletins at build, with the marshal the forge hands it.

## `internal void QAutographTallyShow(CVita vita)`

Writes the two count chips from the vita sheet the guild read, so both sides show the same counts.

## `internal void QAutographModeUpdate()`

Shows the union section or its unsaved notice by the panel's union verdict.
The guild driver calls it whenever it writes its own mode.

## `private void QAutographStartRefine()`

Answers `CGuildUnionCleared`, which the panel raises when a tenure starts.
The union field and its list are emptied and the name takes focus.

## `private void QAutographDraftUpdate(CDraft draft)`

The held draft was read again, so the name field shows its name.

## `private void QAutographNameObserve(object sender, TextChangedEventArgs e)`

Every keystroke hands the raw name to `CGuildNameSet`.

## `private void QAutographUnionObserve(object sender, TextChangedEventArgs e)`

Hands the typed text to `CGuildUnionRead` and its answer to `QAutographUnionRefine`.

## `private void QAutographUnionRefine(IReadOnlyList<CCatalogAuthor> rows)`

Lists the Authors the typed name matches, for the user to fold this one into.

## `private void QAutographChoiceObserve(object sender, RoutedEventArgs e)`

A click on a union row hands its Author id to `CGuildUnionSelect`, which folds the held Author into it.
