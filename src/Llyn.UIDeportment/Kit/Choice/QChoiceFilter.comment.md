# QChoiceFilter.cs
Hash: `12cb4ec034881da9`

## `internal sealed class QChoiceFilter`

Drives the shared filter `PChoiceFilter` inside whichever browse panel holds it.
It filters by language, or by source kind in the authors panel.
It owns the filter's icon, tooltip, rows and mark, so no panel repeats them.
The ticked rows go straight to the panel's aperture, which keeps them and announces a changed filter.

## `internal QChoiceFilter(UserControl choice)`

Takes the placed filter.
Its menu hangs from its own button, and the markup shifts it under the bar's right end.
It ties the button to its popup and sets the filter icon.

## `internal void QChoiceFilterIntroduce(CAperture aperture, string title)`

Hands the filter the aperture its ticks set.
`title` names the tooltip resource.
The rows wait for `QChoiceFilterBuild`, because the languages are known only after a load.

## `internal void QChoiceFilterRefine()`

Shows the mark on the button while the aperture hides any row.
A click redraws it at once, and the owner redraws it when a vista is restored.

## `internal void QChoiceFilterBuild(IReadOnlyList<string> languages)`

Builds the menu from the languages a load answered, ticking all but the aperture's hidden ones.

## `internal void QChoiceFilterBuild(IReadOnlyList<CReferenceKind> kinds)`

Builds the menu from the source kinds a roll answered, ticking all but the aperture's hidden ones.
The kinds carry their own resource keys, so the rows come from `QChoice.QChoiceKindRefine`.

## `internal void QChoiceFilterClose()`

Closes the menu, so nothing stays open over a window that is going.
